using Microsoft.CodeAnalysis;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Neo.Compiler
{
    class Instruction
    {
        private static readonly int[] OperandSizePrefixTable = new int[256];
        private static readonly int[] OperandSizeTable = new int[256];

        public OpCode OpCode;
        public byte[]? Operand;
        public JumpTarget? Target;
        public JumpTarget? Target2;
        public int Offset;
        public Location? SourceLocation;

        public int Size
        {
            get
            {
                int prefixSize = OperandSizePrefixTable[(int)OpCode];
                return prefixSize > 0
                    ? sizeof(OpCode) + Operand!.Length
                    : sizeof(OpCode) + OperandSizeTable[(int)OpCode];
            }
        }

        static Instruction()
        {
            foreach (FieldInfo field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OperandSizeAttribute? attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                if (attribute is null) continue;
                int index = (int)(OpCode)field.GetValue(null)!;
                OperandSizePrefixTable[index] = attribute.SizePrefix;
                OperandSizeTable[index] = attribute.Size;
            }
        }

        public Instruction Clone()
        {
            if (Target is not null || Target2 is not null)
                throw new InvalidOperationException();
            return new Instruction
            {
                OpCode = OpCode,
                Operand = Operand,
                SourceLocation = SourceLocation
            };
        }

        public byte[] ToArray()
        {
            if (Operand is null) return new[] { (byte)OpCode };
            return Operand.Prepend((byte)OpCode).ToArray();
        }

        public void ToString(StringBuilder builder)
        {
            switch (OpCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                case OpCode.PUSHNULL:
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                case OpCode.PUSHM1:
                case OpCode.PUSH0:
                case OpCode.PUSH1:
                case OpCode.PUSH2:
                case OpCode.PUSH3:
                case OpCode.PUSH4:
                case OpCode.PUSH5:
                case OpCode.PUSH6:
                case OpCode.PUSH7:
                case OpCode.PUSH8:
                case OpCode.PUSH9:
                case OpCode.PUSH10:
                case OpCode.PUSH11:
                case OpCode.PUSH12:
                case OpCode.PUSH13:
                case OpCode.PUSH14:
                case OpCode.PUSH15:
                case OpCode.PUSH16:
                    builder.Append("PUSH");
                    break;
                case OpCode.PUSHA:
                case OpCode.JMP:
                case OpCode.JMPIF:
                case OpCode.JMPIFNOT:
                case OpCode.JMPEQ:
                case OpCode.JMPNE:
                case OpCode.JMPGT:
                case OpCode.JMPGE:
                case OpCode.JMPLT:
                case OpCode.JMPLE:
                case OpCode.CALL:
                case OpCode.CALLT:
                case OpCode.TRY:
                case OpCode.ENDTRY:
                case OpCode.SYSCALL:
                case OpCode.INITSSLOT:
                case OpCode.INITSLOT:
                case OpCode.NEWARRAY_T:
                case OpCode.ISTYPE:
                case OpCode.CONVERT:
                    builder.Append(OpCode);
                    break;
                case OpCode.JMP_L:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE_L:
                case OpCode.CALL_L:
                case OpCode.TRY_L:
                case OpCode.ENDTRY_L:
                    builder.Append(OpCode - 1);
                    break;
                case OpCode.LDSFLD0:
                case OpCode.LDSFLD1:
                case OpCode.LDSFLD2:
                case OpCode.LDSFLD3:
                case OpCode.LDSFLD4:
                case OpCode.LDSFLD5:
                case OpCode.LDSFLD6:
                case OpCode.LDSFLD:
                    builder.Append(OpCode.LDSFLD);
                    break;
                case OpCode.STSFLD0:
                case OpCode.STSFLD1:
                case OpCode.STSFLD2:
                case OpCode.STSFLD3:
                case OpCode.STSFLD4:
                case OpCode.STSFLD5:
                case OpCode.STSFLD6:
                case OpCode.STSFLD:
                    builder.Append(OpCode.STSFLD);
                    break;
                case OpCode.LDLOC0:
                case OpCode.LDLOC1:
                case OpCode.LDLOC2:
                case OpCode.LDLOC3:
                case OpCode.LDLOC4:
                case OpCode.LDLOC5:
                case OpCode.LDLOC6:
                case OpCode.LDLOC:
                    builder.Append(OpCode.LDLOC);
                    break;
                case OpCode.STLOC0:
                case OpCode.STLOC1:
                case OpCode.STLOC2:
                case OpCode.STLOC3:
                case OpCode.STLOC4:
                case OpCode.STLOC5:
                case OpCode.STLOC6:
                case OpCode.STLOC:
                    builder.Append(OpCode.STLOC);
                    break;
                case OpCode.LDARG0:
                case OpCode.LDARG1:
                case OpCode.LDARG2:
                case OpCode.LDARG3:
                case OpCode.LDARG4:
                case OpCode.LDARG5:
                case OpCode.LDARG6:
                case OpCode.LDARG:
                    builder.Append(OpCode.LDARG);
                    break;
                case OpCode.STARG0:
                case OpCode.STARG1:
                case OpCode.STARG2:
                case OpCode.STARG3:
                case OpCode.STARG4:
                case OpCode.STARG5:
                case OpCode.STARG6:
                case OpCode.STARG:
                    builder.Append(OpCode.STARG);
                    break;
                default:
                    builder.Append(OpCode);
                    return;
            }
            builder.Append(' ');
            switch (OpCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    builder.Append(new BigInteger(Operand!));
                    break;
                case OpCode.PUSHNULL:
                    builder.Append("<null>");
                    break;
                case OpCode.PUSHDATA1:
                    builder.Append($"[{Convert.ToHexString(Operand.AsSpan(1))}]");
                    if (TryGetString(Operand.AsSpan(1), out string? s))
                        builder.Append($" // {s}");
                    break;
                case OpCode.PUSHDATA2:
                    builder.Append($"[{Convert.ToHexString(Operand.AsSpan(2))}]");
                    if (TryGetString(Operand.AsSpan(1), out s))
                        builder.Append($" // {s}");
                    break;
                case OpCode.PUSHDATA4:
                    builder.Append($"[{Convert.ToHexString(Operand.AsSpan(4))}]");
                    if (TryGetString(Operand.AsSpan(1), out s))
                        builder.Append($" // {s}");
                    break;
                case OpCode.PUSHM1:
                    builder.Append(-1);
                    break;
                case OpCode.PUSH0:
                case OpCode.PUSH1:
                case OpCode.PUSH2:
                case OpCode.PUSH3:
                case OpCode.PUSH4:
                case OpCode.PUSH5:
                case OpCode.PUSH6:
                case OpCode.PUSH7:
                case OpCode.PUSH8:
                case OpCode.PUSH9:
                case OpCode.PUSH10:
                case OpCode.PUSH11:
                case OpCode.PUSH12:
                case OpCode.PUSH13:
                case OpCode.PUSH14:
                case OpCode.PUSH15:
                case OpCode.PUSH16:
                    builder.Append(OpCode - OpCode.PUSH0);
                    break;
                case OpCode.PUSHA:
                case OpCode.JMP:
                case OpCode.JMP_L:
                case OpCode.JMPIF:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE:
                case OpCode.JMPLE_L:
                case OpCode.CALL:
                case OpCode.CALL_L:
                case OpCode.ENDTRY:
                case OpCode.ENDTRY_L:
                    if (Target is null)
                        builder.Append($"<{Offset + (int)new BigInteger(Operand!):x8}>");
                    else
                        builder.Append($"<{Target.Instruction!.Offset:x8}>");
                    break;
                case OpCode.CALLT:
                    builder.Append(BitConverter.ToUInt16(Operand));
                    break;
                case OpCode.TRY:
                case OpCode.TRY_L:
                    if (Target!.Instruction is null)
                        builder.Append("<null>");
                    else
                        builder.Append($"<{Target.Instruction.Offset:x8}>");
                    builder.Append(", ");
                    if (Target2!.Instruction is null)
                        builder.Append("<null>");
                    else
                        builder.Append($"<{Target2.Instruction.Offset:x8}>");
                    break;
                case OpCode.SYSCALL:
                    builder.Append(ApplicationEngine.Services[BitConverter.ToUInt32(Operand)].Name);
                    break;
                case OpCode.INITSSLOT:
                case OpCode.LDSFLD:
                case OpCode.STSFLD:
                case OpCode.LDLOC:
                case OpCode.STLOC:
                case OpCode.LDARG:
                case OpCode.STARG:
                    builder.Append(Operand![0]);
                    break;
                case OpCode.INITSLOT:
                    builder.Append(Operand![0]);
                    builder.Append(", ");
                    builder.Append(Operand[1]);
                    break;
                case OpCode.LDSFLD0:
                case OpCode.LDSFLD1:
                case OpCode.LDSFLD2:
                case OpCode.LDSFLD3:
                case OpCode.LDSFLD4:
                case OpCode.LDSFLD5:
                case OpCode.LDSFLD6:
                    builder.Append(OpCode - OpCode.LDSFLD0);
                    break;
                case OpCode.STSFLD0:
                case OpCode.STSFLD1:
                case OpCode.STSFLD2:
                case OpCode.STSFLD3:
                case OpCode.STSFLD4:
                case OpCode.STSFLD5:
                case OpCode.STSFLD6:
                    builder.Append(OpCode - OpCode.STSFLD0);
                    break;
                case OpCode.LDLOC0:
                case OpCode.LDLOC1:
                case OpCode.LDLOC2:
                case OpCode.LDLOC3:
                case OpCode.LDLOC4:
                case OpCode.LDLOC5:
                case OpCode.LDLOC6:
                    builder.Append(OpCode - OpCode.LDLOC0);
                    break;
                case OpCode.STLOC0:
                case OpCode.STLOC1:
                case OpCode.STLOC2:
                case OpCode.STLOC3:
                case OpCode.STLOC4:
                case OpCode.STLOC5:
                case OpCode.STLOC6:
                    builder.Append(OpCode - OpCode.STLOC0);
                    break;
                case OpCode.LDARG0:
                case OpCode.LDARG1:
                case OpCode.LDARG2:
                case OpCode.LDARG3:
                case OpCode.LDARG4:
                case OpCode.LDARG5:
                case OpCode.LDARG6:
                    builder.Append(OpCode - OpCode.LDARG0);
                    break;
                case OpCode.STARG0:
                case OpCode.STARG1:
                case OpCode.STARG2:
                case OpCode.STARG3:
                case OpCode.STARG4:
                case OpCode.STARG5:
                case OpCode.STARG6:
                    builder.Append(OpCode - OpCode.STARG0);
                    break;
                case OpCode.NEWARRAY_T:
                case OpCode.ISTYPE:
                case OpCode.CONVERT:
                    builder.Append($"<{(StackItemType)Operand![0]}>");
                    break;
                default:
                    builder.Append(Operand.ToHexString());
                    break;
            }
        }

        private static bool TryGetString(Span<byte> span, [NotNullWhen(true)] out string? s)
        {
            try
            {
                s = Utility.StrictUTF8.GetString(span);
                return true;
            }
            catch
            {
                s = null;
                return false;
            }
        }
    }
}
