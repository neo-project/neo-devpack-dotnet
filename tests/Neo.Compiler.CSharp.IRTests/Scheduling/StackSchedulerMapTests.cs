using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.CSharp.IRTests.Scheduling;

[TestClass]
public sealed class StackSchedulerMapTests
{
    [TestMethod]
    public void MapSets_DuplicateMapReferenceBeforeSetItem()
    {
        var function = new VFunction("TestMap");
        var entry = new VBlock("entry");
        var body = new VBlock("entry_body");
        function.Blocks.Add(entry);
        function.Blocks.Add(body);

        entry.Terminator = new VJmp(body);

        var map = new VMapNew(LirType.TByteString, LirType.TByteString);
        body.Nodes.Add(map);

        for (int i = 0; i < 3; i++)
        {
            var key = new VConstByteString(new[] { (byte)('k' + i) });
            var value = new VConstByteString(new[] { (byte)('v' + i) });
            body.Nodes.Add(key);
            body.Nodes.Add(value);
            body.Nodes.Add(new VMapSet(map, key, value));
        }

        body.Terminator = new VRet(null);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var entryBody = result.Function.Blocks.Single(b => b.Label == "entry_body");
        Assert.AreEqual(3, entryBody.Instructions.Count(i => i.Op == LirOpcode.SETITEM), "Scheduler should emit three SETITEM instructions for the map stores.");

        var verification = new LirVerifier().Verify(result.Function);
        Assert.IsTrue(verification.Ok, $"Scheduled LIR failed verification: {string.Join(System.Environment.NewLine, verification.Errors)}");
    }
}
