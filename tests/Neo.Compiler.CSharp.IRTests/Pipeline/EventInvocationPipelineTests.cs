using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.Compiler.HIR;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;
using Neo.Compiler.MIR;

namespace Neo.Compiler.CSharp.IRTests.Pipeline;

[TestClass]
public sealed class EventInvocationPipelineTests
{
    [TestMethod]
    public void RaiseEvent_LowersWithoutStackUnderflow()
    {
        const string source = """
using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class InterfaceContract : SmartContract.Framework.SmartContract
    {
        public static event Action<ByteString, ByteString, BigInteger>? OnTransfer;

        public static void Ping()
        {
        }

        public static void Raise(ByteString from, ByteString to, BigInteger amount)
        {
            OnTransfer?.Invoke(from, to, amount);
        }
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.EnableHir = true;
                options.Optimize = CompilationOptions.OptimizationType.All;
            });

            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");
            var context = contexts[0];

            var manifest = context.CreateManifest();
            CollectionAssert.AreEquivalent(
                new[] { "ping", "raise" },
                manifest.Abi.Methods.Select(m => m.Name).OrderBy(n => n).ToArray(),
                "Manifest ABI should include both ping and raise.");
            Assert.AreEqual(1, manifest.Abi.Events.Count(e => e.Name == "OnTransfer"), "Event manifest entry missing.");

            Assert.IsNotNull(context.HirModule, "HIR module should exist when IR pipeline is enabled.");
            Assert.IsNotNull(context.MirModule, "MIR module should exist when IR pipeline is enabled.");
            Assert.IsNotNull(context.LirModule, "LIR module should exist when IR pipeline is enabled.");

            var raiseKey = context.HirModule.Functions.Keys.Single(k => k.Contains("Raise", StringComparison.Ordinal));
            var hirFunction = context.HirModule.Functions[raiseKey];
            var hirArraySets = hirFunction.Blocks.SelectMany(b => b.Instructions).OfType<HirArraySet>().ToList();
            Assert.AreEqual(3, hirArraySets.Count, "HIR should contain three array stores for event arguments.");

            Assert.IsTrue(context.MirModule.Functions.ContainsKey(raiseKey), "MIR function for Raise was not produced.");
            var mirFunction = context.MirModule.Functions[raiseKey];
            var mirArrayNew = mirFunction.Blocks.SelectMany(b => b.Instructions).OfType<MirArrayNew>().SingleOrDefault();
            Assert.IsNotNull(mirArrayNew, "MIR array creation missing.");
            var mirArraySets = mirFunction.Blocks.SelectMany(b => b.Instructions).OfType<MirArraySet>().ToList();
            Assert.AreEqual(3, mirArraySets.Count, "MIR should contain three SETITEM instructions.");
            Assert.IsTrue(mirArraySets.All(set => ReferenceEquals(set.Array, mirArrayNew)),
                "Each array store should use the shared MirArrayNew value.");

            Assert.IsTrue(
                context.LirModule.TryGetCompilation(raiseKey, out var compilation),
                "LIR compilation for Raise was not stored.");

            Assert.IsNotNull(compilation);
            AssertStackDiscipline(compilation.StackFunction);

            var entryBody = compilation.StackFunction.Blocks.Single(b => b.Label == "entry_body");
            var dupCount = entryBody.Instructions.Count(i => i.Op == LirOpcode.DUP);
            Assert.AreEqual(3, dupCount, "Array writes should duplicate the array reference before each SETITEM.");

            var verification = new LirVerifier().Verify(compilation.StackFunction);
            Assert.IsTrue(verification.Ok, $"LIR verification failed: {string.Join(Environment.NewLine, verification.Errors)}");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private static void AssertStackDiscipline(LirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            var stackHeight = 0;
            foreach (var instruction in block.Instructions)
            {
                var opcodeInfo = LirOpcodeTable.Get(instruction.Op);
                var pop = instruction.PopOverride ?? opcodeInfo.Pop;
                if (pop.HasValue)
                {
                    stackHeight -= pop.Value;
                    Assert.IsTrue(
                        stackHeight >= 0,
                        $"Stack underflow detected in block '{block.Label}' for opcode '{instruction.Op}'.");
                }
                else
                {
                    // Unknown stack effect; assume worst case prevents validation for this block.
                    break;
                }

                var push = instruction.PushOverride ?? opcodeInfo.Push;
                if (push.HasValue)
                {
                    stackHeight += push.Value;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
