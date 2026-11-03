#!/usr/bin/env python3
"""
Model Context Protocol (MCP) stdio server for RandomPlus RimWorld Mod development.
Zero-dependency, pure Python standard library implementation of MCP JSON-RPC 2.0.
"""

import sys
import os
import json
import subprocess
import glob
import re

WORKSPACE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
MSBUILD_PATH = r"C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
MODS_DIR = r"C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RandomPlusOptimized"

TOOLS = [
    {
        "name": "build_mod",
        "description": "Builds the RandomPlus RimWorld mod using MSBuild. Returns build status, output binary size, and any compiler warnings or errors.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "configuration": {
                    "type": "string",
                    "enum": ["Release", "Debug"],
                    "default": "Release",
                    "description": "Build configuration"
                }
            }
        }
    },
    {
        "name": "verify_mod",
        "description": "Runs static code analysis, decompiler artifact detection, and build checks on all C# source files.",
        "inputSchema": {
            "type": "object",
            "properties": {}
        }
    },
    {
        "name": "deploy_mod",
        "description": "Compiles and deploys the complete RandomPlus mod (assemblies, defs, textures, languages, about.xml) directly to RimWorld's local Mods directory for immediate in-game testing.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "destination": {
                    "type": "string",
                    "description": "Optional custom RimWorld Mods directory path."
                }
            }
        }
    },
    {
        "name": "inspect_project",
        "description": "Returns comprehensive statistics about the mod: source file counts, lines of code, binary sizes, git status, and RimWorld installation status.",
        "inputSchema": {
            "type": "object",
            "properties": {}
        }
    }
]

def run_build(configuration="Release"):
    sln_path = os.path.join(WORKSPACE_DIR, "RandomPlus.sln")
    cmd = [MSBUILD_PATH, sln_path, f"/p:Configuration={configuration}", "/v:minimal", "/nologo"]
    try:
        proc = subprocess.run(cmd, cwd=WORKSPACE_DIR, capture_output=True, text=True, timeout=30)
        output = proc.stdout.strip()
        if proc.stderr:
            output += "\n" + proc.stderr.strip()
        dll_path = os.path.join(WORKSPACE_DIR, "Resources", "1.6", "Assemblies", "RandomPlus.dll")
        if proc.returncode == 0 and os.path.exists(dll_path):
            size = os.path.getsize(dll_path)
            return True, f"Build SUCCESS ({configuration})\nDLL: {dll_path} ({size} bytes)\n\nMSBuild Output:\n{output}"
        else:
            return False, f"Build FAILED (Exit Code: {proc.returncode})\n\nOutput:\n{output}"
    except Exception as e:
        return False, f"Build execution error: {str(e)}"

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

    build_ok, build_msg = run_build("Release")

    res = f"Verification Summary:\n"
    res += f"- Scanned {len(files)} C# files\n"
    if artifacts:
        res += f"- Artifacts detected: {len(artifacts)}\n" + "\n".join(artifacts[:20]) + "\n"
    else:
        res += f"- Decompiler / syntax artifacts: 0 (PASSED)\n"
    res += f"- MSBuild check: {'PASSED' if build_ok else 'FAILED'}\n"
    return artifacts == [] and build_ok, res

def run_deploy(custom_dest=None):
    dest = custom_dest or MODS_DIR
    build_ok, build_msg = run_build("Release")
    if not build_ok:
        return False, f"Deploy failed: Build did not succeed.\n{build_msg}"

    try:
        import shutil
        os.makedirs(dest, exist_ok=True)
        res_dir = os.path.join(WORKSPACE_DIR, "Resources")
        for item in os.listdir(res_dir):
            s = os.path.join(res_dir, item)
            d = os.path.join(dest, item)
            if os.path.isdir(s):
                if os.path.exists(d):
                    shutil.rmtree(d)
                shutil.copytree(s, d)
            else:
                shutil.copy2(s, d)
        return True, f"Deploy SUCCESS!\nMod deployed to: {dest}\nAssemblies, About, Defs, Languages, and Textures copied."
    except Exception as e:
        return False, f"Deploy error: {str(e)}"

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
    dll_size = os.path.getsize(dll_path) if os.path.exists(dll_path) else "Not built"

    rimworld_exists = os.path.exists(r"C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64.exe")

    report = (
        f"=== RandomPlus Mod Status ===\n"
        f"Source files: {len(files)}\n"
        f"Total C# Lines of Code: {total_loc}\n"
        f"Compiled 1.6 DLL size: {dll_size} bytes\n"
        f"RimWorld 1.6 Installation detected: {rimworld_exists}\n\n"
        f"Files breakdown:\n" + "\n".join(file_stats)
    )
    return True, report

def handle_request(request):
    req_id = request.get("id")
    method = request.get("method")
    params = request.get("params", {})

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
                    "version": "1.0.0"
                }
            }
        }

    if method == "notifications/initialized":
        return None

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

        if name == "build_mod":
            cfg = args.get("configuration", "Release")
            success, msg = run_build(cfg)
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "result": {
                    "content": [{"type": "text", "text": msg}],
                    "isError": not success
                }
            }
        elif name == "verify_mod":
            success, msg = run_verify()
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "result": {
                    "content": [{"type": "text", "text": msg}],
                    "isError": not success
                }
            }
        elif name == "deploy_mod":
            dest = args.get("destination")
            success, msg = run_deploy(dest)
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "result": {
                    "content": [{"type": "text", "text": msg}],
                    "isError": not success
                }
            }
        elif name == "inspect_project":
            success, msg = run_inspect()
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "result": {
                    "content": [{"type": "text", "text": msg}],
                    "isError": not success
                }
            }
        else:
            return {
                "jsonrpc": "2.0",
                "id": req_id,
                "error": {
                    "code": -32601,
                    "message": f"Tool not found: {name}"
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
            request = json.loads(line)
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
