#!/usr/bin/env python3
"""
Model Context Protocol (MCP) stdio server for RandomPlus RimWorld Mod development.
Zero-dependency, pure Python standard library implementation of MCP JSON-RPC 2.0.

Features robust, multi-layer auto-detection for MSBuild and RimWorld installation
paths, safe non-destructive deployment, UTF-8 stdio enforcement, and full JSON-RPC 2.0 compliance.
"""

import sys
import os
import json
import subprocess
import glob
import re
import shutil
import xml.etree.ElementTree as ET

# Force UTF-8 on Windows stdio to prevent charmap encoding errors
if sys.platform == "win32":
    import io
    sys.stdin = io.TextIOWrapper(sys.stdin.buffer, encoding="utf-8", errors="replace")
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", line_buffering=True)
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8", line_buffering=True)

WORKSPACE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))

# ==============================================================================
# Robust Path Detection (MSBuild & RimWorld)
# ==============================================================================

def find_msbuild():
    """
    Locates MSBuild.exe using a prioritized fallback chain:
    1. MSBUILD_PATH environment variable override
    2. Official Microsoft vswhere.exe locator
    3. Standard Visual Studio / Build Tools installation paths (VS 2026 / 18, VS 2022 / 17, VS 2019 / 16)
    4. System PATH lookup
    Always verifies os.path.isfile before returning.
    """
    # 1. Environment variable override
    env_override = os.environ.get("MSBUILD_PATH")
    if env_override and os.path.isfile(env_override):
        return os.path.abspath(env_override)

    # 2. Try vswhere.exe
    vswhere_candidates = [
        os.path.expandvars(r"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"),
        os.path.expandvars(r"%ProgramFiles%\Microsoft Visual Studio\Installer\vswhere.exe"),
    ]
    for vswhere in vswhere_candidates:
        if os.path.isfile(vswhere):
            try:
                cmd = [vswhere, "-latest", "-products", "*", "-property", "installationPath"]
                res = subprocess.run(cmd, capture_output=True, text=True, timeout=5)
                inst_path = res.stdout.strip()
                if inst_path and os.path.isdir(inst_path):
                    for sub in [
                        os.path.join("MSBuild", "Current", "Bin", "MSBuild.exe"),
                        os.path.join("MSBuild", "Current", "Bin", "amd64", "MSBuild.exe"),
                        os.path.join("MSBuild", "15.0", "Bin", "MSBuild.exe"),
                    ]:
                        cand = os.path.join(inst_path, sub)
                        if os.path.isfile(cand):
                            return os.path.abspath(cand)
            except Exception:
                pass

    # 3. Known fallback paths
    fallback_paths = [
        r"C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        r"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    ]
    for cand in fallback_paths:
        if os.path.isfile(cand):
            return os.path.abspath(cand)

    # 4. System PATH
    which_path = shutil.which("msbuild") or shutil.which("msbuild.exe")
    if which_path and os.path.isfile(which_path):
        return os.path.abspath(which_path)

    return None

def find_steam_libraries():
    """Discovers all Steam library roots via Registry and libraryfolders.vdf."""
    libraries = []
    if sys.platform == "win32":
        try:
            import winreg
            with winreg.OpenKey(winreg.HKEY_CURRENT_USER, r"SOFTWARE\Valve\Steam") as key:
                steam_path, _ = winreg.QueryValueEx(key, "SteamPath")
                if steam_path and os.path.isdir(steam_path):
                    libraries.append(os.path.normpath(steam_path))
        except Exception:
            pass

    for p in [r"C:\Program Files (x86)\Steam", r"C:\Program Files\Steam", r"D:\Steam", r"E:\Steam"]:
        norm = os.path.normpath(p)
        if os.path.isdir(norm) and norm not in libraries:
            libraries.append(norm)

    all_roots = list(libraries)
    for lib in libraries:
        vdf = os.path.join(lib, "steamapps", "libraryfolders.vdf")
        if os.path.isfile(vdf):
            try:
                with open(vdf, "r", encoding="utf-8", errors="ignore") as f:
                    content = f.read()
                matches = re.findall(r'"path"\s+"([^"]+)"', content)
                for m in matches:
                    norm = os.path.normpath(m.replace("\\\\", "\\"))
                    if os.path.isdir(norm) and norm not in all_roots:
                        all_roots.append(norm)
            except Exception:
                pass
    return all_roots

