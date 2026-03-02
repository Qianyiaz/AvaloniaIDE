using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaIDE.Views;

public partial class StartWindow : Window
{
    public StartWindow() => InitializeComponent();
}

public class ProjectItem
{
    public IImage? ImageSource { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}