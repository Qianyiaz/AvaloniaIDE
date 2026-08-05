using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaIDE.Services;
using AvaloniaIDE.Views;

namespace AvaloniaIDE;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var sp = new AppServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = sp.GetService<MainWindow>();
        base.OnFrameworkInitializationCompleted();
    }
}