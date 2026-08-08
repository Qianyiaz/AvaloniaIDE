using Avalonia.Platform.Storage;
using AvaloniaIDE.Services;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class MainWindowViewModel(IWindowManager windowService)
{
    [RelayCommand]
    private void OpenSolution(IReadOnlyList<IStorageFile> storageFileList)
    {
        if (storageFileList.Count <= 0) return;
        windowService.ShowEditWindow(storageFileList[0], windowService.HideMainWindow, windowService.ShowMainWindow);
    }
}