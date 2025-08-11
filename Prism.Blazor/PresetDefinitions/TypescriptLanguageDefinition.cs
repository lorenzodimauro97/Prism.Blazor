namespace Prism.Blazor.PresetDefinitions;

public class TypeScriptLanguageDefinition : ILanguageDefinition
{
    public string Name => "TypeScript";

    private const int PrioDecorator = 80;
    private const int PrioKeyword = 70;
    private const int PrioClassName = 62;
    private const int PrioTypeAnnotation = 60;
    private const int PrioFunction = 55;
    private const int PrioIdentifier = 10;

    private static readonly List<TokenRule> TsRules;

    static TypeScriptLanguageDefinition()
    {
        var jsLang = new JavaScriptLanguageDefinition();
        var jsRules = jsLang.GetRules().ToList();

        jsRules.RemoveAll(r => r.Regex.ToString().Contains(@"?<=\bclass\s+"));
        jsRules.RemoveAll(r => r.CssClass is "function identifier" or "function identifier declaration");
        jsRules.RemoveAll(r => r is { CssClass: "identifier", Priority: PrioIdentifier });

        var tsSpecificRules = new List<TokenRule>
        {
            new(@"@(?:[A-Za-z_]\w*\.)*[A-Za-z_]\w*(?:\s*\((?:[^)]|\((?:[^)]|\([^)]*\))*\))*\))?", PrioDecorator,
                "decorator", null),
            new(
                @"\b(type|interface|enum|declare|readonly|public|private|protected|module|namespace|abstract|as|asserts|any|async|await|bigint|keyof|infer|is|never|out|satisfies|symbol|unknown|unique|override)\b",
                PrioKeyword, "keyword ts", null),
            new(@"(?<=\b(class|interface|enum|type)\s+)([A-Za-z_]\w*)", PrioClassName, "type-name definition", null),
            new(@":\s*([A-Za-z_]\w*(?:\s*\[\s*\])*(?:<[^>]+>)?(?:\s*\|\s*[A-Za-z_]\w*(?:\s*\[\s*\])*(?:<[^>]+>)?)*)",
                PrioTypeAnnotation, "type annotation", null),
            new(@"\b([A-Z][a-zA-Z0-9_]*)\b", PrioTypeAnnotation - 5, "type-name usage", null),
            new(@"(?<=\b(module|namespace)\s+)([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*)", PrioClassName, "module-name", null),
            new(@"\b([A-Za-z_]\w*)\s*(?=\s*\(<[^>]+>|\s*\()", PrioFunction, "function identifier", null),
            new(@"(?<=\bfunction\*?\s+)([A-Za-z_]\w*)", PrioFunction, "function identifier declaration", null),
            new(@"\b[A-Za-z_]\w*\b", PrioIdentifier, "identifier", null),

        };

        TsRules = jsRules.Concat(tsSpecificRules)
            .OrderByDescending(r => r.Priority)
            .ToList();
    }

    public IEnumerable<TokenRule> GetRules() => TsRules;
}