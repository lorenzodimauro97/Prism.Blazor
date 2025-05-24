using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class PlainTextLanguageDefinition : ILanguageDefinition
{
    public string Name => "PlainText";
    public IEnumerable<TokenRule> GetRules() => []; // No rules, just encodes
}