/*
 *
 * Source generator that gets all classes with the attribute [AgentResponse] and generates a json example of the class.
 * 
 * */
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
// using DotAigent.SourceGenerators.SharedLibrary;

namespace DotAigent.SourceGenerators
{
    /// <summary>
    /// Source generator that finds all classes with the [AgentResponse] attribute
    /// and generates a JSON example representing the class structure.
    /// </summary>
    [Generator]
    public class AgentResponseExampleGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Setup logging
            Log("AgentResponseExampleGenerator Initialize");

            // Register for classes with the [AgentResponse] attribute
            IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
                context.SyntaxProvider
                    .CreateSyntaxProvider(
                        predicate: static (s, _) => IsTargetForGeneration(s),
                        transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                    .Where(static m => m is not null)!;

            // Store the class information for incremental generation
            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses =
                context.CompilationProvider.Combine(classDeclarations.Collect());

            // Generate the source code using the inputs
            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static bool IsTargetForGeneration(SyntaxNode node)
        {
            // Check if this is a class declaration with attributes
            return node is ClassDeclarationSyntax classDeclaration
                && classDeclaration.AttributeLists.Count > 0;
        }

        private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // Get the class declaration node
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Look for the [AgentResponse] attribute on the class
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string attributeName = attributeContainingTypeSymbol.ToDisplayString();

                    // Check for either "AgentResponse" or "AgentResponseAttribute"
                    if (attributeName == "AgentResponseAttribute" || attributeName == "AgentResponse")
                    {
                        return classDeclaration;
                    }
                }
            }

            return null;
        }

        private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            if (classes.IsDefaultOrEmpty)
            {
                Log("No classes found with [AgentResponse] attribute");
                return;
            }

            Log($"Found {classes.Length} classes with [AgentResponse] attribute");

            // Process each class
            foreach (var classDeclaration in classes.Distinct())
            {
                try
                {
                    // Get the semantic model for this class
                    var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                    if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
                    {
                        Log($"Could not get symbol for class {classDeclaration.Identifier.Text}");
                        continue;
                    }

                    // Extract namespace and class name
                    string namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
                        ? string.Empty
                        : classSymbol.ContainingNamespace.ToDisplayString();
                    string className = classSymbol.Name;

                    // Generate JSON example
                    string jsonExample = GenerateJsonExample(classSymbol);
                    string escapedJsonExample = EscapeStringLiteral(jsonExample);

                    Log($"Generating JSON example for {className} in {namespaceName}");

                    // Generate the source code
                    string source = GenerateResponseExampleClass(namespaceName, className, escapedJsonExample);

                    // Use namespace in hint name to ensure uniqueness
                    string hintName = string.IsNullOrEmpty(namespaceName)
                        ? $"{className}_ResponseExample.g.cs"
                        : $"{namespaceName.Replace(".", "_")}_{className}_ResponseExample.g.cs";

                    context.AddSource(hintName, source);
                    Log($"Generated {hintName}");
                }
                catch (Exception ex)
                {
                    Log($"Error generating source for {classDeclaration.Identifier.Text}: {ex.Message}");
                }
            }
        }

        private static string GenerateResponseExampleClass(string namespaceName, string className, string escapedJsonExample)
        {
            var sb = new StringBuilder();

            // Add using directive for DotAigent.Core
            sb.AppendLine("using DotAigent.Core;");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.AppendLine($"namespace {namespaceName}");
                sb.AppendLine("{");
            }

            // Indent properly based on namespace presence
            string indent = !string.IsNullOrEmpty(namespaceName) ? "    " : "";

            sb.AppendLine($"{indent}public partial class {className} : IExample");
            sb.AppendLine($"{indent}{{");
            sb.AppendLine($"{indent}    private static string _jsonExample => @\"{escapedJsonExample}\";");
            sb.AppendLine($"{indent}    /// <summary>");
            sb.AppendLine($"{indent}    /// Example JSON representation of this class");
            sb.AppendLine($"{indent}    /// </summary>");
            sb.AppendLine($"{indent}    public string JsonExample => {className}._jsonExample;");
            sb.AppendLine($"{indent}}}");

            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private static string GenerateJsonExample(INamedTypeSymbol classSymbol)
        {
            var properties = classSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic);

            if (!properties.Any())
            {
                Log($"No public properties found for {classSymbol.Name}");
                return "{}";
            }

            var sb = new StringBuilder();
            sb.Append("{\n");

            bool isFirst = true;
            foreach (var property in properties)
            {
                if (!isFirst)
                {
                    sb.Append(",\n");
                }
                sb.Append($"  \"{property.Name}\": {GetExampleValue(property.Type, 1)}");
                isFirst = false;
            }

            sb.Append("\n}");
            return sb.ToString();
        }

        private static string GetExampleValue(ITypeSymbol type, int indentLevel)
        {
            string indent = new string(' ', (indentLevel + 1) * 2);

            if (type is INamedTypeSymbol namedType)
            {
                // Handle strings
                if (namedType.SpecialType == SpecialType.System_String)
                {
                    return "\"example string\"";
                }

                // Handle primitive types
                switch (namedType.SpecialType)
                {
                    case SpecialType.System_Int32:
                    case SpecialType.System_Int64:
                        return "42";
                    case SpecialType.System_Single:
                    case SpecialType.System_Double:
                    case SpecialType.System_Decimal:
                        return "3.14";
                    case SpecialType.System_Boolean:
                        return "true";
                    case SpecialType.System_DateTime:
                        return "\"2023-01-01T12:00:00Z\"";
                }
                // Handle Guid type by checking the full name
                if (namedType.ToDisplayString() == "System.Guid")
                {
                    return "\"00000000-0000-0000-0000-000000000000\"";
                }


                // Handle nullable types
                if (namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                {
                    return GetExampleValue(namedType.TypeArguments[0], indentLevel);
                }

                // Handle lists, arrays, and other collections
                if (namedType.IsGenericType)
                {
                    var genericName = namedType.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    if (genericName.Contains("List") || genericName.Contains("Collection") || genericName.Contains("IEnumerable"))
                    {
                        var elementType = namedType.TypeArguments[0];
                        return $"[\n{indent}{GetExampleValue(elementType, indentLevel + 1)}\n{new string(' ', indentLevel * 2)}]";
                    }
                    else if (genericName.Contains("Dictionary") || genericName.Contains("IDictionary"))
                    {
                        var valueType = namedType.TypeArguments[1];
                        return $"{{\n{indent}\"exampleKey\": {GetExampleValue(valueType, indentLevel + 1)}\n{new string(' ', indentLevel * 2)}}}";
                    }
                }

                // Handle complex objects (recursively generate JSON for them)
                if (namedType.TypeKind == TypeKind.Class || namedType.TypeKind == TypeKind.Struct)
                {
                    var properties = namedType.GetMembers()
                        .OfType<IPropertySymbol>()
                        .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic);

                    if (!properties.Any())
                    {
                        return "{}";
                    }

                    var sb = new StringBuilder();
                    sb.Append("{\n");

                    bool isFirst = true;
                    foreach (var property in properties)
                    {
                        if (!isFirst)
                        {
                            sb.Append(",\n");
                        }
                        sb.Append($"{indent}\"{property.Name}\": {GetExampleValue(property.Type, indentLevel + 1)}");
                        isFirst = false;
                    }

                    sb.Append($"\n{new string(' ', indentLevel * 2)}}}");
                    return sb.ToString();
                }
            }
            else if (type is IArrayTypeSymbol arrayType)
            {
                var elementType = arrayType.ElementType;
                return $"[\n{indent}{GetExampleValue(elementType, indentLevel + 1)}\n{new string(' ', indentLevel * 2)}]";
            }

            // Default for any other types
            return "\"unknown type\"";
        }

        private static string EscapeStringLiteral(string value)
        {
            // For verbatim string literals, double the quote characters
            return value.Replace("\"", "\"\"");
        }
        private static void Log(string message)
        {
            string logFilePath = Path.Combine("/home/thhel/", "incremental_generator.log");
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

    }
}

