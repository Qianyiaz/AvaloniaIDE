using Avalonia.Collections;
using Avalonia.Platform.Storage;

namespace AvaloniaIDE.Models;

public class FileNode(
    string title,
    AvaloniaList<FileNode?>? children,
    IStorageItem storageItem,
    FileNode? parent = null
)
{
    public readonly FileNode? Parent = parent;

    public readonly IStorageItem? StorageItem = storageItem;

    public AvaloniaList<FileNode?>? Children { get; } = children;

    public string Title { get; } = title;

    public static async Task LoadChildren(FileNode folder)
    {
        folder.Children!.Clear();

        await foreach (var item in (folder.StorageItem as IStorageFolder)?.GetItemsAsync()!)
            folder.Children.Add(item switch
            {
                IStorageFolder subfolder => new FileNode(subfolder.Name, [null], subfolder, folder),
                IStorageFile file => new FileNode(file.Name, null, file, folder),
                _ => null!
            });

        if (folder.Children.Count == 0)
            folder.Children.Add(null);
    }
}