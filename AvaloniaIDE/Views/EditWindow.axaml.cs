using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace AvaloniaIDE.Views;

public partial class EditWindow : Window
{
    private readonly IStorageFile _file;
    private readonly FileReader _fileReader;

    public EditWindow(IStorageFile file)
    {
        InitializeComponent();
        _fileReader = new FileReader(Editor);
        _file = file;
        Title = file.Name;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        BuildFileTree((await _file.GetParentAsync())!);

        TabStrip.Items.Add(new MyTabItem
        {
            Header = _file.Name,
            StorageFile = _file
        });
    }

    private void FileTreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView treeView) return;
        if (treeView.SelectedItem is not TreeViewItem { Tag: IStorageFile storageFile }) return;
        
        foreach (var item in TabStrip.Items)
        {
            if (item is not MyTabItem { StorageFile: { } file } || file.Path != storageFile.Path) continue;
            TabStrip.SelectedItem = item;
            return;
        }
        
        var myTabItem = new MyTabItem
        {
            Header = storageFile.Name,
            StorageFile = storageFile
        };
        
        TabStrip.Items.Add(myTabItem);
        TabStrip.SelectedItem = myTabItem;
    }

    private async void TabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TabStrip tabControl) return;
        if (tabControl.SelectedItem is not MyTabItem { StorageFile: { } storageFile }) return;
        Editor.IsReadOnly = false;
        await _fileReader.ReadFile(storageFile);
    }

    private async void BuildFileTree(IStorageFolder rootDirectory)
    {
        FileTreeView.Items.Clear();

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

            FileTreeView.Items.Add(treeViewItem!);
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
            var childNode = new TreeViewItem { Header = item.Name };

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

    private void CloceItem(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not MyTabItem) return;
        TabStrip.Items.Remove(button.DataContext);
        if (TabStrip.Items.Count != 0) return;
        Editor.Clear();
        Editor.Text = "Please open a file to edit.";
        Editor.IsReadOnly = true;
    }

    private void SavedFile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TabStrip.SelectedItem is not MyTabItem { StorageFile: { } storageFile }) return;
        using var stream = storageFile.OpenWriteAsync().GetAwaiter().GetResult();
        Editor.Save(stream);
    }
}

public class FileReader(TextEditor editor)
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
                ".axaml" or ".slnx" or ".user" => RegistryOptions.GetLanguageByExtension(".xml"),
                ".godot" or ".tscn" => RegistryOptions.GetLanguageByExtension(".ini"),
                _ => RegistryOptions.GetLanguageByExtension(extension)
            };

            _textMateInstallation!.SetGrammar(RegistryOptions.GetScopeByLanguageId(languageByExtension.Id));
        }
        catch
        {
            // ignored
        }
    }
}

public class MyTabItem
{
    public string Header { get; set; } = string.Empty;
    public IStorageFile StorageFile { get; init; } = null!;
    public override string ToString() => null!;
}