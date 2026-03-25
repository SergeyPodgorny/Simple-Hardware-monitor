using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TempsOverlay.Models;

namespace TempsOverlay.UI;

public static class StatsUiBuilder
{
    public static void RenderStats(StackPanel panel, HardwareStats stats)
    {
        panel.Children.Clear();

        if (stats.Cpu != null)
        {
            AddCpu(panel, stats.Cpu);
        }

        foreach (var gpu in stats.Gpus)
        {
            AddGpu(panel, gpu);
        }

        foreach (var storage in stats.Storages)
        {
            AddStorage(panel, storage);
        }

        if (stats.Network != null)
        {
            AddNetwork(panel, stats.Network);
        }
    }

    private static void AddCpu(StackPanel panel, CpuStats cpu)
    {
        if (cpu.Temperature.HasValue)
            AddLine(panel, $"CPU Temp: {cpu.Temperature.Value:0} °C");

        if (cpu.AverageClock.HasValue)
            AddLine(panel, $"CPU Clock: {cpu.AverageClock.Value:0} MHz");
    }

    private static void AddGpu(StackPanel panel, GpuStats gpu)
    {
        if (gpu.Temperature.HasValue)
            AddLine(panel, $"{gpu.Name}: {gpu.Temperature.Value:0} °C");
    }

    private static void AddStorage(StackPanel panel, StorageStats storage)
    {
        AddLine(panel, $"{storage.Name}: {storage.Temperature:0} °C");
    }

    private static void AddNetwork(StackPanel panel, NetworkStats network)
    {
        AddLine(panel, $"NET ↓ {network.DownloadSpeedMbps:0.00} MB/s ↑ {network.UploadSpeedMbps:0.00} MB/s");
    }

    private static void AddLine(StackPanel panel, string text)
    {
        panel.Children.Add(new TextBlock
        {
            Text = text,
            Foreground = Brushes.White,
            FontSize = 13
        });
    }
}
