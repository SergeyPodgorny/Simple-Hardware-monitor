namespace TempsOverlay
{
    public static class RuntimeSettings
    {
        /// <summary>Whether network speed is shown in the overlay.</summary>
        public static bool ShowNetworkSpeed { get; set; } = true;
        /// <summary>Whether disk (storage) temperatures are shown.</summary>
        public static bool ShowDiskTemp { get; set; } = true;
        public static bool ShowCpuFrequency { get; set; } = true;
        }
        }