using Mono.Cecil;
using Neo.Compiler.MSIL;
using Neo.IO.Json;
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
            JObject json = new JObject();
            json["__name__"] = "neomodule.";

            //code
            var jsoncode = new JArray();
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
            json["codebin"] = codestr;

            //calls
            JObject methodinfo = new JObject();
            json["call"] = methodinfo;
            foreach (var m in this.mapMethods)
            {
                methodinfo[m.Key] = m.Value.GenJson();
            }

            return json.ToString(true);
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

        public JObject GenJson()
        {
            JObject json = new JObject();
            json["name"] = this.name;
            json["returntype"] = FuncExport.ConvType(this.returntype);
            json["paramcount"] = this.paramtypes.Count;
            JArray jsonparams = new JArray();
            json["params"] = jsonparams;
            for (var i = 0; i < this.paramtypes.Count; i++)
            {
                JObject item = new JObject();
                item["name"] = this.paramtypes[i].name;
                item["type"] = FuncExport.ConvType(this.paramtypes[i].type);
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

            // Ensure method definition

            switch (displayName)
            {
                case StandardMethods.Verify:
                    {
                        if (method.returntype.FullName != FuncExport.Boolean.FullName)
                        {
                            throw new Exception("verify must be: bool verify(...);");
                        }
                        break;
                    }
                case StandardMethods.Deploy:
                    {
                        if (method.paramtypes.Count != 1 ||
                            method.returntype.FullName != FuncExport.Void.FullName ||
                            method.paramtypes[0].type.FullName != FuncExport.Boolean.FullName)
                        {
                            throw new Exception("_deploy must be: void _deploy(bool update);");
                        }
                        break;
                    }
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

        public string GenJson()
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
            return info;
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
