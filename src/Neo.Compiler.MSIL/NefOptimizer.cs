using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler
{
    public class NefOptimizer
    {
        public interface IJump
        {
            /// <summary>
            /// Offset
            /// </summary>
            int Offset { get; set; }
        }

        [DebuggerDisplay("Offset={Offset}")]
        public class JumpI32 : IJump
        {
            private readonly NefInstruction _instruction;

            public int Offset
            {
                get => BinaryPrimitives.ReadInt32LittleEndian(_instruction.Operand.AsSpan());
                set { BinaryPrimitives.WriteInt32LittleEndian(_instruction.Operand, value); }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="instruction">Instruction</param>
            public JumpI32(NefInstruction instruction)
            {
                _instruction = instruction;
            }
        }

        [DebuggerDisplay("Offset={Offset}")]
        public class JumpI8 : IJump
        {
            private readonly NefInstruction _instruction;

            public int Offset
            {
                get => _instruction.Operand[0];
                set { _instruction.Operand[0] = (byte)value; }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="instruction">Instruction</param>
            public JumpI8(NefInstruction instruction)
            {
                _instruction = instruction;
            }
        }

        [DebuggerDisplay("Offset={Offset}, OpCode={OpCode}, Size={Size}")]
        public class NefInstruction
        {
            private static readonly ConstructorInfo InstructionConstructor;
            private static readonly int[] OperandSizePrefixTable = new int[256];

            private readonly Instruction _instruction;

            /// <summary>
            /// Offset
            /// </summary>
            public int Offset { get; internal set; }

            /// <summary>
            /// OpCode
            /// </summary>
            public OpCode OpCode => _instruction.OpCode;

            /// <summary>
            /// Size
            /// </summary>
            public int Size => _instruction.Size;

            /// <summary>
            /// Operand
            /// </summary>
            public byte[] Operand { get; internal set; }

            /// <summary>
            /// Jump
            /// </summary>
            public IJump Jump { get; }

            /// <summary>
            /// Static constructor
            /// </summary>
            static NefInstruction()
            {
                foreach (var field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                    if (attribute == null) continue;
                    int index = (int)(OpCode)field.GetValue(null);
                    OperandSizePrefixTable[index] = attribute.SizePrefix;
                }

                InstructionConstructor = typeof(Instruction).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(u => u.GetParameters().Length == 2)
                    .FirstOrDefault();
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="script">Script</param>
            /// <param name="offset">Offset</param>
            public NefInstruction(byte[] script, int offset)
            {
                Offset = offset;
                _instruction = (Instruction)InstructionConstructor.Invoke(new object[] { script, offset });
                Operand = _instruction.Operand.ToArray();

                switch (_instruction.OpCode)
                {
                    case OpCode.PUSHA:
                    case OpCode.CALL_L:

                    case OpCode.JMP_L:
                    case OpCode.JMPIF_L:
                    case OpCode.JMPLE_L:
                    case OpCode.JMPLT_L:
                    case OpCode.JMPNE_L:
                    case OpCode.JMPIFNOT_L:
                    case OpCode.JMPEQ_L:
                    case OpCode.JMPGE_L:
                    case OpCode.JMPGT_L: Jump = new JumpI32(this); break;

                    case OpCode.CALL:

                    case OpCode.JMP:
                    case OpCode.JMPIF:
                    case OpCode.JMPLE:
                    case OpCode.JMPLT:
                    case OpCode.JMPNE:
                    case OpCode.JMPIFNOT:
                    case OpCode.JMPEQ:
                    case OpCode.JMPGE:
                    case OpCode.JMPGT: Jump = new JumpI8(this); break;
                }
            }

            /// <summary>
            /// Serialize
            /// </summary>
            /// <param name="stream">Stream</param>
            public void Serialize(Stream stream)
            {
                stream.WriteByte((byte)OpCode);

                int operandSizePrefix = OperandSizePrefixTable[(int)OpCode];

                switch (operandSizePrefix)
                {
                    case 0: break;
                    case 1:
                        {
                            stream.WriteByte((byte)Operand.Length);
                            break;
                        }
                    case 2:
                        {
                            var operandSize = BitConverter.GetBytes((ushort)Operand.Length);
                            stream.Write(operandSize, 0, operandSize.Length);
                            break;
                        }
                    case 4:
                        {
                            var operandSize = BitConverter.GetBytes((int)Operand.Length);
                            stream.Write(operandSize, 0, operandSize.Length);
                            break;
                        }
                }

                if (Operand?.Length > 0)
                {
                    stream.Write(Operand, 0, Operand.Length);
                }
            }
        }

        /// <summary>
        /// Instructions
        /// </summary>
        public readonly List<NefInstruction> Instructions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="script">Script</param>
        public NefOptimizer(byte[] script)
        {
            Instructions = new List<NefInstruction>();

            for (int offset = 0; offset < script.Length;)
            {
                var instruction = new NefInstruction(script, offset);
                offset += instruction.Size;

                Instructions.Add(instruction);
            }
        }

        /// <summary>
        /// Optimize
        /// </summary>
        /// <returns>Obtimized contract</returns>
        public byte[] Optimize()
        {
            RemoveNops();

            return Dump();
        }

        /// <summary>
        /// Dump
        /// </summary>
        /// <returns>Nef</returns>
        private byte[] Dump()
        {
            using (var stream = new MemoryStream())
            {
                foreach (var instruction in Instructions)
                {
                    instruction.Serialize(stream);
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Remove nops
        /// </summary>
        private void RemoveNops()
        {
            for (int x = 0; x < Instructions.Count;)
            {
                var ins = Instructions[x];

                if (ins.OpCode == OpCode.NOP)
                {
                    RemoveAt(x);
                }
                else
                {
                    x++;
                }
            }
        }

        /// <summary>
        /// Remove At
        /// </summary>
        /// <param name="index">Index</param>
        private void RemoveAt(int index)
        {
            // Find instructions

            var remove = Instructions[index];
            Instructions.RemoveAt(index);

            // Recalculate offsets

            for (int offset = remove.Offset; index < Instructions.Count; index++)
            {
                var ins = Instructions[index];
                ins.Offset = offset;
                offset += ins.Size;
            }

            // Recalculate jumps

            RecalculateJumps(remove.Offset, remove.Size);
        }

        /// <summary>
        /// Recalculate jumps
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="size">Size</param>
        private void RecalculateJumps(int offset, int size)
        {
            foreach (var instruction in Instructions)
            {
                // Find jumps

                if (instruction.Jump == null) continue;

                // Recalculate positive jumps

                if (instruction.Jump.Offset > 0 && instruction.Jump.Offset >= offset)
                {
                    instruction.Jump.Offset -= size;
                }

                // Recalculate negative jumps

                else if (instruction.Jump.Offset < 0 && instruction.Jump.Offset <= offset)
                {
                    instruction.Jump.Offset += size;
                }
            }
        }
    }
}
