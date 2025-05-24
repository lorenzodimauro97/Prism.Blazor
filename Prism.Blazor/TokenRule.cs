// File: Prism.Blazor\TokenRule.cs
using System.Text.RegularExpressions;

namespace Prism.Blazor;

public class TokenRule(
    string pattern,
    int priority,
    string? cssClass,
    string? inlineStyle,
    RegexOptions options = RegexOptions.None)
{
    public Regex Regex { get; } = new(pattern, RegexOptions.Compiled | options);
    public string? CssClass { get; } = cssClass;
    public string? InlineStyle { get; } = inlineStyle;
    public int Priority { get; } = priority; // Higher number means higher priority
}