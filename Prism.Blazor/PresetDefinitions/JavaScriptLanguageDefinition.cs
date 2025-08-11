using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class JavaScriptLanguageDefinition : ILanguageDefinition
{
    public string Name => "JavaScript";
    
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
        new("//.*", PrioComment, "comment", null),
        new(@"/\*[\s\S]*?\*/", PrioComment, "comment", null, RegexOptions.Multiline),

        new("`", PrioTemplatePunctuation, "template-punctuation", null),
        new(@"\$\{[^}]*\}", PrioTemplatePunctuation, "template-interpolation", null),
        new(
            @"(?<=`)(?:[^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=\})([^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=`)([^`\\$]|\\[\s\S]|\$(?!\{))*(?=\$\{|\})",
            PrioString - 1, "string template-segment", null),

        new(""" "(?:\\.|[^"\\])*" """, PrioString, "string", null),
        new(@"'(?:\\.|[^'\\])*'", PrioString, "string", null),

        new(@"/(?![\s=*/+-])(?:\\.|\[(?:\\.|[^\]\\])*\]|[^/\r\n\\])+/[gimyusv]*", PrioRegex, "regex", null),

        new(
            @"\b(abstract|arguments|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|function|if|import|in|instanceof|let|new|return|static|super|switch|this|throw|try|typeof|var|void|while|with|yield|get|set|of)\b",
            PrioKeyword, "keyword", null),

        new(@"\b(true|false|null|undefined)\b", PrioBooleanNullUndefined, "literal boolean-null",
            null),

        new(@"\b0[xX][0-9a-fA-F]+\b", PrioNumber, "number hex", null),
        new(@"\b0[bB][01]+\b", PrioNumber, "number binary", null),
        new(@"\b0[oO][0-7]+\b", PrioNumber, "number octal", null),
        new(@"\b(?:\d*\.\d+|\d+\.?)(?:[eE][+-]?\d+)?\b", PrioNumber, "number decimal", null),

        new(
            @"\b(Array|Boolean|Date|Error|Function|JSON|Math|Number|Object|Promise|Proxy|Reflect|RegExp|String|Symbol|Map|Set|WeakMap|WeakSet|parseInt|parseFloat|isNaN|isFinite|decodeURI|decodeURIComponent|encodeURI|encodeURIComponent|console|window|document|globalThis|Infinity|NaN)\b",
            PrioGlobalBuiltin, "global builtin", null),

        new(@"(?<=\bclass\s+)([A-Za-z_]\w*)", PrioClassName, "class-name", null),
        new(@"\b([A-Za-z_]\w*)\s*(?=\()", PrioFunction, "function identifier", null),
        new(@"(?<=\bfunction\*?\s+)([A-Za-z_]\w*)", PrioFunction, "function identifier declaration", null),

        new(
            @"=>|(\+\+|--|&&|\|\||\?\?|\*\*|\+\=|-=|\*=|/=|%=|&=|\|=|^=|<<=|>>=|>>>=)|[+\-*/%&|^~<>=!?:]|<<|>>|>>>",
            PrioOperator, "operator", null),

        new(@"\b[A-Za-z_]\w*\b", PrioIdentifier, "identifier", null),
        new(@"[{}()\[\].,;]", PrioPunctuation, "punctuation", null)
    ];

    private static readonly List<TokenRule> Rules;

    static JavaScriptLanguageDefinition()
    {
        var refinedRules = new List<TokenRule>
        {
            new("//.*", PrioComment, "comment", null),
            new(@"/\*[\s\S]*?\*/", PrioComment, "comment", null, RegexOptions.Multiline),

            new("`", PrioTemplatePunctuation + 1, "string template-delimiter", null),
            new(@"\$\{[^}]*\}", PrioTemplatePunctuation, "string template-interpolation", null),
            new(
                @"(?<=`)(?:[^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=\})([^`\\$]|\\[\s\S]|\$(?!\{))*(?=`)|(?<=`)([^`\\$]|\\[\s\S]|\$(?!\{))*(?=\$\{|\})",
                PrioString - 1, "string template-segment", null),


            new(""" "(?:\\.|[^"\\])*" """, PrioString, "string", null),
            new(@"'(?:\\.|[^'\\])*'", PrioString, "string", null),

            new(@"/(?![\s=*/+-])(?:\\.|\[(?:\\.|[^\]\\])*\]|[^/\r\n\\])+/[gimyusv]*", PrioRegex, "regex", null),
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