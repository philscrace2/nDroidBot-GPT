using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Core.nTestar.Base;
using org.testar.monkey.alayer;
using org.testar.protocols;

internal static class ProtocolCompiler
{
    public static string Compile(string protocolSourceDir, string outputAssemblyPath, string assemblyName)
    {
        if (!Directory.Exists(protocolSourceDir))
        {
            throw new DirectoryNotFoundException($"Protocol source directory not found: {protocolSourceDir}");
        }

        string[] sourceFiles = Directory.GetFiles(protocolSourceDir, "*.cs", SearchOption.AllDirectories);
        if (sourceFiles.Length == 0)
        {
            throw new InvalidOperationException($"No protocol .cs files found under: {protocolSourceDir}");
        }

        var syntaxTrees = sourceFiles
            .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file))
            .ToList();

        var references = GetDefaultReferences();

        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        Directory.CreateDirectory(Path.GetDirectoryName(outputAssemblyPath) ?? ".");

        var emitResult = compilation.Emit(outputAssemblyPath);
        if (!emitResult.Success)
        {
            var errors = string.Join(Environment.NewLine, emitResult.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString()));
            throw new InvalidOperationException($"Protocol compilation failed:{Environment.NewLine}{errors}");
        }

        return outputAssemblyPath;
    }

    private static List<MetadataReference> GetDefaultReferences()
    {
        var refs = new List<MetadataReference>();
        AddReference(refs, typeof(object).Assembly);
        AddReference(refs, typeof(Enumerable).Assembly);
        AddReference(refs, typeof(Settings).Assembly);
        AddReference(refs, typeof(Action).Assembly);
        AddReference(refs, typeof(DesktopProtocol).Assembly);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            string location = assembly.Location;
            if (string.IsNullOrWhiteSpace(location))
            {
                continue;
            }

            refs.Add(MetadataReference.CreateFromFile(location));
        }

        return refs;
    }

    private static void AddReference(List<MetadataReference> refs, Assembly assembly)
    {
        if (assembly.IsDynamic)
        {
            return;
        }

        string location = assembly.Location;
        if (string.IsNullOrWhiteSpace(location))
        {
            return;
        }

        if (refs.Any(r => string.Equals(r.Display, location, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        refs.Add(MetadataReference.CreateFromFile(location));
    }
}
