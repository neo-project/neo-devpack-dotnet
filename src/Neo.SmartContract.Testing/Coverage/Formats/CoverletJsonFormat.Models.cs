using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class CoverletJsonFormat : CoverageFormatBase
    {
        internal class BranchInfo
        {
            public int Line { get; set; }
            public int Offset { get; set; }
            public int EndOffset { get; set; }
            public int Path { get; set; }
            public uint Ordinal { get; set; }
            public int Hits { get; set; }
        }

        internal class Lines : SortedDictionary<int, int> { }

        internal class Branches : List<BranchInfo> { }

        internal class Method
        {
            [JsonConstructor]
            public Method()
            {
                Lines = new();
                Branches = new();
            }

            public Lines Lines;
            public Branches Branches;
        }

        internal class Methods : Dictionary<string, Method> { }

        internal class Classes : Dictionary<string, Methods> { }

        internal class Documents : Dictionary<string, Classes> { }

        internal class Modules : Dictionary<string, Documents>
        {
            /// <summary>
            /// Merge with other module
            /// </summary>
            /// <param name="m">Module</param>
            public void Merge(Modules modules)
            {
                foreach (KeyValuePair<string, Documents> module in modules)
                {
                    if (!Keys.Contains(module.Key))
                    {
                        Add(module.Key, module.Value);
                    }
                    else
                    {
                        foreach (KeyValuePair<string, Classes> document in module.Value)
                        {
                            if (!this[module.Key].ContainsKey(document.Key))
                            {
                                this[module.Key].Add(document.Key, document.Value);
                            }
                            else
                            {
                                foreach (KeyValuePair<string, Methods> @class in document.Value)
                                {
                                    if (!this[module.Key][document.Key].ContainsKey(@class.Key))
                                    {
                                        this[module.Key][document.Key].Add(@class.Key, @class.Value);
                                    }
                                    else
                                    {
                                        foreach (KeyValuePair<string, Method> method in @class.Value)
                                        {
                                            if (!this[module.Key][document.Key][@class.Key].ContainsKey(method.Key))
                                            {
                                                this[module.Key][document.Key][@class.Key].Add(method.Key, method.Value);
                                            }
                                            else
                                            {
                                                foreach (KeyValuePair<int, int> line in method.Value.Lines)
                                                {
                                                    if (!this[module.Key][document.Key][@class.Key][method.Key].Lines.ContainsKey(line.Key))
                                                    {
                                                        this[module.Key][document.Key][@class.Key][method.Key].Lines.Add(line.Key, line.Value);
                                                    }
                                                    else
                                                    {
                                                        this[module.Key][document.Key][@class.Key][method.Key].Lines[line.Key] += line.Value;
                                                    }
                                                }

                                                foreach (BranchInfo branch in method.Value.Branches)
                                                {
                                                    Branches branches = this[module.Key][document.Key][@class.Key][method.Key].Branches;
                                                    BranchInfo? branchInfo = branches.FirstOrDefault(b => b.EndOffset == branch.EndOffset && b.Line == branch.Line && b.Offset == branch.Offset && b.Ordinal == branch.Ordinal && b.Path == branch.Path);
                                                    if (branchInfo == null)
                                                        branches.Add(branch);
                                                    else
                                                        branchInfo.Hits += branch.Hits;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// To json
            /// </summary>
            /// <returns>Json representation</returns>
            public string ToJson()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }

            /// <summary>
            /// Write module to path
            /// </summary>
            /// <param name="path">Path</param>
            /// <param name="mergeIfExists">Merge if exists</param>
            public void Write(string path, bool mergeIfExists)
            {
                if (mergeIfExists && File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var module = JsonConvert.DeserializeObject<Modules>(json);

                    if (module is not null)
                    {
                        module.Merge(this);
                        File.WriteAllText(path, module.ToJson());
                        return;
                    }
                }

                File.WriteAllText(path, ToJson());
            }
        }
    }
}
