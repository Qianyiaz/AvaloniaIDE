using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using AvaloniaIDE.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class MainWindowViewModel(Window window)
{
    public AvaloniaList<ProjectItem> Items { get; } = 
    [
        new ()
        {
            Name = "Test", 
            Path = @"C:\Users\Administrator\Desktop\test.txt",
            ImageSource = new Bitmap(AssetLoader.Open(new("avares://AvaloniaIDE/Assets/DeviconTrello.png"))),
        }
    ];
    
    [RelayCommand]
    private void Open(IReadOnlyList<IStorageFile> args)
    {
        new EditWindow(args[0]).Show();
        window.Close();
    }
}