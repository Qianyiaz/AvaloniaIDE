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

    private async void OnItemExpanded(object? sender, RoutedEventArgs e)
    {
        if (sender is TreeViewItem item)
            await LoadChildren(item);
    }


    private void CloseItem(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not MyTabItem) return;
        TabStrip.Items.Remove(button.DataContext);
        if (TabStrip.Items.Count != 0) return;
        Editor.Clear();
        Editor.Text = "Please open a file to edit.";
        Editor.IsReadOnly = true;
    }

    private async void SavedFile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TabStrip.SelectedItem is not MyTabItem { StorageFile: { } storageFile }) return;
        await using var stream = await storageFile.OpenWriteAsync();
        Editor.Save(stream);
    }

    private async void BuildFileTree(IStorageFolder rootDirectory)
    {
        FileTreeView.Items.Clear();

        await foreach (var item in rootDirectory.GetItemsAsync())
        {
            TreeViewItem treeViewItem = null!;
            switch (item)
            {
                case IStorageFolder folder:
                    if (folder.Name is ".git" or "bin" or "obj" or ".vs" or ".idea" or ".godot")
                        continue;
                    
                    treeViewItem = new TreeViewItem { Header = folder.Name, Tag = folder, Items = { null } };
                    treeViewItem.Expanded += OnItemExpanded;
                    break;

                case IStorageFile sfile:
                    treeViewItem = new TreeViewItem
                    {
                        Header = sfile.Name,
                        Tag = sfile
                    };
                    break;
            }

            FileTreeView.Items.Add(treeViewItem);
        }
    }

    private async Task LoadChildren(TreeViewItem item)
    {
        if (item.Tag is not IStorageFolder folder) return;
        item.Items.Clear();
        item.Expanded -= OnItemExpanded;

        await foreach (var child in folder.GetItemsAsync())
        {
            switch (child)
            {
                case IStorageFile file:
                    item.Items.Add(new TreeViewItem { Header = file.Name, Tag = file });
                    break;

                case IStorageFolder subfolder:
                    var childItem = new TreeViewItem { Header = subfolder.Name, Tag = subfolder, Items = { null } };
                    childItem.Expanded += OnItemExpanded;
                    item.Items.Add(childItem);
                    break;
            }
        }
    }
}

public class FileReader(TextEditor editor) : IDisposable
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

    public void Dispose() => _textMateInstallation?.Dispose();
}

public class MyTabItem
{
    public string Header { get; set; } = string.Empty;
    public IStorageFile StorageFile { get; init; } = null!;
}