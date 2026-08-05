using Avalonia.Collections;
using Avalonia.Platform.Storage;

namespace AvaloniaIDE.ViewModels;

public class EditWindowViewModel(IStorageFile storageFile)
{
    public AvaloniaList<FileDocument> Documents { get; } =
    [
         new("Document 1", "Content here..."),
        new("Document 2", "More content...")
    ];
}

public class FileDocument(string title, string content)
{
    public string Title { get; set; } = title;

    public string Content { get; set; } = content;
}