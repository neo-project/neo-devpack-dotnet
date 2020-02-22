using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.Optimizer
{
    public class NefOptimizer
    {
        //public interface IJump
        //{
        //    /// <summary>
        //    /// Offset
        //    /// </summary>
        //    int Offset { get; set; }
        //}

        //[DebuggerDisplay("Offset={Offset}")]
        //public class JumpI32 : IJump
        //{
        //    private readonly NefInstruction _instruction;

        //    public int Offset
        //    {
        //        get => BinaryPrimitives.ReadInt32LittleEndian(_instruction.Operand.AsSpan());
        //        set { BinaryPrimitives.WriteInt32LittleEndian(_instruction.Operand, value); }
        //    }

        //    /// <summary>
        //    /// Constructor
        //    /// </summary>
        //    /// <param name="instruction">Instruction</param>
        //    public JumpI32(NefInstruction instruction)
        //    {
        //        _instruction = instruction;
        //    }
        //}

        //[DebuggerDisplay("Offset={Offset}")]
        //public class JumpI8 : IJump
        //{
        //    private readonly NefInstruction _instruction;

        //    public int Offset
        //    {
        //        get => (sbyte)_instruction.Operand[0];
        //        set { _instruction.Operand[0] = (byte)(sbyte)value; }
        //    }

        //    /// <summary>
        //    /// Constructor
        //    /// </summary>
        //    /// <param name="instruction">Instruction</param>
        //    public JumpI8(NefInstruction instruction)
        //    {
        //        _instruction = instruction;
        //    }
        //}


        /// <summary>
        /// Instructions
        /// </summary>
        private List<NefInstruction> Instructions;

        public NefOptimizer(byte[] script = null)
        {
            if (script != null)
            {
                using (var ms = new System.IO.MemoryStream(script))
                {
                    LoadNef(ms);
                }
            }
        }
        //Step01 Load
        public void LoadNef(System.IO.Stream stream)
        {
            if (Instructions == null)
                Instructions = new List<NefInstruction>();
            else
                Instructions.Clear();
            NefInstruction _inst = null;
            do
            {
                _inst = NefInstruction.ReadFrom(stream);
                if (_inst != null)
                    this.Instructions.Add(_inst);
            } while (_inst != null);

            //Add Labels
        }

        //Step02

        //Step03 Link
        public void LinkNef(System.IO.Stream stream)
        {
            //Recalc Address


            //and Link
            foreach (var _inst in this.Instructions)
            {
                NefInstruction.WriteTo(_inst, stream);
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="script">Script</param>

        //Step1.LoadInstructWithLabel
        //Step2.DoOptimze(blabla);

        static NefOptimizer _optimizer;
        public static byte[] Optimize(byte[] script)
        {
            if (_optimizer == null)
                _optimizer = new NefOptimizer();
            //step01 Load
            using (var ms = new System.IO.MemoryStream(script))
            {
                _optimizer.LoadNef(ms);
            }
            //step02 doOptimize
            _optimizer.RemoveNops();
            _optimizer.RecalculeLongJumps();
            //step03 link
            using (var ms = new System.IO.MemoryStream())
            {
                _optimizer.LinkNef(ms);
                var bytes = ms.ToArray();
                return bytes;
            }
        }

        /// <summary>
        /// Remove nops
        /// </summary>
        public void RemoveNops()
        {
            //for (int x = Instructions.Count - 1; x >= 0; x--)
            //{
            //    var ins = Instructions[x];
            //    if (ins.OpCode == OpCode.NOP)
            //    {
            //        Instructions.RemoveAt(x);
            //    }
            //}
        }

        /// <summary>
        /// Recalculate long jumps
        /// </summary>
        public void RecalculeLongJumps()
        {
            //for (int x = 0; x < Instructions.Count;)
            //{
            //    var ins = Instructions[x++];

            //    if (ins.OpCode == OpCode.PUSHA ||
            //        !(ins.Jump is JumpI32 jmp)) continue;

            //    if (jmp.Offset > sbyte.MaxValue) continue;
            //    if (jmp.Offset < sbyte.MinValue) continue;

            //    // Remove _L

            //    ins.OpCode = (OpCode)(((byte)ins.OpCode) - 1);
            //    ins.Operand = new byte[] { ins.Operand[0] };
            //    ins.Size -= 3;
            //    ins.Jump = new JumpI8(ins);

            //    // Recalculate offsets

            //    for (int index = x; index < Instructions.Count; index++)
            //    {
            //        Instructions[index].Offset -= 3;
            //    }

            //    // Recalculate jumps
            //    RecalculateJumpsForLongJump(Instructions, ins.Offset, 3, ins.Jump.Offset);
            //}
        }

    }
}
