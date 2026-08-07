using System.Text;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;

namespace AvaloniaIDE.Models;

public class FileDocument(TextDocument document, IStorageItem? storageItem = null)
{
    public TextDocument Document { get; } = document;

    public IStorageItem? StorageItem { get; } = storageItem;

    public static async Task<FileDocument> CreateAsync(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        var doc = new TextDocument(content) { FileName = storageFile.Name };
        return new FileDocument(doc, storageFile);
    }
}