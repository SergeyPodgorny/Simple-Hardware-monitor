# TempsOverlay

A Windows hardware monitoring overlay application that displays real-time system statistics including CPU, GPU, and storage temperatures, as well as network speed. The application features a transparent, always-on-top overlay window that can be toggled on/off via a system tray icon.

## Features

- **Real-time Hardware Monitoring**:
  - CPU temperature and frequency monitoring
  - GPU temperature monitoring (NVIDIA and AMD)
  - Storage device temperatures  
  - Network speed monitoring (download/upload in MB/s)

- **User Interface**:
  - Transparent, borderless overlay window that stays on top of other applications
  - Click-through functionality (doesn't intercept mouse clicks)
  - Positioned in the upper-right corner of the screen

- **System Integration**:
  - System tray icon with context menu for:
    - Show/Hide overlay toggle
    - Game Mode switch that restricts display to only CPU and GPU temperatures
    - Exit option for clean application shutdown
  - Close-to-tray functionality (window hides instead of closing)

## Requirements

- Windows OS with .NET 8.0 Runtime
- Administrator privileges for hardware monitoring access

## Installation

1. Download the latest release from the [releases page](#) (coming soon)
2. Run the installer or extract files to a folder
3. Launch `TempsOverlay.exe`

## Usage

1. **Starting the Application**:
   - Double-click the application icon or run `TempsOverlay.exe`
   - The application will start in the system tray with an icon

2. **Controlling the Overlay**:
   - Right-click the system tray icon to access context menu
   - Use "Show/Hide" to toggle the overlay window visibility
   - Enable "Game Mode" to restrict display to only CPU and GPU temperatures
   - Select "Exit" to close the application completely

3. **Overlay Behavior**:
   - The overlay stays on top of all applications
   - Click-through functionality allows interaction with underlying apps
   - Window can be resized and repositioned by dragging edges

## Technical Details

### Architecture

- Built using WPF (.NET 8.0)
- Hardware monitoring powered by LibreHardwareMonitorLib
- System tray integration using Hardcodet.NotifyIcon.Wpf
- Network monitoring via Windows Performance Counters

### Key Components

- `MainWindow.xaml` - Main overlay window with transparency and click-through behavior  
- `HardwareMonitorService.cs` - Collects hardware statistics using LibreHardwareMonitorLib
- `NetworkService.cs` - Monitors network traffic speeds using Windows Performance Counters
- `TrayIconService.cs` - Manages the system tray icon and menu interactions
- `StatsUiBuilder.cs` - Renders the statistics in the UI with conditional display logic

### Configuration

Settings are managed through `RuntimeSettings.cs`:
- Network speed visibility toggle
- Disk temperature visibility toggle  
- CPU frequency visibility toggle
- Game mode toggle

## Building from Source

1. Clone this repository
2. Open solution in Visual Studio or JetBrains Rider
3. Build the project using .NET 8.0 tools
4. Run `TempsOverlay.exe` to test

## Contributing

Contributions are welcome! Please open issues for bugs or feature requests, and submit pull requests with your changes.

## License

This project is licensed under the MIT License - see the LICENSE file for details.