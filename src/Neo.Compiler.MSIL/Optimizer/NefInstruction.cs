using Neo.Compiler.MSIL;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler.Optimizer
{
    [DebuggerDisplay("Offset={Offset}, OpCode={OpCode}, Size={Data}")]
    public class NefInstruction
    {
        private static readonly uint[] OperandSizePrefixTable = new uint[256];
        private static readonly uint[] OperandSizeTable = new uint[256];
        public static uint GetOperandSize(OpCode opcode)
        {
            return OperandSizeTable[(int)opcode];
        }
        public static uint GetOperandPrefixSize(OpCode opcode)
        {
            return OperandSizePrefixTable[(int)opcode];
        }
        static NefInstruction()
        {
            foreach (var field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                if (attribute == null) continue;
                int index = (int)(OpCode)field.GetValue(null);
                OperandSizePrefixTable[index] = (uint)attribute.SizePrefix;
                OperandSizeTable[index] = (uint)attribute.Size;
            }
        }


        /// <summary>
        /// OpCode
        /// </summary>
        public OpCode OpCode { get; private set; }


        /// <summary>
        /// Size
        /// </summary>
        public uint CalcTotalSize
        {
            get
            {
                if (DataPrefixSize > 0)
                    return (uint)(1 + DataPrefixSize + Data.Length);
                else
                    return (uint)(1 + DataSize);
            }
        }
        public uint DataPrefixSize => GetOperandPrefixSize(OpCode);
        public uint DataSize
        {
            get
            {
                if (DataPrefixSize > 0)
                {
                    return Data != null ? (uint)Data.Length : 0;
                }
                else
                {
                    return GetOperandSize(OpCode);
                }
            }
        }
        /// <summary>
        /// Operand
        /// </summary>
        public byte[] Data
        {
            get;
            private set;
        }

        public void SetData(byte[] _Data)
        {
            if (this.DataPrefixSize > 0)
            {
                this.Data = _Data;
            }
            else
            {
                if (this.DataSize == 0)
                {
                    if (_Data != null && _Data.Length > 0)
                        throw new Exception("error DataSize");
                }
                else
                {
                    if (_Data == null)
                        return;
                    if (_Data.Length != this.DataSize)
                        throw new Exception("error DataSize");

                    Data = _Data;

                }
            }
        }
        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; private set; }

        public string[] labels { get; private set; }
        public int AddrLen { get; private set; }

        public void SetOffset(int offset)
        {
            this.Offset = offset;
        }
        public void SetOpCode(OpCode _OpCode)
        {
            this.OpCode = _OpCode;

            uint opprefix = GetOperandPrefixSize(_OpCode);
            if (opprefix == 0)
            {
                uint oplen = GetOperandSize(_OpCode);
                if (oplen > 0)
                {
                    if (Data == null)
                        Data = new byte[oplen];
                    else if (Data.Length != oplen)
                    {
                        byte[] newdata = new byte[oplen];
                        Array.Copy(Data, 0, newdata, 0, Math.Min(Data.Length, oplen));
                    }
                }
                else
                    Data = null;
            }

            AddrLen = 0;
            switch (_OpCode)
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
                case OpCode.JMPGT_L:
                    labels = new string[1];
                    AddrLen = 4;
                    break;
                case OpCode.CALL:

                case OpCode.JMP:
                case OpCode.JMPIF:
                case OpCode.JMPLE:
                case OpCode.JMPLT:
                case OpCode.JMPNE:
                case OpCode.JMPIFNOT:
                case OpCode.JMPEQ:
                case OpCode.JMPGE:
                case OpCode.JMPGT:
                    labels = new string[1];
                    AddrLen = 1;

                    break;
            }
        }
        public NefInstruction(OpCode _OpCode, byte[] _Data = null, int _Offset = -1)
        {
            SetOpCode(_OpCode);
            SetData(_Data);
            SetOffset(_Offset);
        }
        public static NefInstruction ReadFrom(System.IO.Stream stream)
        {

            var offset = (int)stream.Position;


            byte[] buf = new byte[4];
            var readlen = stream.Read(buf, 0, 1);
            if (readlen == 0)
                return null;

            var opcode = (OpCode)buf[0];
            uint datalen = 0;
            var prefixlen = GetOperandPrefixSize(opcode);
            if (prefixlen > 0)
            {
                stream.Read(buf, 0, (int)prefixlen);
                datalen = BitConverter.ToUInt32(buf, 0);
            }
            else
            {
                datalen = GetOperandSize(opcode);
            }

            if (datalen > 0)
            {
                buf = new byte[datalen];
                var readOperandlen = stream.Read(buf, 0, (int)datalen);
                if (readOperandlen != datalen)
                    throw new Exception("error read Instruction");
            }
            else
            {
                buf = null;
            }


            return new NefInstruction(opcode, buf, offset);
        }

        public static void WriteTo(NefInstruction _inst, System.IO.Stream stream)
        {
            stream.WriteByte((byte)_inst.OpCode);

            if (_inst.DataPrefixSize > 0)
            {
                var buflen = BitConverter.GetBytes(_inst.DataSize);
                stream.Write(buflen, 0, (int)_inst.DataPrefixSize);
                if (_inst.DataSize > 0)
                {
                    stream.Write(_inst.Data, 0, (int)_inst.DataSize);
                }
            }
            else if (_inst.DataSize > 0)
            {
                stream.Write(_inst.Data, 0, (int)_inst.DataSize);
            }
        }
    }

}
