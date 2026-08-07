using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace AvaloniaIDE.Views;

public partial class EditWindow : Window
{
    private readonly RegistryOptions _registryOptions = new(ThemeName.DarkPlus);

    public EditWindow() => InitializeComponent();

    private void Editor_OnDocumentChanged(object? sender, DocumentChangedEventArgs e)
    {
        if (e.NewDocument == null) return;

        var textMateInstallation = (sender as TextEditor).InstallTextMate(_registryOptions);
        // mustn't use using keyword here, because it will dispose the textMateInstallation object and break the grammar highlighting

        var extension = Path.GetExtension(e.NewDocument.FileName);
        if (string.IsNullOrEmpty(extension)) return;

        var language = _registryOptions.GetLanguageByExtension(extension switch
        {
            ".axaml" or ".slnx" or ".user" => ".xml",
            ".godot" or ".tscn" => ".ini",
            _ => extension
        });
        if (language == null) return;

        textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(language.Id));
    }
}