# <img src="csRenamer.svg" alt="icon" width="50"/> csRenamer

**csRenamer** is a Windows desktop application built with WPF and .NET 8.0, designed for batch renaming files with ease. Inspired by [pyRenamer](https://github.com/tfree87/pyRenamer), it provides a flexible and user-friendly graphical interface for renaming files using patterns, substitutions, insertions, deletions, and more — all with instant preview.

## Features

csRenamer will support the following features:
- Keep or change file extensions during rename
- Rename files based on patterns (e.g., rename `1-a.txt` → `a-1.txt` using `{#}-{X}.txt`)
- Insert characters at specific positions
- Delete characters from specified positions
- Replace matching characters or sequences
- Convert accented characters to plain ones
- Change capitalization (UPPERCASE, lowercase, Title Case)
- Replace or remove dots, dashes, and spaces
- Remove duplicated characters or symbols
- Manually rename individual files
- Live preview of all filename changes before applying

## pyRenamer Compatibility

This project is a C#/.NET 8.0 clone of [pyRenamer](https://github.com/tfree87/pyRenamer), recreated using Windows-native technology to bring the same powerful renaming functionality to the Windows ecosystem with a modern UI.

## Requirements

- Windows 10 or later
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Optional: Visual Studio 2022+ for development

## Build and Run

To build and run csRenamer locally:

```bash
# Clone the repository
git clone https://github.com/yourusername/csRenamer.git
cd csRenamer

# Build the project
dotnet build

# Run the application
dotnet run --project csRenamer
```

Alternatively, you can open the solution in Visual Studio and press `F5` to run.

### Installer

For regular users, a pre-built Windows installer will be provided — no setup or compilation required. Just download, install, and start renaming!

> The installer will be available on the [Releases](https://github.com/yourusername/csRenamer/releases) page.

## About This Project

csRenamer is a personal reimplementation of the original **pyRenamer** application. It aims to maintain feature parity while taking advantage of the Windows Presentation Foundation (WPF) framework for a polished and responsive user experience. It’s built for users who want to perform complex batch renaming without relying on command-line tools.

## Future Plans

Some upcoming features under consideration:
- Music and image file renaming using metadata (ID3, EXIF, etc.)
- Drag-and-drop file support
- Regex-based renaming
- Dark mode and UI themes
- Multi-language UI (localization)
