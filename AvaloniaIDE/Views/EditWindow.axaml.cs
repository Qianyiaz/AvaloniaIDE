using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace AvaloniaIDE.Views;

public partial class EditWindow : Window
{
    private IStorageFile _file = null!;
    private FileReader _fileReader = null!;

    public EditWindow() => InitializeComponent();

    public void Initialize(IStorageFile file)
    {
        _file = file;
        _fileReader = new FileReader(Editor);

        Title = file.Name;
        Loaded += OnLoaded;
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

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e is { KeyModifiers: KeyModifiers.Control, Key: Key.S })
        {
            SavedFile_OnClick(this, e);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
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

public sealed class FileReader(TextEditor editor) : IDisposable
{
    private static readonly RegistryOptions RegistryOptions = new(ThemeName.DarkPlus);
    private readonly TextMate.Installation? _textMateInstallation = editor.InstallTextMate(RegistryOptions);

    public void Dispose() => _textMateInstallation?.Dispose();

    public async Task ReadFile(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        editor.Load(stream);

        var extension = Path.GetExtension(storageFile.Name).ToLowerInvariant();

        var language = GetLanguageForExtension(extension);
        if (language is not null && _textMateInstallation is not null)
        {
            var scope = RegistryOptions.GetScopeByLanguageId(language.Id);
            _textMateInstallation.SetGrammar(scope);
        }
    }

    private static Language? GetLanguageForExtension(string lowerExtension)
    {
        var mappedExtension = lowerExtension switch
        {
            ".axaml" or ".slnx" or ".user" => ".xml",
            ".godot" or ".tscn" => ".ini",
            _ => lowerExtension
        };

        return RegistryOptions.GetLanguageByExtension(mappedExtension);
    }
}

public class MyTabItem
{
    public string Header { get; set; } = string.Empty;
    public IStorageFile StorageFile { get; init; } = null!;
}