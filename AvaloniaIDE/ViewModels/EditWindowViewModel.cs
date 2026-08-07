using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class EditWindowViewModel : ObservableObject
{
    public AvaloniaList<FileDocument> Documents { get; } = [];

    public AvaloniaList<TreeViewItem> FileItems { get; } = [];

    [ObservableProperty] private TreeViewItem? _selectedFileItem;

    async partial void OnSelectedFileItemChanged(TreeViewItem? value)
    {
        if (value!.Tag is not IStorageFile file) return;
        if (Documents.Any(d => d.Document.FileName == file.Name))
            return;

        Documents.Add(await FileDocument.CreateAsync(file));
    }

    public EditWindowViewModel(IStorageFile storageFile) => _ = LoadDocumentAsync(storageFile);

    private async Task LoadDocumentAsync(IStorageFile storageFile)
    {
        BuildFileTree((await storageFile.GetParentAsync())!);
        Documents.Add(await FileDocument.CreateAsync(storageFile));
    }

    private async void BuildFileTree(IStorageFolder rootDirectory)
    {
        FileItems.Clear();

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

                case IStorageFile file:
                    treeViewItem = new TreeViewItem
                    {
                        Header = file.Name,
                        Tag = file
                    };
                    break;
            }

            FileItems.Add(treeViewItem);
        }
    }

    private async void OnItemExpanded(object? sender, RoutedEventArgs e)
    {
        if (sender is TreeViewItem item)
            await LoadChildren(item);
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
                    item.Items.Add(childItem);
                    childItem.Expanded += OnItemExpanded;
                    break;
            }
        }
    }

    [RelayCommand]
    private void CopyMouse(TextArea textArea) => ApplicationCommands.Copy.Execute(null, textArea);

    [RelayCommand]
    private void CutMouse(TextArea textArea) => ApplicationCommands.Cut.Execute(null, textArea);

    [RelayCommand]
    private void PasteMouse(TextArea textArea) => ApplicationCommands.Paste.Execute(null, textArea);

    [RelayCommand]
    private void SelectAllMouse(TextArea textArea) => ApplicationCommands.SelectAll.Execute(null, textArea);

    [RelayCommand]
    private void UndoMouse(TextArea textArea) => ApplicationCommands.Undo.Execute(null, textArea);
}

public class FileDocument(TextDocument document)
{
    // ReSharper disable once UnusedMember.Global
    public string Title { get; } = document.FileName;
    // This is used reflectively in AvaloniaEdit's TabControl to display the title of the document.

    public static async Task<FileDocument> CreateAsync(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();

        return new FileDocument(new TextDocument(content) { FileName = storageFile.Name });
    }

    public TextDocument Document { get; } = document;
}