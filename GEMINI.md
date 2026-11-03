# RandomPlus Optimized - Development & Architecture Guide

## Overview
RandomPlus is a RimWorld 1.6 C# modification that adds an advanced pawn filter and reroll editor to the scenario starting pawn selection screen. It allows players to specify exact trait combinations, passion levels, skill minimums, age ranges, health conditions, and work capabilities, then rerolls pawns rapidly until a matching pawn is generated.

This repository (`RandomPlusOptimized`) contains the clean, modernized C# codebase optimized for minimal GC allocation, ultra-fast rerolls, and full RimWorld 1.6 compatibility.

---

## Directory Structure
```
RandomPlusOptimized/
├── .agents/                    # Workspace agent configurations
├── Libraries/                  # External dependencies
│   └── 0Harmony.dll            # Harmony 2.x library
├── Resources/                  # RimWorld Mod distribution assets
│   ├── About/                  # About.xml mod metadata and preview images
│   ├── Defs/                   # KeyBindings and other XML defs
│   ├── Languages/              # Localization files (English, etc.)
│   ├── Textures/               # UI textures
│   └── 1.6/Assemblies/         # Output assembly: RandomPlus.dll
├── Source/                     # C# Source code
│   ├── Data/                   # Data containers
│   │   ├── SkillContainer.cs   # Skill filter requirements
│   │   └── TraitContainer.cs   # Trait filter requirements
│   ├── EdB-UI/                 # UI components and panels
│   │   ├── Dialog_Options.cs
│   │   ├── PanelSkills.cs
│   │   ├── PanelTraits.cs
│   │   ├── Style.cs
│   │   └── ...
│   ├── HarmonyPatches.cs       # Harmony patch entry points
│   ├── Page_RandomEditor.cs    # Main UI modal window
│   ├── PawnFilter.cs           # Filter criteria and matching logic
│   ├── RandomSettings.cs       # Reroll loop and RimWorld reflection hooks
│   └── SaveLoader.cs           # Filter preset serialization
├── tools/                      # Development tooling
│   └── mcp_server.py           # Model Context Protocol stdio server
├── build.bat                   # 1-click batch build script
├── build.ps1                   # PowerShell build & deployment script
├── verify.py                   # Static analysis and verification suite
└── RandomPlus.csproj           # C# project file targeting .NET 4.7.2 / RimWorld 1.6
```

---

## Tooling & Fast Commands

### 1. Build
- **Command Prompt:** `build.bat`
- **PowerShell:** `powershell -ExecutionPolicy Bypass -File build.ps1`
- **Deploy to game:** `powershell -ExecutionPolicy Bypass -File build.ps1 -DeployToRimWorld`

### 2. Verify
- Run `python verify.py` to perform full static analysis (decompiler artifacts check, bad type casts, non-nullable checks) and verify clean MSBuild compilation.

### 3. Model Context Protocol (MCP) Server
- Stdio MCP Server: `tools/mcp_server.py`
- Exposes tools:
  - `build_mod` (Configuration: Release/Debug)
  - `verify_mod` (Static checks + MSBuild)
  - `deploy_mod` (Copies compiled mod directly to RimWorld's `Mods/` directory)
  - `inspect_project` (File counts, lines of code, binary sizes)

---

## Performance & Optimization Rules

1. **Zero-Allocation Hot Loops:**
   - The pawn reroll loop (`FastReroll` in `RandomSettings.cs`) can execute thousands of iterations per second.
   - **Never** use LINQ (`.Where()`, `.Any()`, `.Count()`, `.FirstOrDefault()`) in hot loops. Use standard `for (int i = 0; i < count; i++)`.
   - **Never** allocate closures (`delegate`, `x => x.def == target`) inside reroll or match evaluation loops.
   - Avoid creating `new List<...>` or `new StringBuilder` on each reroll attempt; reuse pre-allocated structures.

2. **Reflection Optimization:**
   - Any reflection on RimWorld private methods (e.g. `ExecuteMethodFast`, `FindPawnProperty`) must be resolved **once** and cached as a compiled delegate (`System.Delegate.CreateDelegate` or Expression Trees), not called via `MethodInfo.Invoke` on every reroll.

3. **Check Ordering:**
   - Filter checks should fail fast:
     1. Quick rejection: Gender, Age, Biological restrictions.
     2. Trait matching (direct def comparisons, no string operations).
     3. Skill and Passion matching (fast indexed array lookup).
     4. Health condition and work tag checks.
   - Defer expensive RimWorld generation steps (names, relations, backstories, apparels) until preliminary criteria pass.

4. **RimWorld Compatibility:**
   - Output assembly targets RimWorld 1.6 (`Resources/1.6/Assemblies/RandomPlus.dll`).
   - Maintain full compatibility with Harmony 2.3+ and the Steam release.
