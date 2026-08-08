using Avalonia.Collections;
using Avalonia.Platform.Storage;

namespace AvaloniaIDE.Models;

public class FileNode(string title, AvaloniaList<FileNode?>? subNodes, IStorageItem storageItem)
{
    public AvaloniaList<FileNode?>? SubNodes { get; } = subNodes;

    public string Title { get; } = title;

    public IStorageItem? StorageItem { get; } = storageItem;

    public static async Task LoadChildren(FileNode folder)
    {
        folder.SubNodes!.Clear();

        await foreach (var item in (folder.StorageItem as IStorageFolder)?.GetItemsAsync()!)
            folder.SubNodes.Add(item switch
            {
                IStorageFolder subfolder => new FileNode(subfolder.Name, [null], subfolder),
                IStorageFile file => new FileNode(file.Name, null, file),
                _ => null!
            });
    }
}