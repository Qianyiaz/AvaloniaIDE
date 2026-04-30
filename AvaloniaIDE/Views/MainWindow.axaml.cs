using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaIDE.ViewModels;

namespace AvaloniaIDE.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel(this);
        InitializeComponent();
    }
}

public class ProjectItem
{
    public IImage? ImageSource { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}