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
            if (data.Length <= 75) return _Insert1((VM.OpCode)data.Length, comment, to, data);
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
            if (i > 0 && i <= 16) return _Insert1((VM.OpCode)(byte)i + 0x50, comment, to);
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
            if (data.Length <= 75) return _Convert1by1((VM.OpCode)data.Length, src, to, data);
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
            if (i > 0 && i <= 16) return _Convert1by1((VM.OpCode)(byte)i + 0x50, src, to);
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
            _InsertPush(this.outModule.mapFields.Count, "static var", to);
            _Insert1(VM.OpCode.NEWARRAY, "", to);
            _Insert1(VM.OpCode.TOALTSTACK, "", to);

            foreach (var defvar in this.outModule.staticfields)
            {
                if (this.outModule.mapFields.TryGetValue(defvar.Key, out NeoField field))
                {
                    //array
                    _Insert1(VM.OpCode.DUPFROMALTSTACKBOTTOM, "", to);

                    //index
                    _ConvertPush(field.index, null, to);

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
                        throw new Exception("not support type _insertSharedStaticVarCode\r\n   in: " + to.name + "\r\n");
                    }
                    #endregion
                    _Insert1(VM.OpCode.SETITEM, "", to);
                }
            }
        }

        private void _insertBeginCode(ILMethod from, NeoMethod to)
        {
            ////压入深度临时栈
            //_Insert1(VM.OpCode.DEPTH, "record depth.", to);
            //_Insert1(VM.OpCode.TOALTSTACK, "", to);

            ////初始化临时槽位位置
            //foreach (var src in from.body_Variables)
            //{
            //    to.body_Variables.Add(new ILParam(src.name, src.type));
            //    _InsertPush(0, "body_Variables init", to);
            //}

            //新玩法，用一个数组，应该能减少指令数量
            _InsertPush(from.paramtypes.Count + from.body_Variables.Count, "begincode", to);
            _Insert1(VM.OpCode.NEWARRAY, "", to);
            _Insert1(VM.OpCode.TOALTSTACK, "", to);
            //移动参数槽位
            for (var i = 0; i < from.paramtypes.Count; i++)
            {
                //getarray
                _Insert1(VM.OpCode.FROMALTSTACK, "set param:" + i, to);
                _Insert1(VM.OpCode.DUP, null, to);
                _Insert1(VM.OpCode.TOALTSTACK, null, to);

                _InsertPush(i, "", to); //Array pos

                _InsertPush(2, "", to); //Array item
                _Insert1(VM.OpCode.ROLL, null, to);

                _Insert1(VM.OpCode.SETITEM, null, to);
            }
        }
        private void _insertBeginCodeEntry(NeoMethod to)
        {
            _InsertPush(2, "begincode", to);
            _Insert1(VM.OpCode.NEWARRAY, "", to);
            _Insert1(VM.OpCode.TOALTSTACK, "", to);
            //移动参数槽位
            for (var i = 0; i < 2; i++)
            {
                //getarray
                _Insert1(VM.OpCode.FROMALTSTACK, "set param:" + i, to);
                _Insert1(VM.OpCode.DUP, null, to);
                _Insert1(VM.OpCode.TOALTSTACK, null, to);

                _InsertPush(i, "", to); //Array pos

                _InsertPush(2, "", to); //Array item
                _Insert1(VM.OpCode.ROLL, null, to);

                _Insert1(VM.OpCode.SETITEM, null, to);
            }
        }

        private void _insertEndCode(NeoMethod to, OpCode src)
        {
            ////占位不谢
            _Convert1by1(VM.OpCode.NOP, src, to);

            ////移除临时槽位
            ////drop body_Variables
            //for (var i = 0; i < from.body_Variables.Count; i++)
            //{
            //    _Insert1(VM.OpCode.DEPTH, "body_Variables drop", to, null);
            //    _Insert1(VM.OpCode.DEC, null, to, null);

            //    //push olddepth
            //    _Insert1(VM.OpCode.FROMALTSTACK, null, to);
            //    _Insert1(VM.OpCode.DUP, null, to);
            //    _Insert1(VM.OpCode.TOALTSTACK, null, to);
            //    //(d-1)-olddepth
            //    _Insert1(VM.OpCode.SUB, null, to);

            //    _Insert1(VM.OpCode.XDROP, null, to, null);
            //}
            ////移除参数槽位
            //for (var i = 0; i < from.paramtypes.Count; i++)
            //{
            //    //d
            //    _Insert1(VM.OpCode.DEPTH, "param drop", to, null);

            //    //push olddepth
            //    _Insert1(VM.OpCode.FROMALTSTACK, null, to);
            //    _Insert1(VM.OpCode.DUP, null, to);
            //    _Insert1(VM.OpCode.DEC, null, to);//深度-1
            //    _Insert1(VM.OpCode.TOALTSTACK, null, to);

            //    //(d)-olddepth
            //    _Insert1(VM.OpCode.SUB, null, to);

            //    _Insert1(VM.OpCode.XDROP, null, to, null);

            //}

            //移除深度临时栈
            _Insert1(VM.OpCode.FROMALTSTACK, "endcode", to);
            _Insert1(VM.OpCode.DROP, "", to);
        }
    }
}
