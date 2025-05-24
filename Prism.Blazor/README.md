# Prism.Blazor

[![Build Status](https://img.shields.io/github/actions/workflow/status/YOUR_USERNAME/Prism.Blazor/dotnet.yml?branch=main&style=flat-square)](https://github.com/YOUR_USERNAME/Prism.Blazor/actions)
[![NuGet Version](https://img.shields.io/nuget/v/YOUR_PACKAGE_NAME.svg?style=flat-square)](https://www.nuget.org/packages/YOUR_PACKAGE_NAME/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)

**Prism.Blazor** is a lightweight, extensible, and Blazor-native component for client-side syntax highlighting. It leverages regular expressions to tokenize code and applies CSS classes or inline styles for theming, bringing Prism.js-like capabilities directly into the .NET Blazor ecosystem without JavaScript interop for the core highlighting logic.

## Table of Contents

*   [Why Prism.Blazor?](#why-prismblazor)
*   [Features](#features)
*   [Getting Started](#getting-started)
    *   [Installation](#installation)
    *   [Importing](#importing)
*   [Usage](#usage)
    *   [Basic Highlighting](#basic-highlighting)
    *   [Component Parameters](#component-parameters)
    *   [Available Language Definitions](#available-language-definitions)
*   [Styling the Output](#styling-the-output)
    *   [HTML Structure](#html-structure)
    *   [Styling with CSS Classes](#styling-with-css-classes)
    *   [Using Inline Styles](#using-inline-styles)
*   [Advanced Customization](#advanced-customization)
    *   [Creating Custom Language Definitions](#creating-custom-language-definitions)
    *   [Understanding TokenRule Logic](#understanding-tokenrule-logic)
*   [Building from Source](#building-from-source)
*   [Contributing](#contributing)
*   [License](#license)
*   [Acknowledgements](#acknowledgements)

## Why Prism.Blazor?

*   **Blazor Native:** Pure .NET and Blazor implementation. No JavaScript interop for the core highlighting engine means a smaller deployment size (if you're avoiding JS elsewhere) and a development experience that stays within the C#/Razor paradigm.
*   **Lightweight:** Designed to be lean and efficient for Blazor WebAssembly and Blazor Server applications.
*   **Extensible:** Easily define new language syntaxes or extend existing ones using C#.
*   **Control & Simplicity:** Offers a straightforward approach to syntax highlighting for developers already comfortable with .NET.

## Features

*   **Client-Side Highlighting:** Processes and renders highlighted code directly in the browser.
*   **Regex-Based Tokenization:** Uses regular expressions for flexible and powerful language parsing.
*   **CSS Theming:** Prioritizes CSS classes for styling, allowing for easy customization and theme creation. Inline styles are also supported.
*   **Preset Languages:** Includes a growing set of common language definitions out-of-the-box.
*   **Configurable Messages:** Customize loading and error messages displayed by the component.
*   **Asynchronous Processing:** Highlighting is performed asynchronously to prevent UI blocking.

## Getting Started

### Installation

Prism.Blazor is distributed as a NuGet package. You can install it using the .NET CLI or the NuGet Package Manager in Visual Studio.

**.NET CLI:**
```bash
dotnet add package Prism.Blazor
```

**Package Manager Console:**
```powershell
Install-Package Prism.Blazor
```

### Importing

To make the `CodeHighlighter` component and its related types easily accessible in your Razor files, add the following `using` statements to your project's `_Imports.razor` file:

```csharp
// File: _Imports.razor
@using Prism.Blazor
@using Prism.Blazor.PresetDefinitions 
```

## Usage

### Basic Highlighting

The core of Prism.Blazor is the `CodeHighlighter` component. To use it, provide your code `Content` and a `LanguageDefinition`.

```razor
@page "/code-example"

<h3>C# Code Highlighting</h3>

<CodeHighlighter Content="@csharpCode"
                 LanguageDefinition="@(new CSharpLanguageDefinition())" />

@code {
    private string csharpCode = """
    // Example C# Code
    public class Greeter 
    {
        public string? Name { get; set; }

        public void SayHello() 
        {
            Console.WriteLine($"Hello, {Name ?? "World"}!");
        }
    }
    """;
}
```

### Component Parameters

The `CodeHighlighter` component accepts the following parameters:

*   `Content` (string?): The code string to be highlighted.
*   `LanguageDefinition` (ILanguageDefinition?): An instance of a language definition (e.g., `new CSharpLanguageDefinition()`).
*   `LoadingMessage` (string, default: "Processing code..."): Text displayed while highlighting is in progress.
*   `ErrorMessage` (string, default: "Error highlighting code."): Text displayed if an error occurs during highlighting.

### Available Language Definitions

Prism.Blazor comes with several preset language definitions:

*   `CSharpLanguageDefinition`
*   `CssLanguageDefinition`
*   `JavaScriptLanguageDefinition`
*   `JsxLanguageDefinition`
*   `RazorLanguageDefinition`
*   `SqlLanguageDefinition`
*   `TypeScriptLanguageDefinition`
*   `TsxLanguageDefinition`
*   `XmlLanguageDefinition`
*   `PlainTextLanguageDefinition`: Renders the input text HTML-encoded without applying any syntax highlighting. Useful for displaying plain text snippets safely.

## Styling the Output

### HTML Structure

The `CodeHighlighter` component renders the highlighted code within the following HTML structure:

```html
<pre class="code-highlighter-pre">
    <code class="code-highlighter-code language-{name}">
        <!-- Highlighted content with <span> elements -->
        <span class="{tokenCssClass}" style="{tokenInlineStyle}">...</span>
    </code>
</pre>
```

*   The `language-{name}` class on the `<code>` element is derived from the `Name` property of your `ILanguageDefinition` (e.g., `language-csharp`).
*   Each recognized token is wrapped in a `<span>`.

### Styling with CSS Classes

This is the recommended approach for styling. Each `TokenRule` in a language definition can specify a `CssClass`. This class will be applied to the `<span>` elements wrapping the matched tokens.

1.  **Define your styles:** Add CSS rules to your application's stylesheet (e.g., `app.css` or a component-specific CSS file). The `CodeHighlighter.razor.css` file included in the library provides base styles for the `<pre>` and `<code>` elements. You'll need to add styles for the tokens themselves.

    ```css
    /* Base styles (from CodeHighlighter.razor.css or your global styles) */
    .code-highlighter-pre {
        background-color: #2d2d2d; /* Example: Dark background */
        color: #cccccc;            /* Example: Light default text */
        padding: 1em;
        overflow-x: auto;
        border-radius: 0.3em;
        font-family: Consolas, Monaco, 'Andale Mono', 'Ubuntu Mono', monospace;
        font-size: 0.9em;
        line-height: 1.5;
        border: 1px solid #444;
    }

    .code-highlighter-code {
        white-space: pre;
    }

    .code-highlighter-loading {
        padding: 1em;
        font-style: italic;
        color: #888;
    }

    /* Token-specific styles (you define these based on your language definitions) */
    /* For CSharpLanguageDefinition examples: */
    .code-highlighter-code .keyword { color: #569cd6; font-weight: bold; }
    .code-highlighter-code .type { color: #4ec9b0; }
    .code-highlighter-code .string { color: #ce9178; }
    .code-highlighter-code .comment { color: #6a9955; font-style: italic; }
    .code-highlighter-code .number { color: #b5cea8; }
    .code-highlighter-code .preprocessor { color: #bbb529; }
    .code-highlighter-code .punctuation { color: #d4d4d4; }
    .code-highlighter-code .identifier.method { color: #ffc66d; }
    
    /* For CSSLanguageDefinition examples: */
    .code-highlighter-code.language-css .property-name { color: #9cdcfe; }
    .code-highlighter-code.language-css .keyword.value { color: #569cd6; }
    .code-highlighter-code.language-css .selector.id { color: #569cd6; }
    .code-highlighter-code.language-css .selector.class { color: #4ec9b0; }

    /* Add more rules for other languages and token types */
    ```

    **Note:** The `CssClass` provided in `TokenRule` (e.g., `"keyword"`, `"comment"`) is directly used. The examples above assume you're using simple class names. If your `TokenRule` specifies `cssClass: "token my-custom-keyword"`, then your CSS selector would be `.code-highlighter-code .token.my-custom-keyword`. The preset definitions generally use simple class names like "keyword", "string", etc.

2.  **Ensure styles are loaded:** Make sure your Blazor application loads these CSS files.

### Using Inline Styles

Alternatively, a `TokenRule` can specify an `InlineStyle` string (e.g., `"color: #569cd6; font-weight: bold;"`). This style will be directly applied to the `style` attribute of the `<span>` element. This approach is less flexible for theming and generally not recommended over CSS classes.

## Advanced Customization

### Creating Custom Language Definitions

To highlight a language not included in the presets, or to modify existing behavior, implement the `ILanguageDefinition` interface:

1.  **Create a class that implements `ILanguageDefinition`:**

    ```csharp
    // File: MyLangDefinition.cs
    using Prism.Blazor;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class MyLangDefinition : ILanguageDefinition
    {
        public string Name => "MyLang"; // Used for the CSS class `language-mylang`

        // Define styles or CSS class names
        private const string SpecialKeywordClass = "mylang-special-keyword";
        private const string OperatorStyle = "color: #FF8C00;"; // DarkOrange

        // Define token rules
        private static readonly List<TokenRule> Rules =
        [
            // Rule: Match "BEGIN" or "END" as special keywords
            // Priority 10, uses a CSS class
            new TokenRule(@"\b(BEGIN|END)\b", 10, SpecialKeywordClass, null),

            // Rule: Match "=>" or "->" as operators
            // Priority 5, uses an inline style
            new TokenRule(@"(=>|->)", 5, null, OperatorStyle),
            
            // Add more rules for comments, strings, numbers, etc.
        ];

        public IEnumerable<TokenRule> GetRules() => Rules;
    }
    ```

2.  **Use your custom definition:**

    ```razor
    <CodeHighlighter Content="@myCustomLangCode"
                     LanguageDefinition="@(new MyLangDefinition())" />

    @code {
        private string myCustomLangCode = "BEGIN some process => result END";
    }
    ```

    Remember to add CSS for `.mylang-special-keyword` if you use CSS classes.

### Understanding TokenRule Logic

The heart of the highlighting process lies in `TokenRule` objects and how they are processed. Each `TokenRule` defines:

*   `Regex`: A `System.Text.RegularExpressions.Regex` object for matching a token. The `RegexOptions.Compiled` flag is automatically added.
*   `Priority` (int): A number indicating the rule's importance. Higher numbers mean higher priority.
*   `CssClass` (string?): CSS class(es) to apply if this rule matches.
*   `InlineStyle` (string?): Inline style string to apply if this rule matches.

**Matching and Conflict Resolution:**

When highlighting text, Prism.Blazor performs the following steps:

1.  **Collect All Matches:** It iterates through all `TokenRule`s in the language definition and finds all possible matches in the input string.
2.  **Sort Matches:** All found matches are then sorted according to these criteria, in order:
    *   **Start Index (Ascending):** Matches occurring earlier in the text come first.
    *   **Priority (Descending):** If multiple matches start at the same index, the one from a `TokenRule` with a higher `Priority` value is preferred.
    *   **Match Length (Descending):** If start index and priority are also equal, the longer match is preferred.
3.  **Select Non-Overlapping Matches:** The component iterates through the sorted list of potential matches. It selects the first valid match and then discards any subsequent potential matches that overlap with the already selected one. It then continues this process from the end of the last selected match.

This mechanism ensures that more specific rules (e.g., a keyword "string") can take precedence over more general rules (e.g., a general identifier) if they have higher priority or produce a longer match at the same position.

## Building from Source

If you wish to build the library from source:

1.  Clone the repository:
    ```bash
    git clone https://github.com/lorenzodimauro97/Prism.Blazor.git
    cd Prism.Blazor
    ```
2.  Build the solution:
    ```bash
    dotnet build Prism.Blazor.sln -c Release
    ```
    The compiled library (`.dll`) will be in the `Prism.Blazor/bin/Release/` directory.

## Contributing

Contributions are highly welcome! Whether it's bug reports, feature requests, documentation improvements, or new language definitions, please feel free to:

1.  **Open an Issue:** Discuss the change you wish to make or report a bug.
2.  **Fork the Repository:** Create your own copy.
3.  **Create a Feature Branch:** (`git checkout -b feature/AmazingFeature`)
4.  **Commit your Changes:** (`git commit -m 'Add some AmazingFeature'`)
5.  **Push to the Branch:** (`git push origin feature/AmazingFeature`)
6.  **Open a Pull Request:** Target the `main` branch of the original repository.

Please ensure your code adheres to the existing style and best practices.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

*   Inspired by the original [Prism.js](https://prismjs.com/) library.
*   Color palettes for preset languages are inspired by themes from popular code editors like Visual Studio Code and Rider.