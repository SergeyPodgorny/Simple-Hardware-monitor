using System.Configuration;
using System.Data;
using System.Windows;

using TempsOverlay.Services;

namespace TempsOverlay;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private TrayIconService? _trayIconService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // The tray icon must not depend on MainWindow being constructed yet:
        // with StartupUri, the window is created after OnStartup returns.
        _trayIconService = new TrayIconService();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIconService?.Dispose();
        _trayIconService = null;

        base.OnExit(e);
    }
}
