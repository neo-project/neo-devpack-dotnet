using SourcemapToolkit.SourcemapParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler
{
    class SourceMapTool
    {
        public static string GenMapFile(string name, NeoModule module)
        {
            SourceMap map = new SourceMap();
            map.Version = 3;
            map.File = name + ".avm";
            map.Sources = new List<string>();
            map.Names = new List<string>();



            map.ParsedMappings = new List<MappingEntry>();
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
                            e.OriginalFileName = debugcode;
                            if (map.Names.Contains(e.OriginalName) == false)
                                map.Names.Add(e.OriginalName);
                            if (map.Sources.Contains(e.OriginalFileName) == false)
                                map.Sources.Add(e.OriginalFileName);

                            //pos in c#
                            e.OriginalSourcePosition = new SourcePosition();
                            e.OriginalSourcePosition.ZeroBasedLineNumber = c.Value.debugline;
                            e.OriginalSourcePosition.ZeroBasedColumnNumber = c.Value.debugcol;

                            //pos in avm
                            e.GeneratedSourcePosition = new SourcePosition();
                            //use avm addr as line
                            e.GeneratedSourcePosition.ZeroBasedLineNumber = c.Value.addr;
                            e.GeneratedSourcePosition.ZeroBasedColumnNumber = 0;
                        }
                    }
                }

            }
            var gener = new SourceMapGenerator();
            return gener.SerializeMapping(map);
        }
    }
}
