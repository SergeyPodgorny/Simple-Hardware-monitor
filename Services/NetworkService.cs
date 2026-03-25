using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TempsOverlay.Models;

namespace TempsOverlay.Services;

public class NetworkService : IDisposable
{
    private PerformanceCounter? _netDown;
    private PerformanceCounter? _netUp;
    private bool _initialized;

    public void Initialize()
    {
        if (_initialized)
            return;

        try
        {
            var cat = new PerformanceCounterCategory("Network Interface");
            var iface = cat.GetInstanceNames()
                .FirstOrDefault(n => !n.ToLower().Contains("loopback"));

            if (iface == null)
                return;

            _netDown = new PerformanceCounter("Network Interface", "Bytes Received/sec", iface);
            _netUp = new PerformanceCounter("Network Interface", "Bytes Sent/sec", iface);

            // Первый вызов для сброса буфера
            _netDown.NextValue();
            _netUp.NextValue();

            _initialized = true;
        }
        catch
        {
            // Игнорируем ошибки инициализации сети
        }
    }

    public NetworkStats? GetStats()
    {
        if (!_initialized || _netDown == null || _netUp == null)
            return null;

        double down = _netDown.NextValue() / 1024 / 1024;
        double up = _netUp.NextValue() / 1024 / 1024;

        return new NetworkStats
        {
            DownloadSpeedMbps = down,
            UploadSpeedMbps = up
        };
    }

    public void Dispose()
    {
        _netDown?.Dispose();
        _netUp?.Dispose();
    }
}
