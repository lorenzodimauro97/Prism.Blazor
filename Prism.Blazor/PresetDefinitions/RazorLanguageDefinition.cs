using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions;

public class RazorLanguageDefinition : ILanguageDefinition
{
    public string Name => "Razor";

    private const int PrioRazorComment = 200;
    private const int PrioRazorTransitionAndBlock = 190;
    private const int PrioRazorDirective = 185;
    private const int PrioHtmlComment = 150;
    private const int PrioHtmlDoctype = 140;
    private const int PrioHtmlTag = 130;
    private const int PrioHtmlAttribute = 120;
    private const int PrioHtmlEntity = 110;

    private static readonly List<TokenRule> Rules;

    static RazorLanguageDefinition()
    {
        var csharpLang = new CSharpLanguageDefinition();
        var csharpRules = csharpLang.GetRules().ToList();

        var razorAndHtmlRules = new List<TokenRule>
        {
            new(@"@\*[\s\S]*?\*@", PrioRazorComment, "razor comment", null, RegexOptions.Multiline),
            new(@"@(code|functions)(?=\s*\{)", PrioRazorTransitionAndBlock, "razor directive-block", null),
            new(
                @"@(page|addTagHelper|removeTagHelper|tagHelperPrefix|using|namespace|inject|model|inherits|layout|implements|section|attribute|preservewhitespace|typeparam|rendermode)(?=\s)",
                PrioRazorDirective, "razor directive", null),
            new(@"@(@?[\w]+(?:[:][\w\-]+)*)(?=\s*(=|\s*{|\s*$))", PrioRazorDirective, "razor directive attribute",
                null),
            new(@"@\(", PrioRazorTransitionAndBlock, "razor expression-start", null),
            new(@"@\{", PrioRazorTransitionAndBlock, "razor code-block-start", null),
            new("@(?=[A-Za-z_])", PrioRazorTransitionAndBlock - 5, "razor transition", null),
            new("@@", PrioRazorTransitionAndBlock, "razor escaped-at", null),
            new(@"<!--[\s\S]*?-->", PrioHtmlComment, "html comment", null, RegexOptions.Multiline),
            new("<!DOCTYPE[^>]+>", PrioHtmlDoctype, "html doctype", null, RegexOptions.IgnoreCase),
            new(@"(</?)\s*([A-Z][\w.-]*)(?=[^>]*>)", PrioHtmlTag, "html component-tag", null),
            new(@"([A-Z][\w.-]*)(?=\s*/>)", PrioHtmlTag + 1, "html component-tag self-closing-name", null),
            new(@"(</?)\s*([a-z][\w-]*)(?=[^>]*>)", PrioHtmlTag, "html tag", null),
            new(@"([a-z][\w-]*)(?=\s*/>)", PrioHtmlTag + 1, "html tag self-closing-name", null),
            new("</?|>|/>", PrioHtmlTag - 1, "html punctuation tag", null),
            new(@"(?<=\s)([a-zA-Z0-9\-_:\.]+)(?=\s*=)", PrioHtmlAttribute, "html attribute-name", null),
            new(@"(=\s*)""([^""]*)""", PrioHtmlAttribute - 1, "html attribute-value quoted", null),
            new(@"(=\s*)'([^']*)'", PrioHtmlAttribute - 1, "html attribute-value single-quoted", null),
            new("=", PrioHtmlAttribute - 2, "html attribute-equals", null),
            new("&[a-zA-Z0-9#]+;", PrioHtmlEntity, "html entity", null),
        };

        Rules = razorAndHtmlRules.Concat(csharpRules)
            .OrderByDescending(r => r.Priority)
            .ToList();

        Rules.RemoveAll(r =>
            r.CssClass == "html attribute-value quoted" || r.CssClass == "html attribute-value single-quoted");
        Rules.Add(new TokenRule(@"""(?:\\.|[^""\\])*""", PrioHtmlAttribute - 1, "string html-attribute-value", null));
        Rules.Add(new TokenRule(@"'(?:\\.|[^'\\])*'", PrioHtmlAttribute - 1, "string html-attribute-value", null));

        var refinedTagRules = new List<TokenRule>();
        Rules.RemoveAll(r => r.CssClass?.Contains("html punctuation tag") == true ||
                             r.CssClass?.Contains("html tag") == true ||
                             r.CssClass?.Contains("html component-tag") == true);

        refinedTagRules.Add(new TokenRule("</?", PrioHtmlTag + 2, "html punctuation tag-open", null));
        refinedTagRules.Add(new TokenRule("/?>", PrioHtmlTag + 2, "html punctuation tag-close", null));
        refinedTagRules.Add(new TokenRule(@"(?<=</?)\s*([A-Z][\w.-]*)", PrioHtmlTag + 1, "html component-tag-name",
            null));
        refinedTagRules.Add(new TokenRule(@"(?<=</?)\s*([a-z][\w-]*)", PrioHtmlTag + 1, "html html-tag-name", null));

        Rules.AddRange(refinedTagRules);
        Rules.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public IEnumerable<TokenRule> GetRules() => Rules;
}