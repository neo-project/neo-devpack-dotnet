using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.CSharp.UnitTests.Lir;

[TestClass]
public sealed class LirPeepholeTests
{
    [TestMethod]
    public void Peephole_Removes_DupDrop()
    {
        var function = new LirFunction("dup_drop");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        block.Instructions.Add(new LirInst(LirOpcode.DUP));
        block.Instructions.Add(new LirInst(LirOpcode.DROP));
        block.Instructions.Add(new LirInst(LirOpcode.RET));

        LirPeephole.Run(function);

        Assert.IsFalse(block.Instructions.Any(i => i.Op == LirOpcode.DUP || i.Op == LirOpcode.DROP), "DUP/DROP sequence should be removed.");
    }

    [TestMethod]
    public void Peephole_Removes_PushZero_Add()
    {
        var function = new LirFunction("push0_add");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        block.Instructions.Add(new LirInst(LirOpcode.PUSH0));
        block.Instructions.Add(new LirInst(LirOpcode.ADD));
        block.Instructions.Add(new LirInst(LirOpcode.RET));

        LirPeephole.Run(function);

        Assert.IsFalse(block.Instructions.Any(i => i.Op == LirOpcode.PUSH0 || i.Op == LirOpcode.ADD), "PUSH0/ADD sequence should be removed.");
    }

    [TestMethod]
    public void Peephole_Removes_PushZero_Sub()
    {
        var function = new LirFunction("push0_sub");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        block.Instructions.Add(new LirInst(LirOpcode.PUSH0));
        block.Instructions.Add(new LirInst(LirOpcode.SUB));
        block.Instructions.Add(new LirInst(LirOpcode.RET));

        LirPeephole.Run(function);

        Assert.IsFalse(block.Instructions.Any(i => i.Op == LirOpcode.PUSH0 || i.Op == LirOpcode.SUB), "PUSH0/SUB sequence should be removed.");
    }
}
