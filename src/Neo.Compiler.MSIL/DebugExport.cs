using Mono.Cecil.Cil;
using Neo.IO.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Neo.Compiler
{
    public static class DebugExport
    {
        private static JArray GetSequencePoints(IEnumerable<NeoCode> codes, IReadOnlyDictionary<string, int> docMap, IReadOnlyDictionary<int, int> addrConvTable)
        {
            var points = codes
                .Where(code => code.sequencePoint != null)
                .Select(code => (code.addr, code.sequencePoint));

            var outjson = new JArray();

            foreach (var (address, sequencePoint) in points)
            {
                var value = string.Format("{0}[{1}]{2}:{3}-{4}:{5}",
                    addrConvTable.TryGetValue(address, out var newAddress) ? newAddress : address,
                    docMap[sequencePoint.Document.Url],
                    sequencePoint.StartLine,
                    sequencePoint.StartColumn,
                    sequencePoint.EndLine,
                    sequencePoint.EndColumn);
                outjson.Add(value);
            }

            return outjson;
        }

        private static JArray ConvertParamList(IEnumerable<NeoParam> @params)
        {
            @params ??= Enumerable.Empty<NeoParam>();
            var paramsJson = new JArray();
            foreach (var param in @params)
            {
                var value = string.Format("{0},{1}", param.name, FuncExport.ConvType(param.type));
                paramsJson.Add(value);
            }

            return paramsJson;
        }

        private static JArray GetMethods(NeoModule module, IReadOnlyDictionary<string, int> docMap, IReadOnlyDictionary<int, int> addrConvTable)
        {
            var outjson = new JArray();

            foreach (var method in module.mapMethods.Values)
            {
                if (method.body_Codes.Values.Count == 0)
                {
                    continue;
                }

                var name = string.Format("{0},{1}",
                    method._namespace, method.displayName);

                var range = string.Format("{0}-{1}",
                    method.body_Codes.Values.First().addr,
                    method.body_Codes.Values.Last().addr);

                var methodJson = new JObject();
                methodJson["id"] = method.name;
                methodJson["name"] = name;
                methodJson["range"] = range;
                methodJson["params"] = ConvertParamList(method.paramtypes);
                methodJson["return"] = FuncExport.ConvType(method.returntype);
                methodJson["variables"] = ConvertParamList(method.method?.body_Variables);
                methodJson["sequence-points"] = GetSequencePoints(method.body_Codes.Values, docMap, addrConvTable);
                outjson.Add(methodJson);
            }
            return outjson;
        }

        private static JArray GetEvents(NeoModule module)
        {
            var outjson = new JArray();
            foreach (var @event in module.mapEvents.Values)
            {
                var name = string.Format("{0},{1}",
                    @event._namespace, @event.displayName);

                var eventJson = new JObject();
                eventJson["id"] = @event.name;
                eventJson["name"] = name;
                eventJson["params"] = ConvertParamList(@event.paramtypes);
                outjson.Add(eventJson);
            }
            return outjson;
        }

        private static IReadOnlyDictionary<string, int> GetDocumentMap(NeoModule module)
        {
            return module.mapMethods.Values
                .SelectMany(m => m.body_Codes.Values)
                .Where(code => code.sequencePoint != null)
                .Select(code => code.sequencePoint.Document.Url)
                .Distinct()
                .Select((d, i) => (d, i))
                .ToDictionary(t => t.d, t => t.i);
        }

        private static JArray GetDocuments(IReadOnlyDictionary<string, int> docMap)
        {
            var outjson = new JArray();
            foreach (var doc in docMap.OrderBy(kvp => kvp.Value))
            {
                Debug.Assert(outjson.Count == doc.Value);
                outjson.Add(doc.Key);
            }
            return outjson;
        }

        public static JObject Export(NeoModule module, byte[] script, IReadOnlyDictionary<int, int> addrConvTable)
        {
            var docMap = GetDocumentMap(module);
            addrConvTable ??= ImmutableDictionary<int, int>.Empty;

            var outjson = new JObject();
            outjson["hash"] = ComputeHash(script);
            outjson["documents"] = GetDocuments(docMap);
            outjson["methods"] = GetMethods(module, docMap, addrConvTable);
            outjson["events"] = GetEvents(module);
            return outjson;
        }

        static string ComputeHash(byte[] script)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hash256 = sha256.ComputeHash(script);
            var ripemd160 = new Neo.Cryptography.RIPEMD160Managed();
            var hash = ripemd160.ComputeHash(hash256);

            StringBuilder sb = new StringBuilder();
            sb.Append("0x");
            for (int i = hash.Length - 1; i >= 0; i--)
            {
                sb.Append(hash[i].ToString("x02"));
            }
            return sb.ToString();
        }
    }
}
