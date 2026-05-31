using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace AvaloniaIDE.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private async void OpenSolutionClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open a File",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Solution Files") { Patterns = ["*.sln", "*.slnx"] }]
        });

        if (files.Count == 0)
            return;

        var editWindow = new EditWindow();
        editWindow.Show();
        editWindow.Initialize(files[0]);
        editWindow.Closed += (_, _) => Show();

        Hide();
    }
}