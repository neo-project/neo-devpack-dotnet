using Microsoft.CodeAnalysis;
using Neo.VM;
using System;
using System.Linq;
using System.Reflection;

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

        private int _size;
        public int Size
        {
            get
            {
                if (_size == 0)
                {
                    int prefixSize = OperandSizePrefixTable[(int)OpCode];
                    _size = prefixSize > 0
                        ? sizeof(OpCode) + Operand!.Length
                        : sizeof(OpCode) + OperandSizeTable[(int)OpCode];
                }
                return _size;
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
    }
}
