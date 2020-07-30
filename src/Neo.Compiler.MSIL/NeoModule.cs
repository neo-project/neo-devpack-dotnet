using Mono.Cecil;
using Neo.Compiler.MSIL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Neo.Compiler
{
    public class NeoModule
    {
        public NeoModule(ILogger logger) { }

        public ConvOption option;
        public List<CustomAttribute> attributes = new List<CustomAttribute>();
        public Dictionary<string, NeoMethod> mapMethods = new Dictionary<string, NeoMethod>();
        public Dictionary<string, NeoEvent> mapEvents = new Dictionary<string, NeoEvent>();
        public Dictionary<string, NeoField> mapFields = new Dictionary<string, NeoField>();
        public Dictionary<string, object> staticfieldsWithConstValue = new Dictionary<string, object>();
        public List<ILMethod> staticfieldsCctor = new List<ILMethod>();
        public SortedDictionary<int, NeoCode> totalCodes = new SortedDictionary<int, NeoCode>();

        public byte[] Build()
        {
            List<byte> bytes = new List<byte>();
            foreach (var c in this.totalCodes.Values)
            {
                bytes.Add((byte)c.code);
                if (c.bytes != null)
                    for (var i = 0; i < c.bytes.Length; i++)
                    {
                        bytes.Add(c.bytes[i]);
                    }
            }
            return bytes.ToArray();
        }

        public string GenJson()
        {
            MyJson.JsonNode_Object json = new MyJson.JsonNode_Object();
            json["__name__"] = new MyJson.JsonNode_ValueString("neomodule.");

            //code
            var jsoncode = new MyJson.JsonNode_Array();
            json["code"] = jsoncode;
            foreach (var c in this.totalCodes.Values)
            {
                jsoncode.Add(c.GenJson());
            }
            //code bytes
            var code = this.Build();
            var codestr = "";
            foreach (var c in code)
            {
                codestr += c.ToString("X02");
            }
            json.SetDictValue("codebin", codestr);

            //calls
            MyJson.JsonNode_Object methodinfo = new MyJson.JsonNode_Object();
            json["call"] = methodinfo;
            foreach (var m in this.mapMethods)
            {
                methodinfo[m.Key] = m.Value.GenJson();
            }

            StringBuilder sb = new StringBuilder();
            json.ConvertToStringWithFormat(sb, 4);
            return sb.ToString();
        }

        internal void ConvertFuncAddr()
        {
            foreach (var method in this.mapMethods.Values)
            {
                foreach (var code in method.body_Codes.Values)
                {
                    if (code.code != VM.OpCode.NOP)
                    {
                        method.funcaddr = code.addr;
                        break;
                    }
                }
            }
        }
    }

    public class NeoMethod
    {
        public string lastsfieldname = null; // The last name of static filed, only used by event

        public int lastparam = -1; // The last param
        public int lastCast = -1;

        public string _namespace;
        public string name;
        public string displayName;
        public List<NeoParam> paramtypes = new List<NeoParam>();
        public TypeReference returntype;
        public bool isPublic = true;
        public bool inSmartContract;
        public ILMethod method;
        public ILType type;

        public List<NeoParam> body_Variables = new List<NeoParam>(); // Temporary variable
        public SortedDictionary<int, NeoCode> body_Codes = new SortedDictionary<int, NeoCode>(); // Temporary records and will be merged later
        public int funcaddr;

        public MyJson.JsonNode_Object GenJson()
        {
            MyJson.JsonNode_Object json = new MyJson.JsonNode_Object();
            json.SetDictValue("name", this.name);
            json.SetDictValue("returntype", FuncExport.ConvType(this.returntype));
            json.SetDictValue("paramcount", this.paramtypes.Count);
            MyJson.JsonNode_Array jsonparams = new MyJson.JsonNode_Array();
            json.SetDictValue("params", jsonparams);
            for (var i = 0; i < this.paramtypes.Count; i++)
            {
                MyJson.JsonNode_Object item = new MyJson.JsonNode_Object();
                item.SetDictValue("name", this.paramtypes[i].name);
                item.SetDictValue("type", FuncExport.ConvType(this.paramtypes[i].type));
                jsonparams.Add(item);
            }
            return json;
        }

        public NeoMethod() { }

        public NeoMethod(ILMethod method)
        {
            this.method = method;
            this.type = method.type;

            _namespace = method.method.DeclaringType.FullName;
            name = method.method.FullName;
            displayName = method.method.Name[..1].ToLowerInvariant() + method.method.Name[1..];
            inSmartContract = method.method.DeclaringType.BaseType.FullName == "Neo.SmartContract.Framework.SmartContract";
            isPublic = method.method.IsPublic;

            foreach (var attr in method.method.CustomAttributes)
            {
                ProcessAttribute(attr);
            }
        }

        private void ProcessAttribute(CustomAttribute attr)
        {
            switch (attr.AttributeType.Name)
            {
                case nameof(DisplayNameAttribute):
                    {
                        displayName = (string)attr.ConstructorArguments[0].Value;
                        break;
                    }
            }
        }
    }

    public class NeoEvent
    {
        public string _namespace;
        public string name;
        public string displayName;
        public List<NeoParam> paramtypes = new List<NeoParam>();

        public NeoEvent(ILField value)
        {
            _namespace = value.field.DeclaringType.FullName;
            name = value.field.DeclaringType.FullName + "::" + value.field.Name;
            displayName = value.displayName;
            paramtypes = value.paramtypes;

            if (FuncExport.ConvType(value.returntype) != "Void")
            {
                throw new NotSupportedException($"NEP-3 does not support return types for events. Expected: `System.Void`, Detected: `{value.returntype}`");
            }
        }
    }

    public class NeoCode
    {
        public VM.OpCode code = VM.OpCode.NOP;
        public int addr;
        public byte[] bytes;
        public string debugcode;
        public int debugline = -1;
        public int debugILAddr = -1;
        public string debugILCode;
        public bool needfix = false;//lateparse tag
        public bool needfixfunc = false;
        public int srcaddr;
        public int[] srcaddrswitch;
        public string srcfunc;
        public Mono.Cecil.Cil.SequencePoint sequencePoint;

        public override string ToString()
        {
            string info = "" + addr.ToString("X04") + " " + code.ToString();
            for (var j = 0; j < 16 - code.ToString().Length; j++)
            {
                info += " ";
            }
            info += "[";
            if (bytes != null)
            {
                foreach (var c in bytes)
                {
                    info += c.ToString("X02");
                }
            }
            info += "]";

            if (debugcode != null && debugline >= 0)
            {
                info += "//" + debugcode + "(" + debugline + ")";
            }
            return info;
        }

        public MyJson.JsonNode_ValueString GenJson()
        {
            string info = "" + addr.ToString("X04") + " " + code.ToString();
            for (var j = 0; j < 16 - code.ToString().Length; j++)
            {
                info += " ";
            }
            info += "[";
            if (bytes != null)
            {
                foreach (var c in bytes)
                {
                    info += c.ToString("X02");
                }
            }
            info += "]";

            if (debugILCode != null && debugILAddr >= 0)
            {
                info += "<IL_" + debugILAddr.ToString("X04") + " " + debugILCode + ">";
            }
            if (debugcode != null && debugline >= 0)
            {
                info += "//" + debugcode + "(" + debugline + ")";
            }
            return new MyJson.JsonNode_ValueString(info);
        }
    }

    public class NeoField : NeoParam
    {
        public int index { get; private set; }

        public NeoField(string name, TypeReference type, int index) : base(name, type)
        {
            this.index = index;
        }
    }

    public class NeoParam
    {
        public string name { get; private set; }
        public TypeReference type { get; private set; }

        public NeoParam(string name, TypeReference type)
        {
            this.name = name;
            this.type = type;
        }

        public override string ToString()
        {
            return FuncExport.ConvType(type) + " " + name;
        }
    }
}
