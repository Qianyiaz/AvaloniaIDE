using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace AvaloniaIDE.Models;

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
        if (language is null) return;
        
        _textMateInstallation!.SetGrammar(RegistryOptions.GetScopeByLanguageId(language.Id));
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