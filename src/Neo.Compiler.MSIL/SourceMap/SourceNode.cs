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

        public static void test()
        {
            var node = new SourceNode(new SourceLineColumnName
                {
                    line = 1,
                    column = 2,
                    source = "a.js"
                },
                new List<SourceNodeOrSource>
                {
                    new SourceNodeOrSource(new SourceNode(new SourceLineColumnName
                    {
                        line = 3,
                        column = 4,
                        source = "b.js",
                        name = "uno"
                    })),
                    new SourceNodeOrSource("dos"),
                    new SourceNodeOrSource("tres"),
                    new SourceNodeOrSource(new SourceNode(new SourceLineColumnName
                    {
                        line = 5,
                        column = 6,
                        source = "c.js",
                        name = "quatro"
                    }))
                }
            );

            var map = node.toStringWithSourceMap("my-output-file.js", null);
            var seri = map.serializeMappings();
        }

        public SourceMapGenerator toStringWithSourceMap(string file, string sourceRoot)
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

          walk((chunk, original) => {
            generatedCode += chunk;
            
            if (original.source != null
                && original.line != null
                && original.column != null) {
              
              if (lastOriginalSource != original.source
                || lastOriginalLine != original.line
                || lastOriginalColumn != original.column
                || lastOriginalName != original.name) {
                map.addMapping(
                  original.source,
                  original.line,
                  original.column,
                  generatedLine,
                  generatedColumn,
                  original.name);
              }
              
              lastOriginalSource = original.source;
              lastOriginalLine = original.line;
              lastOriginalColumn = original.column;
              lastOriginalName = original.name;
              sourceMappingActive = true;
              
            } else if (sourceMappingActive) {
              map.addMapping(
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
                  map.addMapping(
                      original.source,
                      original.line,
                      original.column,
                      generatedLine,
                      generatedColumn,
                      original.name);
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
        private void walk(Action<string, SourceLineColumnName> aFn)
        {
            foreach (SourceNodeOrSource chunk in children)
            {
                if (chunk.SourceNode != null) {
                    chunk.SourceNode.walk(aFn);
                } else if (chunk.Source.Length > 0) {
                    aFn(chunk.Source, new SourceLineColumnName(sourceLineColumnName));
                }
            }
        }
    }
}