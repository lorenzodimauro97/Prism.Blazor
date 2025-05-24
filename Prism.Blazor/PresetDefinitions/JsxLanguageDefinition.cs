using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class JsxLanguageDefinition : ILanguageDefinition
{
    public string Name => "JSX";

    // Styles
    private const string CommentStyle = "color: #6A9955;"; // Includes {/**/}
    private const string StringStyle = "color: #CE9178;";
    private const string JsxTagPunctuationStyle = "color: #808080;"; // <, >, />, =
    private const string JsxTagNameStyle = "color: #569CD6;";       // For HTML-like tags (div, span)
    private const string JsxComponentNameStyle = "color: #4EC9B0;";  // For Component tags (MyComponent)
    private const string JsxAttributeNameStyle = "color: #9CDCFE;";  // For prop names
    private const string JsxExpressionPunctuationStyle = "color: #C586C0;"; // { and } in JSX
    private const string JsxTextStyle = "color: #D4D4D4;"; // Text nodes in JSX

    // Priorities
    private const int PrioJsxComment = 110;
    private const int PrioJsxTagPunctuation = 82; // <, >, />, =
    private const int PrioJsxExpressionPunctuation = 81; // { and }
    private const int PrioJsxTagName = 79;       // Must be higher than general identifiers
    private const int PrioJsxAttributeName = 78;
    private const int PrioJsxText = 1;           // Very low, for text between tags

    private static readonly List<TokenRule> JsxRules;

    static JsxLanguageDefinition()
    {
        var jsLang = new JavaScriptLanguageDefinition();
        // Base JS rules, but we'll add JSX rules with higher priority for JSX constructs.
        var baseJsRules = jsLang.GetRules().ToList();

        var jsxSpecificRules = new List<TokenRule>
        {
            // 1. JSX Comments: {/* ... */}
            new(@"\{/\*(?:[^*]|\*(?!/))*\*/\}", PrioJsxComment, "jsx-comment", CommentStyle, RegexOptions.Singleline),

            // 2. JSX Tags and Punctuation
            new("</?", PrioJsxTagPunctuation, "jsx-tag-punctuation open", JsxTagPunctuationStyle), // < or </
            new("/?>", PrioJsxTagPunctuation, "jsx-tag-punctuation close", JsxTagPunctuationStyle), // > or />
            
            // Tag Names - must be matched after < or </
            // PascalCase for components
            new(@"(?<=</?)\s*([A-Z][\w.-]*)", PrioJsxTagName, "jsx-component-name", JsxComponentNameStyle),
            // lowercase or kebab-case for HTML elements
            new(@"(?<=</?)\s*([a-z][\w-]*)", PrioJsxTagName, "jsx-html-tag-name", JsxTagNameStyle),

            // 3. JSX Attribute Names (lookahead for = or end of tag/another attribute)
            new(@"\b([A-Za-z_][\w:-]*)(?=\s*=|\s*[/A-Za-z_.\w-]*>|\s+[A-Za-z_][\w:-]+\s*=)", PrioJsxAttributeName, "jsx-attribute-name", JsxAttributeNameStyle),
            new("=", PrioJsxTagPunctuation, "jsx-attribute-equals", JsxTagPunctuationStyle), // Equals sign for attributes

            // 4. JSX Expression Punctuation: { and }
            // These have high priority to ensure JS inside them is parsed by JS rules.
            new(@"\{(?![^{}]*?\})", PrioJsxExpressionPunctuation, "jsx-expression-brace open", JsxExpressionPunctuationStyle), // Opening { (not part of object literal if that's handled)
            new(@"\}", PrioJsxExpressionPunctuation, "jsx-expression-brace close", JsxExpressionPunctuationStyle),   // Closing }

            // 5. JSX Text Nodes (content between tags not in {}). Very low priority.
            // Matches any character sequence not containing <, >, {, }
            // This is simplified; proper JSX text parsing is complex.
            new("(?<=>)([^<{}]*)(?=<)", PrioJsxText, "jsx-text", JsxTextStyle, RegexOptions.Singleline),
        };

        JsxRules = jsxSpecificRules.Concat(baseJsRules)
                                   .OrderByDescending(r => r.Priority) // For review, actual order handled by ProcessHighlighting
                                   .ToList();
    }

    public IEnumerable<TokenRule> GetRules() => JsxRules;
}