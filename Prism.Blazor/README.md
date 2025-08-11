# Prism.Blazor

[![Build Status](https://img.shields.io/github/actions/workflow/status/lorenzodimauro97/Prism.Blazor/dotnet.yml?branch=main&style=flat-square)](https://github.com/lorenzodimauro97/Prism.Blazor/actions)
[![NuGet Version](https://img.shields.io/nuget/v/S97SP.Prism.Blazor.svg?style=flat-square)](https://www.nuget.org/packages/S97SP.Prism.Blazor/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)

**Prism.Blazor** is a lightweight, extensible, and Blazor-native component for client-side syntax highlighting. It leverages regular expressions to tokenize code and applies CSS classes for theming, bringing Prism.js-like capabilities directly into the .NET Blazor ecosystem without JavaScript interop for the core highlighting logic.

## Table of Contents

*   [Why Prism.Blazor?](#why-prismblazor)
*   [Features](#features)
*   [Getting Started](#getting-started)
*   [Usage](#usage)
*   [Theming](#theming)
*   [Advanced Customization](#advanced-customization)
*   [Building from Source](#building-from-source)
*   [Contributing](#contributing)
*   [License](#license)
*   [Acknowledgements](#acknowledgements)

## Why Prism.Blazor?

*   **Blazor Native:** Pure .NET and Blazor implementation. No JavaScript interop for the core highlighting engine means a smaller deployment size and a development experience that stays within the C#/Razor paradigm.
*   **Lightweight & Themable:** Designed to be lean and efficient. Both the `CodeHighlighter` and `MarkdownRenderer` components support fully customizable, CSS-variable-based theming.
*   **Extensible:** Easily define new language syntaxes or extend existing ones using C#.

## Features

*   **Client-Side Highlighting:** Processes and renders highlighted code directly in the browser.
*   **Regex-Based Tokenization:** Uses regular expressions for flexible and powerful language parsing.
*   **CSS Theming:** A robust theming system based on CSS variables allows for complete control over the appearance of both code and rendered markdown.
*   **Preset Languages:** Includes a growing set of common language definitions out-of-the-box.
*   **Configurable Messages:** Customize loading and error messages.
*   **Asynchronous Processing:** Highlighting is performed asynchronously to prevent UI blocking.
*   **Markdown Rendering:** Includes a `MarkdownRenderer` component that also supports the theming system.

## Getting Started

### Installation
Install via the .NET CLI or NuGet Package Manager:
```bash
dotnet add package S97SP.Prism.Blazor
```

### Importing
Add the following to your project's `_Imports.razor` file:
```csharp
@using Prism.Blazor
@using Prism.Blazor.PresetDefinitions 
```

## Usage

### Basic Code Highlighting
Use the `CodeHighlighter` component, providing your code `Content` and a `LanguageDefinition`.
```razor
<CodeHighlighter Content="@csharpCode"
                 LanguageDefinition="@(new CSharpLanguageDefinition())" />
@code {
    private string csharpCode = "public void SayHello() => Console.WriteLine(\"Hello, World!\");";
}
```

### Markdown Rendering
Use the `MarkdownRenderer` component to render markdown strings to HTML.
```razor
<MarkdownRenderer MarkdownContent="@markdownText" />

@code {
    private string markdownText = "# Title\n\n**Bold text** and `inline code`.";
}
```

### Component Parameters
*   `Content` / `MarkdownContent` (string?): The code or markdown string to render.
*   `LanguageDefinition` (ILanguageDefinition?): (*CodeHighlighter only*) An instance of a language definition.
*   `CustomTheme` (Dictionary<string, string>?): A dictionary of key-value pairs to override the default theme colors. See the [Theming](#theming) section.
*   `UseHardLineBreaks` (bool): (*MarkdownRenderer only*) If true, renders newlines in markdown as `<br>` tags.
*   `LoadingMessage` (string): Text displayed during processing.
*   `ErrorMessage` (string): Text displayed on error.

### Available Language Definitions
*   `CSharpLanguageDefinition`, `CssLanguageDefinition`, `JavaScriptLanguageDefinition`, `JsxLanguageDefinition`, `RazorLanguageDefinition`, `SqlLanguageDefinition`, `TypeScriptLanguageDefinition`, `TsxLanguageDefinition`, `XmlLanguageDefinition`, `PlainTextLanguageDefinition`

## Theming
Both components use a CSS variable-based system for theming. You can override the default dark theme by passing a dictionary to the `CustomTheme` parameter.

### `CodeHighlighter` Theming
The keys for the `CustomTheme` dictionary correspond to token names.
```razor
<CodeHighlighter Content="@csharpCode"
                 LanguageDefinition="@(new CSharpLanguageDefinition())"
                 CustomTheme="@LightThemeForCode" />
@code {
    private string csharpCode = "public void SayHello() => Console.WriteLine(\"Hello, World!\");";

    // Example Light Theme
    private Dictionary<string, string> LightThemeForCode = new()
    {
        { "background", "#f5f2f0" },
        { "text", "#383a42" },
        { "comment", "#a0a1a7" },
        { "keyword", "#a626a4" },
        { "string", "#50a14f" },
        { "number", "#d19a66" },
        { "type-known", "#c18401" },
        { "identifier-method", "#4078f2" },
        { "punctuation", "#383a42" }
    };
}
```

### `MarkdownRenderer` Theming
The keys correspond to markdown element types.
```razor
<MarkdownRenderer MarkdownContent="@markdownText" CustomTheme="@LightThemeForMarkdown" />

@code {
    private string markdownText = "# Hello\nThis is a *light* theme!";

    // Example Light Theme
    private Dictionary<string, string> LightThemeForMarkdown = new()
    {
        { "text", "#24292e" },
        { "heading", "#24292e" },
        { "link", "#0366d6" },
        { "border", "#e1e4e8" },
        { "code-background", "#f6f8fa" },
        { "blockquote-text", "#6a737d" },
        { "blockquote-border", "#dfe2e5" }
    };
}
```

## Advanced Customization
### Creating Custom Language Definitions
Implement the `ILanguageDefinition` interface to define new languages or modify existing ones.
```csharp
public class MyLangDefinition : ILanguageDefinition
{
    public string Name => "MyLang";

    private static readonly List<TokenRule> Rules =
    [
        // Priority 10, will be styled via the 'mylang-special-keyword' CSS class
        new TokenRule(@"\b(BEGIN|END)\b", 10, "mylang-special-keyword", null),
    ];

    public IEnumerable<TokenRule> GetRules() => Rules;
}
```

### Understanding `TokenRule` Logic
The highlighting engine finds all regex matches and sorts them by: 1. Start Index (ascending), 2. Priority (descending), 3. Match Length (descending). It then processes the sorted, non-overlapping matches to build the final highlighted output.

## Building from Source
1.  Clone the repository.
2.  Run `dotnet build Prism.Blazor.sln -c Release`.

## Contributing
Contributions are welcome! Please open an issue to discuss your ideas or submit a pull request.

## License
This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgements
*   Inspired by the original [Prism.js](https://prismjs.com/) library.
*   Color palettes for preset themes are inspired by popular code editors.