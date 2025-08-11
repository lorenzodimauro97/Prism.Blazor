using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions
{
    public class CssLanguageDefinition : ILanguageDefinition
    {
        public string Name => "Css";
        
        private const int PrioComment = 100;
        private const int PrioString = 90;
        private const int PrioAtRuleKeyword = 85;
        private const int PrioImportant = 80;
        private const int PrioFunction = 75;
        private const int PrioPropertyName = 70;
        private const int PrioValueKeyword = 68;
        private const int PrioValueNumeric = 66;
        private const int PrioSelectorId = 60;
        private const int PrioSelectorClass = 59;
        private const int PrioSelectorAttribute = 58;
        private const int PrioSelectorPseudo = 57;
        private const int PrioSelectorUniversal = 56;
        private const int PrioSelectorTag = 55;
        private const int PrioPunctuation = 5;

        private static readonly List<TokenRule> Rules =
        [
            new(@"/\*.*?\*/", PrioComment, "comment", null, RegexOptions.Singleline),
            new(""" "(?:\\.|[^"\r\n])*"|'(?:\\.|[^'\r\n])*' """, PrioString, "string", null),
            new(
                @"@(?:charset|import|namespace|media|supports|document|page|font-face|keyframes|viewport|counter-style|font-feature-values|property|layer|scope)\b",
                PrioAtRuleKeyword, "keyword at-rule", null, RegexOptions.IgnoreCase),
            new(@"!important\b", PrioImportant, "keyword important", null, RegexOptions.IgnoreCase),
            new(
                """\b(?:url|rgb|rgba|hsl|hsla|var|calc|attr|linear-gradient|radial-gradient|repeating-linear-gradient|repeating-radial-gradient|rotate|scale|translate|matrix|perspective|skew|format|local|clamp|min|max|fit-content|minmax|repeat|conic-gradient|paint|layout|element|cross-fade|image-set|image|counter|counters|rect|path|polygon|circle|ellipse|inset|blur|brightness|contrast|drop-shadow|grayscale|hue-rotate|invert|opacity|saturate|sepia|steps|cubic-bezier|frames|symbols)\s*\((?:[^()"']|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*')*\)""",
                PrioFunction, "function", null, RegexOptions.IgnoreCase),
            new(@"(?:(?:--|-)[\w-]+|\b[a-z][\w-]*)(?=\s*:)", PrioPropertyName, "property-name",
                null, RegexOptions.IgnoreCase),
            new(
                @"\b(?:transparent|currentColor|none|hidden|visible|auto|inherit|initial|unset|revert|normal|italic|oblique|bold|bolder|lighter|serif|sans-serif|monospace|cursive|fantasy|system-ui|ui-serif|ui-sans-serif|ui-monospace|ui-rounded|emoji|math|fangsong" +
                "|small-caps|all-small-caps|petite-caps|all-petite-caps|unicase|titling-caps" +
                "|left|right|center|justify|start|end|match-parent|justify-all|justify-self" +
                "|top|bottom|middle|baseline|sub|super|text-top|text-bottom" +
                "|absolute|relative|fixed|static|sticky" +
                "|block|inline|inline-block|flex|inline-flex|grid|inline-grid|table|inline-table|table-row|table-cell|list-item|contents|flow-root|none" +
                "|solid|dashed|dotted|double|groove|ridge|inset|outset" +
                "|repeat|no-repeat|repeat-x|repeat-y|round|space" +
                "|scroll|fixed|local" +
                "|contain|cover|closest-side|farthest-side|closest-corner|farthest-corner|辺|角" +
                "|ease|linear|ease-in|ease-out|ease-in-out|step-start|step-end" +
                "|forwards|backwards|both|running|paused" +
                "|hover|active|focus|visited|link|target|disabled|enabled|checked|indeterminate|required|optional|valid|invalid|in-range|out-of-range|read-only|read-write" +
                "|root|empty|first-child|last-child|only-child|first-of-type|last-of-type|only-of-type|not|is|where|has|any-link|default|lang|dir|scope|all|revert-layer" +
                "|true|false" +
                "|aliceblue|antiquewhite|aqua|aquamarine|azure|beige|bisque|black|blanchedalmond|blue|blueviolet|brown|burlywood|cadetblue|chartreuse|chocolate|coral|cornflowerblue|cornsilk|crimson|cyan" +
                "|darkblue|darkcyan|darkgoldenrod|darkgray|darkgreen|darkgrey|darkkhaki|darkmagenta|darkolivegreen|darkorange|darkorchid|darkred|darksalmon|darkseagreen|darkslateblue|darkslategray|darkslategrey|darkturquoise|darkviolet" +
                "|deeppink|deepskyblue|dimgray|dimgrey|dodgerblue" +
                "|firebrick|floralwhite|forestgreen|fuchsia" +
                "|gainsboro|ghostwhite|gold|goldenrod|gray|green|greenyellow|grey" +
                "|honeydew|hotpink" +
                "|indianred|indigo|ivory" +
                "|khaki" +
                "|lavender|lavenderblush|lawngreen|lemonchiffon|lightblue|lightcoral|lightcyan|lightgoldenrodyellow|lightgray|lightgreen|lightgrey|lightpink|lightsalmon|lightseagreen|lightskyblue|lightslategray|lightslategrey|lightsteelblue|lightyellow|lime|limegreen|linen" +
                "|magenta|maroon|mediumaquamarine|mediumblue|mediumorchid|mediumpurple|mediumseagreen|mediumslateblue|mediumspringgreen|mediumturquoise|mediumvioletred|midnightblue|mintcream|mistyrose|moccasin" +
                "|navajowhite|navy" +
                "|oldlace|olive|olivedrab|orange|orangered|orchid" +
                "|palegoldenrod|palegreen|paleturquoise|palevioletred|papayawhip|peachpuff|peru|pink|plum|powderblue|purple" +
                "|rebeccapurple|red|rosybrown|royalblue" +
                "|saddlebrown|salmon|sandybrown|seagreen|seashell|sienna|silver|skyblue|slateblue|slategray|slategrey|snow|springgreen|steelblue" +
                "|tan|teal|thistle|tomato|turquoise" +
                "|violet" +
                "|wheat|white|whitesmoke" +
                @"|yellow|yellowgreen)\b",
                PrioValueKeyword, "keyword value", null, RegexOptions.IgnoreCase),
            new(@"#(?:[0-9a-fA-F]{3,4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})\b", PrioValueNumeric, "number hexcolor",
                null),
            new(
                @"\b-?(?:\d+\.\d+|\d+)(?:%|px|em|rem|vw|vh|vmin|vmax|cm|mm|in|pt|pc|Q|deg|grad|rad|turn|s|ms|kHz|Hz|dpi|dpcm|dppx|fr|ch|ex|ic|cap|lh|rlh|vb|vi|mozmm)?\b",
                PrioValueNumeric, "number unit", null,
                RegexOptions.IgnoreCase),
            new(@"#[\w-]+", PrioSelectorId, "selector id", null),
            new(@"\.[\w-]+", PrioSelectorClass, "selector class", null),
            new(@"\[[^\]]+\]", PrioSelectorAttribute, "selector attribute",
                null),
            new(""":{1,2}[\w-]+(?:\((?:[^()"']|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*')*\))?""",
                PrioSelectorPseudo, "selector pseudo", null,
                RegexOptions.IgnoreCase),
            new(@"\*", PrioSelectorUniversal, "selector universal", null),
            new(@"\b[a-zA-Z_][\w-]*\b", PrioSelectorTag, "selector tag", null,
                RegexOptions.IgnoreCase),
            new("[{}();:,>+~*/]", PrioPunctuation, "punctuation", null)

        ];

        public IEnumerable<TokenRule> GetRules() => Rules;
    }
}