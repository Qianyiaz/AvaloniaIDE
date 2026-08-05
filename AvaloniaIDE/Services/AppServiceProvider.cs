using AvaloniaIDE.ViewModels;
using AvaloniaIDE.Views;
using Jab;

namespace AvaloniaIDE.Services;

[ServiceProvider]
[Singleton<MainWindow>]
[Transient<MainWindowViewModel>]
[Singleton<IMainWindowService, MainWindowService>]
public partial class AppServiceProvider;