using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaIDE.ViewModels;

namespace AvaloniaIDE.Views;

public partial class EditWindow : Window
{
    private readonly EditWindowViewModel _vm;
    private readonly IStorageFile _file;

    public EditWindow(IStorageFile file)
    {
        InitializeComponent();
        _file = file;
        Title = file.Name;
        _vm = new EditWindowViewModel(Editor);
        DataContext = _vm;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        await _vm.ReadFile(_file);
        _vm.BuildFileTree((await _file.GetParentAsync())!);
    }

    private async void FileTreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView treeView) return;
        if (treeView.SelectedItem is not TreeViewItem { Tag: IStorageFile storageFile }) return;
        TabControl.Items.Add(new TabItem { Header = storageFile.Name, Tag = storageFile });
        await _vm.ReadFile(storageFile);
    }

    private async void TabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TabControl tabControl) return;
        if (tabControl.SelectedItem is not TabItem { Tag: IStorageFile storageFile }) return;
        await _vm.ReadFile(storageFile);
    }
}