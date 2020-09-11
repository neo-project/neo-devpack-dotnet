using System;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.MSIL
{
    /// <summary>
    /// Convert IL to NeoVM opcode
    /// </summary>
    public partial class ModuleConverter
    {
        private NeoCode Insert1(VM.OpCode code, string comment, NeoMethod to, byte[] data = null)
        {
            NeoCode _code = new NeoCode();
            int startaddr = addr;
            _code.addr = addr;
            {
                _code.debugcode = comment;
                _code.debugline = 0;
            }

            addr++;

            _code.code = code;

            if (data != null)
            {
                _code.bytes = data;
                addr += _code.bytes.Length;
            }
            to.body_Codes[startaddr] = _code;
            return _code;
        }

        private NeoCode InsertPush(byte[] data, string comment, NeoMethod to)
        {
            if (data.Length == 0) return Insert1(VM.OpCode.PUSH0, comment, to);
            byte prefixLen;
            VM.OpCode code;
            if (data.Length <= byte.MaxValue)
            {
                prefixLen = sizeof(byte);
                code = VM.OpCode.PUSHDATA1;
            }
            else if (data.Length <= ushort.MaxValue)
            {
                prefixLen = sizeof(ushort);
                code = VM.OpCode.PUSHDATA2;
            }
            else
            {
                prefixLen = sizeof(uint);
                code = VM.OpCode.PUSHDATA4;
            }
            byte[] bytes = new byte[data.Length + prefixLen];
            Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, bytes, 0, prefixLen);
            Buffer.BlockCopy(data, 0, bytes, prefixLen, data.Length);
            return Insert1(code, comment, to, bytes);
        }

        private NeoCode InsertPush(int i, string comment, NeoMethod to)
        {
            if (i == 0) return Insert1(VM.OpCode.PUSH0, comment, to);
            if (i == -1) return Insert1(VM.OpCode.PUSHM1, comment, to);
            if (i > 0 && i <= 16) return Insert1(VM.OpCode.PUSH0 + (byte)i, comment, to);
            return InsertPush(((BigInteger)i).ToByteArray(), comment, to);
        }

        private NeoCode Convert1by1(VM.OpCode code, OpCode src, NeoMethod to, byte[] data = null)
        {
            NeoCode _code = new NeoCode();
            int startaddr = addr;
            _code.addr = addr;
            if (src != null)
            {
                addrconv[src.addr] = addr;
                _code.debugcode = src.debugcode;
                _code.debugline = src.debugline;
                _code.debugILAddr = src.addr;
                _code.debugILCode = src.code.ToString();
                _code.sequencePoint = src.sequencePoint;
            }

            addr++;

            _code.code = code;

            if (data != null)
            {
                _code.bytes = data;
                addr += _code.bytes.Length;
            }
            to.body_Codes[startaddr] = _code;
            return _code;
        }

        private void ConvertPushNumber(BigInteger i, OpCode src, NeoMethod to)
        {
            if (i == 0) Convert1by1(VM.OpCode.PUSH0, src, to);
            else if (i == -1) Convert1by1(VM.OpCode.PUSHM1, src, to);
            else if (i > 0 && i <= 16) Convert1by1(VM.OpCode.PUSH0 + (byte)i, src, to);
            else
            {
                ConvertPushDataArray(i.ToByteArray(), src, to);
                Insert1(VM.OpCode.CONVERT, "", to, new byte[] { (byte)VM.Types.StackItemType.Integer });
            }
        }

        private void ConvertPushBoolean(bool b, OpCode src, NeoMethod to)
        {
            if (!b)
                Convert1by1(VM.OpCode.PUSH0, src, to);
            else
                Convert1by1(VM.OpCode.PUSH1, src, to);
            Insert1(VM.OpCode.CONVERT, "", to, new byte[] { (byte)VM.Types.StackItemType.Boolean });
        }

        private void ConvertPushDataArray(byte[] data, OpCode src, NeoMethod to)
        {
            byte prefixLen;
            VM.OpCode code;
            if (data.Length <= byte.MaxValue)
            {
                prefixLen = sizeof(byte);
                code = VM.OpCode.PUSHDATA1;
            }
            else if (data.Length <= ushort.MaxValue)
            {
                prefixLen = sizeof(ushort);
                code = VM.OpCode.PUSHDATA2;
            }
            else
            {
                prefixLen = sizeof(uint);
                code = VM.OpCode.PUSHDATA4;
            }
            byte[] bytes = new byte[data.Length + prefixLen];
            Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, bytes, 0, prefixLen);
            Buffer.BlockCopy(data, 0, bytes, prefixLen, data.Length);
            Convert1by1(code, src, to, bytes);
        }

        private void ConvertPushString(string str, OpCode src, NeoMethod to)
        {
            var data = Utility.StrictUTF8.GetBytes(str);
            ConvertPushDataArray(data, src, to);
        }

        private void ConvertPushStringArray(string[] strArray, OpCode src, NeoMethod to)
        {
            for (int i = strArray.Length - 1; i >= 0; i--)
            {
                var str = strArray[i];
                ConvertPushString(str, src, to);
            }
            ConvertPushNumber(strArray.Length, src, to);
            Insert1(VM.OpCode.PACK, "", to);
        }

        private int ConvertPushI8WithConv(ILMethod from, long i, OpCode src, NeoMethod to)
        {
            var next = from.GetNextCodeAddr(src.addr);
            var code = from.body_Codes[next].code;
            BigInteger outv;
            if (code == CodeEx.Conv_U || code == CodeEx.Conv_U8)
            {
                ulong v = (ulong)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;
            }
            else if (code == CodeEx.Conv_U1)
            {
                byte v = (byte)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U2)
            {
                ushort v = (ushort)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;
            }
            else if (code == CodeEx.Conv_U4)
            {
                uint v = (uint)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;

            }
            else if (code == CodeEx.Call)
            {
                var call = from.body_Codes[next];
                if (call.tokenMethod == "System.Numerics.BigInteger System.Numerics.BigInteger::op_Implicit(System.UInt64)")
                {
                    // Be careful with converting ulong to biginteger
                    ulong v = (ulong)i;
                    outv = v;
                    ConvertPushNumber(outv, src, to);
                    return 1;
                }
            }

            ConvertPushNumber(i, src, to);
            return 0;
        }

        private int ConvertPushI4WithConv(ILMethod from, int i, OpCode src, NeoMethod to)
        {
            var next = from.GetNextCodeAddr(src.addr);
            var code = from.body_Codes[next].code;
            BigInteger outv;

            if (code == CodeEx.Conv_U || code == CodeEx.Conv_U8)
            {
                ulong v = (uint)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;
            }
            else if (code == CodeEx.Conv_U1)
            {
                byte v = (byte)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U2)
            {
                ushort v = (ushort)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U4)
            {
                uint v = (uint)i;
                outv = v;
                ConvertPushNumber(outv, src, to);
                return 1;

            }
            else
            {
                ConvertPushNumber(i, src, to);
                return 0;
            }
        }

        private void InsertSharedStaticVarCode(NeoMethod to)
        {
            //insert init constvalue part
            byte count = (byte)this.outModule.mapFields.Count;
            if (count > 0)
            {
                Insert1(VM.OpCode.INITSSLOT, "", to, new byte[] { count }); // INITSSLOT with a u8 len
            }

            foreach (var defvar in this.outModule.staticfieldsWithConstValue)
            {
                if (this.outModule.mapFields.TryGetValue(defvar.Key, out NeoField field))
                {
                    //value
                    #region insertValue
                    //this static var had a default value.
                    var _src = defvar.Value;
                    if (_src is byte[])
                    {
                        ConvertPushDataArray((byte[])_src, null, to);
                    }
                    else if (_src is int intsrc)
                    {
                        ConvertPushNumber(intsrc, null, to);
                    }
                    else if (_src is long longsrc)
                    {
                        ConvertPushNumber(longsrc, null, to);
                    }
                    else if (_src is bool bsrc)
                    {
                        ConvertPushBoolean(bsrc, null, to);
                    }
                    else if (_src is string strsrc)
                    {
                        ConvertPushString(strsrc, null, to);
                    }
                    else if (_src is BigInteger bisrc)
                    {
                        ConvertPushNumber(bisrc, null, to);
                    }
                    else if (_src is string[] strArray)
                    {
                        ConvertPushStringArray(strArray, null, to);
                    }
                    else
                    {
                        //no need to init null
                        Convert1by1(VM.OpCode.PUSHNULL, null, to);
                    }
                    #endregion

                    if (field.index < 7)
                    {
                        Insert1(VM.OpCode.STSFLD0 + (byte)field.index, "", to);
                    }
                    else
                    {
                        var fieldIndex = (byte)field.index;
                        Insert1(VM.OpCode.STSFLD, "", to, new byte[] { fieldIndex });
                    }
                }
            }

            //insert code part
            foreach (var cctor in this.outModule.staticfieldsCctor)
            {
                FillMethod(cctor, to, false);
            }
        }

        private void InsertBeginCode(ILMethod from, NeoMethod to)
        {
            if (from.paramtypes.Count > MAX_PARAMS_COUNT)
                throw new Exception("too much params in:" + from);
            if (from.body_Variables.Count > MAX_LOCAL_VARIABLES_COUNT)
                throw new Exception("too much local variables in:" + from);

            byte paramcount = (byte)from.paramtypes.Count;
            byte varcount = (byte)from.body_Variables.Count;
            if (paramcount + varcount > 0)
            {
                Insert1(VM.OpCode.INITSLOT, "begincode", to, new byte[] { varcount, paramcount });
            }
        }
    }
}
