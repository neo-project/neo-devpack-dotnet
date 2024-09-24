using System.Numerics;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler;

partial class MethodConvert
{
    private Instruction Nop()
    {
        return AddInstruction(OpCode.NOP);
    }

    private Instruction PushM1()
    {
        return AddInstruction(OpCode.PUSHM1);
    }

    private Instruction Push0()
    {
        return AddInstruction(OpCode.PUSH0);
    }

    private Instruction Push1()
    {
        return AddInstruction(OpCode.PUSH1);
    }

    private Instruction Push2()
    {
        return AddInstruction(OpCode.PUSH2);
    }

    private Instruction Push3()
    {
        return AddInstruction(OpCode.PUSH3);
    }

    private Instruction Push4()
    {
        return AddInstruction(OpCode.PUSH4);
    }

    private Instruction Push5()
    {
        return AddInstruction(OpCode.PUSH5);
    }

    #region Constants

    private Instruction Abort()
    {
        return AddInstruction(OpCode.ABORT);
    }

    private Instruction Assert()
    {
        return AddInstruction(OpCode.ASSERT);
    }
    private Instruction Throw()
    {
        return AddInstruction(OpCode.THROW);
    }
    #endregion

    #region Stack

    private Instruction Depth()
    {
        return AddInstruction(OpCode.DEPTH);
    }

    private Instruction Drop(int count = 1)
    {
        for (var i = 0; i < count - 1; i++)
        {
            AddInstruction(OpCode.DROP);
        }
        return AddInstruction(OpCode.DROP);
    }

    private Instruction Nip()
    {
        return AddInstruction(OpCode.NIP);
    }