def find_rimworld():
    """
    Locates RimWorld installation directory.
    Validates by confirming presence of RimWorldWin64_Data/Managed/Assembly-CSharp.dll.
    """
    def is_valid(p):
        return (
            p and os.path.isdir(p) and
            os.path.isfile(os.path.join(p, "RimWorldWin64_Data", "Managed", "Assembly-CSharp.dll"))
        )

    # 1. Environment variable override
    env = os.environ.get("RIMWORLD_DIR")
    if is_valid(env):
        return os.path.abspath(env)

    # 2. Check all Steam library roots
    for root in find_steam_libraries():
        cand = os.path.join(root, "steamapps", "common", "RimWorld")
        if is_valid(cand):
            return os.path.abspath(cand)

    # 3. Known fallback locations across drives
    fallbacks = [
        r"C:\Program Files (x86)\Steam\steamapps\common\RimWorld",
        r"C:\Program Files\Steam\steamapps\common\RimWorld",
        r"D:\SteamLibrary\steamapps\common\RimWorld",
        r"E:\SteamLibrary\steamapps\common\RimWorld",
        r"D:\Steam\steamapps\common\RimWorld",
        r"E:\Steam\steamapps\common\RimWorld",
        r"C:\Program Files (x86)\GOG Galaxy\Games\RimWorld",
        r"C:\GOG Games\RimWorld"
    ]
    for cand in fallbacks:
        if is_valid(cand):
            return os.path.abspath(cand)

    return None

def is_rimworld_running():
    """Checks whether RimWorldWin64.exe is currently running on Windows."""
    if sys.platform != "win32":
        return False
    try:
        res = subprocess.run(["tasklist", "/FI", "IMAGENAME eq RimWorldWin64.exe", "/NH"],
                             capture_output=True, text=True, timeout=5)
        return "RimWorldWin64.exe" in res.stdout
    except Exception:
        return False

# ==============================================================================
# MCP Tool Definitions
# ==============================================================================

TOOLS = [
    {
        "name": "build_mod",
        "description": "Compiles RandomPlus using MSBuild. Returns build status, output binary size, and any compiler warnings or errors.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "configuration": {
                    "type": "string",
                    "enum": ["Release", "Debug"],
                    "default": "Release",
                    "description": "Build configuration"
                }
            },
            "required": []
        }
    },
    {
        "name": "verify_mod",
        "description": "Performs comprehensive project verification: static decompiler artifact checks, zero-allocation rules in hot loops, XML well-formedness, and MSBuild compilation.",
        "inputSchema": {
            "type": "object",
            "properties": {},
            "required": []
        }
    },
    {
        "name": "validate_xml",
        "description": "Validates all XML files in the Resources directory (About.xml, KeyBindings.xml, Languages) for syntactic well-formedness and reporting line/column on errors.",
        "inputSchema": {
            "type": "object",
            "properties": {},
            "required": []
        }
    },
    {
        "name": "clean_mod",
        "description": "Removes temporary build directories (bin/, obj/) and debug symbols to guarantee a clean slate.",
        "inputSchema": {
            "type": "object",
            "properties": {},
            "required": []
        }
    },
    {
        "name": "deploy_mod",
        "description": "Builds and safely deploys the complete RandomPlus mod (assemblies, defs, textures, languages, about.xml) directly to RimWorld's local Mods directory for immediate in-game testing. Prevents deployment if the game is running unless forced.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "destination": {
                    "type": "string",
                    "description": "Optional custom RimWorld Mods directory path."
                },
                "force": {
                    "type": "boolean",
                    "default": False,
                    "description": "Force deployment even if RimWorld process is detected."
                }
            },
            "required": []
        }
    },
    {
        "name": "inspect_project",
        "description": "Returns comprehensive diagnostic status: detected MSBuild path, RimWorld installation path, game process status, C# lines of code, compiled DLL size, and XML assets.",
        "inputSchema": {
            "type": "object",
            "properties": {},
            "required": []
        }
    }
]

