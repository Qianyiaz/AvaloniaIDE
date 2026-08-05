using Avalonia.Platform.Storage;
using AvaloniaIDE.Services;
using AvaloniaIDE.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class MainWindowViewModel(IMainWindowService mainWindowService) : ObservableObject
{
    [RelayCommand]
    private void OpenSolution(IReadOnlyList<IStorageFile> storageFileList)
    {
        if (storageFileList.Count <= 0) return;

        var window = new EditWindow
        {
            DataContext = new EditWindowViewModel(storageFileList[0])
        };
        window.Show();

        window.Closed += (_, _) => mainWindowService.ShowWindow();
        mainWindowService.HideWindow();
    }
}