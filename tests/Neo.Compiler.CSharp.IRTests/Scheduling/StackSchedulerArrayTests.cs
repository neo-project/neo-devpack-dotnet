using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.CSharp.IRTests.Scheduling;

[TestClass]
public sealed class StackSchedulerArrayTests
{
    [TestMethod]
    public void ArraySets_DuplicateArrayReferenceBeforeSetItem()
    {
        var function = new VFunction("Test");
        var entry = new VBlock("entry");
        var body = new VBlock("entry_body");
        function.Blocks.Add(entry);
        function.Blocks.Add(body);

        entry.Terminator = new VJmp(body);

        var len = new VConstInt(3);
        body.Nodes.Add(len);
        var array = new VArrayNew(len, LirType.TByteString);
        body.Nodes.Add(array);

        for (int i = 0; i < 3; i++)
        {
            var index = new VConstInt(i);
            body.Nodes.Add(index);

            var value = new VConstByteString(new[] { (byte)('a' + i) });
            body.Nodes.Add(value);

            body.Nodes.Add(new VArraySet(array, index, value));
        }

        body.Terminator = new VRet(null);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var entryBody = result.Function.Blocks.Single(b => b.Label == "entry_body");
        var dupCount = entryBody.Instructions.Count(i => i.Op == LirOpcode.DUP);
        Assert.AreEqual(3, dupCount, "Scheduler should duplicate the array reference before each SETITEM.");

        var verification = new LirVerifier().Verify(result.Function);
        Assert.IsTrue(verification.Ok, $"Scheduled LIR failed verification: {string.Join(System.Environment.NewLine, verification.Errors)}");
    }
}
