using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace DotAigent.SourceGenerators;

[Generator]
public class AgentResponseExampleGenerator : IIncrementalGenerator
{
    private static void Log(string message)
    {
        string logFilePath = Path.Combine("/home/thhel/", "incremental_generator.log");
        File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
    }
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
                        fullyQualifiedMetadataName: "DotAigent.Core.AgentResponseAttribute",
                        predicate: static (syntaxNode, cancellationToken) => syntaxNode is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                        transform: static (context, cancellationToken) =>
                        {
                            try
                            {
                                var model = new Model(
                                 // Note: this is a simplified example. You will also need to handle the case where the type is in a global namespace, nested, etc.
                                 context.TargetSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                                 context.TargetSymbol.Name,
                                 GenerateJsonExample(context.TargetSymbol as INamedTypeSymbol));
                                Log($"Model: {model}");
                                return model;
                            }
                            catch (Exception e)
                            {
                                Log($"Error: {e}");
                                throw;
                            }
                        }
                    );
            context.RegisterSourceOutput(pipeline, static (context, model) =>
            {
                try
                {
                    Log("Source called");
                    var codeBuilder = new StringBuilder();

                    codeBuilder.AppendLine("using DotAigent.Core;");

                    if (!string.IsNullOrEmpty(model.Namespace))
                    {
                        codeBuilder.AppendLine($"namespace {model.Namespace}");
                        codeBuilder.AppendLine("{");
                    }
                    codeBuilder.AppendLineIndent(1, $"public partial class {model.ClassName} : IExample");
                    codeBuilder.AppendLineIndent(1, "{");
                    codeBuilder.AppendLineIndent(2, $"public static string JsonExample => {model.JsonExample};");

                    codeBuilder.AppendLineIndent(1, "}");
                    if (!string.IsNullOrEmpty(model.Namespace))
                    {
                        codeBuilder.AppendLine("}");
                    }

                    var sourceText = SourceText.From(codeBuilder.ToString(), Encoding.UTF8);

                    context.AddSource($"{model.ClassName}.g.cs", sourceText);

                }
                catch (Exception e)
                {
                    Log($"Error: {e}");
                    throw;
                }
            });
        }
        catch (Exception e)
        {
            Log($"Error: {e}");
            throw;
        }
    }

    private static string GenerateJsonExample(INamedTypeSymbol classSymbol)
    {
        var properties = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.GetMethod != null && p.GetMethod.DeclaredAccessibility == Accessibility.Public)
            .ToList();

        if (properties.Count == 0)
            return "@\"{}\"";

        var jsonLines = properties.Select(p => $"    \"\"{p.Name}\"\": {GetExampleValue(p.Type)}");
        return "@\"{\n" + string.Join(",\n", jsonLines) + "\n}\"";
    }

    private static string GetExampleValue(ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Int32 => "42",
            SpecialType.System_String => "\"example\"",
            SpecialType.System_Boolean => "true",
            SpecialType.System_Double => "3.14",
            _ => "null"
        };
    }

}
internal class Model(string namespaceName, string className, string jsonExample)
{
    public string Namespace { get; } = namespaceName;
    public string ClassName { get; } = className;
    public string JsonExample { get; } = jsonExample;
}

//     public void Execute(GeneratorExecutionContext context)
//     {
//         // Retrieve the syntax receiver
//         if (context.SyntaxReceiver is not SyntaxReceiver receiver || receiver.CandidateClasses.Count == 0)
//             return;
//
//         // Get the compilation and attribute symbol
//         var compilation = context.Compilation;
//         var attributeSymbol = compilation.GetTypeByMetadataName(AttributeName);
//         if (attributeSymbol == null)
//             return; // Attribute not found in compilation
//
//         // Collect classes with the attribute
//         var classesWithAttribute = new List<INamedTypeSymbol>();
//         foreach (var candidate in receiver.CandidateClasses)
//         {
//             var model = compilation.GetSemanticModel(candidate.SyntaxTree);
//             if (model.GetDeclaredSymbol(candidate) is INamedTypeSymbol classSymbol && classSymbol.GetAttributes().Any(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, attributeSymbol)))
//             {
//                 classesWithAttribute.Add(classSymbol);
//             }
//         }
//
//         if (classesWithAttribute.Count == 0)
//             return;
//
//         // Generate the source code
//         var ifStatements = new List<string>();
//         foreach (var classSymbol in classesWithAttribute)
//         {
//             var fullyQualifiedName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
//             var jsonString = GenerateJsonString(classSymbol);
//             ifStatements.Add($"if (typeof(T) == typeof({fullyQualifiedName}))\n                return {jsonString};");
//         }
//
//         var methodBody = ifStatements.Count != 0
//             ? string.Join("\n            else ", ifStatements) + "\n            else\n                throw new NotSupportedException($\"No example for type {{typeof(T)}}\");"
//             : "throw new NotSupportedException($\"No example for type {typeof(T)}\");";
//
//         var source = $@"
// using System;
// using System.CodeDom.Compiler;
//
// namespace DotAigent.Generated
// {{
//     [GeneratedCode(""DotAigent.SourceGenerators.AgentResponseExampleGenerator"", ""1.0.0.0"")]
//     public static class ResponseExamples
//     {{
//         public static string GetExample<T>() where T : class
//         {{
//             {methodBody}
//         }}
//     }}
// }}";
//
//         context.AddSource("ResponseExamples.g.cs", SourceText.From(source, Encoding.UTF8));
//     }
//
//     private static string GenerateJsonString(INamedTypeSymbol classSymbol)
//     {
//         var properties = classSymbol.GetMembers()
//             .OfType<IPropertySymbol>()
//             .Where(p => p.GetMethod != null && p.GetMethod.DeclaredAccessibility == Accessibility.Public)
//             .ToList();
//
//         if (properties.Count == 0)
//             return "@\"{}\"";
//
//         var jsonLines = properties.Select(p => $"    \"\"{p.Name}\"\": {GetExampleValue(p.Type)}");
//         return "@\"{\n" + string.Join(",\n", jsonLines) + "\n}\"";
//     }
//
//     private static string GetExampleValue(ITypeSymbol type)
//     {
//         return type.SpecialType switch
//         {
//             SpecialType.System_Int32 => "42",
//             SpecialType.System_String => "\"example\"",
//             SpecialType.System_Boolean => "true",
//             SpecialType.System_Double => "3.14",
//             _ => "null"
//         };
//     }
// }
