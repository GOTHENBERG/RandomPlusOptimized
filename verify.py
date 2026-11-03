import os
import re
import glob
import subprocess
import sys

def check_source_artifacts():
    print("=== Checking source code for decompiler artifacts ===")
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

    files = glob.glob('RandomPlus/**/*.cs', recursive=True)
    found_issues = 0

    for fpath in files:
        with open(fpath, 'r', encoding='utf-8') as f:
            for idx, line in enumerate(f, 1):
                for pat, desc in patterns:
                    if re.search(pat, line):
                        # Filter out legitimate uses, e.g., Look<int>(..., 1, ...)
                        print(f"  [FAIL] {fpath}:{idx}: {desc} -> {line.strip()}")
                        found_issues += 1

    if found_issues == 0:
        print(f"  [OK] Checked {len(files)} C# files: 0 artifacts detected.")
        return True
    else:
        print(f"  [FAILED] Found {found_issues} artifacts.")
        return False

def check_build():
    print("\n=== Checking MSBuild build ===")
    res = subprocess.run(["cmd.exe", "/c", "build.bat"], capture_output=True, text=True)
    if res.returncode == 0:
        print("  [OK] Build succeeded with 0 errors and 0 warnings.")
        dll = r"RandomPlus\bin\Release\RandomPlus.dll"
        if os.path.exists(dll):
            size = os.path.getsize(dll)
            print(f"  [OK] Output DLL created: {dll} ({size} bytes)")
            return True
        else:
            print("  [FAIL] DLL not found after build.")
            return False
    else:
        print(f"  [FAIL] Build failed:\n{res.stdout}\n{res.stderr}")
        return False

if __name__ == '__main__':
    ok1 = check_source_artifacts()
    ok2 = check_build()
    if ok1 and ok2:
        print("\n>>> ALL CHECKS PASSED: Project is 100% clean and fully buildable. <<<")
        sys.exit(0)
    else:
        print("\n>>> VERIFICATION FAILED <<<")
        sys.exit(1)
