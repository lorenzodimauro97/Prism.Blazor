using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Prism.Blazor.Parsers;

public static class MarkdownParser
{
    private static readonly Regex FencedCodeBlockRegex = new(@"^```(\w*)\n([\s\S]*?)\n```\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
    private static readonly Regex HeaderRegex = new(@"^(#{1,6})\s+(.*)", RegexOptions.Compiled);
    private static readonly Regex HrRegex = new(@"^(\*\*\*|---|___)\s*$", RegexOptions.Compiled);
    private static readonly Regex BlockquoteRegex = new(@"^>\s?(.*)", RegexOptions.Compiled);
    private static readonly Regex UlItemRegex = new(@"^\s*([-*+])\s+(.*)", RegexOptions.Compiled);
    private static readonly Regex OlItemRegex = new(@"^\s*(\d+)\.\s+(.*)", RegexOptions.Compiled);

    private static readonly Regex InlineCodeRegex = new(@"`(.+?)`", RegexOptions.Compiled);
    private static readonly Regex ImageRegex = new(@"!\[(.*?)\]\((.*?)\)", RegexOptions.Compiled);
    private static readonly Regex LinkRegex = new(@"\[(.*?)\]\((.*?)\)", RegexOptions.Compiled);
    private static readonly Regex BoldRegex = new(@"(\*\*|__)(?=\S)(.+?)(?<=\S)\1", RegexOptions.Compiled);
    private static readonly Regex ItalicRegex = new(@"(\*|_)(?=\S)(.+?)(?<=\S)\1", RegexOptions.Compiled);
    private static readonly Regex StrikethroughRegex = new(@"~~(?=\S)(.+?)(?<=\S)~~", RegexOptions.Compiled);

    public static string ToHtml(string markdown, bool useHardLineBreaks = false)
    {
        if (string.IsNullOrEmpty(markdown)) return string.Empty;

        var text = markdown.Replace("\r\n", "\n").Replace("\r", "\n");

        var codeBlockPlaceholders = new Dictionary<string, string>();
        int placeholderIdCounter = 0;
        text = FencedCodeBlockRegex.Replace(text, m =>
        {
            var lang = m.Groups[1].Value;
            var code = m.Groups[2].Value;
            var encodedCode = HttpUtility.HtmlEncode(code);
            var placeholder = $"##CODEBLOCK_PLACEHOLDER_{placeholderIdCounter++}##";
            var langAttribute = !string.IsNullOrEmpty(lang) ? $" class=\"language-{HttpUtility.HtmlAttributeEncode(lang.ToLowerInvariant())}\"" : "";
            codeBlockPlaceholders[placeholder] = $"<pre><code{langAttribute}>{encodedCode}</code></pre>";
            return placeholder;
        });

        var sb = new StringBuilder();
        var lines = text.Split('\n');
        var currentParagraphContent = new StringBuilder();
        bool inList = false;
        string? listType = null; 

        Action FinalizeParagraph = () => {
            if (currentParagraphContent.Length > 0)
            {
                sb.Append($"<p>{ProcessInlines(currentParagraphContent.ToString())}</p>\n");
                currentParagraphContent.Clear();
            }
        };
        
        Action FinalizeList = () => {
            if (inList)
            {
                sb.Append(listType == "ul" ? "</ul>\n" : "</ol>\n");
                inList = false;
                listType = null;
            }
        };

        for(int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmedLine = line.Trim();
            
            if (codeBlockPlaceholders.TryGetValue(trimmedLine, out var placeholder))
            {
                FinalizeParagraph(); FinalizeList();
                sb.Append(placeholder).Append('\n');
                continue;
            }
            
            if (string.IsNullOrWhiteSpace(line))
            {
                FinalizeParagraph(); FinalizeList();
                continue;
            }

            var headerMatch = HeaderRegex.Match(line);
            if (headerMatch.Success)
            {
                FinalizeParagraph(); FinalizeList();
                var level = headerMatch.Groups[1].Value.Length;
                var content = ProcessInlines(headerMatch.Groups[2].Value.Trim());
                sb.Append($"<h{level}>{content}</h{level}>\n");
                continue;
            }

            if (HrRegex.IsMatch(line))
            {
                FinalizeParagraph(); FinalizeList();
                sb.Append("<hr />\n");
                continue;
            }

            var blockquoteMatch = BlockquoteRegex.Match(line);
            if (blockquoteMatch.Success)
            {
                FinalizeParagraph(); FinalizeList();
                var content = ProcessInlines(blockquoteMatch.Groups[1].Value.Trim());
                sb.Append($"<blockquote><p>{content}</p></blockquote>\n");
                continue;
            }
            
            var ulMatch = UlItemRegex.Match(line);
            if (ulMatch.Success)
            {
                FinalizeParagraph();
                if (!inList || listType != "ul") { FinalizeList(); sb.Append("<ul>\n"); inList = true; listType = "ul"; }
                var content = ProcessInlines(ulMatch.Groups[2].Value.Trim());
                sb.Append($"<li>{content}</li>\n");
                continue;
            }
            
            var olMatch = OlItemRegex.Match(line);
            if (olMatch.Success)
            {
                FinalizeParagraph();
                if (!inList || listType != "ol") { FinalizeList(); sb.Append("<ol>\n"); inList = true; listType = "ol"; }
                var content = ProcessInlines(olMatch.Groups[2].Value.Trim());
                sb.Append($"<li>{content}</li>\n");
                continue;
            }

            FinalizeList(); 
            if (currentParagraphContent.Length > 0)
            {
                currentParagraphContent.Append(useHardLineBreaks ? "<br />\n" : " ");
            }
            currentParagraphContent.Append(trimmedLine); 
        }

        FinalizeParagraph(); 
        FinalizeList();      

        return sb.ToString();
    }

    private static string ProcessInlines(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var current = text;

        current = InlineCodeRegex.Replace(current, m => $"<code>{HttpUtility.HtmlEncode(m.Groups[1].Value)}</code>");
        current = ImageRegex.Replace(current, m =>
            $"<img src=\"{HttpUtility.HtmlAttributeEncode(m.Groups[2].Value)}\" alt=\"{HttpUtility.HtmlAttributeEncode(m.Groups[1].Value)}\" />");
        current = LinkRegex.Replace(current, m =>
            $"<a href=\"{HttpUtility.HtmlAttributeEncode(m.Groups[2].Value)}\">{HttpUtility.HtmlEncode(m.Groups[1].Value)}</a>");
        
        current = BoldRegex.Replace(current, m => $"<strong>{HttpUtility.HtmlEncode(m.Groups[2].Value)}</strong>");
        current = ItalicRegex.Replace(current, m => $"<em>{HttpUtility.HtmlEncode(m.Groups[2].Value)}</em>");
        current = StrikethroughRegex.Replace(current, m => $"<del>{HttpUtility.HtmlEncode(m.Groups[2].Value)}</del>");

        return current;
    }
}