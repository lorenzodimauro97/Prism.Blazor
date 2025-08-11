using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class JsxLanguageDefinition : ILanguageDefinition
{
    public string Name => "JSX";
    
    private const int PrioJsxComment = 110;
    private const int PrioJsxTagPunctuation = 82;
    private const int PrioJsxExpressionPunctuation = 81;
    private const int PrioJsxTagName = 79;
    private const int PrioJsxAttributeName = 78;
    private const int PrioJsxText = 1;

    private static readonly List<TokenRule> JsxRules;

    static JsxLanguageDefinition()
    {
        var jsLang = new JavaScriptLanguageDefinition();
        var baseJsRules = jsLang.GetRules().ToList();

        var jsxSpecificRules = new List<TokenRule>
        {
            new(@"\{/\*(?:[^*]|\*(?!/))*\*/\}", PrioJsxComment, "jsx-comment", null, RegexOptions.Singleline),
            new("</?", PrioJsxTagPunctuation, "jsx-tag-punctuation open", null),
            new("/?>", PrioJsxTagPunctuation, "jsx-tag-punctuation close", null),
            new(@"(?<=</?)\s*([A-Z][\w.-]*)", PrioJsxTagName, "jsx-component-name", null),
            new(@"(?<=</?)\s*([a-z][\w-]*)", PrioJsxTagName, "jsx-html-tag-name", null),
            new(@"\b([A-Za-z_][\w:-]*)(?=\s*=|\s*[/A-Za-z_.\w-]*>|\s+[A-Za-z_][\w:-]+\s*=)", PrioJsxAttributeName, "jsx-attribute-name", null),
            new("=", PrioJsxTagPunctuation, "jsx-attribute-equals", null),
            new(@"\{(?![^{}]*?\})", PrioJsxExpressionPunctuation, "jsx-expression-brace open", null),
            new(@"\}", PrioJsxExpressionPunctuation, "jsx-expression-brace close", null),
            new("(?<=>)([^<{}]*)(?=<)", PrioJsxText, "jsx-text", null, RegexOptions.Singleline),
        };

        JsxRules = jsxSpecificRules.Concat(baseJsRules)
            .OrderByDescending(r => r.Priority)
            .ToList();
    }

    public IEnumerable<TokenRule> GetRules() => JsxRules;
}