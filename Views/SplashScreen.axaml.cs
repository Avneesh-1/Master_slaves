using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using MasterSlaves.Avalonia.ViewModels;

namespace MasterSlaves.Avalonia.Views;

public partial class SplashScreen : Window
{
    private DispatcherTimer? _timer;

    public SplashScreen()
    {
        InitializeComponent();
        
        // Completely remove window decorations
        this.CanResize = false;
        this.ShowInTaskbar = false;
        this.WindowState = WindowState.Normal;
        
        // Set window to be completely borderless
        this.ExtendClientAreaToDecorationsHint = true;
        this.ExtendClientAreaTitleBarHeightHint = -1;
        
        InitializeTimer();
        
        // Subscribe to window closing event
        this.Closing += OnWindowClosing;
    }

    private void InitializeTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private async void OnTimerTick(object? sender, EventArgs e)
    {
        _timer?.Stop();
        
        // Close splash screen and open main window
        await Task.Delay(500); // Small delay for smooth transition
        
        // Get stored services from App static properties
        if (App.MasterService != null)
        {
            var configurationService = new MasterSlaves.Core.Services.ConfigurationService();
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(App.MasterService, configurationService)
            };
            
            mainWindow.Show();
            this.Close();
        }
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        _timer?.Stop();
    }
} 