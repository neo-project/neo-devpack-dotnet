// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Neo.Compiler;

internal static class SarifDiagnosticWriter
{
    public static byte[] Serialize(IEnumerable<Diagnostic> diagnostics)
    {
        var results = diagnostics
            .DistinctBy(d => (d.Id, d.Severity, d.Location, d.GetMessage(CultureInfo.InvariantCulture)))
            .OrderBy(d => d.Location.GetMappedLineSpan().Path, StringComparer.Ordinal)
            .ThenBy(d => d.Location.SourceSpan.Start)
            .ThenBy(d => d.Location.SourceSpan.Length)
            .ThenBy(d => d.Id, StringComparer.Ordinal)
            .ThenBy(d => d.Severity)
            .ThenBy(d => d.GetMessage(CultureInfo.InvariantCulture), StringComparer.Ordinal)
            .ToArray();
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            writer.WriteString("$schema", "https://json.schemastore.org/sarif-2.1.0.json");
            writer.WriteString("version", "2.1.0");
            writer.WriteStartArray("runs");
            writer.WriteStartObject();
            writer.WriteString("columnKind", "utf16CodeUnits");
            writer.WriteStartObject("tool");
            writer.WriteStartObject("driver");
            writer.WriteString("name", "Neo.Compiler.CSharp");
            writer.WriteStartArray("rules");
            foreach (var diagnostic in results.DistinctBy(d => d.Id).OrderBy(d => d.Id, StringComparer.Ordinal))
            {
                writer.WriteStartObject();
                writer.WriteString("id", diagnostic.Id);
                var title = diagnostic.Descriptor.Title.ToString(CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(title))
                {
                    writer.WriteStartObject("shortDescription");
                    writer.WriteString("text", title);
                    writer.WriteEndObject();
                }
                if (Uri.TryCreate(diagnostic.Descriptor.HelpLinkUri, UriKind.Absolute, out var helpUri))
                    writer.WriteString("helpUri", helpUri.AbsoluteUri);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.WriteEndObject();
            writer.WriteStartArray("results");
            foreach (var diagnostic in results)
            {
                writer.WriteStartObject();
                writer.WriteString("ruleId", diagnostic.Id);
                writer.WriteString("level", diagnostic.Severity switch
                {
                    DiagnosticSeverity.Error => "error",
                    DiagnosticSeverity.Warning => "warning",
                    DiagnosticSeverity.Info => "note",
                    _ => "none"
                });
                writer.WriteStartObject("message");
                writer.WriteString("text", diagnostic.GetMessage(CultureInfo.InvariantCulture));
                writer.WriteEndObject();
                WriteLocation(writer, diagnostic.Location);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        return stream.ToArray();
    }

    private static void WriteLocation(Utf8JsonWriter writer, Location location)
    {
        if (!location.IsInSource) return;
        var span = location.GetMappedLineSpan();
        if (!span.IsValid || string.IsNullOrEmpty(span.Path)) return;
        writer.WriteStartArray("locations");
        writer.WriteStartObject();
        writer.WriteStartObject("physicalLocation");
        writer.WriteStartObject("artifactLocation");
        var sourceDirectory = Path.GetDirectoryName(location.SourceTree?.FilePath);
        var path = Path.GetFullPath(span.Path, string.IsNullOrEmpty(sourceDirectory) ? Environment.CurrentDirectory : Path.GetFullPath(sourceDirectory));
        writer.WriteString("uri", new Uri(path).AbsoluteUri);
        writer.WriteEndObject();
        writer.WriteStartObject("region");
        writer.WriteNumber("startLine", span.StartLinePosition.Line + 1);
        writer.WriteNumber("startColumn", span.StartLinePosition.Character + 1);
        if (span.EndLinePosition > span.StartLinePosition)
        {
            writer.WriteNumber("endLine", span.EndLinePosition.Line + 1);
            writer.WriteNumber("endColumn", span.EndLinePosition.Character + 1);
        }
        writer.WriteEndObject();
        writer.WriteEndObject();
        writer.WriteEndObject();
        writer.WriteEndArray();
    }
}
