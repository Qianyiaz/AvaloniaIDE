using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace AvaloniaIDE.Services;

public interface IMainWindowService
{
    void HideWindow();

    void ShowWindow();
}

public class MainWindowService : IMainWindowService
{
    private Window? MainWindow => (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
        .MainWindow;

    public void HideWindow() => MainWindow?.Hide();

    public void ShowWindow()
    {
        MainWindow?.Show();
        MainWindow?.Activate();
    }
}