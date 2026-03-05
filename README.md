# FrostedTXT

FrostedTXT is a lean WPF (.NET) MVVM text editor focused on three goals:

- Crisp frosted visuals with tint-based translucency.
- Always-on draft autosave to prevent data loss.
- Safe file persistence with atomic writes.

## Features

- Multi-tab editor (`DocumentTabViewModel` per tab).
- Top-right `...` dropdown for New/Open/Save/Save As/Settings/About/Exit.
- Live settings for font, zoom, blur mode, translucency, and word wrap.
- Debounced autosave per tab (`AutoSaveService`, default 500 ms).
- Crash recovery from draft files (`DraftRecoveryService`).
- Session restore of open tabs and selected tab (`TabSessionService`).
- Atomic save for document and JSON writes (`AtomicFileWriter`).
- Blur fallback strategy (`WindowEffectsService`): DWM system backdrop first, User32 accent fallback, then no-blur safe behavior.

## Repository Layout

```text
/FrostedTXT
  LICENSE
  README.md
  .gitignore
  /src/FrostedTXT.App
  /tests/FrostedTXT.Tests
```

## Storage Paths

FrostedTXT stores app data under:

`%LocalAppData%\FrostedTXT\`

- `settings\settings.json`
- `session.json`
- `drafts\<DraftId>.txt`
- `drafts\<DraftId>.meta.json`
- `logs\app.log` (optional folder reserved)

## Autosave + Crash Recovery

- Every tab has a stable `DraftId` (`Guid`).
- On text changes, autosave is debounced and writes:
  - Draft text file (`.txt`)
  - Draft metadata file (`.meta.json`)
- Draft writes are independent from Save/Save As.
- On startup, draft metadata and content are scanned and restored into tabs.

## Atomic Write Behavior

All persistent writes use temp-and-replace flow:

1. Write to `target.tmp`
2. Replace destination atomically (`File.Replace`) when destination exists
3. Optional `.bak` backup for selected writes

This is used for:

- Real file saves (`DocumentService`)
- JSON state writes (`JsonFileStore`, including settings/session/draft meta)

## Blur and Translucency Behavior

- Text remains fully opaque.
- Translucency uses a tint overlay alpha (`BackgroundOpacity`) and never `Window.Opacity`.
- Backdrop blur is applied by interop:
  - Prefer DWM system backdrop when available.
  - Fallback to User32 accent blur/acrylic-like policy.
  - If unsupported/failing, app stays functional without blur.

## Build and Run

## Requirements

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 (or newer) with WPF workload, or `dotnet` CLI

### CLI

```powershell
cd src/FrostedTXT.App
dotnet restore
dotnet build
dotnet run
```

### Visual Studio

1. Open `src/FrostedTXT.App/FrostedTXT.App.csproj`.
2. Set as startup project.
3. Run (`F5`).

## Notes

- The app is intentionally lightweight MVVM without heavy external frameworks.
- IO responsibilities are isolated in Services/Infrastructure; Views and XAML stay UI-focused.
