using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using AvaloniaIDE.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class EditWindowViewModel : ObservableObject
{
    [ObservableProperty] private FileNode? _selectedFileItem;

    public EditWindowViewModel(IStorageFile storageFile) => _ = LoadDocumentAsync(storageFile);

    public AvaloniaList<FileDocument> Documents { get; } = [];

    public AvaloniaList<FileNode> FileItems { get; } = [];

    async partial void OnSelectedFileItemChanged(FileNode? value)
    {
        if (value!.StorageItem is not IStorageFile file) return;
        if (Documents.Any(d => d.StorageItem!.Path == file.Path)) return;

        Documents.Add(await FileDocument.CreateAsync(file));
    }

    private async Task LoadDocumentAsync(IStorageFile storageFile)
    {
        await foreach (var item in (await storageFile.GetParentAsync())!.GetItemsAsync())
            FileItems.Add(item switch
            {
                IStorageFolder folder => new(folder.Name, [null], folder),
                IStorageFile file => new(file.Name, null, file),
                _ => null!
            });

        Documents.Add(await FileDocument.CreateAsync(storageFile));
    }

    [RelayCommand]
    private async Task TreeViewItemExpanded(TreeViewItem item) =>
        await FileNode.LoadChildren(item.DataContext as FileNode);

    [RelayCommand]
    private void CopyMouse(TextArea textArea) => ApplicationCommands.Copy.Execute(null, textArea);

    [RelayCommand]
    private void CutMouse(TextArea textArea) => ApplicationCommands.Cut.Execute(null, textArea);

    [RelayCommand]
    private void PasteMouse(TextArea textArea) => ApplicationCommands.Paste.Execute(null, textArea);

    [RelayCommand]
    private void SelectAllMouse(TextArea textArea) => ApplicationCommands.SelectAll.Execute(null, textArea);

    [RelayCommand]
    private void UndoMouse(TextArea textArea) => ApplicationCommands.Undo.Execute(null, textArea);
}