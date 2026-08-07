# AvaloniaIDE
A simple, AOT-friendly .NET IDE built with Avalonia.

AvaloniaIDE supports features such as:

- Multi-document editing (windows/tabs)
- Code/text editor with syntax highlighting (via AvaloniaEdit.TextMate)
- File tree and file/node navigation
- Dockable layouts (via Dock.Model.Avalonia)
- MVVM architecture (CommunityToolkit.Mvvm)
- AOT-friendly publish configuration for smaller native executables
- Development hot-reload and diagnostics support (Debug configuration)

> But most of other editing skills are not served currently.
## Stack
- Language(s): C#
- Framework / runtime: .NET 10.0
- UI framework: [Avalonia 12.x](https://github.com/AvaloniaUI/Avalonia)
- Thanks to following libraries:
  - [AvaloniaEdit.TextMate](https://www.nuget.org/packages/AvaloniaEdit.TextMate) — TextMate grammars for editor syntax highlighting
  - [Avalonia.Themes.Fluent](https://www.nuget.org/packages/Avalonia.Themes.Fluent)
  - [Dock.Model.Avalonia](https://www.nuget.org/packages/Dock.Model.Avalonia) — docking/layout model for Avalonia
  - [DialogHost.Avalonia](https://www.nuget.org/packages/DialogHost.Avalonia) — dialog host implementation
  - [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) — MVVM helpers
  - [Jab](https://www.nuget.org/packages/Jab) — lightweight DI / utilities
  - [Xaml.Behaviors.Avalonia](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia)
  - [Dock.Avalonia.Themes.Fluent](https://www.nuget.org/packages/Dock.Avalonia.Themes.Fluent)
  - [Xaml.Behaviors.SourceGenerators](https://www.nuget.org/packages/Xaml.Behaviors.SourceGenerators) (private assets,SourceGenerators,AOT-friendly)
  - Debug-only: [HotAvalonia](https://www.nuget.org/packages/HotAvalonia), [AvaloniaUI.DiagnosticsSupport](https://www.nuget.org/packages/AvaloniaUI.DiagnosticsSupport), [Avalonia.Markup.Xaml.Loader](https://www.nuget.org/packages/Avalonia.Markup.Xaml.Loader)

## How it fits together
Program.cs boots Avalonia and loads App.axaml. MainWindow is the primary UI, Views bind to ViewModels in the ViewModels folder (MVVM). 

The editor component uses AvaloniaEdit.TextMate for syntax highlighting;.

Services provide window management and app-wide services.

## How to run it

Prerequisites:
- [.NET 10 SDK](https://get.dot.net/10) installed (dotnet CLI available)
- Recommended: Visual Studio / Rider / VS Code with C# tooling for development

Commands:

```bash
# Clone the repo and enter
git clone https://github.com/Qianyiaz/AvaloniaIDE.git
cd AvaloniaIDE

# Restore and build (Debug)
dotnet restore
dotnet build

# Run (default Debug)
dotnet run --project AvaloniaIDE

# Replace <RID> with target runtime identifier, e.g. win-x64
dotnet publish AvaloniaIDE -c Release -r <RID>
```
