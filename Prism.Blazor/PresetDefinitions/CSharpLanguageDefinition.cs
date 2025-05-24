using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class CSharpLanguageDefinition : ILanguageDefinition
{
    public string Name => "CSharp";

    // Rider-inspired Color Palette (approximations)
    private const string KeywordStyle = "color: #569cd6; font-weight: bold;";
    private const string TypeStyle = "color: #4EC9B0;";
    private const string IdentifierStyle = "color: #9876AA;"; // General identifiers
    private const string UserTypeStyle = "color: #2E8B57;"; // SeaGreen for user-defined types like AppSettings (distinct from built-in)
    private const string MethodNameStyle = "color: #FFC66D;";
    private const string StringLiteralStyle = "color: #6A8759;";
    private const string CommentStyle = "color: #629755;";
    private const string PreprocessorStyle = "color: #BBB529;";
    private const string NumberStyle = "color: #6897BB;";
    private const string PunctuationStyle = "color: #D4D4D4;";
    private const string NamespaceIdentifierStyle = "color: #B0B0B0;"; // A slightly different grey for namespace parts

    // Priorities (higher number = higher priority)
    private const int PrioComment = 100;
    private const int PrioString = 90;
    private const int PrioPreprocessor = 80;
    private const int PrioKeyword = 70;
    private const int PrioKnownType = 60; // Built-in types
    private const int PrioNumber = 50;
    private const int PrioMethodName = 40;
    // For user types, property names etc., we'll use a general identifier rule for now or more specific ones.
    // Let's add a specific rule for Class/Struct/Interface/Enum names (User Types)
    private const int PrioUserTypeName = 35; // e.g., AppSettings in 'class AppSettings'
    private const int PrioGeneralIdentifier = 10; // General variables, parameters, field/property names
    private const int PrioPunctuation = 5;


    private static readonly List<TokenRule> Rules =
    [
        new("//.*", PrioComment, "comment", CommentStyle),
        new(@"/\*.*?\*/", PrioComment, "comment", CommentStyle, RegexOptions.Singleline),

        // 2. Strings & Chars
        new("""
            @"(?:""|[^"])*"
            """, PrioString, "string", StringLiteralStyle),
        new("""
            "(?:\\.|[^"])*"
            """, PrioString, "string", StringLiteralStyle),
        new(@"'(?:\\.|[^'])*'", PrioString, "string", StringLiteralStyle),

        // 3. Preprocessor directives
        new(@"#\s*\w+", PrioPreprocessor, "preprocessor", PreprocessorStyle),

        // 4. Keywords (Very high priority for code structure)
        new(
            @"\b(public|private|protected|internal|static|class|struct|interface|enum|namespace|using|return|if|else|while|for|foreach|in|switch|case|default|break|continue|try|catch|finally|throw|new|this|base|typeof|sizeof|is|as|get|set|add|remove|value|var|dynamic|async|await|yield|true|false|null|void)\b",
            PrioKeyword, "keyword", KeywordStyle),

        // 5. Known System Types (High priority)

        new(
            @"\b(string|String|int|Int32|bool|Boolean|List|Dictionary|IEnumerable|Task|Object|object|decimal|Decimal|double|Double|float|Single|char|Char|byte|Byte|sbyte|SByte|short|Int16|ushort|UInt16|uint|UInt32|long|Int64|ulong|UInt64)\b",
            PrioKnownType, "type known", TypeStyle),

        // 6. Numbers

        new(@"\b\d+(\.\d+)?([eE][+-]?\d+)?([fFdDmM])?\b", PrioNumber, "number", NumberStyle),
        new(@"0x[0-9a-fA-F]+\b", PrioNumber, "number", NumberStyle),

        // 7. User-defined type names (e.g., class AppSettings, struct MyStruct)
        //    Looks for identifiers that follow 'class', 'struct', 'interface', 'enum'
        //    Or for generic type parameters like T in List<T> (more complex, omit for now for simplicity)
        new(@"(?<=\b(class|struct|interface|enum)\s+)([A-Za-z_]\w*)", PrioUserTypeName, "type user",
            UserTypeStyle),
        // Also, type names when used as parameters or variable types, e.g. "AppSettings settings"
        // This is harder without full parsing. A heuristic could be PascalCase identifiers if not matched by keywords/known types.
        // For now, AppSettings in "public AppSettings AppSettings" will be PrioGeneralIdentifier or PrioUserTypeName if we add another rule for it.
        // Let's make a simple PascalCase rule for types that are not keywords or known types.
        // This rule should be lower priority than methods if a method name is also PascalCase.
        new(@"\b([A-Z][a-z0-9]+)+(\<.+\>)?\b", PrioUserTypeName - 2, "type user-pascal",
            UserTypeStyle), // Slightly lower prio for general PascalCase

        // 8. Identifiers (Method Names - heuristic: followed by an opening parenthesis)
        //    Make sure this doesn't clash with type constructors like 'new List<string>()'
        //    'new' is a keyword. 'List' is a type. So method regex won't apply to 'List'.
        new(@"\b[A-Za-z_]\w*(?=\s*\()", PrioMethodName, "identifier method", MethodNameStyle),

        // 9. Namespace parts (heuristic: PascalCase identifiers often part of namespaces, before a dot)
        // This is tricky, as it can overlap with type names.
        new(@"\b[A-Z][a-zA-Z0-9_]*(?=\s*\.)", PrioGeneralIdentifier + 1, "identifier namespace-part",
            NamespaceIdentifierStyle),


        // 10. General Identifiers (Properties, Fields, local variables, parameters)
        //     This is a fallback, so lower priority.
        new(@"\b[A-Za-z_]\w*\b", PrioGeneralIdentifier, "identifier", IdentifierStyle),

        // 11. Operators & Punctuation (Lowest specific code priority)
        new(@"=>|==|!=|<=|>=|\+\+|--|&&|\|\||\?\?|\?\.|->|<<|>>|\+=|-=|\*=|/=|%=|&=|\|=|^=", PrioPunctuation,
            "operator", PunctuationStyle),
        new(@"[(){}\[\].;,+\-*/%&|<>=!~^?:@]", PrioPunctuation, "punctuation", PunctuationStyle)

    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}