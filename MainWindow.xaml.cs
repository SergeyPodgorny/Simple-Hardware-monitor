using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using TempsOverlay.Services;
using TempsOverlay.UI;

using TempsOverlay;


namespace TempsOverlay;

public partial class MainWindow : Window
{
    private readonly HardwareMonitorService _hardwareMonitorService;
    private readonly NetworkService _networkService;
    private readonly TrayService _trayService;
    private readonly DispatcherTimer _timer;

    // WinAPI click-through
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_LAYERED = 0x80000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public MainWindow()
    {
        InitializeComponent();

        // Позиция (правый верхний угол)
        Left = SystemParameters.WorkArea.Width - Width - 10;
        Top = 10;

        // Сервисы
        _hardwareMonitorService = new HardwareMonitorService();
        _networkService = new NetworkService();
        _networkService.Initialize();
        _trayService = new TrayService();
        _trayService.Initialize();

        // Таймер обновления
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (_, _) => UpdateStats();
        _timer.Start();

        Loaded += (_, _) => EnableClickThrough();
    }

    private void UpdateStats()
    {
        var hardwareStats = _hardwareMonitorService.GetStats();
        var networkStats = _networkService.GetStats();

        // Объединяем статистику сети с аппаратной
                var combinedStats = new Models.HardwareStats
                {
                    Cpu = hardwareStats.Cpu,
                    Gpus = hardwareStats.Gpus,
                    Storages = hardwareStats.Storages,
                    Network = RuntimeSettings.ShowNetworkSpeed ? networkStats : null
                };


        StatsUiBuilder.RenderStats(Panel, combinedStats);
    }

    private void EnableClickThrough()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        int style = GetWindowLong(hwnd, GWL_EXSTYLE);

        SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        _hardwareMonitorService.Dispose();
        _networkService.Dispose();
        _trayService.Dispose();
        base.OnClosed(e);
    }
}
