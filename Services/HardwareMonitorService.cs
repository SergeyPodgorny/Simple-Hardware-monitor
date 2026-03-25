using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using TempsOverlay.Models;

namespace TempsOverlay.Services;

public class HardwareMonitorService : IDisposable
{
    private readonly Computer _computer;

    public HardwareMonitorService()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsStorageEnabled = true
        };
        _computer.Open();
    }

    public HardwareStats GetStats()
    {
        var cpu = GetCpuStats();
        var gpus = GetGpuStats();
        var storages = GetStorageStats();

        return new HardwareStats
        {
            Cpu = cpu,
            Gpus = gpus,
            Storages = storages
        };
    }

    private CpuStats? GetCpuStats()
    {
        var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
        if (cpu == null)
            return null;

        cpu.Update();

        var temp = cpu.Sensors
            .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

        var clocks = cpu.Sensors
            .Where(s =>
                s.SensorType == SensorType.Clock &&
                s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) &&
                s.Value.HasValue)
            .Select(s => s.Value!.Value);

        return new CpuStats
        {
            Temperature = temp?.Value,
            AverageClock = clocks.Any() ? clocks.Average() : null
        };
    }

    private IReadOnlyList<GpuStats> GetGpuStats()
    {
        var gpuTypes = new[] { HardwareType.GpuAmd, HardwareType.GpuNvidia };
        var result = new List<GpuStats>();

        foreach (var gpu in _computer.Hardware.Where(h => gpuTypes.Contains(h.HardwareType)))
        {
            gpu.Update();

            var temp = gpu.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

            result.Add(new GpuStats
            {
                Name = gpu.HardwareType.ToString(),
                Temperature = temp?.Value
            });
        }

        return result;
    }

    private IReadOnlyList<StorageStats> GetStorageStats()
    {
        var result = new List<StorageStats>();

        foreach (var storage in _computer.Hardware.Where(h => h.HardwareType == HardwareType.Storage))
        {
            storage.Update();

            var temp = storage.Sensors
                .FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Value.HasValue);

            result.Add(new StorageStats
            {
                Name = storage.Name,
                Temperature = temp?.Value
            });
        }

        return result;
    }

    public void Dispose()
    {
        _computer.Close();
    }
}
