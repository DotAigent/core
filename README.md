# DotAIgent - Agentic AI framwork for .NET

This is a .NET frameowrk for building AI agents using .NET


AIAgent use AIModel
AIAgent use Tool
Tool has input properties
Tool has output structure
Install the required NuGet packages:

    Install-Package AngleSharp
    Install-Package Html2Markdown

using AngleSharp;
using Html2Markdown;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Step 1: Set up AngleSharp configuration
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);

        // Step 2: Scrape the webpage
        var url = "https://example.com"; // Replace with the target URL
        var document = await context.OpenAsync(url);

        // Extract content (e.g., all <p> elements)
        var paragraphs = document.QuerySelectorAll("p")
            .Select(p => p.InnerHtml) // Get the inner HTML of each paragraph
            .Aggregate((a, b) => a + "\n\n" + b); // Combine with Markdown-friendly spacing

        if (string.IsNullOrEmpty(paragraphs))
        {
            Console.WriteLine("No paragraphs found on the page.");
            return;
        }

        // Step 3: Convert HTML to Markdown
        var converter = new Converter();
        var markdown = converter.Convert(paragraphs);

        // Output the result
        Console.WriteLine("Markdown Output:");
        Console.WriteLine("----------------");
        Console.WriteLine(markdown);
    }
}
