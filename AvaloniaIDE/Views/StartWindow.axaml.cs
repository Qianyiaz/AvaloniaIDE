using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace AvaloniaIDE.Views;

public partial class StartWindow : Window
{
    public StartWindow() => InitializeComponent();
}

public class ProjectItem
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public Bitmap? ImageSource { get; set; }
}