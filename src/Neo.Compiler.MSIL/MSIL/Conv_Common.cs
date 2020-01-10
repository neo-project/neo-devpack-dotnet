using System;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.MSIL
{
    /// <summary>
    /// 从ILCode 向小蚁 VM 转换的转换器
    /// </summary>
    public partial class ModuleConverter
    {
        private NeoCode _Insert1(VM.OpCode code, string comment, NeoMethod to, byte[] data = null)
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

        private NeoCode _InsertPush(byte[] data, string comment, NeoMethod to)
        {
            if (data.Length == 0) return _Insert1(VM.OpCode.PUSH0, comment, to);
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
            return _Insert1(code, comment, to, bytes);
        }

        private NeoCode _InsertPush(int i, string comment, NeoMethod to)
        {
            if (i == 0) return _Insert1(VM.OpCode.PUSH0, comment, to);
            if (i == -1) return _Insert1(VM.OpCode.PUSHM1, comment, to);
            if (i > 0 && i <= 16) return _Insert1(VM.OpCode.PUSH0 + (byte)i, comment, to);
            return _InsertPush(((BigInteger)i).ToByteArray(), comment, to);
        }

        private NeoCode _Convert1by1(VM.OpCode code, OpCode src, NeoMethod to, byte[] data = null)
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

        private NeoCode _ConvertPush(byte[] data, OpCode src, NeoMethod to)
        {
            if (data.Length == 0) return _Convert1by1(VM.OpCode.PUSH0, src, to);
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
            return _Convert1by1(code, src, to, bytes);
        }

        private NeoCode _ConvertPush(long i, OpCode src, NeoMethod to)
        {
            if (i == 0) return _Convert1by1(VM.OpCode.PUSH0, src, to);
            if (i == -1) return _Convert1by1(VM.OpCode.PUSHM1, src, to);
            if (i > 0 && i <= 16) return _Convert1by1(VM.OpCode.PUSH0 + (byte)i, src, to);
            return _ConvertPush(((BigInteger)i).ToByteArray(), src, to);
        }
        private int _ConvertPushI8WithConv(ILMethod from, long i, OpCode src, NeoMethod to)
        {
            var next = from.GetNextCodeAddr(src.addr);
            var code = from.body_Codes[next].code;
            BigInteger outv;
            if (code == CodeEx.Conv_U || code == CodeEx.Conv_U8)
            //code == CodeEx.Conv_U1 || code ==CodeEx.Conv_U2 || code==CodeEx.Conv_U4|| code== CodeEx.Conv_U8)
            {
                ulong v = (ulong)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;
            }
            else if (code == CodeEx.Conv_U1)
            {
                byte v = (byte)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U2)
            {
                ushort v = (ushort)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U4)
            {
                uint v = (uint)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;

            }
            else if (code == CodeEx.Call)
            {
                var call = from.body_Codes[next];
                if (call.tokenMethod == "System.Numerics.BigInteger System.Numerics.BigInteger::op_Implicit(System.UInt64)")
                {
                    //如果是ulong转型到biginteger，需要注意
                    ulong v = (ulong)i;
                    outv = v;
                    _ConvertPush(outv.ToByteArray(), src, to);
                    return 1;
                }
            }

            _ConvertPush(i, src, to);
            return 0;
        }

        private int _ConvertPushI4WithConv(ILMethod from, int i, OpCode src, NeoMethod to)
        {
            var next = from.GetNextCodeAddr(src.addr);
            var code = from.body_Codes[next].code;
            BigInteger outv;
            if (code == CodeEx.Conv_U || code == CodeEx.Conv_U8)
            //code == CodeEx.Conv_U1 || code ==CodeEx.Conv_U2 || code==CodeEx.Conv_U4|| code== CodeEx.Conv_U8)
            {
                ulong v = (uint)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;
            }
            else if (code == CodeEx.Conv_U1)
            {
                byte v = (byte)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U2)
            {
                ushort v = (ushort)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;

            }
            else if (code == CodeEx.Conv_U4)
            {
                uint v = (uint)i;
                outv = v;
                _ConvertPush(outv.ToByteArray(), src, to);
                return 1;

            }
            else
            {
                _ConvertPush(i, src, to);
                return 0;
            }
        }

        private void _insertSharedStaticVarCode(NeoMethod to)
        {
            if (this.outModule.mapFields.Count > 255)
                throw new Exception("too mush static fields");
            //insert init constvalue part
            byte count = (byte)this.outModule.mapFields.Count;
            //INITSSLOT with a u8 len
            if (count > 0)
            {
                _Insert1(VM.OpCode.INITSSLOT, "", to, new byte[] { count });
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
                        var bytesrc = (byte[])_src;
                        _ConvertPush(bytesrc, null, to);
                    }
                    else if (_src is int intsrc)
                    {
                        _ConvertPush(intsrc, null, to);
                    }
                    else if (_src is long longsrc)
                    {
                        _ConvertPush(longsrc, null, to);
                    }
                    else if (_src is bool bsrc)
                    {
                        _ConvertPush(bsrc ? 1 : 0, null, to);
                    }
                    else if (_src is string strsrc)
                    {
                        var bytesrc = Encoding.UTF8.GetBytes(strsrc);
                        _ConvertPush(bytesrc, null, to);
                    }
                    else if (_src is BigInteger bisrc)
                    {
                        byte[] bytes = bisrc.ToByteArray();
                        _ConvertPush(bytes, null, to);
                    }
                    else
                    {
                        //no need to init null
                        _Convert1by1(VM.OpCode.PUSHNULL, null, to);
                    }
                    #endregion

                    if (field.index < 7)
                    {
                        _Insert1(VM.OpCode.STSFLD0 + (byte)field.index, "", to);
                    }
                    else
                    {
                        var fieldIndex = (byte)field.index;
                        _Insert1(VM.OpCode.STSFLD, "", to, new byte[] { fieldIndex });
                    }
                }
            }

            //insert code part
            foreach (var cctor in this.outModule.staticfieldsCctor)
            {
                FillMethod(cctor, to, false);
            }
        }

        private void _insertBeginCode(ILMethod from, NeoMethod to)
        {
            if (from.paramtypes.Count > 255)
                throw new Exception("too mush params in:" + from);
            if (from.body_Variables.Count > 255)
                throw new Exception("too mush local varibles in:" + from);

            byte paramcount = (byte)from.paramtypes.Count;
            byte varcount = (byte)from.body_Variables.Count;
            if (paramcount + varcount > 0)
            {
                _Insert1(VM.OpCode.INITSLOT, "begincode", to, new byte[] { varcount, paramcount });
            }
        }
        private void _insertBeginCodeEntry(NeoMethod to)
        {
            byte paramcount = (byte)2;
            byte varcount = (byte)0;
            _Insert1(VM.OpCode.INITSLOT, "begincode", to, new byte[] { varcount, paramcount });
        }

        private void _insertEndCode(NeoMethod to, OpCode src)
        {
            //no need to clear altstack.

            ////移除深度临时栈
            //_Insert1(VM.OpCode.FROMALTSTACK, "endcode", to);
            //_Insert1(VM.OpCode.DROP, "", to);
        }
    }
}
