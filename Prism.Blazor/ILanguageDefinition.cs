namespace Prism.Blazor;

public interface ILanguageDefinition
{
    string Name { get; }
    IEnumerable<TokenRule> GetRules();
}