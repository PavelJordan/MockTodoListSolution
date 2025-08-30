using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaToDoListTrackerAndVisualizer.ViewModels;
using AvaloniaToDoListTrackerAndVisualizer.Views;
using System.Globalization;
using AvaloniaToDoListTrackerAndVisualizer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaToDoListTrackerAndVisualizer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private MainWindowViewModel? _mainWindowViewModel = null;

    public override async void OnFrameworkInitializationCompleted()
    {
        Lang.Resources.Culture = new CultureInfo("en-US");
        
        BindingPlugins.DataValidators.RemoveAt(0);
        
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        
        var services = collection.BuildServiceProvider();
        
        _mainWindowViewModel = services.GetService<MainWindowViewModel>()!;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow()
            {
                DataContext = _mainWindowViewModel
            };

            desktop.ShutdownRequested += DesktopOnShutDownRequested;
            await _mainWindowViewModel.LoadFiles();
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
    
    private bool _canClose = false;
    
    private async void DesktopOnShutDownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        try
        {
            e.Cancel = !_canClose;
            if (!_canClose)
            {
                await _mainWindowViewModel!.SaveFiles();
                _canClose = true;
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        }
        catch (Exception)
        {
            _canClose = true;
        }
    }
}


public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<ITaskApplicationFileService, TaskApplicationFileService>();
        collection.AddTransient<MainWindowViewModel>();
    }
}
