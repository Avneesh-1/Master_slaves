using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MasterSlaves.Avalonia.ViewModels;
using MasterSlaves.Avalonia.Views;
using MasterSlaves.Core.Services;
using MasterSlaves.Communication.Services;

namespace MasterSlaves.Avalonia;

public partial class App : Application
{
    // Static properties to store services
    public static HardwareCommunicationService? HardwareService { get; private set; }
    public static MasterService? MasterService { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            // Register services
            HardwareService = new HardwareCommunicationService();
            MasterService = new MasterService(HardwareService);
            
            // Start with splash screen
            desktop.MainWindow = new SplashScreen();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}