using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace AvaloniaIDE.ViewModels;

public class EditWindowViewModel(TextEditor editor)
{
    private static readonly RegistryOptions RegistryOptions = new(ThemeName.DarkPlus);
    private readonly TextMate.Installation? _textMateInstallation = editor.InstallTextMate(RegistryOptions);

    public async Task ReadFile(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        editor.Load(stream);

        var extension = Path.GetExtension(storageFile.Name).ToLowerInvariant();
        try
        {
            var languageByExtension = extension switch
            {
                ".axaml" or ".slnx" => RegistryOptions.GetLanguageByExtension(".xml"),
                _ => RegistryOptions.GetLanguageByExtension(extension)
            };

            _textMateInstallation!.SetGrammar(RegistryOptions.GetScopeByLanguageId(languageByExtension.Id));
        }
        catch
        {
            // ignored
        }
    }

    public AvaloniaList<TreeViewItem> FileTree { get; } = [];

    public async void BuildFileTree(IStorageFolder rootDirectory)
    {
        FileTree.Clear();

        TreeViewItem? treeViewItem = null;
        await foreach (var item in rootDirectory.GetItemsAsync())
        {
            switch (item)
            {
                case IStorageFolder folder:
                    treeViewItem = await CreateItem(folder);
                    treeViewItem.Tag = folder;
                    break;

                case IStorageFile sfile:
                    treeViewItem = new TreeViewItem
                    {
                        Header = sfile.Name,
                        Tag = sfile
                    };
                    break;
            }

            FileTree.Add(treeViewItem!);
        }
    }

    private async Task<TreeViewItem> CreateItem(IStorageFolder folder)
    {
        var folderNode = new TreeViewItem
        {
            Header = folder.Name,
            Tag = folder
        };

        await foreach (var item in folder.GetItemsAsync())
        {
            var childNode = new TreeViewItem
            {
                Header = item.Name
            };

            switch (item)
            {
                case IStorageFile childFile:
                    childNode.Tag = childFile;
                    folderNode.Items.Add(childNode);
                    break;

                case IStorageFolder childFolder:
                    folderNode.Items.Add(await CreateItem(childFolder));
                    break;
            }
        }

        return folderNode;
    }
}