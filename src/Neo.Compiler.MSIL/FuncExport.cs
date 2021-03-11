extern alias scfx;

using Mono.Cecil;
using Neo.IO.Json;
using Neo.SmartContract;
using System;
using System.Collections.Generic;
using System.Linq;
using IApiInterface = scfx.Neo.SmartContract.Framework.IApiInterface;

namespace Neo.Compiler
{
    public class FuncExport
    {
        public static readonly TypeReference Void = new TypeReference("System", "Void", ModuleDefinition.ReadModule(typeof(object).Assembly.Location, new ReaderParameters(ReadingMode.Immediate)), null);
        public static readonly TypeReference Boolean = new TypeReference("System", "Boolean", ModuleDefinition.ReadModule(typeof(object).Assembly.Location, new ReaderParameters(ReadingMode.Immediate)), null);

        internal static string ConvType(TypeReference t)
        {
            if (t is null) return "Null";

            var type = t.FullName;

            TypeDefinition definition = t.Resolve();
            if (definition is not null)
            {
                if (definition.IsEnum) return "Integer";
                foreach (var i in definition.Interfaces)
                {
                    if (i.InterfaceType.Name == nameof(IApiInterface))
                    {
                        return "InteropInterface";
                    }
                }
            }

            switch (type)
            {
                case "System.Boolean": return "Boolean";
                case "System.Char":
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Numerics.BigInteger": return "Integer";
                case "Neo.SmartContract.Framework.ByteString":
                case "System.Byte[]": return "ByteArray";
                case "System.String": return "String";
                case "IInteropInterface": return "InteropInterface";
                case "System.Void": return "Void";
                case "System.Object": return "Any";
                case "Neo.UInt160": return "Hash160";
                case "Neo.UInt256": return "Hash256";
                case "Neo.Cryptography.ECC.ECPoint": return "PublicKey";
            }

            if (t.IsArray) return "Array";

            if (type.StartsWith("System.Action") || type.StartsWith("System.Func") || type.StartsWith("System.Delegate"))
                return $"Unknown:Pointers are not allowed to be public '{type}'";
            if (type.StartsWith("System.ValueTuple`") || type.StartsWith("System.Tuple`"))
                return "Array";

            return "Unknown:" + type;
        }

        public static JObject GenerateAbi(NeoModule module, Dictionary<int, int> addrConvTable)
        {
            var outjson = new JObject();

            //functions
            var methods = new JArray();
            outjson["methods"] = methods;

            HashSet<string> names = new HashSet<string>();
            foreach (var function in module.mapMethods.OrderBy(u => u.Value.funcaddr))
            {
                var mm = function.Value;
                if (mm.inSmartContract == false)
                    continue;
                if (mm.isPublic == false)
                    continue;

                if (!names.Add(function.Value.displayName))
                {
                    throw new Exception("abi not allow same name functions");
                }
                var funcsign = new JObject();
                methods.Add(funcsign);
                funcsign["name"] = function.Value.displayName;
                funcsign["offset"] = addrConvTable?[function.Value.funcaddr] ?? function.Value.funcaddr;
                funcsign["safe"] = function.Value.method?.method.CustomAttributes.Any(u => u.AttributeType.FullName == "Neo.SmartContract.Framework.SafeAttribute") == true;
                JArray funcparams = new JArray();
                funcsign["parameters"] = funcparams;
                if (mm.paramtypes != null)
                {
                    foreach (var v in mm.paramtypes)
                    {
                        var item = new JObject();
                        funcparams.Add(item);

                        item["name"] = v.name;
                        item["type"] = ConvType(v.type);
                    }
                }

                var rtype = ConvType(mm.returntype);
                if (rtype.StartsWith("Unknown:"))
                {
                    throw new Exception($"Unknown return type '{mm.returntype}' for method '{function.Value.name}'");
                }

                funcsign["returntype"] = rtype;
            }

            //events
            var eventsigns = new JArray();
            outjson["events"] = eventsigns;
            foreach (var events in module.mapEvents)
            {
                var mm = events.Value;
                var funcsign = new JObject();
                eventsigns.Add(funcsign);

                funcsign["name"] = events.Value.displayName;
                JArray funcparams = new JArray();
                funcsign["parameters"] = funcparams;
                if (mm.paramtypes != null)
                {
                    foreach (var v in mm.paramtypes)
                    {
                        var item = new JObject();
                        funcparams.Add(item);

                        item["name"] = v.name;
                        item["type"] = ConvType(v.type);
                    }
                }
                //event do not have returntype in nep3
                //var rtype = ConvType(mm.returntype);
                //funcsign["returntype", rtype);
            }

            return outjson;
        }

