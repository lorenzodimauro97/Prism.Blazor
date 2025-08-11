using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class XmlLanguageDefinition : ILanguageDefinition
{
    public string Name => "Xml";
    
    private const int PrioComment = 100;
    private const int PrioCData = 95;
    private const int PrioPi = 90;
    private const int PrioDoctype = 85;
    private const int PrioTag = 70;
    private const int PrioAttribute = 60;
    private const int PrioEntity = 50;

    private static readonly List<TokenRule> Rules =
    [
        new(@"<!--[\s\S]*?-->", PrioComment, "comment xml-comment", null, RegexOptions.Multiline),
        new(@"<!\[CDATA\[[\s\S]*?]]>", PrioCData, "cdata xml-cdata", null, RegexOptions.Multiline),
        new(@"<\?[\s\S]*?\?>", PrioPi, "processing-instruction xml-pi", null, RegexOptions.Multiline),
        new("<!DOCTYPE[^>]+>", PrioDoctype, "doctype xml-doctype", null, RegexOptions.IgnoreCase),
        new("</?", PrioTag, "punctuation tag-open xml-tag-punctuation", null),
        new("/?>", PrioTag, "punctuation tag-close xml-tag-punctuation", null),
        new(@"(?<=(</?)\s*)([A-Za-z_][\w:.-]*)", PrioTag - 1, "tag-name xml-tag-name", null),
        new(@"(?<=\s)([A-Za-z_][\w:.-]*)(?=\s*=)", PrioAttribute, "attribute-name xml-attr-name", null),
        new("=", PrioAttribute - 1, "operator attribute-equals xml-attr-equals-operator", null),
        new(@"""(?:\\.|[^""\\])*""", PrioAttribute - 2, "string attribute-value xml-attr-value", null),
        new(@"'(?:\\.|[^'\\])*'", PrioAttribute - 2, "string attribute-value xml-attr-value", null),
        new(@"&(?:[a-zA-Z0-9]+|#\d+|#x[0-9a-fA-F]+);", PrioEntity, "entity xml-entity", null),
    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}