using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// This file is responsible for the parsing of the IL DLL. The core parsing is using mono.cecil, which is prearranged to make it easier to use
/// </summary>
namespace Neo.Compiler.MSIL
{
    public class ILModule
    {
        public Mono.Cecil.ModuleDefinition module = null;
        public List<string> moduleref = new List<string>();
        public Dictionary<string, ILType> mapType = new Dictionary<string, ILType>();
        public ILogger logger;

        public ILModule(ILogger _logger = null)
        {
            this.logger = _logger;
        }

        public void LoadModule(System.IO.Stream dllStream, System.IO.Stream pdbStream)
        {
            this.module = Mono.Cecil.ModuleDefinition.ReadModule(dllStream);
            if (pdbStream != null)
            {
                var debugInfoLoader = new Mono.Cecil.Pdb.PdbReaderProvider();

                module.ReadSymbols(debugInfoLoader.GetSymbolReader(module, pdbStream));
            }
            if (module.HasAssemblyReferences)
            {
                foreach (var ar in module.AssemblyReferences)
                {
                    if (moduleref.Contains(ar.Name) == false)
                        moduleref.Add(ar.Name);
                    if (moduleref.Contains(ar.FullName) == false)
                        moduleref.Add(ar.FullName);
                }
            }
            //mapModule[module.Name] = module;
            if (module.HasTypes)
            {
                foreach (var t in module.Types)
                {
                    if (t.FullName.Contains(".My."))//vb skip the system class
                        continue;

                    mapType[t.FullName] = new ILType(this, t, logger);
                    if (t.HasNestedTypes)
                    {
                        foreach (var nt in t.NestedTypes)
                        {
                            mapType[nt.FullName] = new ILType(this, nt, logger);
                        }
                    }
                }
            }
        }
    }

    public class ILType
    {
        public Dictionary<string, ILField> fields = new Dictionary<string, ILField>();
        public Dictionary<string, ILMethod> methods = new Dictionary<string, ILMethod>();
        public List<Mono.Cecil.CustomAttribute> attributes = new List<Mono.Cecil.CustomAttribute>();

        public ILType(ILModule module, Mono.Cecil.TypeDefinition type, ILogger logger)
        {
            if (type.HasCustomAttributes && type.IsClass)
            {
                attributes.AddRange(type.CustomAttributes);
            }

            foreach (Mono.Cecil.FieldDefinition f in type.Fields)
            {
                this.fields.Add(f.Name, new ILField(this, f));
            }
            foreach (Mono.Cecil.MethodDefinition m in type.Methods)
            {
                if (m.IsStatic == false)
                {
                    var method = new ILMethod(this, null, logger);
                    method.fail = "only export static method";
                    methods[m.FullName] = method;
                }
                else
                {
                    var method = new ILMethod(this, m, logger);
                    if (methods.ContainsKey(m.FullName))
                    {
                        throw new Exception("already have a func named:" + type.FullName + "::" + m.Name);
                    }
                    methods[m.FullName] = method;
                }
            }
        }
    }

    public class ILField
    {
        public bool isEvent = false;
        public TypeReference type;
        public string name;
        public string displayName;
        public TypeReference returntype;
        public List<NeoParam> paramtypes = new List<NeoParam>();
        public FieldDefinition field;

