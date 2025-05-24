using System.Text.RegularExpressions;

namespace Prism.Blazor.PresetDefinitions
{
    public class CssLanguageDefinition : ILanguageDefinition
    {
        public string Name => "Css";

        // Style constants inspired by VS Code's default dark theme
        private const string CommentStyle = "color: #6A9955;"; // Editor comment green
        private const string StringStyle = "color: #CE9178;"; // Editor string orange
        private const string AtRuleKeywordStyle = "color: #C586C0; font-weight: bold;"; // Magenta for @media, @keyframes
        private const string ImportantStyle = "color: #C586C0; font-weight: bold;"; // Magenta for !important
        private const string FunctionStyle = "color: #DCDCAA;"; // Yellow for url(), rgb(), var()
        private const string PropertyNameStyle = "color: #9CDCFE;"; // Light blue for 'color', 'font-size'
        private const string ValueKeywordStyle = "color: #569CD6;"; // Blue for 'auto', 'inherit', 'red'
        private const string ValueNumericStyle = "color: #B5CEA8;"; // Light green for numbers, units, hex colors
        private const string SelectorIdStyle = "color: #569CD6;"; // Blue for #myid (same as value keywords)
        private const string SelectorClassStyle = "color: #4EC9B0;"; // Teal for .myclass
        private const string SelectorAttributeStyle = "color: #9CDCFE;"; // Light blue for [type="text"] (same as property names)
        private const string SelectorPseudoStyle = "color: #C586C0;"; // Magenta for :hover, ::before (same as at-rules)
        private const string SelectorTagStyle = "color: #D7BA7D;"; // Yellow-ish for div, h1 (like HTML tags)
        private const string PunctuationStyle = "color: #D4D4D4;"; // Default editor text color

        // Priorities (higher number = higher priority)
        private const int PrioComment = 100;
        private const int PrioString = 90;
        private const int PrioAtRuleKeyword = 85;
        private const int PrioImportant = 80;
        private const int PrioFunction = 75;
        private const int PrioPropertyName = 70; // Includes custom properties like --my-var
        private const int PrioValueKeyword = 68; // CSS keywords like 'auto', 'inherit', color names
        private const int PrioValueNumeric = 66; // Numbers, units, hex colors
        private const int PrioSelectorId = 60;
        private const int PrioSelectorClass = 59;
        private const int PrioSelectorAttribute = 58;
        private const int PrioSelectorPseudo = 57;
        private const int PrioSelectorUniversal = 56;
        private const int PrioSelectorTag = 55; // General element names
        private const int PrioPunctuation = 5;

        private static readonly List<TokenRule> Rules =
        [
            new(@"/\*.*?\*/", PrioComment, "comment", CommentStyle, RegexOptions.Singleline),

            // 2. Strings
            new("""
                "(?:\\.|[^"\r\n])*"|'(?:\\.|[^'\r\n])*'
                """, PrioString, "string", StringStyle),

            // 3. At-Rules Keywords (e.g., @media, @keyframes)
            new(
                @"@(?:charset|import|namespace|media|supports|document|page|font-face|keyframes|viewport|counter-style|font-feature-values|property|layer|scope)\b",
                PrioAtRuleKeyword, "keyword at-rule", AtRuleKeywordStyle, RegexOptions.IgnoreCase),

            // 4. !important directive

            new(@"!important\b", PrioImportant, "keyword important", ImportantStyle, RegexOptions.IgnoreCase),

            // 5. Functions (e.g., url(), rgb(), var(), calc())
            // This matches the function name and its parentheses e.g. rgb(...), var(...)
            new(
                """\b(?:url|rgb|rgba|hsl|hsla|var|calc|attr|linear-gradient|radial-gradient|repeating-linear-gradient|repeating-radial-gradient|rotate|scale|translate|matrix|perspective|skew|format|local|clamp|min|max|fit-content|minmax|repeat|conic-gradient|paint|layout|element|cross-fade|image-set|image|counter|counters|rect|path|polygon|circle|ellipse|inset|blur|brightness|contrast|drop-shadow|grayscale|hue-rotate|invert|opacity|saturate|sepia|steps|cubic-bezier|frames|symbols)\s*\((?:[^()"']|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*')*\)""",
                PrioFunction, "function", FunctionStyle, RegexOptions.IgnoreCase),

            // 6. Property Names (e.g., color, font-size, -webkit-transition, --my-custom-property)

            new(@"(?:(?:--|-)[\w-]+|\b[a-z][\w-]*)(?=\s*:)", PrioPropertyName, "property-name",
                PropertyNameStyle, RegexOptions.IgnoreCase),

            // 7. CSS Value Keywords (long list including color names and general keywords)
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
                // Color Names
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
                PrioValueKeyword, "keyword value", ValueKeywordStyle, RegexOptions.IgnoreCase),

            // 8. Numeric Values (Hex colors, numbers with units, plain numbers)

            new(@"#(?:[0-9a-fA-F]{3,4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})\b", PrioValueNumeric, "number hexcolor",
                ValueNumericStyle), // Hex colors
            new(
                @"\b-?(?:\d+\.\d+|\d+)(?:%|px|em|rem|vw|vh|vmin|vmax|cm|mm|in|pt|pc|Q|deg|grad|rad|turn|s|ms|kHz|Hz|dpi|dpcm|dppx|fr|ch|ex|ic|cap|lh|rlh|vb|vi|mozmm)?\b",
                PrioValueNumeric, "number unit", ValueNumericStyle,
                RegexOptions.IgnoreCase), // Numbers with optional units

            // 9. Selectors

            new(@"#[\w-]+", PrioSelectorId, "selector id", SelectorIdStyle), // ID selector
            new(@"\.[\w-]+", PrioSelectorClass, "selector class", SelectorClassStyle), // Class selector
            new(@"\[[^\]]+\]", PrioSelectorAttribute, "selector attribute",
                SelectorAttributeStyle), // Attribute selector
            new(""":{1,2}[\w-]+(?:\((?:[^()"']|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*')*\))?""",
                PrioSelectorPseudo, "selector pseudo", SelectorPseudoStyle,
                RegexOptions.IgnoreCase), // Pseudo-classes/elements
            new(@"\*", PrioSelectorUniversal, "selector universal", SelectorTagStyle), // Universal selector *
            new(@"\b[a-zA-Z_][\w-]*\b", PrioSelectorTag, "selector tag", SelectorTagStyle,
                RegexOptions.IgnoreCase), // Tag selectors (element names)

            // 10. Punctuation
            new("[{}();:,>+~*/]", PrioPunctuation, "punctuation", PunctuationStyle)

        ];

        public IEnumerable<TokenRule> GetRules() => Rules;
    }
}