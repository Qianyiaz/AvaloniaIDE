using Avalonia.Controls;
using Avalonia.Metadata;
using Xaml.Behaviors.SourceGenerators;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "Avalonia.Controls")]
[assembly: GenerateEventCommand(typeof(TreeViewItem), "Expanded", ParameterPath = "Source", UseDispatcher = true)]