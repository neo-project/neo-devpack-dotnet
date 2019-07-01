using System;
using System.Collections.Generic;

namespace Neo.Compiler.SourceMap
{
    public class SourceNode
    {
        private static int NEWLINE_CODE = 10;
        
        public SourceLineColumnName sourceLineColumnName;
        public IList<SourceNodeOrSource> children;

        public SourceNode(SourceLineColumnName sourceLineColumnName, IList<SourceNodeOrSource> children)
        {
            this.sourceLineColumnName = sourceLineColumnName;
            this.children = children;
        }

        public SourceNode(SourceLineColumnName sourceLineColumnName)
        {
            this.sourceLineColumnName = sourceLineColumnName;
            children = new List<SourceNodeOrSource>();
        }

        public SourceNode()
        {
            this.sourceLineColumnName = new SourceLineColumnName();
            children = new List<SourceNodeOrSource>();
        }

        public static void Test()
        {
            var node = new SourceNode(new SourceLineColumnName
                {
                    Line = 1,
                    Column = 2,
                    Source = "a.js"
                },
                new List<SourceNodeOrSource>
                {
                    new SourceNodeOrSource(new SourceNode(new SourceLineColumnName
                    {
                        Line = 3,
                        Column = 4,
                        Source = "b.js",
                        Name = "uno"
                    })),
                    new SourceNodeOrSource("dos"),
                    new SourceNodeOrSource("tres"),
                    new SourceNodeOrSource(new SourceNode(new SourceLineColumnName
                    {
                        Line = 5,
                        Column = 6,
                        Source = "c.js",
                        Name = "quatro"
                    }))
                }
            );

            var map = node.ToStringWithSourceMap("my-output-file.js", null);
            var seri = map.SerializeMappings();
        }

        public SourceMapGenerator ToStringWithSourceMap(string file, string sourceRoot)
        {
            var generatedCode = "";
            var generatedLine = 1;
            var generatedColumn = 0;
                
            var map = new SourceMapGenerator(file, sourceRoot);
            var sourceMappingActive = false;
            string lastOriginalSource = null;
            int? lastOriginalLine = null;
            int? lastOriginalColumn = null;
            string lastOriginalName = null;

          Walk((chunk, original) => {
            generatedCode += chunk;
            
            if (original.Source != null
                && original.Line != null
                && original.Column != null) {
              
              if (lastOriginalSource != original.Source
                || lastOriginalLine != original.Line
                || lastOriginalColumn != original.Column
                || lastOriginalName != original.Name) {
                map.AddMapping(
                  original.Source,
                  original.Line,
                  original.Column,
                  generatedLine,
                  generatedColumn,
                  original.Name);
              }
              
              lastOriginalSource = original.Source;
              lastOriginalLine = original.Line;
              lastOriginalColumn = original.Column;
              lastOriginalName = original.Name;
              sourceMappingActive = true;
              
            } else if (sourceMappingActive) {
              map.AddMapping(
                  null,
                  null,
                  null,
                  generatedLine,
                  generatedColumn,
                  null);
              
              lastOriginalSource = null;
              sourceMappingActive = false;
            }
            
            for (var idx = 0; idx < chunk.Length; idx++) {
              if (chunk.ToCharArray()[idx] == NEWLINE_CODE) {
                generatedLine++;
                generatedColumn = 0;
                // Mappings end at eol
                if (idx + 1 == chunk.Length) {
                  lastOriginalSource = null;
                  sourceMappingActive = false;
                } else if (sourceMappingActive) {
                  map.AddMapping(
                      original.Source,
                      original.Line,
                      original.Column,
                      generatedLine,
                      generatedColumn,
                      original.Name);
                }
              } else {
                generatedColumn++;
              }
            }
          });
//      
//          return { code: generatedCode, map };
            return map;
        }
        
        


        /**
         * Walk over the tree of code snippets in this node and its children. The
         * walking function is called once for each snippet of code and is passed that
         * snippet and the its original associated source's line/column location.
         *
         * @param aFn The traversal function.
         */
        private void Walk(Action<string, SourceLineColumnName> aFn)
        {
            foreach (SourceNodeOrSource chunk in children)
            {
                if (chunk.SourceNode != null) {
                    chunk.SourceNode.Walk(aFn);
                } else if (chunk.Source.Length > 0) {
                    aFn(chunk.Source, new SourceLineColumnName(sourceLineColumnName));
                }
            }
        }
    }
}