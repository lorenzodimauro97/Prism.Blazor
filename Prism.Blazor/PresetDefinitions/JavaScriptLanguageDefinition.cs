using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class JavaScriptLanguageDefinition : ILanguageDefinition
{
    public string Name => "JavaScript";

    private const string CommentStyle = "color: #6A9955;";
    private const string StringStyle = "color: #CE9178;";
    private const string RegexStyle = "color: #d16969;";
    private const string KeywordStyle = "color: #C586C0;";
    private const string BooleanNullUndefinedStyle = "color: #569cd6;";
    private const string NumberStyle = "color: #B5CEA8;";
    private const string OperatorStyle = "color: #D4D4D4;";
    private const string PunctuationStyle = "color: #D4D4D4;";
    private const string IdentifierStyle = "color: #9CDCFE;";
    private const string FunctionStyle = "color: #DCDCAA;";
    private const string ClassNameStyle = "color: #4EC9B0;";
    private const string GlobalBuiltinStyle = "color: #4FC1FF;";
    private const string TemplateTagPunctuationStyle = "color: #C586C0;";

    private const int PrioComment = 100;
    private const int PrioString = 90;
    private const int PrioRegex = 85;
    private const int PrioTemplatePunctuation = 92;
    private const int PrioKeyword = 70;
    private const int PrioBooleanNullUndefined = 68;
    private const int PrioGlobalBuiltin = 65;
    private const int PrioClassName = 62;
    private const int PrioFunction = 55;
    private const int PrioNumber = 50;
    private const int PrioOperator = 20;
    private const int PrioIdentifier = 10;
    private const int PrioPunctuation = 5;

    private static readonly List<TokenRule> InitialRules =
    [
        new("//.*", PrioComment, "comment", CommentStyle),
        new(@"/\*[\s\S]*?\*/", PrioComment, "comment", CommentStyle, RegexOptions.Multiline),

        new("`", PrioTemplatePunctuation, "template-punctuation", TemplateTagPunctuationStyle),
        new(@"\$\{[^}]*\}", PrioTemplatePunctuation, "template-interpolation", TemplateTagPunctuationStyle),
        new(
            @"(?<=`)(?:[^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=\})([^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=`)([^`\\$]|\\[\s\S]|\$(?!\{))*(?=\$\{|\})",
            PrioString - 1, "string template-segment", StringStyle),

        new("""
            "(?:\\.|[^"\\])*"
            """, PrioString, "string", StringStyle),
        new(@"'(?:\\.|[^'\\])*'", PrioString, "string", StringStyle),

        new(@"/(?![\s=*/+-])(?:\\.|\[(?:\\.|[^\]\\])*\]|[^/\r\n\\])+/[gimyusv]*", PrioRegex, "regex", RegexStyle),

        new(
            @"\b(abstract|arguments|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|function|if|import|in|instanceof|let|new|return|static|super|switch|this|throw|try|typeof|var|void|while|with|yield|get|set|of)\b",
            PrioKeyword, "keyword", KeywordStyle),

        new(@"\b(true|false|null|undefined)\b", PrioBooleanNullUndefined, "literal boolean-null",
            BooleanNullUndefinedStyle),

        new(@"\b0[xX][0-9a-fA-F]+\b", PrioNumber, "number hex", NumberStyle),
        new(@"\b0[bB][01]+\b", PrioNumber, "number binary", NumberStyle),
        new(@"\b0[oO][0-7]+\b", PrioNumber, "number octal", NumberStyle),
        new(@"\b(?:\d*\.\d+|\d+\.?)(?:[eE][+-]?\d+)?\b", PrioNumber, "number decimal", NumberStyle),

        new(
            @"\b(Array|Boolean|Date|Error|Function|JSON|Math|Number|Object|Promise|Proxy|Reflect|RegExp|String|Symbol|Map|Set|WeakMap|WeakSet|parseInt|parseFloat|isNaN|isFinite|decodeURI|decodeURIComponent|encodeURI|encodeURIComponent|console|window|document|globalThis|Infinity|NaN)\b",
            PrioGlobalBuiltin, "global builtin", GlobalBuiltinStyle),

        new(@"(?<=\bclass\s+)([A-Za-z_]\w*)", PrioClassName, "class-name", ClassNameStyle),
        new(@"\b([A-Za-z_]\w*)\s*(?=\()", PrioFunction, "function identifier", FunctionStyle),
        new(@"(?<=\bfunction\*?\s+)([A-Za-z_]\w*)", PrioFunction, "function identifier declaration", FunctionStyle),

        new(
            @"=>|(\+\+|--|&&|\|\||\?\?|\*\*|\+\=|-=|\*=|/=|%=|&=|\|=|^=|<<=|>>=|>>>=)|[+\-*/%&|^~<>=!?:]|<<|>>|>>>",
            PrioOperator, "operator", OperatorStyle),

        new(@"\b[A-Za-z_]\w*\b", PrioIdentifier, "identifier", IdentifierStyle),
        new(@"[{}()\[\].,;]", PrioPunctuation, "punctuation", PunctuationStyle)
    ];

    private static readonly List<TokenRule> Rules;

    static JavaScriptLanguageDefinition()
    {
        var refinedRules = new List<TokenRule>
        {
            new("//.*", PrioComment, "comment", CommentStyle),
            new(@"/\*[\s\S]*?\*/", PrioComment, "comment", CommentStyle, RegexOptions.Multiline),

            new("`", PrioTemplatePunctuation + 1, "string template-delimiter", StringStyle),
            new(@"\$\{[^}]*\}", PrioTemplatePunctuation, "string template-interpolation", StringStyle),
            new(
                @"(?<=`)(?:[^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=\})([^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=`)([^`\\$]|\\[\s\S]|\$(?!\{))*(?=\$\{|\})",
                PrioString - 1, "string template-segment", StringStyle),


            new("""
                "(?:\\.|[^"\\])*"
                """, PrioString, "string", StringStyle),
            new(@"'(?:\\.|[^'\\])*'", PrioString, "string", StringStyle),

            new(@"/(?![\s=*/+-])(?:\\.|\[(?:\\.|[^\]\\])*\]|[^/\r\n\\])+/[gimyusv]*", PrioRegex, "regex", RegexStyle),
        };

        refinedRules.AddRange(InitialRules.FindAll(r =>
            r.CssClass != "comment" &&
            r.CssClass != "string" &&
            r.CssClass != "regex" &&
            r.CssClass != "template-punctuation" &&
            r.CssClass != "template-interpolation" &&
            r.CssClass != "string template-segment"
        ));

        Rules = refinedRules;
    }

    public IEnumerable<TokenRule> GetRules() => Rules;
}