        public ILField(ILType type, FieldDefinition field)
        {
            this.type = field.FieldType;
            this.name = field.Name;
            this.displayName = this.name;
            this.field = field;
            foreach (var ev in field.DeclaringType.Events)
            {
                if (ev.Name == field.Name && ev.EventType.FullName == field.FieldType.FullName)
                {
                    this.isEvent = true;
                    Mono.Collections.Generic.Collection<CustomAttribute> ca = ev.CustomAttributes;
                    foreach (var attr in ca)
                    {
                        if (attr.AttributeType.Name == nameof(DisplayNameAttribute))
                        {
                            this.displayName = (string)attr.ConstructorArguments[0].Value;
                        }
                    }
                    if (!(field.FieldType is TypeDefinition eventtype))
                    {
                        try
                        {
                            eventtype = field.FieldType.Resolve();
                        }
                        catch
                        {
                            throw new Exception("can't parese event type from:" + field.FieldType.FullName + ".maybe it is System.Action<xxx> which is defined in mscorlib.dll，copy this dll in.");
                        }
                    }
                    if (eventtype != null)
                    {
                        foreach (var m in eventtype.Methods)
                        {
                            if (m.Name == "Invoke")
                            {
                                this.returntype = m.ReturnType;
                                foreach (var src in m.Parameters)
                                {
                                    if (src.ParameterType.IsGenericParameter)
                                    {
                                        var gtype = src.ParameterType as GenericParameter;

                                        var srcgtype = field.FieldType as GenericInstanceType;
                                        var rtype = srcgtype.GenericArguments[gtype.Position];
                                        this.paramtypes.Add(new NeoParam(src.Name, rtype));
                                    }
                                    else
                                    {
                                        this.paramtypes.Add(new NeoParam(src.Name, src.ParameterType));
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        public override string ToString()
        {
            return FuncExport.ConvType(type);
        }
    }

    public class ILCatchInfo
    {
        public int addrBegin;
        public int addrEnd;
        public string catchType;
    }

    public class ILTryInfo
    {
        public int addr_Try_Begin = -1;
        public int addr_Try_End = -1;
        public int addr_Try_End_F = -1;//IL try catch，try final is 2 different Block,need to process that.
        public Dictionary<string, ILCatchInfo> catch_Infos = new Dictionary<string, ILCatchInfo>();
        public int addr_Finally_Begin = -1;
        public int addr_Finally_End = -1;
    }

    public class ILMethod
    {
        public ILType type = null;
        public MethodDefinition method;
        public TypeReference returntype;
        public List<NeoParam> paramtypes = new List<NeoParam>();
        public bool hasParam = false;
        public List<NeoParam> body_Variables = new List<NeoParam>();
        public SortedDictionary<int, OpCode> body_Codes = new SortedDictionary<int, OpCode>();
        public string fail = null;
        public List<ILTryInfo> tryInfos = new List<ILTryInfo>();
        public List<int> tryPositions = new List<int>();
        public ILMethod(ILType type, MethodDefinition method, ILogger logger = null)
        {
            this.type = type;
            this.method = method;

            if (method != null)
            {
                returntype = method.ReturnType;
                if (method.HasParameters)
                {
                    hasParam = true;
                    foreach (var p in method.Parameters)
                    {
                        this.paramtypes.Add(new NeoParam(p.Name, p.ParameterType));
                    }
                }
                if (method.HasBody)
                {
                    var bodyNative = method.Body;
                    if (bodyNative.HasVariables)
                    {
                        foreach (var v in bodyNative.Variables)
                        {
                            var indexname = method.DebugInformation.TryGetName(v, out var varname)
                                ? varname
                                : v.VariableType.Name + ":" + v.Index;
                            this.body_Variables.Add(new NeoParam(indexname, v.VariableType));
                        }
                    }
                    for (int i = 0; i < bodyNative.Instructions.Count; i++)
                    {
                        var code = bodyNative.Instructions[i];
                        OpCode c = new OpCode
                        {
                            code = (CodeEx)(int)code.OpCode.Code,
                            addr = code.Offset
                        };

                        var sp = method.DebugInformation.GetSequencePoint(code);
                        if (sp != null && !sp.IsHidden)
                        {
                            c.debugcode = sp.Document.Url;
                            c.debugline = sp.StartLine;
                            c.sequencePoint = sp;
                        }
                        c.InitToken(code.Operand);
                        this.body_Codes.Add(c.addr, c);
                    }
                    if (method.Body.HasExceptionHandlers)
                    {
                        var mapTryInfos = new Dictionary<string, ILTryInfo>();
                        foreach (var e in method.Body.ExceptionHandlers)
                        {
                            if (e.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Catch)
                            {
                                var key = e.TryStart.Offset + "_" + e.TryEnd.Offset;
                                if (mapTryInfos.ContainsKey(key) == false)
                                    mapTryInfos[key] = new ILTryInfo();

                                var tryinfo = mapTryInfos[key];
                                tryinfo.addr_Try_Begin = e.TryStart.Offset;
                                tryinfo.addr_Try_End = e.TryEnd.Offset;

                                var catchtypestr = e.CatchType.FullName;

                                tryinfo.catch_Infos[catchtypestr] = new ILCatchInfo()
                                {
                                    addrBegin = e.HandlerStart.Offset,
                                    addrEnd = e.HandlerEnd.Offset,
                                    catchType = catchtypestr
                                };
                            }
                            else if (e.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Finally)
                            {
                                int start = e.TryStart.Offset;
                                int end = e.TryEnd.Offset;
                                Mono.Cecil.Cil.ExceptionHandler handler = null;
                                foreach (var tryinfocatch in method.Body.ExceptionHandlers)
                                {
                                    if (tryinfocatch.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Catch && tryinfocatch.TryStart.Offset == e.TryStart.Offset && tryinfocatch.TryEnd.Offset <= e.TryEnd.Offset)
                                    {//find include catch element.
                                        if (handler == null)
                                            handler = tryinfocatch;
                                        else if (handler.TryEnd.Offset < tryinfocatch.TryEnd.Offset)
                                            handler = tryinfocatch;
                                    }
                                }
                                if (handler != null)
                                {
                                    start = handler.TryStart.Offset;
                                    end = handler.TryEnd.Offset;
                                }

                                var key = start + "_" + end;

                                if (mapTryInfos.ContainsKey(key) == false)
                                    mapTryInfos[key] = new ILTryInfo();

                                var tryinfo = mapTryInfos[key];
                                tryinfo.addr_Try_Begin = start;
                                tryinfo.addr_Try_End = end;
                                tryinfo.addr_Try_End_F = e.TryEnd.Offset;

                                tryinfo.addr_Finally_Begin = e.HandlerStart.Offset;
                                tryinfo.addr_Finally_End = e.HandlerEnd.Offset;
                            }
                            else
                            {
                                throw new Exception("not support yet." + e.HandlerType);
                            }
                        }
                        this.tryInfos = new List<ILTryInfo>(mapTryInfos.Values);
                        foreach (var info in this.tryInfos)
                        {
                            if (this.tryPositions.Contains(info.addr_Try_Begin) == false)
                                this.tryPositions.Add(info.addr_Try_Begin);
                        }
                    }
                }
            }
        }

        public enum TryCodeType
        {
            None,
            Try,
            Try_Final,
            Catch,
            Final,
        }
        public ILTryInfo GetTryInfo(int addr, out TryCodeType type)
        {
            type = TryCodeType.None;
            ILTryInfo last = null;
            int begin = -1;
            int end = -1;
            foreach (var info in tryInfos)
            {
                //check try first
                if (info.addr_Try_Begin <= addr && addr < info.addr_Try_End)
                {
                    if (last == null)
                    {
                        type = TryCodeType.Try;
                        last = info;
                        begin = info.addr_Try_Begin;
                        end = last.addr_Try_End;
                    }
                    else if (begin <= info.addr_Try_Begin && last.addr_Try_End < end)
                    {
                        type = TryCodeType.Try;
                        last = info;
                        begin = info.addr_Try_Begin;
                        end = last.addr_Try_End;
                    }
                }

                //then code in final but not in try
                if (info.addr_Try_Begin <= addr && addr < info.addr_Try_End_F)
                {
                    if (last == null)
                    {
                        type = TryCodeType.Try_Final;
                        last = info;
                        begin = info.addr_Try_Begin;
                        end = last.addr_Try_End_F;
                    }
                    else if (begin <= info.addr_Try_Begin && last.addr_Try_End_F < end)
                    {
                        type = TryCodeType.Try_Final;
                        last = info;
                        begin = info.addr_Try_Begin;
                        end = last.addr_Try_End_F;
                    }
                }

                //find match finally
                if (info.addr_Finally_Begin <= addr && addr < info.addr_Finally_End)
                {
                    if (last == null)
                    {
                        type = TryCodeType.Final;
                        last = info;
                        begin = info.addr_Finally_Begin;
                        end = last.addr_Finally_End;
                    }
                    else if (begin <= info.addr_Finally_Begin && last.addr_Finally_End < end)
                    {
                        type = TryCodeType.Final;
                        last = info;
                        begin = info.addr_Finally_Begin;
                        end = last.addr_Finally_End;
                    }
                }
                //find match catch
                foreach (var c in info.catch_Infos)
                {
                    if (c.Value.addrBegin <= addr && addr < c.Value.addrEnd)
                    {
                        if (last == null)
                        {
                            type = TryCodeType.Catch;
                            last = info;
                            begin = c.Value.addrBegin;
                            end = c.Value.addrEnd;
                        }
                        else if (begin <= c.Value.addrBegin && c.Value.addrBegin < end)
                        {
                            type = TryCodeType.Catch;
                            last = info;
                            begin = c.Value.addrBegin;
                            end = c.Value.addrEnd;
                        }
                    }
                }
            }
            return last;
        }

        public int GetLastCodeAddr(int srcaddr)
        {
            int last = -1;
            for (var i = 0; i < srcaddr; i++)
            {
                if (i == srcaddr)
                    break;
                if (this.body_Codes.ContainsKey(i))
                    last = i;// method.body_Codes[i];
            }
            return last;
        }

        public int GetNextCodeAddr(int srcaddr)
        {
            bool bskip = false;
            foreach (var key in this.body_Codes.Keys)
            {
                if (key == srcaddr)
                {
                    bskip = true;
                    continue;
                }
                if (bskip)
                {
                    return key;
                }
            }
            return -1;
        }
    }

    public enum CodeEx
    {
        Nop,
        Break,
        Ldarg_0,
        Ldarg_1,
        Ldarg_2,
        Ldarg_3,
        Ldloc_0,
        Ldloc_1,
        Ldloc_2,
        Ldloc_3,
        Stloc_0,
        Stloc_1,
        Stloc_2,
        Stloc_3,
        Ldarg_S,
        Ldarga_S,
        Starg_S,
        Ldloc_S,
        Ldloca_S,
        Stloc_S,
        Ldnull,
        Ldc_I4_M1,
        Ldc_I4_0,
        Ldc_I4_1,
        Ldc_I4_2,
        Ldc_I4_3,
        Ldc_I4_4,
        Ldc_I4_5,
        Ldc_I4_6,
        Ldc_I4_7,
        Ldc_I4_8,
        Ldc_I4_S,
        Ldc_I4,
        Ldc_I8,
        Ldc_R4,
        Ldc_R8,
        Dup,
        Pop,
        Jmp,
        Call,
        Calli,
        Ret,
        Br_S,
        Brfalse_S,
        Brtrue_S,
        Beq_S,
        Bge_S,
        Bgt_S,
        Ble_S,
        Blt_S,
        Bne_Un_S,
        Bge_Un_S,
        Bgt_Un_S,
        Ble_Un_S,
        Blt_Un_S,
        Br,
        Brfalse,
        Brtrue,
        Beq,
        Bge,
        Bgt,
        Ble,
        Blt,
        Bne_Un,
        Bge_Un,
        Bgt_Un,
        Ble_Un,
        Blt_Un,
        Switch,
        Ldind_I1,
        Ldind_U1,
        Ldind_I2,
        Ldind_U2,
        Ldind_I4,
        Ldind_U4,
        Ldind_I8,
        Ldind_I,
        Ldind_R4,
        Ldind_R8,
        Ldind_Ref,
        Stind_Ref,
        Stind_I1,
        Stind_I2,
        Stind_I4,
        Stind_I8,
        Stind_R4,
        Stind_R8,
        Add,
        Sub,
        Mul,
        Div,
        Div_Un,
        Rem,
        Rem_Un,
        And,
        Or,
        Xor,
        Shl,
        Shr,
        Shr_Un,
        Neg,
        Not,
        Conv_I1,
        Conv_I2,
        Conv_I4,
        Conv_I8,
        Conv_R4,
        Conv_R8,
        Conv_U4,
        Conv_U8,
        Callvirt,
        Cpobj,
        Ldobj,
        Ldstr,
        Newobj,
        Castclass,
        Isinst,
        Conv_R_Un,
        Unbox,
        Throw,
        Ldfld,
        Ldflda,
        Stfld,
        Ldsfld,
        Ldsflda,
        Stsfld,
        Stobj,
        Conv_Ovf_I1_Un,
        Conv_Ovf_I2_Un,
        Conv_Ovf_I4_Un,
        Conv_Ovf_I8_Un,
        Conv_Ovf_U1_Un,
        Conv_Ovf_U2_Un,
        Conv_Ovf_U4_Un,
        Conv_Ovf_U8_Un,
        Conv_Ovf_I_Un,
        Conv_Ovf_U_Un,
        Box,
        Newarr,
        Ldlen,
        Ldelema,
        Ldelem_I1,
        Ldelem_U1,
        Ldelem_I2,
        Ldelem_U2,
        Ldelem_I4,
        Ldelem_U4,
        Ldelem_I8,
        Ldelem_I,
        Ldelem_R4,
        Ldelem_R8,
        Ldelem_Ref,
        Stelem_I,
        Stelem_I1,
        Stelem_I2,
        Stelem_I4,
        Stelem_I8,
        Stelem_R4,
        Stelem_R8,
        Stelem_Ref,
        Ldelem_Any,
        Stelem_Any,
        Unbox_Any,
        Conv_Ovf_I1,
        Conv_Ovf_U1,
        Conv_Ovf_I2,
        Conv_Ovf_U2,
        Conv_Ovf_I4,
        Conv_Ovf_U4,
        Conv_Ovf_I8,
        Conv_Ovf_U8,
        Refanyval,
        Ckfinite,
        Mkrefany,
        Ldtoken,
        Conv_U2,
        Conv_U1,
        Conv_I,
        Conv_Ovf_I,
        Conv_Ovf_U,
        Add_Ovf,
        Add_Ovf_Un,
        Mul_Ovf,
        Mul_Ovf_Un,
        Sub_Ovf,
        Sub_Ovf_Un,
        Endfinally,
        Leave,
        Leave_S,
        Stind_I,
        Conv_U,
        Arglist,
        Ceq,
        Cgt,
        Cgt_Un,
        Clt,
        Clt_Un,
        Ldftn,
        Ldvirtftn,
        Ldarg,
        Ldarga,
        Starg,
        Ldloc,
        Ldloca,
        Stloc,
        Localloc,
        Endfilter,
        Unaligned,
        Volatile,
        Tail,
        Initobj,
        Constrained,
        Cpblk,
        Initblk,
        No,
        Rethrow,
        Sizeof,
        Refanytype,
        Readonly,
    }

    public class OpCode
    {
        public TokenValueType tokenValueType = TokenValueType.Nothing;
        public int addr;
        public CodeEx code;
        public int debugline = -1;
        public string debugcode;
        public Mono.Cecil.Cil.SequencePoint sequencePoint;
        public object tokenUnknown;
        public int tokenAddr_Index;
        //public int tokenAddr;
        public int[] tokenAddr_Switch;
        public string tokenType;
        public string tokenField;
        public string tokenMethod;
        public int tokenI32;
        public Int64 tokenI64;
        public float tokenR32;
        public double tokenR64;
        public string tokenStr;

        public override string ToString()
        {
            var info = "IL_" + addr.ToString("X04") + " " + code + " ";
            if (this.tokenValueType == TokenValueType.Method)
                info += tokenMethod;
            if (this.tokenValueType == TokenValueType.String)
                info += tokenStr;

            if (debugline >= 0)
            {
                info += "(" + debugline + ")";
            }
            return info;
        }

        public enum TokenValueType
        {
            Nothing,
            Addr,
            AddrArray,
            String,
            Type,
            Field,
            Method,
            I32,
            I64,
            OTher,
        }

        public void InitToken(object _p)
        {
            this.tokenUnknown = _p;
            switch (code)
            {
                case CodeEx.Leave:
                case CodeEx.Leave_S:
                case CodeEx.Br:
                case CodeEx.Br_S:
                case CodeEx.Brtrue:
                case CodeEx.Brtrue_S:
                case CodeEx.Brfalse:
                case CodeEx.Brfalse_S:
                //compare control flow
                case CodeEx.Beq:
                case CodeEx.Beq_S:
                case CodeEx.Bne_Un:
                case CodeEx.Bne_Un_S:
                case CodeEx.Bge:
                case CodeEx.Bge_S:
                case CodeEx.Bge_Un:
                case CodeEx.Bge_Un_S:
                case CodeEx.Bgt:
                case CodeEx.Bgt_S:
                case CodeEx.Bgt_Un:
                case CodeEx.Bgt_Un_S:
                case CodeEx.Ble:
                case CodeEx.Ble_S:
                case CodeEx.Ble_Un:
                case CodeEx.Ble_Un_S:
                case CodeEx.Blt:
                case CodeEx.Blt_S:
                case CodeEx.Blt_Un:
                case CodeEx.Blt_Un_S:
                    //this.tokenAddr = ((Mono.Cecil.Cil.Instruction)_p).Offset;
                    this.tokenAddr_Index = ((Mono.Cecil.Cil.Instruction)_p).Offset;
                    this.tokenValueType = TokenValueType.Addr;
                    break;
                case CodeEx.Isinst:
                case CodeEx.Constrained:
                case CodeEx.Box:
                case CodeEx.Initobj:
                case CodeEx.Castclass:
                case CodeEx.Newarr:
                    this.tokenType = (_p as Mono.Cecil.TypeReference).FullName;
                    this.tokenValueType = TokenValueType.Type;
                    this.tokenUnknown = _p;
                    break;
                case CodeEx.Ldfld:
                case CodeEx.Ldflda:
                case CodeEx.Ldsfld:
                case CodeEx.Ldsflda:
                case CodeEx.Stfld:
                case CodeEx.Stsfld:
                    this.tokenField = (_p as Mono.Cecil.FieldReference).FullName;
                    this.tokenUnknown = _p;
                    this.tokenValueType = TokenValueType.Field;
                    break;
                case CodeEx.Call:
                case CodeEx.Callvirt:
                case CodeEx.Newobj:
                case CodeEx.Ldftn:
                case CodeEx.Ldvirtftn:

                    this.tokenMethod = (_p as Mono.Cecil.MethodReference).FullName;
                    this.tokenUnknown = _p;
                    this.tokenValueType = TokenValueType.Method;
                    break;
                case CodeEx.Ldc_I4:
                    this.tokenI32 = (int)_p;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_S:
                    this.tokenI32 = (int)Convert.ToDecimal(_p);
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_M1:
                    this.tokenI32 = -1;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_0:
                    this.tokenI32 = 0;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_1:
                    this.tokenI32 = 1;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_2:
                    this.tokenI32 = 2;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_3:
                    this.tokenI32 = 3;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_4:
                    this.tokenI32 = 4;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_5:
                    this.tokenI32 = 5;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_6:
                    this.tokenI32 = 6;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_7:
                    this.tokenI32 = 7;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I4_8:
                    this.tokenI32 = 8;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Ldc_I8:
                    this.tokenI64 = (Int64)_p;
                    this.tokenValueType = TokenValueType.I64;
                    break;
                case CodeEx.Ldc_R4:
                    this.tokenR32 = (float)_p;
                    this.tokenValueType = TokenValueType.OTher;
                    break;
                case CodeEx.Ldc_R8:
                    this.tokenR64 = (double)_p;
                    this.tokenValueType = TokenValueType.OTher;
                    break;

                case CodeEx.Ldstr:
                    this.tokenStr = _p as string;
                    this.tokenValueType = TokenValueType.String;
                    break;

                case CodeEx.Ldloca:
                case CodeEx.Ldloca_S:
                case CodeEx.Ldloc_S:
                case CodeEx.Stloc_S:
                    this.tokenI32 = ((Mono.Cecil.Cil.VariableDefinition)_p).Index;
                    this.tokenValueType = TokenValueType.I32;

                    //this.tokenUnknown = _p;
                    break;
                case CodeEx.Ldloc:
                case CodeEx.Stloc:
                    this.tokenI32 = (int)_p;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Stloc_0:
                case CodeEx.Ldloc_0:
                    this.tokenI32 = 0;
                    this.tokenValueType = TokenValueType.I32;

                    break;
                case CodeEx.Stloc_1:
                case CodeEx.Ldloc_1:
                    this.tokenI32 = 1;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Stloc_2:
                case CodeEx.Ldloc_2:
                    this.tokenI32 = 2;
                    this.tokenValueType = TokenValueType.I32;
                    break;
                case CodeEx.Stloc_3:
                case CodeEx.Ldloc_3:
                    this.tokenI32 = 3;
                    this.tokenValueType = TokenValueType.I32;
                    break;

                case CodeEx.Ldarga:
                case CodeEx.Ldarga_S:
                case CodeEx.Starg:
                case CodeEx.Starg_S:
                    this.tokenI32 = (_p as Mono.Cecil.ParameterDefinition).Index;
                    this.tokenValueType = TokenValueType.I32;

                    break;
                case CodeEx.Switch:
                    {
                        Mono.Cecil.Cil.Instruction[] e = _p as Mono.Cecil.Cil.Instruction[];
                        tokenAddr_Switch = new int[e.Length];
                        for (int i = 0; i < e.Length; i++)
                        {
                            tokenAddr_Switch[i] = (e[i].Offset);
                        }
                        this.tokenValueType = TokenValueType.AddrArray;

                    }
                    break;
                case CodeEx.Ldarg:
                    this.tokenI32 = (int)_p;
                    this.tokenValueType = TokenValueType.I32;

                    break;
                case CodeEx.Ldarg_S:
                    this.tokenI32 = (_p as Mono.Cecil.ParameterReference).Index;
                    this.tokenValueType = TokenValueType.I32;

                    break;
                case CodeEx.Volatile:
                case CodeEx.Ldind_I1:
                case CodeEx.Ldind_U1:
                case CodeEx.Ldind_I2:
                case CodeEx.Ldind_U2:
                case CodeEx.Ldind_I4:
                case CodeEx.Ldind_U4:
                case CodeEx.Ldind_I8:
                case CodeEx.Ldind_I:
                case CodeEx.Ldind_R4:
                case CodeEx.Ldind_R8:
                case CodeEx.Ldind_Ref:
                    this.tokenValueType = TokenValueType.Nothing;
                    break;

                case CodeEx.Ldtoken:
                    var def = (_p as Mono.Cecil.FieldDefinition);
                    var tref = _p as Mono.Cecil.TypeReference;
                    if (tref != null)
                    {
                        tref.Resolve();
                    }
                    this.tokenUnknown = def.InitialValue;
                    this.tokenValueType = TokenValueType.Nothing;
                    break;
                default:
                    this.tokenUnknown = _p;
                    this.tokenValueType = TokenValueType.Nothing;
                    break;
            }
        }
    }
}
