using Avalonia.Controls;
using AvaloniaIDE.ViewModels;

namespace AvaloniaIDE.Views;

public partial class MainWindow : Window
{
    // ReSharper disable once MemberCanBePrivate.Global
    public MainWindow() => InitializeComponent();

    // ReSharper disable once UnusedMember.Global
    public MainWindow(MainWindowViewModel vm) : this() => DataContext = vm;
}