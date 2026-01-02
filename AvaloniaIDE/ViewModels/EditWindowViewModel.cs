using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaIDE.ViewModels;

public partial class EditWindowViewModel
{
    public EditWindowViewModel(IStorageFile? file)
    {
        if (file != null)
            BuildFileTree(file.GetParentAsync().Result);
    }

    public AvaloniaList<TreeViewItem> FileTree { get; } = [];

    private async void BuildFileTree(IStorageFolder? rootDirectory)
    {
        FileTree.Clear();
        if (rootDirectory == null)
            return;

        TreeViewItem? file = null;
        await foreach (var item in rootDirectory.GetItemsAsync())
        {
            switch (item)
            {
                case IStorageFolder folder:
                    file = await Wafsef(folder);
                    file?.Tag = folder;
                    break;

                case IStorageFile sfile:
                    file = new TreeViewItem
                    {
                        Header = sfile.Name,
                        Tag = sfile
                    };
                    break;
            }

            if (file != null) 
                FileTree.Add(file);
        }
    }

    private async Task<TreeViewItem?> Wafsef(IStorageFolder folder)
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
                    var sub = await Wafsef(childFolder);
                    if (sub != null)
                        folderNode.Items.Add(sub);
                    break;
            }
        }

        return folderNode;
    }

    [RelayCommand]
    private void CopyMouse(TextArea textArea)
    {
        ApplicationCommands.Copy.Execute(null, textArea);
    }

    [RelayCommand]
    private void CutMouse(TextArea textArea)
    {
        ApplicationCommands.Cut.Execute(null, textArea);
    }

    [RelayCommand]
    private void PasteMouse(TextArea textArea)
    {
        ApplicationCommands.Paste.Execute(null, textArea);
    }

    [RelayCommand]
    private void SelectAllMouse(TextArea textArea)
    {
        ApplicationCommands.SelectAll.Execute(null, textArea);
    }
}