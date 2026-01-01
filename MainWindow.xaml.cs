using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace TempsOverlay
{
    public partial class MainWindow : Window
    {
        private readonly Computer _computer;
        private readonly DispatcherTimer _timer;

        private PerformanceCounter _netDown;
        private PerformanceCounter _netUp;

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

            // позиция (правый верх)
            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = 10;

            // железо
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsStorageEnabled = true
            };
            _computer.Open();

            InitNetwork();

            // таймер
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (_, _) => UpdateStats();
            _timer.Start();

            Loaded += (_, _) => EnableClickThrough();
        }

        // ================== UPDATE ==================

        private void UpdateStats()
        {
            Panel.Children.Clear();

            foreach (var hw in _computer.Hardware)
            {
                hw.Update();

                switch (hw.HardwareType)
                {
                    case HardwareType.Cpu:
                        AddCpu(hw);
                        break;

                    case HardwareType.GpuAmd:
                    case HardwareType.GpuNvidia:
                        AddTemp($"{hw.HardwareType}", hw);
                        break;

                    case HardwareType.Storage:
                        AddStorage(hw);
                        break;
                }
            }

            AddNetwork();
        }

        // ================== CPU ==================

        private void AddCpu(IHardware cpu)
        {
            var temp = cpu.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

            var clocks = cpu.Sensors
                            .Where(s =>
                            s.SensorType == SensorType.Clock &&
                            s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) &&
                            s.Value.HasValue)
                            .Select(s => s.Value.Value);

            AddLine($"CPU Temp: {temp?.Value:0} °C");

            if (clocks.Any())
                AddLine($"CPU Clock: {clocks.Average():0} MHz");
        }

        // ================== TEMP ==================

        private void AddTemp(string name, IHardware hw)
        {
            var temp = hw.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

            if (temp != null)
                AddLine($"{name}: {temp.Value:0} °C");
        }

        // ================== STORAGE ==================

        private void AddStorage(IHardware hw)
        {
            var temp = hw.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

            AddLine($"{hw.Name}: {temp?.Value:0} °C");
        }

        // ================== NETWORK ==================

        private void InitNetwork()
        {
            var cat = new PerformanceCounterCategory("Network Interface");
            var iface = cat.GetInstanceNames()
                .FirstOrDefault(n => !n.ToLower().Contains("loopback"));

            if (iface == null)
                return;

            _netDown = new PerformanceCounter("Network Interface", "Bytes Received/sec", iface);
            _netUp = new PerformanceCounter("Network Interface", "Bytes Sent/sec", iface);

            _netDown.NextValue();
            _netUp.NextValue();
        }

        private void AddNetwork()
        {
            if (_netDown == null)
                return;

            double down = _netDown.NextValue() / 1024 / 1024;
            double up = _netUp.NextValue() / 1024 / 1024;

            AddLine($"NET ↓ {down:0.00} MB/s ↑ {up:0.00} MB/s");
        }

        // ================== UI ==================

        private void AddLine(string text)
        {
            Panel.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 13
            });
        }

        // ================== CLICK-THROUGH ==================

        private void EnableClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);

            SetWindowLong(hwnd, GWL_EXSTYLE,
                style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
    }
}