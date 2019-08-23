using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler
{
    public class NeoModule
    {
        public NeoModule(ILogger logger)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NeoMethod() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="method">Method</param>
        public NeoMethod(MethodDefinition method)
        {
            _namespace = method.DeclaringType.FullName;
            name = method.FullName;
            displayName = method.Name;
            inSmartContract = method.DeclaringType.BaseType.Name == "SmartContract";
            isPublic = method.IsPublic;

            foreach (var attr in method.CustomAttributes)
            {
                ProcessAttribute(attr);
            }
        }

        private void ProcessAttribute(CustomAttribute attr)
        {
            switch (attr.AttributeType.Name)
            {
                case "DisplayNameAttribute":
                    {
                        displayName = (string)attr.ConstructorArguments[0].Value;
                        break;
                    }
                case "ReadOnlyAttribute":
                    {
                        isReadOnly = (bool)attr.ConstructorArguments[0].Value;
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
        public string returntype;
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
        public override string ToString()
        {
            //string info = "AL_" + addr.ToString("X04") + " " + code.ToString();
            //if (bytes != null)
            //    info += " len=" + bytes.Length;
            //if (debugcode != null && debugline >= 0)
            //{
            //    info += "    " + debugcode + "(" + debugline + ")";
            //}

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

        public void FromJson(MyJson.JsonNode_Object json)
        {
        }
    }
    public class NeoField : NeoParam
    {
        public NeoField(string name, string type, int index) : base(name, type)
        {
            this.index = index;
        }
        public int index
        {
            get;
            private set;
        }
    }

    public class NeoParam
    {
        public NeoParam(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
        public string name
        {
            get;
            private set;
        }
        public string type
        {
            get;
            private set;
        }
        public override string ToString()
        {
            return type + " " + name;
        }
    }
}
