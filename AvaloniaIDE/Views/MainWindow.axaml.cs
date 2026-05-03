using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaIDE.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
}

public class ProjectItem
{
    public IImage? ImageSource { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}