using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaIDE.ViewModels;
using AvaloniaIDE.Views;

namespace AvaloniaIDE.Services;

public interface IWindowManager
{
    void ShowEditWindow(IStorageFile file, Action? onLoaded, Action onClosed);

    void HideMainWindow();

    void ShowMainWindow();
}

public class WindowManager : IWindowManager
{
    private Window? MainWindow => (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
        .MainWindow;

    public void ShowEditWindow(IStorageFile file, Action? onLoaded, Action? onClosed)
    {
        var editWindow = new EditWindow
        {
            DataContext = new EditWindowViewModel(file)
        };

        void OnLoaded(object? sender, EventArgs e)
        {
            editWindow.Loaded -= OnLoaded;
            onLoaded?.Invoke();
        }

        void OnClosed(object? sender, EventArgs e)
        {
            editWindow.Closed -= OnClosed;
            onClosed?.Invoke();
        }

        editWindow.Loaded += OnLoaded;
        editWindow.Closed += OnClosed;
        editWindow.Show();
    }

    public void HideMainWindow() => MainWindow?.Hide();

    public void ShowMainWindow()
    {
        MainWindow?.Show();
        MainWindow?.Activate();
    }
}