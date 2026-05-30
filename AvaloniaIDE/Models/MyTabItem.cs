using Avalonia.Platform.Storage;

namespace AvaloniaIDE.Models;

public class MyTabItem
{
    public string Header { get; set; } = string.Empty;
    public IStorageFile StorageFile { get; init; } = null!;
}