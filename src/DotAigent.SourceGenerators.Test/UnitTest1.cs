using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace DotAigent.SourceGenerators.Test;

// public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
//     where TSourceGenerator : IIncrementalGenerator, new()
// {
//     public class Test : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
//     {
//         public Test()
//         {
//         }
//
//         protected override CompilationOptions CreateCompilationOptions()
//         {
//             var compilationOptions = base.CreateCompilationOptions();
//             return compilationOptions.WithSpecificDiagnosticOptions(
//                  compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
//         }
//
//         public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;
//
//         private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
//         {
//             string[] args = { "/warnaserror:nullable" };
//             var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
//             var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;
//
//             return nullableWarnings;
//         }
//
//         protected override ParseOptions CreateParseOptions()
//         {
//             return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
//         }
//     }
// }

// public class UnitTest1
// {
//     private const string _exampleClassDefinition = """
//         namespace GeneratorTests;
//
//         public partial class ExampleClass 
//         {
//             public int Sum { get; }
//         }
//         """;
//
//     private static Compilation CreateCompilation(params string[] source)
//     {
//         var path = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
//         var runtimeAssemblyPath = Path.Combine(path, "System.Runtime.dll");
//
//         var runtimeReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
//
//         return CSharpCompilation.Create("compilation",
//                 source.Select(s => CSharpSyntaxTree.ParseText(s)),
//                 [
//                     MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
//                     MetadataReference.CreateFromFile(runtimeAssemblyPath),
//                     // MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
//                     // MetadataReference.CreateFromFile(typeof(External.IExternalService).Assembly.Location),
//                 ],
//                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
//     }
//
//     // [Fact]
//     // public void StaticMethodReturningServices()
//     // {
//     //     var compilation = CreateCompilation(_exampleClassDefinition,
//     //         """
//     //         using ServiceScan.SourceGenerator;
//     //         using Microsoft.Extensions.DependencyInjection;
//     //
//     //         namespace GeneratorTests;
//     //
//     //         public static partial class ServicesExtensions
//     //         {
//     //             [GenerateServiceRegistrations(AssignableTo = typeof(IService))]
//     //             public static partial IServiceCollection AddServices(IServiceCollection services);
//     //         }
//     //         """);
//     //
//     //     var results = CSharpGeneratorDriver
//     //         .Create(_generator)
//     //         .RunGenerators(compilation)
//     //         .GetRunResult();
//     //
//     //     var expected = """
//     //         using Microsoft.Extensions.DependencyInjection;
//     //
//     //         namespace GeneratorTests;
//     //
//     //         public static partial class ServicesExtensions
//     //         {
//     //             public static partial IServiceCollection AddServices( IServiceCollection services)
//     //             {
//     //                 return services
//     //                     .AddTransient<GeneratorTests.IService, GeneratorTests.MyService>();
//     //             }
//     //         }
//     //         """;
//     //
//     //     Assert.Equal(expected, results.GeneratedTrees[1].ToString());
//     // }
//     [Fact]
//     public void Test1()
//     {
//             var userSource = 
//             """
//             namespace GeneratorTests;
//
//             public partial class ExampleClass 
//             {
//                 public int Sum { get; }
//             }
//             """;
//
//         var syntaxTree = CSharpSyntaxTree.ParseText(userSource);
//             var references = new[]
//             {
//                 MetadataReference.CreateFromFile(typeof(object).Assembly.Location) // System.Private.CoreLib
//             };
//
//             var compilation = CSharpCompilation.Create(
//                 "TestCompilation",
//                 [syntaxTree],
//                 references,
//                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
//             );
//         var driver = CSharpGeneratorDriver.Create(new AgentResponseExampleGenerator());
//         driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
//
//         // Step 3: Verify results
//             Assert.Empty(diagnostics); // No errors or warnings
//             Assert.Equal(2, outputCompilation.SyntaxTrees.Count()); // Original + generated
//
//         // Step 4: Check the generated code
//         var generatedTree = outputCompilation.SyntaxTrees.ElementAt(1); // Skip the original source
//         var generatedCode = generatedTree.ToString();
//         var expected = 
//             """
//             using DotAigent.Core;
//                 public partial class AgentSumOutput : IExample
//                 {
//                     public static string JsonExample => @"{
//                 ""Sum"": 42
//             }";
//                 }
//             """;
//         // var source = results.Results[0].GeneratedSources.Single().SourceText.ToString();
//         Assert.Equal(expected, generatedCode);
//
//     }
// }
