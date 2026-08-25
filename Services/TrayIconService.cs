using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;

namespace TempsOverlay.Services;

public sealed class TrayIconService : IDisposable
{
    private const string AppName = "TempsOverlay";

    private readonly TaskbarIcon _icon;
    private readonly MenuItem _toggleItem;
    private readonly MenuItem _gameModeItem;
    private bool _disposed;

    public TrayIconService()
    {
        _toggleItem = new MenuItem { Header = "Show overlay" };
        _toggleItem.Click += (_, _) => ToggleOverlay();

        _gameModeItem = new MenuItem
        {
            Header = "Game Mode",
            IsCheckable = true,
            IsChecked = RuntimeSettings.GameMode
        };
        _gameModeItem.Click += (_, _) => RuntimeSettings.GameMode = _gameModeItem.IsChecked;

        var exitItem = new MenuItem { Header = "Exit" };
        exitItem.Click += (_, _) => Exit();

        var menu = new ContextMenu();
        menu.Items.Add(_toggleItem);
        menu.Items.Add(_gameModeItem);
        menu.Items.Add(new Separator());
        menu.Items.Add(exitItem);
        // Labels/check states must reflect the current state every time the menu opens,
        // e.g. after the window was closed via Alt+F4 instead of the tray toggle.
        menu.Opened += (_, _) => RefreshMenuState();

        _icon = new TaskbarIcon
        {
            ToolTipText = AppName,
            Icon = ExtractIconFromResource(),
            ContextMenu = menu
        };

        RefreshMenuState();
    }

    private Icon ExtractIconFromResource()
    {
        // Load icon from pc.ico file in the 'icon' directory
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string iconPath = Path.Combine(currentDirectory, "icon", "pc.ico");

        if (File.Exists(iconPath))
        {
            try
            {
                return new Icon(iconPath);
            }
            catch (ArgumentException)
            {
                // If that fails, we'll use the system icon instead
                return SystemIcons.Application;
            }
        }

        // Fallback to system application icon if pc.ico is not found or can't be loaded
        return SystemIcons.Application;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _icon.Dispose();
    }

    private void ToggleOverlay()
    {
        var window = MainWindow.Instance;
        if (window == null) return;

        if (window.IsOverlayVisible)
            window.HideOverlay();
        else
            window.ShowOverlay();

        RefreshMenuState();
    }

    private void RefreshMenuState()
    {
        var visible = MainWindow.Instance?.IsOverlayVisible ?? false;
        _toggleItem.Header = visible ? "Hide overlay" : "Show overlay";
        _gameModeItem.IsChecked = RuntimeSettings.GameMode;
    }

    private void Exit()
    {
        MainWindow.Instance?.PrepareExit();
        Dispose();
        Application.Current.Shutdown();
    }
}
