using System.Text;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;

namespace AvaloniaIDE.Models;

public class FileDocument(TextDocument document, IStorageItem? storageItem = null)
{
    // ReSharper disable once UnusedMember.Global
    public string Title { get; } = document.FileName;
    // This is used reflectively in AvaloniaEdit's TabControl to display the title of the document.

    public static async Task<FileDocument> CreateAsync(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();

        return new(new(content) { FileName = storageFile.Name }, storageFile);
    }

    public IStorageItem? StorageItem { get; } = storageItem;

    public TextDocument Document { get; } = document;
}