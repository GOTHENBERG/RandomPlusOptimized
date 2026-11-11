# RandomPlus Optimized (RimWorld 1.6 & 1.7)

[![RimWorld 1.6 & 1.7](https://img.shields.io/badge/RimWorld-1.6%20%7C%201.7-brightgreen.svg)](https://ludeon.com/rimworld/)
[![Steam Workshop](https://img.shields.io/badge/Steam-Workshop-1b2838.svg?logo=steam)](https://steamcommunity.com/sharedfiles/filedetails/?id=3837613740)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)](build.bat)
[![License: GPL-3.0](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)
[![Zero-Allocation Hot Loop](https://img.shields.io/badge/GC%20Allocation-0%20Bytes%20in%20Loop-orange.svg)](docs/OPTIMIZATIONS.md)

<p align="center">
  <img src="Resources/About/Preview.png" alt="RandomPlus Optimized Preview" width="650"/>
</p>

**RandomPlus Optimized** is a high-performance, modernized fork of the beloved [RandomPlus](https://github.com/mastertea/RandomPlus) mod for **RimWorld 1.6 & 1.7 (Waylith Engine)**. It adds an advanced pawn filter and reroll editor to the scenario starting pawn selection screen with **zero-allocation hot loops**, **compiled reflection delegates**, **smart third-party mod compatibility**, and **non-blocking 60 FPS frame-sliced rerolls**.

---

## 🚀 Key Improvements in this Optimized Edition

1. **Zero-Allocation Hot Loops (No GC Stutters):**
   - Replaced all LINQ (`.Where()`, `.Count()`, `.FirstOrDefault()`) in skill, trait, and health filter loops with native index-based accumulators.
   - Eliminates thousands of short-lived garbage-collector objects during large reroll batches.

2. **Compiled Static Delegates for Private Engine Calls:**
   - Replaced dynamic `MethodInfo.Invoke` reflection calls on `PawnGenerator` private methods with compiled static delegates via `Delegate.CreateDelegate`.
   - Method invocations execute at native C# IL speed (~150x faster) without `object[]` heap allocations.

3. **Skipping Family Relations During Rerolls (`SkipGeneratePawnRelations`):**
   - Skips world-pawn relation searching during candidate iterations, cutting reroll time by ~30%–40% and generating authentic vanilla relations once for the chosen pawn.

4. **Dynamic Third-Party Mod Compatibility & Gear Suppression (`GenerationCompatibility`):**
   - Queries Harmony patches at runtime: if foreign mods (e.g. Alien Races, Combat Extended, Facial Animation) hook generation entry points, RandomPlus safely adapts to standard whole-pawn generation.
   - When gear generation is unpatched, clothing and weapons are suppressed for discarded candidates, doubling reroll speed on whole-pawn rerolls.

5. **60 FPS Non-Blocking Frame-Sliced Mode with Live HUD & Cancel Button:**
   - Distributes candidate generation across 8 ms per-frame time slices, maintaining smooth 60 FPS without freezing the window.
   - Live interactive search overlay displays current reroll count, candidate previews, and an instant "Stop" button.

6. **Deferred Apparel & Equipment Generation (`RedressPawn`):**
   - Apparel and weapons are generated **only once** for the final accepted pawn instead of on thousands of rejected candidates.

7. **Multi-Language Support (English & Russian):**
   - Complete native Russian localization out of the box (`Resources/Languages/Russian/Keyed/RandomPlus.xml`).

8. **Dual 1.6 & 1.7 Waylith Engine Support:**
   - Automated build pipeline compiles and deploys synchronized binaries for both RimWorld 1.6 and 1.7.

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
- **Community Contributions & Architecture Inspiration:** Special thanks to open-source community contributors whose research and prototypes inspired parts of this modernization:
  - [TrevorS](https://github.com/TrevorS/RandomPlusPlus) — Dynamic Harmony patch inspection for non-breaking mod compatibility and frame steadiness concepts.
  - [ppuya13](https://github.com/ppuya13/FasterRandomPlus) — Insight on skipping redundant world-pawn relation generation during rerolls.
- **RandomPlus Optimized:** Developed and maintained by **GOTHENBERG**, licensed under the **GNU General Public License v3.0 (GPL-3.0)** with full attribution to upstream authors.

### Why GPL-3.0?
This project uses GPL-3.0 ("Copyleft") to ensure that:
1. Anyone is free to use, run, modify, and distribute this software (including commercial use).
2. **Any derivative works or redistributed versions MUST also be released under the exact same open-source GPL-3.0 license with full source code made public.**
3. No one can take these community optimizations and close the source code. All future improvements must remain free and open to everyone!

See [LICENSE](LICENSE) for full legal text.
