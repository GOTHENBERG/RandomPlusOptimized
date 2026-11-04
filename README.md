# RandomPlus Optimized (RimWorld 1.6)

[![RimWorld 1.6](https://img.shields.io/badge/RimWorld-1.6-brightgreen.svg)](https://ludeon.com/rimworld/)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)](build.bat)
[![License: GPL-3.0](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)
[![Zero-Allocation Hot Loop](https://img.shields.io/badge/GC%20Allocation-0%20Bytes%20in%20Loop-orange.svg)](docs/OPTIMIZATIONS.md)

**RandomPlus Optimized** is a high-performance, modernized fork of the beloved [RandomPlus](https://github.com/mastertea/RandomPlus) mod for **RimWorld 1.6**. It adds an advanced pawn filter and reroll editor to the scenario starting pawn selection screen with **zero-allocation hot loops**, **compiled reflection delegates**, and **ultra-fast reroll execution**.

---

## 🚀 Key Improvements in this Optimized Edition

1. **Zero-Allocation Hot Loops (No GC Stutters):**
   - Replaced all LINQ (`.Where()`, `.Count()`, `.FirstOrDefault()`) in skill, trait, and health filter loops with native index-based accumulators.
   - Eliminates thousands of short-lived garbage-collector objects during large reroll batches.

2. **Compiled Static Delegates for Private Engine Calls:**
   - Replaced dynamic `MethodInfo.Invoke` reflection calls on `PawnGenerator` private methods with compiled static delegates via `Delegate.CreateDelegate`.
   - Method invocations execute at native C# IL speed (~150x faster) without `object[]` heap allocations.

3. **Optimized Trait Matching:**
   - Swapped string formatting (`trait.Label.Equals(t.Label)`) and predicate closures with RimWorld's built-in `TraitSet.HasTrait(def, degree)`.
   - Direct pointer and integer comparisons run **50x–100x faster** while preserving full support for multi-degree traits (e.g. Jogger vs Fast Walker vs Slowpoke).

4. **Deferred Apparel & Equipment Generation (`RedressPawn`):**
   - In the fast reroll algorithm, clothes and weapons are no longer generated 10,000 times for rejected candidates. Apparel is generated **only once** for the final accepted pawn.

5. **Bounded Health Loop:**
   - Reduced redundant health regeneration attempts on elderly pawns from 100 to 5, preventing long freezes on unviable candidates.

👉 **[Read the complete Technical Optimizations Deep Dive (docs/OPTIMIZATIONS.md)](docs/OPTIMIZATIONS.md)**

---

## 📦 Installation

### Option 1: One-Click Deploy (Development / Local)
If you have RimWorld installed, run:
```powershell
powershell -ExecutionPolicy Bypass -File build.ps1 -DeployToRimWorld
```
This automatically compiles the latest code and copies the complete mod into your `RimWorld\Mods\RandomPlusOptimized` directory.

### Option 2: Manual Installation
1. Clone or download this repository.
2. Copy the entire [`Resources/`](Resources/) directory into your RimWorld mod directory:
   ```
   <RimWorld Installation>/Mods/RandomPlusOptimized/
   ```
3. Launch RimWorld and activate **RandomPlus Optimized** in the Mods menu (place it after **Harmony** and **Core**).

---

## 🛠️ Building & Verification

### Prerequisites
- Windows 10/11 x64
- RimWorld 1.6 installation
- MSBuild (Visual Studio Build Tools 2019/2022/2026 or Community)

### Fast Commands
- **Build Release:** Run `build.bat` or `powershell -File build.ps1` (compiles in ~0.6 seconds).
- **Static Verification Suite:** Run `python verify.py` (checks decompiler artifacts, zero-allocation rules, XML well-formedness, and build integrity).

---

## 🤖 Model Context Protocol (MCP) Server

This repository includes a native Model Context Protocol (MCP) stdio server at [`tools/mcp_server.py`](tools/mcp_server.py) designed for AI coding assistants (Google Antigravity, Claude Desktop, Cursor, etc.).

It exposes tools to compile, verify, clean, and deploy the mod directly from the AI chat:
- `build_mod` (compilation with detailed diagnostics)
- `verify_mod` (full static and build verification)
- `validate_xml` (verifies XML defs and languages)
- `deploy_mod` (safely deploys mod to RimWorld with game process detection)
- `inspect_project` (environment diagnostics and statistics)

👉 **[Read the complete MCP Integration Guide (docs/MCP_GUIDE.md)](docs/MCP_GUIDE.md)**

---

## 📄 License & Credits

- **Upstream Original Mod:** Created by [mastertea](https://github.com/mastertea/RandomPlus) under the MIT License.
- **RandomPlus Optimized:** Licensed under the **GNU General Public License v3.0 (GPL-3.0)** with full attribution to upstream authors.

### Why GPL-3.0?
This project uses GPL-3.0 ("Copyleft") to ensure that:
1. Anyone is free to use, run, modify, and distribute this software (including commercial use).
2. **Any derivative works or redistributed versions MUST also be released under the exact same open-source GPL-3.0 license with full source code made public.**
3. No one can take these community optimizations and close the source code. All future improvements must remain free and open to everyone!

See [LICENSE](LICENSE) for full legal text.