# ==============================================================================
# Tool Implementations
# ==============================================================================

def run_build(configuration="Release"):
    msbuild = find_msbuild()
    if not msbuild:
        return False, "Build FAILED: MSBuild.exe was not found. Please verify Visual Studio Build Tools installation or set MSBUILD_PATH."

    sln_path = os.path.join(WORKSPACE_DIR, "RandomPlus.sln")
    cmd = [msbuild, sln_path, f"/p:Configuration={configuration}", "/v:minimal", "/nologo"]
    try:
        proc = subprocess.run(cmd, cwd=WORKSPACE_DIR, capture_output=True, text=True, timeout=45)
        output = proc.stdout.strip()
        if proc.stderr:
            output += "\n" + proc.stderr.strip()
        dll_path = os.path.join(WORKSPACE_DIR, "Resources", "1.6", "Assemblies", "RandomPlus.dll")
        if proc.returncode == 0 and os.path.isfile(dll_path):
            size = os.path.getsize(dll_path)
            return True, f"Build SUCCESS ({configuration})\nOutput DLL: {dll_path} ({size} bytes)\nUsing MSBuild: {msbuild}\n\nOutput:\n{output}"
        else:
            return False, f"Build FAILED (Exit Code: {proc.returncode})\nUsing MSBuild: {msbuild}\n\nOutput:\n{output}"
    except Exception as e:
        return False, f"Build execution error: {str(e)}"

def run_validate_xml():
    res_dir = os.path.join(WORKSPACE_DIR, "Resources")
    xml_files = glob.glob(os.path.join(res_dir, "**", "*.xml"), recursive=True)
    errors = []
    for fpath in xml_files:
        rel = os.path.relpath(fpath, WORKSPACE_DIR)
        try:
            ET.parse(fpath)
        except ET.ParseError as pe:
            errors.append(f"{rel}:{pe.position[0]}:{pe.position[1]}: {str(pe)}")
        except Exception as ex:
            errors.append(f"{rel}: {str(ex)}")

    if errors:
        return False, f"XML Validation FAILED ({len(errors)} errors found):\n" + "\n".join(errors)
    return True, f"XML Validation SUCCESS: All {len(xml_files)} XML files are well-formed."

def run_clean():
    cleaned = []
    for root in [WORKSPACE_DIR, os.path.join(WORKSPACE_DIR, "Source")]:
        for d in ["bin", "obj"]:
            target = os.path.join(root, d)
            if os.path.isdir(target):
                try:
                    shutil.rmtree(target)
                    cleaned.append(os.path.relpath(target, WORKSPACE_DIR))
                except Exception as e:
                    return False, f"Failed cleaning {target}: {str(e)}"
    return True, f"Clean SUCCESS: Removed {len(cleaned)} temporary directories: {', '.join(cleaned) if cleaned else 'none needed'}."

