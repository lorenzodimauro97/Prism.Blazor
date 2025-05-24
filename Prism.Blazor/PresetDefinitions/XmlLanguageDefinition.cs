using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class XmlLanguageDefinition : ILanguageDefinition
{
    public string Name => "Xml";

// Styles
    private const string CommentStyle = "color: #6A9955;"; // Green (like C# comment)
    private const string PiStyle = "color: #C586C0;"; // Magenta (like processing instruction)
    private const string CDataStyle = "color: #808080; font-style: italic;"; // Grey, italic
    private const string DoctypeStyle = "color: #A9A9A9; font-weight: bold;"; // DarkGray, bold
    private const string TagPunctuationStyle = "color: #808080;"; // Grey for <, >, />, =
    private const string TagNameStyle = "color: #569CD6;"; // Blue (like HTML tags)
    private const string AttributeNameStyle = "color: #9CDCFE;"; // Light blue
    private const string AttributeValueStyle = "color: #CE9178;"; // Orange-brown
    private const string EntityStyle = "color: #4EC9B0;"; // Teal

// Priorities
    private const int PrioComment = 100;
    private const int PrioCData = 95;
    private const int PrioPi = 90; // Processing Instructions
    private const int PrioDoctype = 85;
    private const int PrioTag = 70; // Covers tag name and punctuation
    private const int PrioAttribute = 60; // Covers attribute name, equals, and value
    private const int PrioEntity = 50;

    private static readonly List<TokenRule> Rules =
    [
        new(@"<!--[\s\S]*?-->", PrioComment, "comment xml-comment", CommentStyle, RegexOptions.Multiline),
        new(@"<!\[CDATA\[[\s\S]*?]]>", PrioCData, "cdata xml-cdata", CDataStyle, RegexOptions.Multiline),
        new(@"<\?[\s\S]*?\?>", PrioPi, "processing-instruction xml-pi", PiStyle, RegexOptions.Multiline),
        new("<!DOCTYPE[^>]+>", PrioDoctype, "doctype xml-doctype", DoctypeStyle, RegexOptions.IgnoreCase),

        new("</?", PrioTag, "punctuation tag-open xml-tag-punctuation", TagPunctuationStyle),
        new("/?>", PrioTag, "punctuation tag-close xml-tag-punctuation", TagPunctuationStyle),

        new(@"(?<=(</?)\s*)([A-Za-z_][\w:.-]*)", PrioTag - 1, "tag-name xml-tag-name", TagNameStyle),

        new(@"(?<=\s)([A-Za-z_][\w:.-]*)(?=\s*=)", PrioAttribute, "attribute-name xml-attr-name", AttributeNameStyle),
        new("=", PrioAttribute - 1, "operator attribute-equals xml-attr-equals-operator", TagPunctuationStyle),
        new(@"""(?:\\.|[^""\\])*""", PrioAttribute - 2, "string attribute-value xml-attr-value", AttributeValueStyle),
        new(@"'(?:\\.|[^'\\])*'", PrioAttribute - 2, "string attribute-value xml-attr-value", AttributeValueStyle),

        new(@"&(?:[a-zA-Z0-9]+|#\d+|#x[0-9a-fA-F]+);", PrioEntity, "entity xml-entity", EntityStyle),
    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}