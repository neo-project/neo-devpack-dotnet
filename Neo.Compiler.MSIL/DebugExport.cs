using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using vmtool;

namespace Neo.Compiler
{
    static class DebugExport
    {
        private static MyJson.JsonNode_Array GetSequencePoints(IEnumerable<NeoCode> codes, IDictionary<string, int> docMap)
        {
            var points = codes
                .Where(code => code.sequencePoint != null)
                .Select(code => (code.addr, code.sequencePoint));

            var outjson = new MyJson.JsonNode_Array();

            foreach (var (address, sequencePoint) in points)
            {
                var range = string.Format("{0}:{1}-{2}:{3}",
                    sequencePoint.StartLine,
                    sequencePoint.StartColumn,
                    sequencePoint.EndLine,
                    sequencePoint.EndColumn);

                var spjson = new MyJson.JsonNode_Object();
                spjson.SetDictValue("addr", address);
                spjson.SetDictValue("doc", docMap[sequencePoint.Document.Url]);
                spjson.SetDictValue("range", range);

                outjson.Add(spjson);
            }

            return outjson;
        }

        static string ConvertType(string type)
        {
            if (type == "System.Object")
                return string.Empty;

            return FuncExport.ConvType(type);
        }

        static MyJson.JsonNode_Array GetParameters(IList<NeoParam> @params)
        {
            var paramsJson = new MyJson.JsonNode_Array();
            foreach (var param in @params)
            {
                var paramJson = new MyJson.JsonNode_Object();
                paramJson.SetDictValue("name", param.name);
                paramJson.SetDictValue("type", ConvertType(param.type));
                paramsJson.Add(paramJson);
            }

            return paramsJson;
        }

        static MyJson.JsonNode_Array GetMethods(NeoModule module, IDictionary<string, int> docMap)
        {

            var outjson = new MyJson.JsonNode_Array();

            foreach (var method in module.mapMethods.Values)
            {
                var range = string.Format("{0}-{1}",
                    method.body_Codes.Values.First().addr,
                    method.body_Codes.Values.Last().addr);


                var methodJson = new MyJson.JsonNode_Object();
                methodJson.SetDictValue("id", method.name);
                methodJson.SetDictValue("namespace", method._namespace);
                methodJson.SetDictValue("name", method.displayName);
                methodJson.SetDictValue("range", range);
                methodJson.SetDictValue("params", GetParameters(method.paramtypes));
                methodJson.SetDictValue("return", ConvertType(method.returntype));

                var varaiablesJson = new MyJson.JsonNode_Array();
                foreach (var variable in method.body_Variables)
                {
                    var variableJson = new MyJson.JsonNode_Object();
                    variableJson.SetDictValue("name", variable.name);
                    variableJson.SetDictValue("type", ConvertType(variable.type));
                    varaiablesJson.Add(variableJson);
                }
                methodJson.SetDictValue("variables", varaiablesJson);

                methodJson.SetDictValue("sequence-points", GetSequencePoints(method.body_Codes.Values, docMap));
                outjson.Add(methodJson);
            }
            return outjson;
        }

        static MyJson.JsonNode_Array GetEvents(NeoModule module)
        {
            var outjson = new MyJson.JsonNode_Array();
            foreach (var @event in module.mapEvents.Values)
            {
                var eventJson = new MyJson.JsonNode_Object();
                eventJson.SetDictValue("id", @event.name);
                eventJson.SetDictValue("namespace", @event._namespace);
                eventJson.SetDictValue("name", @event.displayName);
                eventJson.SetDictValue("params", GetParameters(@event.paramtypes));
                outjson.Add(eventJson);
            }
            return outjson;
        }

        static IDictionary<string, int> GetDocumentMap(NeoModule module)
        {
            return module.mapMethods.Values
                .SelectMany(m => m.body_Codes.Values)
                .Where(code => code.sequencePoint != null)
                .Select(code => code.sequencePoint.Document.Url)
                .Distinct()
                .Select((d, i) => (d, i))
                .ToDictionary(t => t.d, t => t.i);
        }

        static MyJson.JsonNode_Array GetDocuments(IDictionary<string, int> docMap)
        {
            var outjson = new MyJson.JsonNode_Array();
            foreach (var doc in docMap.OrderBy(kvp => kvp.Value))
            {
                Debug.Assert(outjson.Count == doc.Value);
                outjson.Add(new MyJson.JsonNode_ValueString(doc.Key));
            }
            return outjson;
        }

        public static MyJson.JsonNode_Object Export(NeoModule am)
        {
            var docMap = GetDocumentMap(am);

            var outjson = new MyJson.JsonNode_Object();
            outjson.SetDictValue("entrypoint", am.mainMethod);
            outjson.SetDictValue("documents", GetDocuments(docMap));
            outjson.SetDictValue("methods", GetMethods(am, docMap));
            outjson.SetDictValue("events", GetEvents(am));
            return outjson;
        }
    }
}
