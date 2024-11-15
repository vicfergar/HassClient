using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HassClient.Models;
using HassClient.WS;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HassClient.Docs.Tests
{
    public class ReadmeTests
    {
        [Test]
        public void Readme_CodeSamples_ShouldBeValid()
        {
            // Arrange
            var readmePath = Path.Combine(GetSolutionDirectory(), "../README.md");
            var readmeContent = File.ReadAllText(readmePath);
            
            // Extract all C# code blocks
            var codeBlocks = ExtractCodeBlocks(readmeContent);

            // Act & Assert
            foreach (var (code, lineNumber) in codeBlocks)
            {
                try 
                {
                    AssertCompileCode(code);
                }
                catch (AssertionException ex)
                {
                    throw new AssertionException($"Code block at line {lineNumber} failed to compile:\n{ex.Message}");
                }
            }
        }

        private static string GetSolutionDirectory()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory?.FullName ?? throw new Exception("Solution directory not found");
        }

        private static IEnumerable<(string Code, int LineNumber)> ExtractCodeBlocks(string markdown)
        {
            var regex = new Regex(@"```csharp\s*\n(.*?)\n\s*```", RegexOptions.Singleline);
            var matches = regex.Matches(markdown);
            
            foreach (Match match in matches)
            {
                // Count newlines before this match to determine line number
                int lineNumber = markdown[..match.Index].Count(c => c == '\n') + 1;
                
                var code = string.Join("\n", 
                    match.Groups[1].Value.Split('\n')
                        .Where(line => !line.TrimStart().StartsWith("#")) // Remove lines starting with #
                        .Where(line => !string.IsNullOrWhiteSpace(line))  // Remove empty lines
                        .Select(line => line.TrimEnd())                   // Remove trailing whitespace
                );
                
                yield return (code, lineNumber);
            }
        }

        private static void AssertCompileCode(string code)
        {
            var wrappedCode = WrapCodeIfNeeded(code);
            var syntaxTree = CSharpSyntaxTree.ParseText(wrappedCode);
            var compilation = CSharpCompilation.Create("DynamicAssembly",
                new[] { syntaxTree },
                GetRequiredReferences(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            
            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.GetMessage()));
                throw new AssertionException($"Code block failed to compile with errors:\n{errors}\n\nCode:\n{code}");
            }
        }

        private static string WrapCodeIfNeeded(string code)
        {
            if (code.Contains("class "))
                return code;

            var hassApiInit = code.Contains("var hassWSApi") ? "" : 
                "HassWSApi hassWSApi = new HassWSApi();\n";

            if (code.Contains("private ") || code.Contains("public "))
                return WrapWithUsings(WrapInClass(code, hassApiInit));
        
            return WrapWithUsings(WrapInClassAndMethod(code, hassApiInit));
        }

        private static string WrapWithUsings(string code) => @$"
using HassClient;
using HassClient.Models;
using HassClient.WS;
using HassClient.WS.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicNamespace 
{{
    {code}
}}";
        private static string WrapInClass(string code, string hassApiInit) => @$"
public class TestClass 
{{
    {hassApiInit}{code}
}}";

        private static string WrapInClassAndMethod(string code, string hassApiInit) => @$"
public class TestClass 
{{
    public async Task TestMethod()
    {{
        {hassApiInit}{code}
    }}
}}";

        private static MetadataReference[] GetRequiredReferences()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            return new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Service).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(HassWSApi).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "netstandard.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Linq.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Threading.Tasks.dll"))
            };
        }
    }
} 