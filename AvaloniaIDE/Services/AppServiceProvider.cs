using AvaloniaIDE.ViewModels;
using AvaloniaIDE.Views;
using Jab;

namespace AvaloniaIDE.Services;

[ServiceProvider]
[Singleton<MainWindow>]
[Transient<MainWindowViewModel>]
[Singleton<IWindowManager, WindowManager>]
public partial class AppServiceProvider;