using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using AvaloniaIDE.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class MainViewModel
{
    public AvaloniaList<ProjectItem> Items { get; } =
    [
        new()
        {
            Name = "Test",
            Path = @"C:\Users\Administrator\Desktop\test.txt",
            ImageSource = new Bitmap(AssetLoader.Open(new("avares://AvaloniaIDE/Assets/DeviconTrello.png"))),
        }
    ];

    [RelayCommand]
    private async Task OpenAsnyc()
    {
        if (Application.Current!.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        var mainWindow = desktop.MainWindow;
        var topLevel = TopLevel.GetTopLevel(mainWindow);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open a File",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Solution Files") { Patterns = ["*.sln", "*.slnx"] }]
        });

        var editWindow = new EditWindow();
        editWindow.Show();
        editWindow.Initialize(files[0]);

        mainWindow!.Hide();
    }
}