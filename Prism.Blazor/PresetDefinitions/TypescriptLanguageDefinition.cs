namespace Prism.Blazor.PresetDefinitions;

public class TypeScriptLanguageDefinition : ILanguageDefinition
{
    public string Name => "TypeScript";

    // Styles (consistent with JavaScript)
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
    private const string ClassNameStyle = "color: #4EC9B0;"; // Also for Interfaces, Enums, Type Aliases
    private const string GlobalBuiltinStyle = "color: #4FC1FF;";
    private const string DecoratorStyle = "color: #DCDCAA;"; // Same as function
    private const string TypeStyle = "color: #4EC9B0;"; // For type annotations, generic parameters
    private const string ModuleNamespaceStyle = "color: #9CDCFE;"; // Like identifiers for now
    private const string TemplateTagPunctuationStyle = "color: #C586C0;";


    // Priorities (higher number = higher priority)
    private const int PrioComment = 100;
    private const int PrioString = 90;
    private const int PrioRegex = 85;
    private const int PrioTemplatePunctuation = 92;
    private const int PrioDecorator = 80;
    private const int PrioKeyword = 70;
    private const int PrioTypeKeywords = 72; // For type, interface, enum, etc.
    private const int PrioBooleanNullUndefined = 68;
    private const int PrioGlobalBuiltin = 65;
    private const int PrioClassName = 62;     // Class, Interface, Enum, Type Alias names
    private const int PrioTypeAnnotation = 60; // For : type
    private const int PrioFunction = 55;
    private const int PrioNumber = 50;
    private const int PrioOperator = 20;
    private const int PrioIdentifier = 10;
    private const int PrioPunctuation = 5;


    private static readonly List<TokenRule> TsRules;

    static TypeScriptLanguageDefinition()
    {
        var jsLang = new JavaScriptLanguageDefinition();
        var jsRules = jsLang.GetRules().ToList();

        // Remove JS-specific class name rule if we have a broader TS one
        jsRules.RemoveAll(r => r.Regex.ToString().Contains(@"?<=\bclass\s+"));
        // Remove JS function name rule to replace with a more general one or TS specific one if needed
        jsRules.RemoveAll(r => r.CssClass is "function identifier" or "function identifier declaration");
        // Remove JS identifier to replace with TS-aware one.
        jsRules.RemoveAll(r => r is { CssClass: "identifier", Priority: PrioIdentifier });


        var tsSpecificRules = new List<TokenRule>
        {
            // 1. Decorators
            new(@"@(?:[A-Za-z_]\w*\.)*[A-Za-z_]\w*(?:\s*\((?:[^)]|\((?:[^)]|\([^)]*\))*\))*\))?", PrioDecorator, "decorator", DecoratorStyle),

            // 2. TypeScript Keywords (additions or distinct handling)
            new(
                @"\b(type|interface|enum|declare|readonly|public|private|protected|module|namespace|abstract|as|asserts|any|async|await|bigint|keyof|infer|is|never|out|satisfies|symbol|unknown|unique|override)\b",
                PrioKeyword, "keyword ts", KeywordStyle), // 'as' needs to be careful not to overmatch variable names. Word boundary helps.
            
            // 3. Type names (Class, Interface, Enum, Type Alias declarations)
            new(@"(?<=\b(class|interface|enum|type)\s+)([A-Za-z_]\w*)", PrioClassName, "type-name definition", ClassNameStyle),
            
            // 4. Type Annotations & Generics (simplified)
            // Matches ': SomeType', ': string[]', ': A | B', etc.
            // Also common generic types like 'Promise<string>' or 'Array<number>'
            new(@":\s*([A-Za-z_]\w*(?:\s*\[\s*\])*(?:<[^>]+>)?(?:\s*\|\s*[A-Za-z_]\w*(?:\s*\[\s*\])*(?:<[^>]+>)?)*)", PrioTypeAnnotation, "type annotation", TypeStyle),
            // General PascalCase identifiers are often types (heuristic)
            new(@"\b([A-Z][a-zA-Z0-9_]*)\b", PrioTypeAnnotation - 5, "type-name usage", TypeStyle),


            // 5. Module/Namespace related keywords (if not covered)
            new(@"(?<=\b(module|namespace)\s+)([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*)", PrioClassName, "module-name", ModuleNamespaceStyle),

            // 6. Function Names (re-add with TS context)
            new(@"\b([A-Za-z_]\w*)\s*(?=\s*\(<[^>]+>|\s*\()", PrioFunction, "function identifier", FunctionStyle), // func<T>(...) or func(...)
            new(@"(?<=\bfunction\*?\s+)([A-Za-z_]\w*)", PrioFunction, "function identifier declaration", FunctionStyle),
            
            // 7. Identifiers (fallback, TS-aware if possible, but general rule is often fine)
             new(@"\b[A-Za-z_]\w*\b", PrioIdentifier, "identifier", IdentifierStyle),

        };

        // Combine JS rules (already modified) with TS specific rules.
        // Ensure TS specific rules that might overlap with JS general rules have appropriate priority.
        TsRules = jsRules.Concat(tsSpecificRules)
                           .OrderByDescending(r => r.Priority) // Not strictly necessary due to processing logic, but good for review
                           .ToList();
        
        // Ensure correct template string handling from JS is preserved or adapted
        // The JS static constructor already refines its rules.
    }

    public IEnumerable<TokenRule> GetRules() => TsRules;
}