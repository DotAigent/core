// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using System;
// using System.IO;
// using System.Linq;
//
// namespace DotAigent.SourceGenerators;
//
// [Generator]
// public class AgentResponseGenerator : IIncrementalGenerator
// {
//     public void Initialize(IncrementalGeneratorInitializationContext context)
//     {
//         Log("AgentResponseGenerator Initialize");
//
//         var classSymbols = context.SyntaxProvider
//             .ForAttributeWithMetadataName(
//                 "AgentResponseAttribute",
//                 predicate: (node, _) => node is ClassDeclarationSyntax,
//                 transform: (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol
//             )
//             .Collect()
//             .SelectMany((symbols, _) => symbols.Distinct(SymbolEqualityComparer.Default));
//
//         context.RegisterSourceOutput(classSymbols, GenerateSource);
//     }
//
//     private static void GenerateSource(SourceProductionContext context, ISymbol classSymbol)
//     {
//         try
//         {
//             if (classSymbol == null)
//             {
//                 Log("Class symbol is null");
//                 return;
//             }
//
//             // Extract namespace and class name from the symbol
//             string namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToDisplayString();
//             string className = classSymbol.Name;
//
//             string jsonExample = GenerateJsonExample(classSymbol as INamedTypeSymbol);
//             string escapedJsonExample = EscapeStringLiteral(jsonExample);
//
//             Log($"Generating source for {className}");
//             Log($"Namespace: {namespaceName}");
//             Log($"JsonExample: {escapedJsonExample}");
//
//             // Generate the source code
//             string source = string.IsNullOrEmpty(namespaceName)
//                 ? $@"
// public partial class {className}  
// {{
//     public static string JsonExample => @""{escapedJsonExample}"";
// }}
// "
//                 : $@"
// namespace {namespaceName}
// {{
//     public partial class {className}  
//     {{
//         public static string JsonExample => @""{escapedJsonExample}"";
//     }}
// }}
// ";
//
//             // Use namespace in hint name to ensure uniqueness
//             string hintName = string.IsNullOrEmpty(namespaceName) 
//                 ? $"{className}_ResponseExample.g.cs" 
//                 : $"{namespaceName.Replace(".", "_")}_{className}_ResponseExample.g.cs";
//             context.AddSource(hintName, source);
//         }
//         catch (Exception ex)
//         {
//             Log($"Error generating source for {classSymbol?.Name ?? "unknown"}: {ex.Message}");
//         }
//     }
//
//     private static string GenerateJsonExample(INamedTypeSymbol classSymbol)
//     {
//         var properties = classSymbol.GetMembers()
//             .OfType<IPropertySymbol>()
//             .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic);
//
//         if (!properties.Any())
//             return "{}";
//
//         var propertyExamples = properties.Select(p => $"\"{p.Name}\": {GetExampleValue(p.Type)}");
//         return "{" + string.Join(", ", propertyExamples) + "}";
//     }
//
//     private static string GetExampleValue(ITypeSymbol type)
//     {
//         if (type is INamedTypeSymbol namedType)
//         {
//             if (namedType.SpecialType == SpecialType.System_String)
//                 return "\"string\"";
//             if (namedType.TypeKind == TypeKind.Class && namedType.Name != "String")
//                 return GenerateJsonExample(namedType);
//             if (namedType.IsGenericType && namedType.Name == "List")
//                 return "[" + GetExampleValue(namedType.TypeArguments[0]) + "]";
//         }
//         else if (type is IArrayTypeSymbol arrayType)
//         {
//             return "[" + GetExampleValue(arrayType.ElementType) + "]";
//         }
//
//         switch (type.SpecialType)
//         {
//             case SpecialType.System_Int32:
//             case SpecialType.System_Int64:
//             case SpecialType.System_Single:
//             case SpecialType.System_Double:
//                 return "0";
//             case SpecialType.System_Boolean:
//                 return "false";
//             default:
//                 return "\"unknown\"";
//         }
//     }
//
//     private static string EscapeStringLiteral(string value)
//     {
//         return value.Replace("\"", "\"\"");
//     }
//
//     private static void Log(string message)
//     {
//         string logFilePath = Path.Combine("/home/thhel/", "incremental_generator.log");
//         File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
//     }
// }
