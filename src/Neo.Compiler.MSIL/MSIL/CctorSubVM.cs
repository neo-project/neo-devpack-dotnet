using Neo.Cryptography;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                return (byte[])src;
            }
            else if (src.GetType() == typeof(int))
            {
                return (int)src;
            }
            else if (src.GetType() == typeof(string))
            {
                return (string)src;
            }
            else if (src.GetType() == typeof(Boolean))
            {
                return (bool)src;
            }
            else if (src.GetType() == typeof(string[]))
            {
                return (string[])src;
            }
            else // TODO support more types
            {
                return null;
            }
        }

        public static bool Parse(ILMethod from, NeoModule to)
        {
            bool constValue = true;
            bool ret = false;
            calcStack = new Stack<object>();
            Dictionary<int, object> location = new System.Collections.Generic.Dictionary<int, object>();
            bool bEnd = false;
            foreach (var src in from.body_Codes.Values)
            {
                if (bEnd || ret)
                    break;

                switch (src.code)
                {
                    case CodeEx.Add:
                        //add two numbers is easy,but this will make code more complex.
                        //need to find a safe way to process all kind of calc.
                        throw new Exception("not support opcode [Add] in cctor.");
                    case CodeEx.Nop:
                        continue;
                    case CodeEx.Stloc:
                        location[src.tokenI32] = calcStack.Pop();
                        break;
                    case CodeEx.Stloc_0:
                        location[0] = calcStack.Pop();
                        break;
                    case CodeEx.Stloc_1:
                        location[1] = calcStack.Pop();
                        break;
                    case CodeEx.Stloc_2:
                        location[2] = calcStack.Pop();
                        break;
                    case CodeEx.Stloc_3:
                        location[3] = calcStack.Pop();
                        break;
                    case CodeEx.Ldloc:
                        calcStack.Push(location[src.tokenI32]);
                        break;
                    case CodeEx.Ldloc_0:
                        calcStack.Push(location[0]);
                        break;
                    case CodeEx.Ldloc_1:
                        calcStack.Push(location[1]);
                        break;
                    case CodeEx.Ldloc_2:
                        calcStack.Push(location[2]);
                        break;
                    case CodeEx.Ldloc_3:
                        calcStack.Push(location[3]);
                        break;
                    case CodeEx.Conv_I1:
                    case CodeEx.Conv_I2:
                    case CodeEx.Conv_I4:
                    case CodeEx.Conv_I8:
                    case CodeEx.Conv_U1:
                    case CodeEx.Conv_U2:
                    case CodeEx.Conv_U4:
                    case CodeEx.Conv_U8:
                        continue;

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
                            else if (src.tokenType == "System.String")
                            {
                                var count = (int)calcStack.Pop();
                                if (count > MaxArraySize) throw new ArgumentException("MaxArraySize found");
                                string[] data = new string[count];
                                calcStack.Push(data);
                            }
                            else
                            {
                                //other type mean is not a constValue
                                constValue = false;
                                continue;
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
                            else if (m.DeclaringType.FullName == "System.Numerics.BigInteger")
                            {
                                if (m.Name == "op_Implicit")
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
                                else if (m.Name == "Parse")
                                {
                                    switch (calcStack.Count)
                                    {
                                        case 1:
                                            {
                                                var p = calcStack.Pop();
                                                if (p is string pstr)
                                                {
                                                    p = System.Numerics.BigInteger.Parse(pstr);
                                                    calcStack.Push(p);
                                                    break;
                                                }

                                                throw new InvalidOperationException("Unsupported call to BigInteger.Parse");
                                            }
                                        case 2:
                                            {
                                                var s = calcStack.Pop();
                                                var p = calcStack.Pop();
                                                if (p is string pstr)
                                                {
                                                    if (s is int)
                                                    {
                                                        p = System.Numerics.BigInteger.Parse(pstr, (NumberStyles)s);
                                                        calcStack.Push(p);
                                                        break;
                                                    }
                                                    else if (s is IFormatProvider)
                                                    {
                                                        p = System.Numerics.BigInteger.Parse(pstr, (IFormatProvider)s);
                                                        calcStack.Push(p);
                                                        break;
                                                    }
                                                }

                                                throw new InvalidOperationException("Unsupported call to BigInteger.Parse");
                                            }
                                        default: throw new InvalidOperationException("Unsupported call to BigInteger.Parse");
                                    }
                                }
                            }
                            else
                            {
                                foreach (var attr in m.Resolve().CustomAttributes)
                                {
                                    if (attr.AttributeType.FullName == "Neo.SmartContract.Framework.NonemitWithConvertAttribute")
                                    {
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
                                            var text = (string)calcStack.Pop();
                                            var bytes = Base58.Decode(text);
                                            var hash = bytes.Skip(1).Take(20).ToArray();
                                            calcStack.Push(hash);
                                        }
                                        else if (attrname == "HexToBytes")//HexString2Bytes to bytes[]
                                        {
                                            var reverse = (int)calcStack.Pop() != 0;
                                            var text = (string)calcStack.Pop();
                                            var hex = text.HexString2Bytes();
                                            if (reverse) hex = hex.Reverse().ToArray();
                                            calcStack.Push(hex);
                                        }
                                    }
                                    if (attr.AttributeType.FullName == "Neo.SmartContract.Framework.SyscallAttribute")
                                    {
                                        // If there is a syscall in cctor, we should add it into staticfieldsCctor directly.
                                        // Then the initializemethod will handle it.
                                        ret = true;
                                        constValue = false;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case CodeEx.Stsfld:
                        {
                            var field = src.tokenUnknown as Mono.Cecil.FieldReference;
                            var fname = field.FullName;
                            if (calcStack.Count == 0)
                            {
                                constValue = false;
                                to.staticfieldsWithConstValue[fname] = null;
                            }
                            else
                            {
                                to.staticfieldsWithConstValue[fname] = calcStack.Pop();
                            }
                            // field.DeclaringType.FullName + "::" + field.Name;
                        }
                        break;
                    case CodeEx.Stelem_Ref:
                        {
                            var refValue = calcStack.Pop();
                            if (refValue is string) // Currently, we only support string ref
                            {
                                var strValue = (string)refValue;
                                var index = (int)calcStack.Pop();
                                var array = calcStack.Pop() as string[];
                                if (array is null)
                                {
                                    constValue = false;
                                    break;
                                }
                                array[index] = strValue;
                            }
                            break;
                        }
                    case CodeEx.Stelem_I1:
                        {
                            var v = (byte)(int)calcStack.Pop();
                            var index = (int)calcStack.Pop();
                            var array = calcStack.Pop() as byte[];
                            array[index] = v;
                        }
                        break;
                    default:
                        throw new Exception("not support opcode " + src + " in cctor.");
                }
            }
            if (constValue == false)
            {
                if (to.staticfieldsCctor.Contains(from) == false)
                    to.staticfieldsCctor.Add(from);
            }

            return constValue;
        }
    }
}