        private static object BuildSupportedStandards(Mono.Collections.Generic.Collection<CustomAttributeArgument> supportedStandardsAttribute)
        {
            if (supportedStandardsAttribute == null || supportedStandardsAttribute.Count == 0)
            {
                return "[]";
            }

            var entry = supportedStandardsAttribute.First();
            string extra = "[";
            foreach (var item in entry.Value as CustomAttributeArgument[])
            {
                extra += ($"\"{ScapeJson(item.Value.ToString())}\",");
            }
            extra = extra[0..^1];
            extra += "]";

            return extra;
        }

        private static string BuildExtraAttributes(ICollection<Mono.Collections.Generic.Collection<CustomAttributeArgument>> extraAttributes)
        {
            if (extraAttributes == null || extraAttributes.Count == 0)
            {
                return "null";
            }

            string extra = "{";
            foreach (var extraAttribute in extraAttributes)
            {
                var key = ScapeJson(extraAttribute[0].Value.ToString());
                var value = ScapeJson(extraAttribute[1].Value.ToString());
                extra += ($"\"{key}\":\"{value}\",");
            }
            extra = extra[0..^1];
            extra += "}";

            return extra;
        }

        private static string ScapeJson(string value)
        {
            return value.Replace("\"", "");
        }

        public static string GenerateManifest(JObject abi, NeoModule module, MethodToken[] tokens)
        {
            var sbABI = abi.ToString(false);
            var extraAttributes = module == null ? Array.Empty<Mono.Collections.Generic.Collection<CustomAttributeArgument>>() : module.attributes.Where(u => u.AttributeType.FullName == "Neo.SmartContract.Framework.ManifestExtraAttribute").Select(attribute => attribute.ConstructorArguments).ToArray();
            var supportedStandardsAttribute = module?.attributes.Where(u => u.AttributeType.FullName == "Neo.SmartContract.Framework.SupportedStandardsAttribute").Select(attribute => attribute.ConstructorArguments).FirstOrDefault();
            var extra = BuildExtraAttributes(extraAttributes);
            var supportedStandards = BuildSupportedStandards(supportedStandardsAttribute);
            var name = module.attributes
                .Where(u => u.AttributeType.FullName == "System.ComponentModel.DisplayNameAttribute")
                .Select(u => ScapeJson((string)u.ConstructorArguments.FirstOrDefault().Value))
                .FirstOrDefault() ?? module.Name;

            var permissions = string.Join(",", module.attributes
               .Where(u => u.AttributeType.FullName == "Neo.SmartContract.Framework.ContractPermissionAttribute")
               .Select(u =>
               {
                   var methods = (CustomAttributeArgument[])u.ConstructorArguments[1].Value;
                   var hash = ScapeJson((string)u.ConstructorArguments[0].Value);
                   return (hash, (methods?.Length ?? 0) == 0 ? new string[] { "*" } : methods.Select(u => (string)u.Value).ToArray());
               })
            .Concat(tokens.Select(u => (u.Hash.ToString(), new string[] { u.Method })))
            .GroupBy(u => u.Item1, u => u.Item2)
            .Select(u =>
            {
                var list = new HashSet<string>();
                foreach (var kv in u.ToArray()) foreach (var k in kv) list.Add(k);
                return ContractPermissionToManifest(u.Key, list.ToArray());
            }));

            if (string.IsNullOrEmpty(permissions)) permissions = @"{""contract"":""*"",""methods"":""*""}";

            return
                @"{""groups"":[],""abi"":" + sbABI +
                @",""permissions"":[" + permissions + @"],""trusts"":[],""name"":""" + name + @""",""supportedstandards"":" + supportedStandards + @",""extra"":" + extra + "}";
        }

        private static string ContractPermissionToManifest(string hash, string[] methods)
        {
            var jsonMethods = ((methods?.Length ?? 0) == 0 || methods[0] == "*") ?
                "\"*\"" : $"[{string.Join(',', methods.Select(u => "\"" + ScapeJson(u) + "\""))}]";

            return $"{{\"contract\":\"{ScapeJson(hash)}\",\"methods\":{jsonMethods}}}";
        }
    }
}
