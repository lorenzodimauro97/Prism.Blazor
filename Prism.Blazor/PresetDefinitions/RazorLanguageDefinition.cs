using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class RazorLanguageDefinition : ILanguageDefinition
{
    public string Name => "Razor";

    // Styles
    private const string HtmlCommentStyle = "color: #6A9955;"; // Greenish, like C# comments
    private const string HtmlDoctypeStyle = "color: #c0c0c0;"; // Light grey
    private const string HtmlTagPunctuationStyle = "color: #808080;"; // Grey for <, >, />
    private const string HtmlTagNameStyle = "color: #569CD6;";       // Blue for tag names (div, span)
    private const string HtmlComponentTagNameStyle = "color: #4EC9B0;"; // Teal for component tags (MyComponent)
    private const string HtmlAttributeNameStyle = "color: #9CDCFE;";  // Light blue for attribute names
    private const string HtmlAttributeValueStyle = "color: #CE9178;"; // Orange/brown for attribute values
    private const string HtmlEntityStyle = "color: #4EC9B0;"; // Teal for   <

    private const string RazorDirectiveStyle = "color: #C586C0; font-weight: bold;"; // Magenta, bold (like C# keywords) for @page, @code etc.
    private const string RazorTransitionStyle = "color: #C586C0;"; // Magenta for @ symbols starting expressions/blocks
    private const string RazorCommentStyle = "color: #6A9955; font-style: italic;"; // Greenish, italic
    private const string RazorBraceStyle = "color: #D4D4D4;"; // Default text color for { } in razor blocks/expressions

    // Priorities (higher number = higher priority)
    // Razor specific must be high to correctly delimit C# and HTML regions
    private const int PrioRazorComment = 200;
    private const int PrioRazorTransitionAndBlock = 190; // For @, @{, @(, @code etc.
    private const int PrioRazorDirective = 185;      // For @page, @using etc. (the keyword part)
    
    private const int PrioHtmlComment = 150;
    private const int PrioHtmlDoctype = 140;
    private const int PrioHtmlTag = 130;             // Includes punctuation and tag name
    private const int PrioHtmlAttribute = 120;       // Includes name and value
    private const int PrioHtmlEntity = 110;

    // C# rules will use their own defined priorities, which should generally be lower than the Razor structural elements.
    // For example, a C# keyword (PrioKeyword = 70 in CSharpLangDef) should be less important than RazorComment.

    private static readonly List<TokenRule> Rules;

    static RazorLanguageDefinition()
    {
        var csharpLang = new CSharpLanguageDefinition();
        var csharpRules = csharpLang.GetRules().ToList();

        var razorAndHtmlRules = new List<TokenRule>
        {
            // 1. Razor Comment (highest priority)
            new(@"@\*[\s\S]*?\*@", PrioRazorComment, "razor comment", RazorCommentStyle, RegexOptions.Multiline),

            // 2. Razor @code, @functions blocks and other directives starting with @
            new(@"@(code|functions)(?=\s*\{)", PrioRazorTransitionAndBlock, "razor directive-block", RazorDirectiveStyle),
            // Common Razor directives
            new(@"@(page|addTagHelper|removeTagHelper|tagHelperPrefix|using|namespace|inject|model|inherits|layout|implements|section|attribute|preservewhitespace|typeparam|rendermode)(?=\s)", PrioRazorDirective, "razor directive", RazorDirectiveStyle),
            
            // Specific handling for @bind and event handlers like @onclick
            // Matches @bind, @bind-Value, @bind:event, @onclick, @onmousemove:preventDefault, etc.
            new(@"@(@?[\w]+(?:[:][\w\-]+)*)(?=\s*(=|\s*{|\s*$))", PrioRazorDirective, "razor directive attribute", RazorDirectiveStyle),

            // 3. Razor explicit expressions and code blocks delimiters
            new(@"@\(", PrioRazorTransitionAndBlock, "razor expression-start", RazorTransitionStyle),
            new(@"@\{", PrioRazorTransitionAndBlock, "razor code-block-start", RazorTransitionStyle),
            // Closing braces/parentheses for Razor blocks/expressions will be matched by C# punctuation rules or a generic punctuation rule if needed.
            // We can add specific styles for them if C# punctuation isn't desired here.
            // For now, relying on C# or a general punctuation rule from C# set for ')' and '}'
            // Or, add specific rules for them if they need unique Razor styling:
            // new TokenRule(@"\)", PrioRazorTransitionAndBlock -1 , "razor expression-end", RazorBraceStyle), // Ensure it's contextually after @(
            // new TokenRule(@"\}", PrioRazorTransitionAndBlock -1, "razor code-block-end", RazorBraceStyle), // Ensure it's contextually after @{ or @code{
            // The above are tricky with regex alone for context. The C# punctuation rules are safer.

            // 4. Single @ transition for implicit expressions (e.g., @MyVariable)
            // This needs to be less greedy than directives.
            // It should ideally only match '@' when followed by a valid C# identifier start.
            new("@(?=[A-Za-z_])", PrioRazorTransitionAndBlock - 5, "razor transition", RazorTransitionStyle),
            
            // 5. Escaped @@
            new("@@", PrioRazorTransitionAndBlock, "razor escaped-at", RazorTransitionStyle),


            // HTML Rules
            // 6. HTML Comments
            new(@"<!--[\s\S]*?-->", PrioHtmlComment, "html comment", HtmlCommentStyle, RegexOptions.Multiline),

            // 7. HTML Doctype
            new("<!DOCTYPE[^>]+>", PrioHtmlDoctype, "html doctype", HtmlDoctypeStyle, RegexOptions.IgnoreCase),

            // 8. HTML Tags (including <, >, /> and tag name)
            //    - Component Tag (PascalCase)
            new(@"(</?)\s*([A-Z][\w.-]*)(?=[^>]*>)", PrioHtmlTag, "html component-tag", HtmlComponentTagNameStyle),
            new(@"([A-Z][\w.-]*)(?=\s*/>)", PrioHtmlTag +1, "html component-tag self-closing-name", HtmlComponentTagNameStyle), // for <MyComponent /> name part
            //    - Standard HTML Tag (lowercase or kebab-case)
            new(@"(</?)\s*([a-z][\w-]*)(?=[^>]*>)", PrioHtmlTag, "html tag", HtmlTagNameStyle),
            new(@"([a-z][\w-]*)(?=\s*/>)", PrioHtmlTag +1, "html tag self-closing-name", HtmlTagNameStyle), // for <div /> name part
            //    - Tag Punctuation
            new("</?|>|/>", PrioHtmlTag -1, "html punctuation tag", HtmlTagPunctuationStyle), // Lower prio so tag name rules match first for coloring

            // 9. HTML Attribute Names (inside tags)
            //    Must be careful not to match content outside tags or C# code.
            //    This regex is complex because it needs to be inside a tag.
            //    A simpler approach: match attribute name then equals then value.
            new(@"(?<=\s)([a-zA-Z0-9\-_:\.]+)(?=\s*=)", PrioHtmlAttribute, "html attribute-name", HtmlAttributeNameStyle),
            // 10. HTML Attribute Values (double and single quoted)
            new(@"(=\s*)""([^""]*)""", PrioHtmlAttribute -1, "html attribute-value quoted", HtmlAttributeValueStyle),
            new(@"(=\s*)'([^']*)'", PrioHtmlAttribute -1, "html attribute-value single-quoted", HtmlAttributeValueStyle),
            // Unquoted attribute values (less common in Razor context due to C#) - simplistic
            // new TokenRule(@"(=\s*)([^\s>""]+)", PrioHtmlAttribute -2, "html attribute-value unquoted", HtmlAttributeValueStyle),
            new("=", PrioHtmlAttribute -2, "html attribute-equals", HtmlTagPunctuationStyle), // The = sign itself

            // 11. HTML Entities
            new("&[a-zA-Z0-9#]+;", PrioHtmlEntity, "html entity", HtmlEntityStyle),
        };

        Rules = razorAndHtmlRules.Concat(csharpRules)
                                  .OrderByDescending(r => r.Priority) // For review; actual order is by index, then priority, then length
                                  .ToList();
        
        // Refinement for attribute values to capture the quotes correctly
        Rules.RemoveAll(r => r.CssClass == "html attribute-value quoted" || r.CssClass == "html attribute-value single-quoted");
        Rules.Add(new TokenRule(@"""(?:\\.|[^""\\])*""", PrioHtmlAttribute -1, "string html-attribute-value", HtmlAttributeValueStyle));
        Rules.Add(new TokenRule(@"'(?:\\.|[^'\\])*'", PrioHtmlAttribute -1, "string html-attribute-value", HtmlAttributeValueStyle));
        
        // Ensure HTML tags are matched robustly, especially their names vs punctuation
        // The current approach tries to match the tag name within the lookahead/lookbehind of punctuation.
        // Let's refine tag matching:
        // 1. Punctuation first: <, </, >, />
        // 2. Then tag names if they follow < or </
        var refinedTagRules = new List<TokenRule>();
        Rules.RemoveAll(r => r.CssClass?.Contains("html punctuation tag") == true || 
                               r.CssClass?.Contains("html tag") == true || 
                               r.CssClass?.Contains("html component-tag") == true);

        refinedTagRules.Add(new TokenRule("</?", PrioHtmlTag + 2, "html punctuation tag-open", HtmlTagPunctuationStyle));
        refinedTagRules.Add(new TokenRule("/?>", PrioHtmlTag + 2, "html punctuation tag-close", HtmlTagPunctuationStyle));
        // Component Tag Name (PascalCase, after < or </)
        refinedTagRules.Add(new TokenRule(@"(?<=</?)\s*([A-Z][\w.-]*)", PrioHtmlTag + 1, "html component-tag-name", HtmlComponentTagNameStyle));
        // Standard HTML Tag Name (lowercase or kebab-case, after < or </)
        refinedTagRules.Add(new TokenRule(@"(?<=</?)\s*([a-z][\w-]*)", PrioHtmlTag + 1, "html html-tag-name", HtmlTagNameStyle));

        Rules.AddRange(refinedTagRules);
        Rules.Sort((a, b) => b.Priority.CompareTo(a.Priority)); // Re-sort for clarity in definition, not for runtime
    }

    public IEnumerable<TokenRule> GetRules() => Rules;
}