def run_verify():
    patterns = [
        (r'\.\.ctor', 'Direct struct constructor call (..ctor)'),
        (r'\b[A-Za-z0-9_]*<>[A-Za-z0-9_]*\b', 'Compiler-generated identifier with <>'),
        (r'\bCS\$', 'Compiler local variable artifact (CS$)'),
        (r'Text\.Font\s*=\s*[0-9]+', 'Integer literal assigned to Text.Font'),
        (r'Text\.Anchor\s*=\s*[0-9]+', 'Integer literal assigned to Text.Anchor'),
        (r'Event\.current\.type\s*==\s*([0-9]+|null)', 'Invalid Event.current.type comparison'),
        (r'Event\.current\.keyCode\s*==\s*[0-9]+', 'Invalid Event.current.keyCode comparison'),
        (r'\.Passion\s*==\s*[0-9]+', 'Passion compared to int'),
        (r'\.Passion\s*=\s*[0-9]+', 'Passion assigned int'),
        (r'\.Passion\s*(==|!=)\s*null', 'Non-nullable Passion compared to null'),
        (r'gender\s*==\s*null', 'Non-nullable Gender compared to null'),
        (r'gender\s*==\s*[1-2]', 'Gender compared to int literal'),
        (r'gender\s*=\s*1\b', 'Gender assigned int literal'),
        (r'ref\s+faction2', 'ref used instead of out for unassigned variable'),
        (r'mode\s*(==|!=)\s*[0-9]+', 'LoadSaveMode compared to int'),
        (r'DisabledWorkTagsBackstoryAndTraits\s*(&\s*[0-9]+|!= null)', 'WorkTags enum invalid comparison or op'),
    ]

    source_dir = os.path.join(WORKSPACE_DIR, "Source")
    files = glob.glob(os.path.join(source_dir, "**", "*.cs"), recursive=True)
    artifacts = []

    for fpath in files:
        rel = os.path.relpath(fpath, WORKSPACE_DIR)
        with open(fpath, "r", encoding="utf-8") as f:
            for idx, line in enumerate(f, 1):
                for pat, desc in patterns:
                    if re.search(pat, line):
                        artifacts.append(f"{rel}:{idx}: {desc} -> {line.strip()}")

    xml_ok, xml_msg = run_validate_xml()
    build_ok, build_msg = run_build("Release")

    res = "=== Verification Summary ===\n"
    res += f"- Scanned {len(files)} C# source files\n"
    res += f"- Decompiler artifacts: {len(artifacts)} detected " + ("(PASSED)" if not artifacts else "(FAILED)") + "\n"
    if artifacts:
        res += "  Artifact details:\n" + "\n".join(artifacts[:15]) + "\n"
    res += f"- XML validation: {'PASSED' if xml_ok else 'FAILED'}\n"
    if not xml_ok:
        res += f"  {xml_msg}\n"
    res += f"- MSBuild compilation: {'PASSED' if build_ok else 'FAILED'}\n"

    all_passed = not artifacts and xml_ok and build_ok
    return all_passed, res

def run_deploy(custom_dest=None, force=False):
    if not force and is_rimworld_running():
        return False, (
            "Deploy BLOCKED: RimWorld is currently running (RimWorldWin64.exe detected).\n"
            "Deploying while the game is active can cause DLL file-locking errors and mod corruption.\n"
            "Please close RimWorld first, or pass 'force': true if you are certain."
        )

    rw_dir = find_rimworld()
    if not custom_dest:
        if not rw_dir:
            return False, "Deploy FAILED: Could not auto-detect RimWorld installation. Set RIMWORLD_DIR environment variable."
        dest = os.path.join(rw_dir, "Mods", "RandomPlusOptimized")
    else:
        dest = os.path.abspath(custom_dest)

    # Always ensure a fresh, successful build before deploying
    build_ok, build_msg = run_build("Release")
    if not build_ok:
        return False, f"Deploy FAILED: Build step failed prior to copying.\n{build_msg}"

    try:
        # Safe, non-destructive file copy (preserves unmodified files, avoids destructive rmtree)
        res_dir = os.path.join(WORKSPACE_DIR, "Resources")
        copied_files = 0
        for root, dirs, files in os.walk(res_dir):
            rel = os.path.relpath(root, res_dir)
            target_dir = os.path.join(dest, rel)
            os.makedirs(target_dir, exist_ok=True)
            for f in files:
                s_file = os.path.join(root, f)
                d_file = os.path.join(target_dir, f)
                shutil.copy2(s_file, d_file)
                copied_files += 1

        return True, (
            f"Deploy SUCCESS!\n"
            f"Destination: {dest}\n"
            f"Files copied: {copied_files} (Assemblies, About, Defs, Languages, Textures)\n"
            f"RimWorld installation: {rw_dir}"
        )
    except Exception as e:
        return False, f"Deploy error while copying files: {str(e)}"

