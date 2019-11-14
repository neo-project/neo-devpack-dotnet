using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MSIL
{
    class CctorSubVM
    {
        private const ushort MaxArraySize = ushort.MaxValue;
        private static Stack<object> calcStack;

        public static object Dup(object src)
        {
            if (src.GetType() == typeof(byte[]))
            {
                byte[] _src = (byte[])src;
                return _src;
            }
            else if (src.GetType() == typeof(int))
            {
                int v = (int)src;
                return v;
            }
            else if (src.GetType() == typeof(string))
            {
                string v = (string)src;
                string v2 = v;
                return v2;
            }
            else if (src.GetType() == typeof(Boolean))
            {
                Boolean v = (Boolean)src;
                return v;
            }
            else
            {
                return null;
            }
        }

        public static byte[] HexString2Bytes(string str)
        {
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }

        public static void Parse(ILMethod from, NeoModule to)
        {
            calcStack = new Stack<object>();
            bool bEnd = false;
            foreach (var src in from.body_Codes.Values)
            {
                if (bEnd)
                    break;

                switch (src.code)
                {
                    case CodeEx.Ret:
                        bEnd = true;
                        break;
                    case CodeEx.Ldc_I4_M1:
                        calcStack.Push((int)-1);
                        break;
                    case CodeEx.Ldc_I4_0:
                        calcStack.Push((int)0);
                        break;
                    case CodeEx.Ldc_I4_1:
                        calcStack.Push((int)1);
                        break;
                    case CodeEx.Ldc_I4_2:
                        calcStack.Push((int)2);
                        break;
                    case CodeEx.Ldc_I4_3:
                        calcStack.Push((int)3);
                        break;
                    case CodeEx.Ldc_I4_4:
                        calcStack.Push((int)4);
                        break;
                    case CodeEx.Ldc_I4_5:
                        calcStack.Push((int)5);
                        break;
                    case CodeEx.Ldc_I4_6:
                        calcStack.Push((int)6);
                        break;
                    case CodeEx.Ldc_I4_7:
                        calcStack.Push((int)7);
                        break;
                    case CodeEx.Ldc_I4_8:
                        calcStack.Push((int)8);
                        break;
                    case CodeEx.Ldc_I4:
                    case CodeEx.Ldc_I4_S:
                        calcStack.Push((int)src.tokenI32);
                        break;
                    case CodeEx.Ldc_I8:
                        calcStack.Push((long)src.tokenI64);
                        break;
                    case CodeEx.Newarr:
                        {
                            if ((src.tokenType == "System.Byte") || (src.tokenType == "System.SByte"))
                            {
                                var count = (int)calcStack.Pop();
                                if (count > MaxArraySize) throw new ArgumentException("MaxArraySize found");
                                byte[] data = new byte[count];
                                calcStack.Push(data);
                            }
                            else
                            {
                                throw new Exception("only byte[] can be defined in here.");
                            }
                        }
                        break;
                    case CodeEx.Dup:
                        {
                            var _src = calcStack.Peek();
                            var _dest = Dup(_src);
                            calcStack.Push(_dest);
                        }
                        break;
                    case CodeEx.Ldtoken:
                        {
                            calcStack.Push(src.tokenUnknown);
                        }
                        break;
                    case CodeEx.Ldstr:
                        {
                            calcStack.Push(src.tokenStr);
                        }
                        break;
                    case CodeEx.Call:
                        {
                            var m = src.tokenUnknown as Mono.Cecil.MethodReference;
                            if (m.DeclaringType.FullName == "System.Runtime.CompilerServices.RuntimeHelpers" && m.Name == "InitializeArray")
                            {
                                var p1 = (byte[])calcStack.Pop();
                                var p2 = (byte[])calcStack.Pop();
                                for (var i = 0; i < p2.Length; i++)
                                {
                                    p2[i] = p1[i];
                                }
                            }
                            else if (m.DeclaringType.FullName == "System.Numerics.BigInteger" && m.Name == "op_Implicit")
                            {
                                var type = m.Parameters[0].ParameterType.FullName;
                                if (type == "System.UInt64")
                                {
                                    var p = (ulong)(long)calcStack.Pop();
                                    calcStack.Push(new System.Numerics.BigInteger(p).ToByteArray());
                                }
                                else if (type == "System.UInt32")
                                {
                                    var p = (ulong)(int)calcStack.Pop();
                                    calcStack.Push(new System.Numerics.BigInteger(p).ToByteArray());
                                }
                                else if (type == "System.Int64")
                                {
                                    var p = (long)calcStack.Pop();
                                    calcStack.Push(new System.Numerics.BigInteger(p).ToByteArray());
                                }
                                else
                                {
                                    var p = (int)calcStack.Pop();
                                    calcStack.Push(new System.Numerics.BigInteger(p).ToByteArray());
                                }
                            }
                            else
                            {
                                foreach (var attr in m.Resolve().CustomAttributes)
                                {
                                    if (attr.AttributeType.FullName == "Neo.SmartContract.Framework.NonemitWithConvertAttribute")
                                    {
                                        var text = (string)calcStack.Pop();
                                        var value = (int)attr.ConstructorArguments[0].Value;
                                        var type = attr.ConstructorArguments[0].Type.Resolve();
                                        string attrname = "";
                                        foreach (var f in type.Fields)
                                        {
                                            if (f.Constant != null && (int)f.Constant == value)
                                            {
                                                attrname = f.Name;
                                                break;
                                            }
                                        }
                                        if (attrname == "ToScriptHash")//AddressString2ScriptHashBytes to bytes
                                        {
                                            var bytes = NEO.AllianceOfThinWallet.Cryptography.Base58.Decode(text);
                                            var hash = bytes.Skip(1).Take(20).ToArray();
                                            calcStack.Push(hash);
                                        }
                                        else if (attrname == "HexToBytes")//HexString2Bytes to bytes[]
                                        {
                                            if (text.IndexOf("0x") == 0) text = text.Substring(2);
                                            var hex = HexString2Bytes(text);
                                            calcStack.Push(hex);
                                        }
                                        else if (attrname == "ToBigInteger")
                                        {
                                            var n = System.Numerics.BigInteger.Parse(text);
                                            calcStack.Push(n);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case CodeEx.Stsfld:
                        {
                            var field = src.tokenUnknown as Mono.Cecil.FieldReference;
                            var fname = field.DeclaringType.FullName + "::" + field.Name;
                            to.staticfields[fname] = calcStack.Pop();
                        }
                        break;
                    case CodeEx.Stelem_I1:
                        {
                            var v = (byte)(int)calcStack.Pop();
                            var index = (int)calcStack.Pop();
                            var array = calcStack.Pop() as byte[];
                            array[index] = v;
                        }
                        break;
                }
            }
        }
    }
}