    /// <summary>
    /// Remove n items from the stack.
    ///
    /// <remarks>
    /// Try to use <see cref="OpCode.DROP"/> as much as possible,
    /// it is more efficient.
    /// </remarks>
    ///
    /// </summary>
    /// <param name="count">Number of stack items to be removed.</param>
    /// <returns></returns>
    private Instruction XDrop(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.XDROP);
    }

    private Instruction Clear()
    {
        return AddInstruction(OpCode.CLEAR);
    }

    private Instruction Dup()
    {
        return AddInstruction(OpCode.DUP);
    }

    private Instruction Over()
    {
        return AddInstruction(OpCode.OVER);
    }

    private Instruction Pick(int? index)
    {
        if (index.HasValue)
            Push(index.Value);
        return AddInstruction(OpCode.PICK);
    }

    private Instruction Tuck()
    {
        return AddInstruction(OpCode.TUCK);
    }

    private Instruction Swap()
    {
        return AddInstruction(OpCode.SWAP);
    }

    private Instruction Rot()
    {
        return AddInstruction(OpCode.ROT);
    }

    private Instruction Roll(int? index)
    {
        if (index.HasValue)
            Push(index.Value);
        return AddInstruction(OpCode.ROLL);
    }

    private Instruction Reverse3()
    {
        return AddInstruction(OpCode.REVERSE3);
    }

    private Instruction Reverse4()
    {
        return AddInstruction(OpCode.REVERSE4);
    }

    private Instruction ReverseN(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.REVERSEN);
    }

    #endregion

    #region Splice

    private Instruction NewBuffer(int? size)
    {
        if (size.HasValue)
            Push(size.Value);
        return AddInstruction(OpCode.NEWBUFFER);
    }

    private Instruction Memcpy()
    {
        return AddInstruction(OpCode.MEMCPY);
    }

    private Instruction Cat()
    {
        return AddInstruction(OpCode.CAT);
    }

    private Instruction SubStr()
    {
        return AddInstruction(OpCode.SUBSTR);
    }

    private Instruction Left(int? length)
    {
        if (length.HasValue)
            Push(length.Value);
        return AddInstruction(OpCode.LEFT);
    }

    private Instruction Right(int? length)
    {
        if (length.HasValue)
            Push(length.Value);
        return AddInstruction(OpCode.RIGHT);
    }

    #endregion

    #region Bitwise logic

    private Instruction Invert()
    {
        return AddInstruction(OpCode.INVERT);
    }

    private Instruction And()
    {
        return AddInstruction(OpCode.AND);
    }

    private Instruction Or()
    {
        return AddInstruction(OpCode.OR);
    }

    private Instruction Xor()
    {
        return AddInstruction(OpCode.XOR);
    }

    private Instruction Equal()
    {
        return AddInstruction(OpCode.EQUAL);
    }

    private Instruction NotEqual()
    {
        return AddInstruction(OpCode.NOTEQUAL);
    }

    #endregion

    #region Arithmetic

    private Instruction Sign()
    {
        return AddInstruction(OpCode.SIGN);
    }

    private Instruction Abs()
    {
        return AddInstruction(OpCode.ABS);
    }

    private Instruction Negate()
    {
        return AddInstruction(OpCode.NEGATE);
    }

    private Instruction Inc()
    {
        return AddInstruction(OpCode.INC);
    }

    private Instruction Dec()
    {
        return AddInstruction(OpCode.DEC);
    }

    private Instruction Add()
    {
        return AddInstruction(OpCode.ADD);
    }

    private Instruction Sub()
    {
        return AddInstruction(OpCode.SUB);
    }

    private Instruction Mul()
    {
        return AddInstruction(OpCode.MUL);
    }

    private Instruction Div()
    {
        return AddInstruction(OpCode.DIV);
    }

    private Instruction Mod()
    {
        return AddInstruction(OpCode.MOD);
    }

    private Instruction Pow()
    {
        return AddInstruction(OpCode.POW);
    }

    private Instruction Sqrt()
    {
        return AddInstruction(OpCode.SQRT);
    }

    private Instruction ModMul()
    {
        return AddInstruction(OpCode.MODMUL);
    }

    private Instruction ModPow()
    {
        return AddInstruction(OpCode.MODPOW);
    }

    private Instruction ShL()
    {
        return AddInstruction(OpCode.SHL);
    }

    private Instruction ShR()
    {
        return AddInstruction(OpCode.SHR);
    }

    private Instruction Not()
    {
        return AddInstruction(OpCode.NOT);
    }

    private Instruction BoolAnd()
    {
        return AddInstruction(OpCode.BOOLAND);
    }

    private Instruction BoolOr()
    {
        return AddInstruction(OpCode.BOOLOR);
    }

    private Instruction Nz()
    {
        return AddInstruction(OpCode.NZ);
    }

    private Instruction NumEqual()
    {
        return AddInstruction(OpCode.NUMEQUAL);
    }

    private Instruction NumNotEqual()
    {
        return AddInstruction(OpCode.NUMNOTEQUAL);
    }

    private Instruction Lt()
    {
        return AddInstruction(OpCode.LT);
    }

    private Instruction Le()
    {
        return AddInstruction(OpCode.LE);
    }

    private Instruction Gt()
    {
        return AddInstruction(OpCode.GT);
    }

    private Instruction Ge()
    {
        return AddInstruction(OpCode.GE);
    }

    private Instruction Min()
    {
        return AddInstruction(OpCode.MIN);
    }

    private Instruction Max()
    {
        return AddInstruction(OpCode.MAX);
    }

    private Instruction Within(BigInteger min, BigInteger max)
    {
        Push(min);
        Push(max + 1);
        return AddInstruction(OpCode.WITHIN);
    }

    private void WithinCheck(BigInteger min, BigInteger max)
    {
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Within(min, max);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    #endregion

    #region Compound-type

    private Instruction PackMap()
    {
        return AddInstruction(OpCode.PACKMAP);
    }

    private Instruction PackStruct()
    {
        return AddInstruction(OpCode.PACKSTRUCT);
    }

    private Instruction Pack(int? size)
    {
        if (size.HasValue)
            Push(size.Value);
        return AddInstruction(OpCode.PACK);
    }

    private Instruction UnPack()
    {
        return AddInstruction(OpCode.UNPACK);
    }

    private Instruction NewArray0()
    {
        return AddInstruction(OpCode.NEWARRAY0);
    }

    private Instruction NewArray(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.NEWARRAY);
    }

    private Instruction NewStruct0()
    {
        return AddInstruction(OpCode.NEWSTRUCT0);
    }

    private Instruction NewStruct(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.NEWSTRUCT);
    }

    private Instruction Size()
    {
        return AddInstruction(OpCode.SIZE);
    }

    private Instruction HasKey()
    {
        return AddInstruction(OpCode.HASKEY);
    }

    private Instruction Keys()
    {
        return AddInstruction(OpCode.KEYS);
    }

    private Instruction Values()
    {
        return AddInstruction(OpCode.VALUES);
    }

    private Instruction PickItem(int? index)
    {
        if (index.HasValue)
            Push(index.Value);
        return AddInstruction(OpCode.PICKITEM);
    }

    private Instruction Append()
    {
        return AddInstruction(OpCode.APPEND);
    }

    private Instruction SetItem()
    {
        return AddInstruction(OpCode.SETITEM);
    }

    private Instruction ReverseItems()
    {
        return AddInstruction(OpCode.REVERSEITEMS);
    }

    private Instruction Remove()
    {
        return AddInstruction(OpCode.REMOVE);
    }

    private Instruction ClearItems()
    {
        return AddInstruction(OpCode.CLEARITEMS);
    }

    private Instruction PopItem()
    {
        return AddInstruction(OpCode.POPITEM);
    }

    #endregion

    #region Types

    private Instruction Isnull()
    {
        return AddInstruction(OpCode.ISNULL);
    }

    private Instruction Istype(StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.ISTYPE,
            Operand = [(byte)type]
        });
    }

    private Instruction CONVERT(StackItemType type)
    {
        return ChangeType(type);
    }

    #endregion

    #region Extensions

    private Instruction Abortmsg()
    {
        return AddInstruction(OpCode.ABORTMSG);
    }

    private Instruction Assertmsg()
    {
        return AddInstruction(OpCode.ASSERTMSG);
    }

    private Instruction JumpIfNot(JumpTarget target)
    {
        return Jump(OpCode.JMPIFNOT, target);
    }

    private Instruction Jump(JumpTarget target)
    {
        return Jump(OpCode.JMP, target);
    }

    #endregion
}
