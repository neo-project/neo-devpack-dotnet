using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR.Backend;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests.Lir;

[TestClass]
public sealed class OpcodeCoverageTests
{
    private static IReadOnlyDictionary<string, string> LoadBaseline()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "Neo.Compiler.CSharp.UnitTests.Lir.OpcodeCoverageBaseline.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return baseline ?? throw new InvalidOperationException("Failed to deserialize opcode coverage baseline.");
    }

    [TestMethod]
    public void OpcodeCoverage_MatchesBaseline()
    {
        var baseline = LoadBaseline();
        var supported = NeoEmitter.GetSupportedOpcodes();

        var actual = Enum.GetValues<OpCode>()
            .ToDictionary(op => op.ToString(), op => supported.Contains(op) ? "Supported" : "Unsupported", StringComparer.Ordinal);

        var messages = new List<string>();

        foreach (var (opcode, status) in actual.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            if (!baseline.TryGetValue(opcode, out var expected))
            {
                messages.Add($"Missing baseline entry for opcode '{opcode}'. Actual status: {status}.");
                continue;
            }

            if (!string.Equals(expected, status, StringComparison.Ordinal))
                messages.Add($"Opcode '{opcode}' expected '{expected}' but was '{status}'.");
        }

        foreach (var opcode in baseline.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (!actual.ContainsKey(opcode))
                messages.Add($"Baseline lists opcode '{opcode}' but it no longer exists in NeoVM.");
        }

        if (messages.Count > 0)
        {
            Assert.Fail("Opcode coverage diverged from the baseline:\n  - " + string.Join("\n  - ", messages));
        }
    }
}
