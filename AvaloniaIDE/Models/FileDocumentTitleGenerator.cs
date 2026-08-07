using Dock.Model.Avalonia.Controls;

namespace AvaloniaIDE.Models;

public class FileDocumentTitleGenerator : DockItemContainerGenerator
{
    protected override string GetItemTitle(object item)
    {
        if (item is not FileDocument fd) return base.GetItemTitle(item);
        
        var name = fd.Document.FileName;
        return !string.IsNullOrEmpty(name) ? name : "Untitled";
    }
}