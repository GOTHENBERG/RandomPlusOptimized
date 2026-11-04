# RandomPlus Optimized: MCP Server Integration Guide

This repository includes a native, zero-dependency **Model Context Protocol (MCP)** stdio server located at [`tools/mcp_server.py`](../tools/mcp_server.py).

It provides AI development assistants (Google Antigravity, Claude Desktop, Cursor, etc.) with custom tools to build, verify, and deploy RandomPlus directly into RimWorld without leaving the conversation.

---

## Available MCP Tools

| Tool | Description | Parameters |
| :--- | :--- | :--- |
| `build_mod` | Compiles RandomPlus using MSBuild into `Resources/1.6/Assemblies/RandomPlus.dll`. | `configuration`: `"Release"` (default) or `"Debug"` |
| `verify_mod` | Runs static decompiler artifact checks, zero-allocation rules, XML validation, and test compilation. | *None* |
| `validate_xml` | Validates syntactic well-formedness of all XML defs, languages, and `About.xml`. | *None* |
| `clean_mod` | Removes temporary build folders (`bin/`, `obj/`). | *None* |
| `deploy_mod` | Safely compiles and copies the complete mod into RimWorld's local `Mods/RandomPlusOptimized` directory for instant in-game testing. | `destination` *(optional)*, `force` *(optional bool)* |
| `inspect_project` | Returns diagnostic info: detected MSBuild path, RimWorld installation path, game process status, and lines of code breakdown. | *None* |

---

## 1. Connecting to Google Antigravity

In Antigravity, MCP servers are registered in the global configuration file:
`~/.gemini/config/mcp_config.json` (on Windows: `C:\Users\<YourUsername>\.gemini\config\mcp_config.json`).

Add the server entry under `mcpServers`:

```json
{
  "mcpServers": {
    "rimworld-randomplus": {
      "command": "python",
      "args": [
        "c:/Users/arman/Downloads/RandomPlusOptimized/tools/mcp_server.py"
      ]
    }
  }
}
```

Once saved, Antigravity automatically discovers the server and loads all tools.

---

## 2. Connecting to Claude Desktop

In Claude Desktop, open your configuration file:
- **Windows:** `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`

Add the server:

```json
{
  "mcpServers": {
    "rimworld-randomplus": {
      "command": "python",
      "args": [
        "C:\\Users\\<YourUsername>\\Downloads\\RandomPlusOptimized\\tools\\mcp_server.py"
      ]
    }
  }
}
```

Restart Claude Desktop, and the hammer icon will display the RandomPlus tools.

---

## 3. Connecting to Cursor / Windsurf

In Cursor or Windsurf settings under **Features > MCP Servers**:
- **Name:** `rimworld-randomplus`
- **Type:** `command`
- **Command:** `python C:\Users\<YourUsername>\Downloads\RandomPlusOptimized\tools\mcp_server.py`

---

## Technical Highlights of the MCP Server

- **Zero External Dependencies:** Built entirely with Python 3's standard library (`sys`, `os`, `json`, `subprocess`, `shutil`, `xml.etree`).
- **Multi-Tier Auto-Detection:** Automatically locates MSBuild via Microsoft's `vswhere.exe`, registry, and Visual Studio paths. Automatically locates RimWorld across all Steam libraries by parsing `libraryfolders.vdf`.
- **Locking & Crash Protection:** Detects if `RimWorldWin64.exe` is currently running before deploying to prevent DLL file-locking errors.
- **UTF-8 Stdio Enforcement:** Forces standard I/O into UTF-8 on Windows to eliminate character encoding crashes.
