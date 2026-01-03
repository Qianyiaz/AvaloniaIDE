using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaIDE.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class StartWindowViewModel
{
    private readonly Window _window;

    public StartWindowViewModel(Window window)
    {
        _window = window;

        for (var i = 1; i < 11; i++)
            Items.Add(new() { Name = $"CAT{i}", Path = @"C:\Users\Unknown\RiderProjects" });
    }

    public AvaloniaList<ProjectItem> Items { get; } = [];

    public IEnumerable<string> ItemStrings => Items.Select(projectItem => projectItem.Name);

    [RelayCommand]
    private void Open(IReadOnlyList<IStorageFile> args)
    {
        new EditWindow(args[0]).Show();
        _window.Close();
    }
}