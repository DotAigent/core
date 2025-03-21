using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using System.Threading;




namespace DotAigent.SourceGenerators;

// Record to track class info for incremental generation
public record AgentResponseClassInfo(string Namespace, string ClassName, string ClassHash, INamedTypeSymbol ClassSymbol);

[Generator]
public class AgentResponseGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Log("AgentResponseGenerator Initialize");

        // Step 1: Find all class declarations with [AgentResponse] attribute
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Any(),
                transform: (ctx, _) => GetClassWithAgentResponse(ctx))
            .Where(cds => cds != null)!;

        // Step 2: Transform class declarations into class info records with the semantic model and a hash to track changes
        IncrementalValueProvider<Compilation> compilationProvider = context.CompilationProvider;
        IncrementalValuesProvider<(ClassDeclarationSyntax ClassDeclaration, Compilation Compilation)> classesAndCompilations = 
            classDeclarations.Combine(compilationProvider);
            
        // Step 3: Transform to class info with the symbol
        IncrementalValuesProvider<AgentResponseClassInfo> classInfos = classesAndCompilations
            .Select((tuple, ct) => GetClassInfo(tuple.ClassDeclaration, tuple.Compilation, ct));

        // Step 4: Register the source output action
        context.RegisterSourceOutput(classInfos, GenerateSource);
    }

    private static ClassDeclarationSyntax? GetClassWithAgentResponse(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
                if (symbol is IMethodSymbol attributeConstructor &&
                    attributeConstructor.ContainingType.Name == "AgentResponseAttribute")
                {
                    return classDeclaration;
                }
            }
        }
        return null;
    }

    private static AgentResponseClassInfo GetClassInfo(ClassDeclarationSyntax classDeclaration, Compilation compilation, CancellationToken ct)
    {
        var namespaceName = GetNamespace(classDeclaration);
        var className = classDeclaration.Identifier.Text;
        // Create a hash of the class declaration to track changes
        var hash = classDeclaration.GetHashCode().ToString();
        
        // Get the symbol for the class
        var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

        return new AgentResponseClassInfo(namespaceName, className, hash, symbol);
    }


    private static string GetNamespace(ClassDeclarationSyntax classDeclaration)
    {
        // Get namespace from either namespace declaration or file-scoped namespace
        var namespaceDeclaration = classDeclaration.Parent as NamespaceDeclarationSyntax;
        if (namespaceDeclaration != null)
            return namespaceDeclaration.Name.ToString();

        var fileScopedNamespace = classDeclaration.SyntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<FileScopedNamespaceDeclarationSyntax>()
            .FirstOrDefault();

        if (fileScopedNamespace != null)
            return fileScopedNamespace.Name.ToString();

        return string.Empty; // Global namespace
    }

  private static void GenerateSource(SourceProductionContext context, AgentResponseClassInfo classInfo)
    {
        try
        {
            var classSymbol = classInfo.ClassSymbol;
            
            if (classSymbol == null)
            {
                Log($"Could not find symbol for class {classInfo.ClassName} in namespace {classInfo.Namespace}");
                return;
            }


            string jsonExample = GenerateJsonExample(classSymbol);
            string escapedJsonExample = EscapeStringLiteral(jsonExample);

            Log($"Generating source for {classInfo.ClassName}");
            Log($"Namespace: {classInfo.Namespace}");
            Log($"JsonExample: {escapedJsonExample}");

            // Generate the source code
            var source = string.IsNullOrEmpty(classInfo.Namespace)
                ? $@"
    public partial class {classInfo.ClassName}  
    {{
        public static string JsonExample => @""{escapedJsonExample}"";
    }}
"
                : $@"
namespace {classInfo.Namespace}
{{
    public partial class {classInfo.ClassName}  
    {{
        public static string JsonExample => @""{escapedJsonExample}"";
    }}
}}";

            context.AddSource($"{classInfo.ClassName}_ResponseExample.g.cs", source);
        }
        catch (Exception ex)
        {
            Log($"Error generating source for {classInfo.ClassName}: {ex.Message}");
        }
    }

    private static string GenerateJsonExample(INamedTypeSymbol classSymbol)
    {
        var properties = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic);

        if (!properties.Any())
            return "{}";

        var propertyExamples = properties.Select(p => $"\"{p.Name}\": {GetExampleValue(p.Type)}");
        return "{" + string.Join(", ", propertyExamples) + "}";
    }

    private static string GetExampleValue(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType)
        {
            if (namedType.SpecialType == SpecialType.System_String)
                return "\"string\"";
            if (namedType.TypeKind == TypeKind.Class && namedType.Name != "String")
                return GenerateJsonExample(namedType);
            if (namedType.IsGenericType && namedType.Name == "List")
                return "[" + GetExampleValue(namedType.TypeArguments[0]) + "]";
        }
        else if (type is IArrayTypeSymbol arrayType)
        {
            return "[" + GetExampleValue(arrayType.ElementType) + "]";
        }

        switch (type.SpecialType)
        {
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
                return "0";
            case SpecialType.System_Boolean:
                return "false";
            default:
                return "\"unknown\"";
        }
    }

    private static string EscapeStringLiteral(string value)
    {
        // Escape double quotes and other special characters for C# string literal
        return value.Replace("\"", "\"\"");
    }
    private static void Log(string message)
    {
        string logFilePath = Path.Combine("/home/thhel/", "incremental_generator.log");
        File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
    }

}
