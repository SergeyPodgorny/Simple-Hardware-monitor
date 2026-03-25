using System.Collections.Generic;

namespace TempsOverlay.Models;

public class HardwareStats
{
    public CpuStats? Cpu { get; init; }
    public IReadOnlyList<GpuStats> Gpus { get; init; } = [];
    public IReadOnlyList<StorageStats> Storages { get; init; } = [];
    public NetworkStats? Network { get; init; }
}
