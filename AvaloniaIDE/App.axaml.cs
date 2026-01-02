using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaIDE.ViewModels;
using AvaloniaIDE.Views;

namespace AvaloniaIDE;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var window = new StartWindow();
        window.DataContext = new StartWindowViewModel(window);
        desktop.MainWindow = window;
    }
}