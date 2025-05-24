using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class TsxLanguageDefinition : ILanguageDefinition
{
    public string Name => "TSX";

    // Priorities from JSX definition
    private const int PrioJsxComment = 110;
    private const int PrioJsxTagPunctuation = 82;
    private const int PrioJsxExpressionPunctuation = 81;
    private const int PrioJsxTagName = 79;
    private const int PrioJsxAttributeName = 78;
    private const int PrioJsxText = 1;

    // Styles (consistent with JSX and TypeScript)
    private const string CommentStyle = "color: #6A9955;";
    private const string JsxTagPunctuationStyle = "color: #808080;";
    private const string JsxTagNameStyle = "color: #569CD6;";
    private const string JsxComponentNameStyle = "color: #4EC9B0;";
    private const string JsxAttributeNameStyle = "color: #9CDCFE;";
    private const string JsxExpressionPunctuationStyle = "color: #C586C0;";
    private const string JsxTextStyle = "color: #D4D4D4;";

    private static readonly List<TokenRule> TsxRules;

    static TsxLanguageDefinition()
    {
        var tsLang = new TypeScriptLanguageDefinition();
        var baseTsRules = tsLang.GetRules().ToList();

        // JSX-specific rules (same as in JsxLanguageDefinition)
        var jsxSpecificRules = new List<TokenRule>
        {
            new(@"\{/\*(?:[^*]|\*(?!/))*\*/\}", PrioJsxComment, "jsx-comment", CommentStyle, RegexOptions.Singleline),
            new("</?", PrioJsxTagPunctuation, "jsx-tag-punctuation open", JsxTagPunctuationStyle),
            new("/?>", PrioJsxTagPunctuation, "jsx-tag-punctuation close", JsxTagPunctuationStyle),
            new(@"(?<=</?)\s*([A-Z][\w.-]*)", PrioJsxTagName, "jsx-component-name", JsxComponentNameStyle),
            new(@"(?<=</?)\s*([a-z][\w-]*)", PrioJsxTagName, "jsx-html-tag-name", JsxTagNameStyle),
            new(@"\b([A-Za-z_][\w:-]*)(?=\s*=|\s*[/A-Za-z_.\w-]*>|\s+[A-Za-z_][\w:-]+\s*=)", PrioJsxAttributeName, "jsx-attribute-name", JsxAttributeNameStyle),
            new("=", PrioJsxTagPunctuation, "jsx-attribute-equals", JsxTagPunctuationStyle),
            new(@"\{(?![^{}]*?\})", PrioJsxExpressionPunctuation, "jsx-expression-brace open", JsxExpressionPunctuationStyle),
            new(@"\}", PrioJsxExpressionPunctuation, "jsx-expression-brace close", JsxExpressionPunctuationStyle),
            new("(?<=>)([^<{}]*)(?=<)", PrioJsxText, "jsx-text", JsxTextStyle, RegexOptions.Singleline),
        };

        TsxRules = jsxSpecificRules.Concat(baseTsRules)
                                    .OrderByDescending(r => r.Priority)
                                    .ToList();
    }

    public IEnumerable<TokenRule> GetRules() => TsxRules;
}