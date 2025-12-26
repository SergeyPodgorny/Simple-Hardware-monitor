using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LibreHardwareMonitor.Hardware;

namespace TempsOverlay
{
    public partial class MainWindow : Window
    {
        private readonly Computer _computer;
        private readonly DispatcherTimer _timer;

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

            // таймер обновления
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (_, _) => UpdateTemps();
            _timer.Start();

            MouseEnter += (_, _) => Opacity = 0.15;
            MouseLeave += (_, _) => Opacity = 0.75;
        }

        private void UpdateTemps()
        {
            TempPanel.Children.Clear();

            foreach (var hw in _computer.Hardware)
            {
                hw.Update();

                var temp = hw.Sensors
                    .FirstOrDefault(s =>
                        s.SensorType == SensorType.Temperature &&
                        s.Value.HasValue);

                if (temp != null)
                {
                    TempPanel.Children.Add(new System.Windows.Controls.TextBlock
                    {
                        Text = $"{hw.HardwareType}: {temp.Value:0} °C",
                        Foreground = System.Windows.Media.Brushes.White,
                        FontSize = 14
                    });
                }
            }
        }
    }
}