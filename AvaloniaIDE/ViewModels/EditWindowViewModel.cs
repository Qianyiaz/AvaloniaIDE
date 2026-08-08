using Avalonia.Collections;
using Avalonia.Platform.Storage;
using AvaloniaIDE.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class EditWindowViewModel : ObservableObject
{
    [ObservableProperty] private FileNode? _selectedFileItem;

    public EditWindowViewModel(IStorageFile storageFile) => _ = InitializeAsync(storageFile);

    public AvaloniaList<FileDocument> Documents { get; } = [];

    public AvaloniaList<FileNode> FileItems { get; } = [];

    async partial void OnSelectedFileItemChanged(FileNode? value)
    {
        if (value?.StorageItem is not IStorageFile file) return;
        if (Documents.All(d => d.StorageItem!.Path != file.Path))
            Documents.Add(await FileDocument.CreateAsync(file));

        SetProperty(ref _selectedFileItem, null, nameof(SelectedFileItem));
    }

    private async Task InitializeAsync(IStorageFile storageFile)
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
    private async Task TreeViewItemExpanded(FileNode? node)
    {
        if (node != null)
            await FileNode.LoadChildren(node);
    }
}