using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class CSharpLanguageDefinition : ILanguageDefinition
{
    public string Name => "CSharp";

    private const int PrioComment = 100;
    private const int PrioString = 90;
    private const int PrioPreprocessor = 80;
    private const int PrioKeyword = 70;
    private const int PrioKnownType = 60;
    private const int PrioNumber = 50;
    private const int PrioMethodName = 40;
    private const int PrioUserTypeName = 35;
    private const int PrioGeneralIdentifier = 10;
    private const int PrioPunctuation = 5;


    private static readonly List<TokenRule> Rules =
    [
        new("//.*", PrioComment, "comment", null),
        new(@"/\*.*?\*/", PrioComment, "comment", null, RegexOptions.Singleline),
        new("""@"(?:""|[^"])*" """, PrioString, "string", null),
        new("""(?:\\.|[^"])*" """, PrioString, "string", null),
        new(@"'(?:\\.|[^'])*'", PrioString, "string", null),
        new(@"#\s*\w+", PrioPreprocessor, "preprocessor", null),
        new(
            @"\b(public|private|protected|internal|static|class|struct|interface|enum|namespace|using|return|if|else|while|for|foreach|in|switch|case|default|break|continue|try|catch|finally|throw|new|this|base|typeof|sizeof|is|as|get|set|add|remove|value|var|dynamic|async|await|yield|true|false|null|void)\b",
            PrioKeyword, "keyword", null),
        new(
            @"\b(string|String|int|Int32|bool|Boolean|List|Dictionary|IEnumerable|Task|Object|object|decimal|Decimal|double|Double|float|Single|char|Char|byte|Byte|sbyte|SByte|short|Int16|ushort|UInt16|uint|UInt32|long|Int64|ulong|UInt64)\b",
            PrioKnownType, "type known", null),
        new(@"\b\d+(\.\d+)?([eE][+-]?\d+)?([fFdDmM])?\b", PrioNumber, "number", null),
        new(@"0x[0-9a-fA-F]+\b", PrioNumber, "number", null),
        new(@"(?<=\b(class|struct|interface|enum)\s+)([A-Za-z_]\w*)", PrioUserTypeName, "type user",
            null),
        new(@"\b([A-Z][a-z0-9]+)+(\<.+\>)?\b", PrioUserTypeName - 2, "type user-pascal",
            null),
        new(@"\b[A-Za-z_]\w*(?=\s*\()", PrioMethodName, "identifier method", null),
        new(@"\b[A-Z][a-zA-Z0-9_]*(?=\s*\.)", PrioGeneralIdentifier + 1, "identifier namespace-part",
            null),
        new(@"\b[A-Za-z_]\w*\b", PrioGeneralIdentifier, "identifier", null),
        new(@"=>|==|!=|<=|>=|\+\+|--|&&|\|\||\?\?|\?\.|->|<<|>>|\+=|-=|\*=|/=|%=|&=|\|=|^=", PrioPunctuation,
            "operator", null),
        new(@"[(){}\[\].;,+\-*/%&|<>=!~^?:@]", PrioPunctuation, "punctuation", null)

    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}