# Changelog

## [0.11.0] - 2026-02-22

### Added

- Added APIs in `Microsoft.Windows.Storage.Pickers.Projection`
- Added `sbyte`, `short`, `ulong` and `decimal` types as supported RPC values

### Changed

- Updated WindowsAppSDK to 1.8.260209005

### Fixed

- Fixed an issue where interface implementations inside generic arguments could not be instantiated

## [0.10.0] - 2026-02-15

### Added

- Added all controls by using source generator

### Changed

- Changed namespaces of WinUIShell types to follow WindowsAppSDK namespaces

## [0.9.0] - 2025-09-16

### Added

- Added `FontFamily`, `FontStretch`, `FontStyle` and `FontWeight` properties to `Control`
- Added `Focus` method to `UIElement`

## [0.8.0] - 2025-09-15

### Added

- Added `ComboBox`
- Added `AddKeyDown` method to `UIElement`
- Added `AddActivated`, `AddSizeChanged` and `AddVisibilityChanged` methods to `Window`

## [0.7.0] - 2025-09-14

### Added

- Added `ListView`
- Added `ListBox`
- Added `GridView`
- Added `ImageIcon`
- Added `BitmapImage`

## [0.6.0] - 2025-09-07

### Added

- Added `RunspaceMode` property to `EventCallback` class
- Added `Set-WUIRunspacePoolOption` function

### Changed

- `*EventArgs` classes now inherit from `WinUIShellObject` which has `Id` property
- Changed the constructor of `Page` class from internal to public

### Fixed

- Fixed a bug where event callbacks did not fire sometimes on the PowerShell Extension terminal in VSCode

## [0.5.0] - 2025-07-26

### Added

- Added `[XamlReader]::Load()`
- Added `FindName` method to `FrameworkElement`
- Added `Name` and `RequestedTheme` properties to `FrameworkElement`
- Added IEnumerator implementation to `WinUIShellObjectList`
- Added `MenuFlyout`, `MenuFlyoutItem`
- Added `Flyout` property to `Button`
- Added `FullScreenPreseenter` and `OverlappedPresenter`

### Changed

- Removed `Resource`

## [0.4.0] - 2025-06-20

### Changed

- `WinUIShell.Server.exe` is now a self-contained app

## [0.3.0] - 2025-06-14

### Added

- Added `NavigationView`
- Added `Frame`
- Added `Page`
- Added `Tag` property to `FrameworkElement`

## [0.2.0] - 2025-04-15

### Added

- Added basic properties to `TextBlock`
- Added basic properties to `TextBox`
- Added `ScrollView`
- Added `Brush` resource

## [0.1.0] - 2025-04-07

### Added

- Added `CornerRadius`
- Added `ToggleSwitch`
- Added `ProgressRing`

### Fixed

- Fixed the wrong sender parameter of event callbacks

## [0.0.1] - 2025-04-01

### Added

- Initial release
