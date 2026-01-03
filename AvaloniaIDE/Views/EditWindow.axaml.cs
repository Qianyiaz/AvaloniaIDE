using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaIDE.ViewModels;

namespace AvaloniaIDE.Views;

public partial class EditWindow : Window
{
    private readonly EditWindowViewModel _vm;
    private readonly IStorageFile _file;
        
    public EditWindow(IStorageFile file)
    {
        InitializeComponent();
        _file = file;
        Title = file.Name;
        _vm = new EditWindowViewModel(Editor);
        DataContext = _vm;
        Editor.TextArea.TextEntered += TextArea_TextEntered;
    }
    
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        await _vm.ReadFile(_file);
        _vm.BuildFileTree((await _file.GetParentAsync())!);
    }

    private void TextArea_TextEntered(object? sender, TextInputEventArgs e)
    {
        if (e.Text != ".") return;
        var completionWindow = new CompletionWindow(Editor.TextArea);
        var data = completionWindow.CompletionList.CompletionData;

        data.Add(new MyCompletionData("Method1", "ababababa"));
        data.Add(new MyCompletionData("Method2", "abababab"));
        completionWindow.Show();
    }

    private async void FileTreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView treeView) return;
        if (treeView.SelectedItem is TreeViewItem { Tag: IStorageFile storageFile })
            await _vm.ReadFile(storageFile);
    }
}

public class MyCompletionData(string text, object description) : ICompletionData
{
    public IImage? Image => null;
    public string Text { get; } = text;
    public object Description { get; } = description;
    public object Content => Text;
    public double Priority => 0;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, Text);
    }
}