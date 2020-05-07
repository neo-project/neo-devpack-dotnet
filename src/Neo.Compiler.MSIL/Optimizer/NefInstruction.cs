using Neo.VM;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Neo.Compiler.Optimizer
{
    [DebuggerDisplay("Offset={Offset}, OpCode={OpCode}")]
    public class NefInstruction : INefItem
    {
        private static readonly uint[] OperandSizePrefixTable = new uint[256];
        private static readonly uint[] OperandSizeTable = new uint[256];

        public OpCode OpCode { get; private set; }
        public uint Size => (1 + DataPrefixSize + DataSize);

        private uint DataPrefixSize => GetOperandPrefixSize(OpCode);
        private uint DataSize => DataPrefixSize > 0 ? (uint)(Data?.Length ?? 0) : GetOperandSize(OpCode);

        public byte[] Data { get; private set; }
        public int OffsetInit { get; private set; }
        public int Offset { get; private set; }
        public string[] Labels { get; private set; }

        /// <summary>
        /// address type
        /// like JMP =1 byte
        /// or JMP_L =4 bytes
        /// </summary>
        public int AddressSize { get; private set; }

        public int AddressCountInData => Labels == null ? 0 : Labels.Length;

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
                OperandSizePrefixTable[index] = (uint)attribute.SizePrefix;
                OperandSizeTable[index] = (uint)attribute.Size;
            }
        }

        public NefInstruction(OpCode opCode, byte[] data, int offset)
        {
            SetOpCode(opCode);
            SetData(data);
            OffsetInit = offset;
            SetOffset(offset);
        }

        public void SetData(byte[] data)
        {
            data ??= Array.Empty<byte>();
            if (DataPrefixSize == 0 && data.Length != DataSize)
                throw new Exception("error DataSize");

            Data = data;
        }

        public static uint GetOperandSize(OpCode opcode)
        {
            return OperandSizeTable[(int)opcode];
        }

        public static uint GetOperandPrefixSize(OpCode opcode)
        {
            return OperandSizePrefixTable[(int)opcode];
        }

        public int GetAddressInData(int index)
        {
            //Include Address
            if (AddressSize == 0)
                throw new Exception("this data have not Addresses");

            switch (AddressSize)
            {
                // 1 byte is stored as signed byte
                case 1: return (sbyte)Data[index];
                case 4:
                    {
                        byte[] buf = new byte[4];
                        Array.Copy(Data, AddressSize * index, buf, 0, AddressSize);
                        return BitConverter.ToInt32(buf, 0);
                    }
                default: throw new Exception("this data have not a valid address");
            }
        }

        public void SetAddressInData(int index, int addr)
        {
            if (AddressSize == 0)
                throw new Exception("this data have not Addresses");

            byte[] buf = BitConverter.GetBytes(addr);
            Array.Copy(buf, 0, Data, AddressSize * index, AddressSize);
        }

        public void SetOffset(int offset)
        {
            Offset = offset;
        }

        /// <summary>
        /// Change Opcode in this instruction
        /// If new Opcode has a different data length,then cut current data,or add zero 
        /// </summary>
        /// <param name="opCode">New Opcode</param>
        public void SetOpCode(OpCode opcode)
        {
            this.OpCode = opcode;

            //next part is for keep data when you recall SetOpCode
            //do not need to care about data who include address,link will refill it.it just need a right length.
            //if your data need to be changed,should all SetData after this.
            uint opprefix = GetOperandPrefixSize(opcode);
            if (opprefix == 0)
            {
                uint oplen = GetOperandSize(opcode);
                if (Data == null)
                {
                    //if do not have a old Data,just new it.
                    Data = new byte[oplen];
                }
                else if (oplen != Data.Length)
                {
                    //if have a old Data,but length is not right
                    //create a newdata,and copy data
                    byte[] newdata = new byte[oplen];
                    if (oplen > 0)
                    {
                        Array.Copy(Data, 0, newdata, 0, Math.Min(Data.Length, oplen));
                    }
                    Data = newdata;
                }
                //else have a old Data,and length is same
                //do nothing
            }

            var oldlabels = Labels;
            Labels = null;
            AddressSize = 0;
            switch (opcode)
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
                case OpCode.ENDTRY_L:
                    {
                        Labels = new string[1]; // is an address
                        if (oldlabels != null && oldlabels.Length >= 1)
                            Labels[0] = oldlabels[0];
                        AddressSize = 4; // 32 bit
                        break;
                    }
                case OpCode.TRY_L:
                    {
                        Labels = new string[2]; // is an address
                        if (oldlabels != null && oldlabels.Length >= 2)
                        {
                            Labels[0] = oldlabels[0];
                            Labels[1] = oldlabels[1];
                        }
                        AddressSize = 4; // 32 bit
                        break;
                    }
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
                case OpCode.ENDTRY:
                    {
                        Labels = new string[1]; // an address
                        if (oldlabels != null && oldlabels.Length >= 1)
                            Labels[0] = oldlabels[0];
                        AddressSize = 1; //8 bit
                        break;
                    }
                case OpCode.TRY:
                    {
                        Labels = new string[2]; // an address
                        if (oldlabels != null && oldlabels.Length >= 2)
                        {
                            Labels[0] = oldlabels[0];
                            Labels[1] = oldlabels[1];
                        }
                        AddressSize = 1; //8 bit
                        break;
                    }
                default: break;
            }
        }

        public static NefInstruction ReadFrom(Stream stream)
        {
            var offset = (int)stream.Position;

            byte[] buf = new byte[4];
            var readlen = stream.Read(buf, 0, 1);
            if (readlen == 0)
                return null;

            uint datalen;
            var opcode = (OpCode)buf[0];
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

            var data = new byte[datalen];
            if (datalen > 0)
            {
                var readOperandlen = stream.Read(data, 0, (int)datalen);
                if (readOperandlen != datalen)
                    throw new Exception("error read Instruction");
            }

            return new NefInstruction(opcode, data, offset);
        }

        public void WriteTo(Stream stream)
        {
            stream.WriteByte((byte)OpCode);

            if (DataPrefixSize > 0)
            {
                var buflen = BitConverter.GetBytes(DataSize);
                stream.Write(buflen, 0, (int)DataPrefixSize);
            }
            if (DataSize > 0)
            {
                stream.Write(Data, 0, (int)DataSize);
            }
        }
    }
}
