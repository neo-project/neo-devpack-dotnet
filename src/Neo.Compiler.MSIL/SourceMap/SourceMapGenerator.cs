using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Compiler.SourceMap
{
    public class SourceMapGenerator
    {
        // A single base 64 digit can contain 6 bits of data. For the base 64 variable
        // length quantities we use in the source map spec, the first bit is the sign,
        // the next four bits are the actual value, and the 6th bit is the
        // continuation bit. The continuation bit tells us whether there are more
        // digits in this value following this digit.
        //
        //   Continuation
        //   |    Sign
        //   |    |
        //   V    V
        //   101011
        
        private static int VLQ_BASE_SHIFT = 5;
        
        // binary: 100000
        private static int VLQ_BASE = 1 << VLQ_BASE_SHIFT;
        
        // binary: 011111
        private static int VLQ_BASE_MASK = VLQ_BASE - 1;
        
        // binary: 100000
        private static int VLQ_CONTINUATION_BIT = VLQ_BASE;


        private static char[] intToCharMap =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();

        private string file;
        private string sourceRoot;
        private IList<Mapping> mappings = new List<Mapping>();
        private IList<string> sources = new List<string>();
        private IList<string> names = new List<string>();
        private IDictionary<string, string> sourcesContents;

        public SourceMapGenerator()
        {
        }

        public SourceMapGenerator(string file, string sourceRoot)
        {
            this.file = file;
            this.sourceRoot = sourceRoot;
        }

        public void addMapping(string source, int? originalLine, int? originalColumn, int generatedLine, int generatedColumn, string name) {
            if (source != null) {
                if (!sources.Contains(source)) {
                    sources.Add(source);
                }
            }
            
            if (name != null) {
                if (!names.Contains(name)) {
                    names.Add(name);
                }
            }

            mappings.Add(new Mapping
            {
                GeneratedLine = generatedLine,
                GeneratedColumn = generatedColumn,
                OriginalLine = originalLine,
                OriginalColumn = originalColumn,
                Source = source,
                Name = name
            });
        }
        
        /**
         * Set the source content for a source file.
         */
        public void SetSourceContent(string aSourceFile, string aSourceContent) {
            var source = aSourceFile;
            if (sourceRoot != null) {
                source = relative(sourceRoot, source);
            }

            if (aSourceContent != null) {
                // Add the source content to the _sourcesContents map.
                // Create a new _sourcesContents map if the property is null.
                if (sourcesContents == null) {
                    sourcesContents = new Dictionary<string, string>();
                }
                sourcesContents[source] = aSourceContent;
            } else if (this.sourcesContents != null) {
                // Remove the source file from the _sourcesContents map.
                // If the _sourcesContents map is empty, set the property to null.
                sourcesContents.Remove(source);
                if (sourcesContents.Count == 0) {
                    sourcesContents = null;
                }
            }
        }
        
        /**
         * reference:
         * https://github.com/mozilla/source-map/blob/ddaac7f00a67aefbf184bae16cb90b30ddc84a67/lib/source-map-generator.js
         */
        public string serializeMappings() {
            var previousGeneratedColumn = 0;
            var previousGeneratedLine = 1;
            var previousOriginalColumn = 0;
            var previousOriginalLine = 0;
            var previousName = 0;
            var previousSource = 0;
            var result = "";

            for (var i = 0; i < mappings.Count; i++)
            {
                var mapping = mappings.ElementAt(i);
                
                var next = "";

                if (mapping.GeneratedLine != previousGeneratedLine) {
                    previousGeneratedColumn = 0;
                    while (mapping.GeneratedLine != previousGeneratedLine) {
                        next += ";";
                        previousGeneratedLine++;
                    }
                } else if (i > 0) {
                    if (CompareByGeneratedPositionsInflated(mapping, mappings.ElementAt(i - 1)) == 0) {
                        continue;
                    }
                    next += ",";
                }

                next += Base64VLQEncode(mapping.GeneratedColumn
                                         - previousGeneratedColumn);
                previousGeneratedColumn = mapping.GeneratedColumn;

                if (mapping.Source != null) {
                    var sourceIdx = sources.IndexOf(mapping.Source);
                    next += Base64VLQEncode(sourceIdx - previousSource);
                    previousSource = sourceIdx;

                    // lines are stored 0-based in SourceMap spec version 3
                    next += Base64VLQEncode((mapping.OriginalLine ?? 0) - 1 - previousOriginalLine);
                    previousOriginalLine = (mapping.OriginalLine ?? 0) - 1;

                    next += Base64VLQEncode((mapping.OriginalColumn ?? 0) - previousOriginalColumn);
                    previousOriginalColumn = mapping.OriginalColumn ?? 0;

                    if (mapping.Name != null) {
                        var nameIdx = names.IndexOf(mapping.Name);
                        next += Base64VLQEncode(nameIdx - previousName);
                        previousName = nameIdx;
                    }
                }

                result += next;
            }

            return result;
        }

        private int CompareByGeneratedPositionsInflated(Mapping mappingA, Mapping mappingB)
        {
            var cmp = mappingA.GeneratedLine - mappingB.GeneratedLine;
            if (cmp != 0) {
                return cmp;
            }

            cmp = mappingA.GeneratedColumn - mappingB.GeneratedColumn;
            if (cmp != 0) {
                return cmp;
            }

            cmp = string.Compare(mappingA.Source, mappingB.Source);
            if (cmp != 0) {
                return cmp;
            }

            cmp = mappingA.OriginalLine ?? 0 - mappingB.OriginalLine ?? 0;
            if (cmp != 0) {
                return cmp;
            }

            cmp = mappingA.OriginalColumn ?? 0 - mappingB.OriginalColumn ?? 0;
            if (cmp != 0) {
                return cmp;
            }

            return string.Compare(mappingA.Name, mappingB.Name);
        }

        private static string Base64VLQEncode(int aValue)
        {
            var encoded = "";

            var vlq = toVLQSigned(aValue);

            do {
                var digit = vlq & VLQ_BASE_MASK;
                vlq = (int) ((uint) vlq >> VLQ_BASE_SHIFT);
                if (vlq > 0) {
                    // There are still more digits in this value, so we must make sure the
                    // continuation bit is marked.
                    digit |= VLQ_CONTINUATION_BIT;
                }
                encoded += Base64Encode(digit);
            } while (vlq > 0);

            return encoded;
        }

        private static string Base64Encode(int number)
        {
            if (0 <= number && number < intToCharMap.Length) {
                return intToCharMap[number].ToString();
            }
            throw new Exception("Must be between 0 and 63: " + number);
        }
        
        private static int toVLQSigned(int aValue) {
            return aValue < 0
                ? (-aValue << 1) + 1
                : (aValue << 1) + 0;
        }
        
        private string relative(string aRoot, string aPath) {
            if (aRoot == "") {
                aRoot = ".";
            }

            aRoot = new Regex("/$").Replace(aRoot, "");

            // It is possible for the path to be above the root. In this case, simply
            // checking whether the root is a prefix of the path won't work. Instead, we
            // need to remove components from the root one by one, until either we find
            // a prefix that fits, or we run out of components to remove.
            var level = 0;
            while (aPath.IndexOf(aRoot + "/") != 0) {
                var index = aRoot.LastIndexOf("/");
                if (index < 0) {
                    return aPath;
                }

                // If the only part of the root that is left is the scheme (i.e. http://,
                // file:///, etc.), one or more slashes (/), or simply nothing at all, we
                // have exhausted all components, so the path is not relative to the root.
                aRoot = aRoot.Take(index).ToString();
                if (new Regex("^([^/]+:/)?/*$").IsMatch(aRoot)) {
                  return aPath;
                }
            
                ++level;
              }
            
              // Make sure we add a "../" for each component we removed from the root.
              return String.Concat(Enumerable.Repeat("../", level)) + aPath.Substring(aRoot.Length + 1);
        }
    }
}