using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaIDE.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class MainWindowViewModel(Window window)
{
    public AvaloniaList<ProjectItem> Items { get; } = [];

    [RelayCommand]
    private void Open(IReadOnlyList<IStorageFile> args)
    {
        new EditWindow(args[0]).Show();
        window.Close();
    }
}