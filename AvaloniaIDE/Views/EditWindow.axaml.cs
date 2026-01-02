using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using AvaloniaIDE.ViewModels;
using TextMateSharp.Grammars;

namespace AvaloniaIDE.Views;

public partial class EditWindow : Window
{
    public EditWindow(IStorageFile? file)
    {
        InitializeComponent();
        DataContext = new EditWindowViewModel(file);

        var  registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMateInstallation = Editor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".cs").Id));
        
        Editor.TextArea.TextEntered += TextArea_TextEntered;
    }

    private void TextArea_TextEntered(object? sender, TextInputEventArgs e)
    {
        if (e.Text != ".") return;
        var completionWindow = new CompletionWindow(Editor.TextArea);
        var data = completionWindow.CompletionList.CompletionData;
        
        data.Add(new MyCompletionData("Method1","ababababa"));
        data.Add(new MyCompletionData("Method2","abababab"));
        completionWindow.Show();
    }
}


public class MyCompletionData(string text, object description) : ICompletionData
{
    public IImage? Image => null;
    public string Text { get; } = text;
    public object Description { get; }  = description;
    public object Content => Text;
    public double Priority => 0;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, Text);
    }
}