using SourcemapToolkit.SourcemapParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler
{
    class SourceMapTool
    {
        public static string GenMapFile(NeoModule module)
        {
            SourceMap map = new SourceMap();

            foreach (var m in module.mapMethods)
            {
                //item.SetDictValue("name", m.Value.displayName);
                //item.SetDictValue("addr", m.Value.funcaddr.ToString("X04"));
                //Neo.Compiler.MyJson.JsonNode_Array infos = new Neo.Compiler.MyJson.JsonNode_Array();
                //item.SetDictValue("map", infos);
                foreach (var c in m.Value.body_Codes)
                {


                    if (c.Value.debugcode != null)
                    {


                        var debugcode = c.Value.debugcode.ToLower();
                        if (debugcode.Contains(".cs"))
                        {
                            var e = new MappingEntry();
                            map.ParsedMappings.Add(e);
                            e.OriginalName = m.Value.displayName;
                            e.OriginalSourcePosition = new SourcePosition();
                            e.OriginalFileName = debugcode;
                            e.OriginalSourcePosition.ZeroBasedLineNumber = c.Value.debugline;
                            e.OriginalSourcePosition.ZeroBasedColumnNumber = 0;
                        }
                    }
                }

            }
            var gener = new SourceMapGenerator();
            return gener.SerializeMapping(map);
        }
    }
}