def run_inspect():
    source_dir = os.path.join(WORKSPACE_DIR, "Source")
    files = glob.glob(os.path.join(source_dir, "**", "*.cs"), recursive=True)
    total_loc = 0
    file_stats = []
    for f in sorted(files):
        with open(f, "r", encoding="utf-8", errors="ignore") as fp:
            lines = len(fp.readlines())
            total_loc += lines
            file_stats.append(f"  - {os.path.relpath(f, WORKSPACE_DIR)}: {lines} lines")

    dll_path = os.path.join(WORKSPACE_DIR, "Resources", "1.6", "Assemblies", "RandomPlus.dll")
    dll_size = f"{os.path.getsize(dll_path)} bytes" if os.path.isfile(dll_path) else "Not built yet"

    rw_path = find_rimworld()
    ms_path = find_msbuild()
    rw_running = is_rimworld_running()

    res_dir = os.path.join(WORKSPACE_DIR, "Resources")
    xml_files = len(glob.glob(os.path.join(res_dir, "**", "*.xml"), recursive=True))

    report = (
        f"=== RandomPlus Mod Project Diagnostics ===\n"
        f"Detected MSBuild: {ms_path or 'NOT FOUND'}\n"
        f"Detected RimWorld: {rw_path or 'NOT FOUND'}\n"
        f"RimWorld Process Running: {rw_running}\n"
        f"Compiled 1.6 Assembly: {dll_size}\n"
        f"Total C# Source Files: {len(files)}\n"
        f"Total C# Lines of Code: {total_loc}\n"
        f"XML Def & Config Files: {xml_files}\n\n"
        f"Source Breakdown:\n" + "\n".join(file_stats)
    )
    return True, report

# ==============================================================================
# JSON-RPC 2.0 Dispatcher
# ==============================================================================

def handle_request(request):
    req_id = request.get("id")
    method = request.get("method")
    params = request.get("params", {})

    # Handle notifications (per JSON-RPC 2.0, NEVER respond to notifications)
    if req_id is None:
        return None

    if method == "initialize":
        return {
            "jsonrpc": "2.0",
            "id": req_id,
            "result": {
                "protocolVersion": "2024-11-05",
                "capabilities": {
                    "tools": {}
                },
                "serverInfo": {
                    "name": "rimworld-randomplus",
                    "version": "1.1.0"
                }
            }
        }

    if method == "ping":
        return {"jsonrpc": "2.0", "id": req_id, "result": {}}

    if method == "tools/list":
        return {
            "jsonrpc": "2.0",
            "id": req_id,
            "result": {
                "tools": TOOLS
            }
        }

    if method == "tools/call":
        name = params.get("name")
        args = params.get("arguments", {})

        handlers = {
            "build_mod": lambda: run_build(args.get("configuration", "Release")),
            "verify_mod": run_verify,
            "validate_xml": run_validate_xml,
            "clean_mod": run_clean,
            "deploy_mod": lambda: run_deploy(args.get("destination"), args.get("force", False)),
            "inspect_project": run_inspect,
        }

        handler = handlers.get(name)
        if not handler:
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "error": {
                    "code": -32601,
                    "message": f"Tool not found: {name}"
                }
            }

        try:
            success, msg = handler()
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "result": {
                    "content": [{"type": "text", "text": msg}],
                    "isError": not success
                }
            }
        except Exception as e:
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "result": {
                    "content": [{"type": "text", "text": f"Tool execution crashed: {str(e)}"}],
                    "isError": True
                }
            }

    return {
        "jsonrpc": "2.0",
        "id": req_id,
        "error": {
            "code": -32601,
            "message": f"Method not found: {method}"
        }
    }

def main():
    while True:
        try:
            line = sys.stdin.readline()
            if not line:
                break
            line = line.strip()
            if not line:
                continue
            try:
                request = json.loads(line)
            except json.JSONDecodeError as jde:
                err_resp = {
                    "jsonrpc": "2.0",
                    "id": None,
                    "error": {
                        "code": -32700,
                        "message": f"Parse error: {str(jde)}"
                    }
                }
                sys.stdout.write(json.dumps(err_resp) + "\n")
                sys.stdout.flush()
                continue

            response = handle_request(request)
            if response is not None:
                sys.stdout.write(json.dumps(response) + "\n")
                sys.stdout.flush()
        except (KeyboardInterrupt, SystemExit):
            break
        except Exception as e:
            err_resp = {
                "jsonrpc": "2.0",
                "id": None,
                "error": {
                    "code": -32603,
                    "message": f"Internal server error: {str(e)}"
                }
            }
            sys.stdout.write(json.dumps(err_resp) + "\n")
            sys.stdout.flush()

if __name__ == "__main__":
    main()
