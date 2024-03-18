using Neo.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Testing.Coverage
{
    public partial class NeoDebugInfo
    {
        static readonly Regex spRegex = new(@"^(\d+)\[(-?\d+)\](\d+)\:(\d+)\-(\d+)\:(\d+)$");

        public const string MANIFEST_FILE_EXTENSION = ".manifest.json";
        public const string NEF_DBG_NFO_EXTENSION = ".nefdbgnfo";
        public const string DEBUG_JSON_EXTENSION = ".debug.json";

        public readonly UInt160 Hash;
        public readonly string DocumentRoot;
        public readonly IReadOnlyList<string> Documents;
        public readonly IReadOnlyList<Method> Methods;

        public NeoDebugInfo(UInt160 hash, string documentRoot, IReadOnlyList<string> documents, IReadOnlyList<Method> methods)
        {
            Hash = hash;
            DocumentRoot = documentRoot;
            Documents = documents;
            Methods = methods;
        }

        public static bool TryLoad(string path, [MaybeNullWhen(false)] out NeoDebugInfo debugInfo)
        {
            if (path.EndsWith(NEF_DBG_NFO_EXTENSION))
            {
                return TryLoadCompressed(path, out debugInfo);
            }
            else if (path.EndsWith(DEBUG_JSON_EXTENSION))
            {
                return TryLoadUncompressed(path, out debugInfo);
            }
            else
            {
                debugInfo = default;
                return false;
            }
        }

        public static bool TryLoadManifestDebugInfo(string manifestPath, [MaybeNullWhen(false)] out NeoDebugInfo debugInfo)
        {
            if (string.IsNullOrEmpty(manifestPath))
            {
                debugInfo = default;
                return false;
            }

            var basePath = Path.Combine(Path.GetDirectoryName(manifestPath), GetBaseName(manifestPath, MANIFEST_FILE_EXTENSION));

            var nefdbgnfoPath = Path.ChangeExtension(basePath, NEF_DBG_NFO_EXTENSION);
            if (TryLoadCompressed(nefdbgnfoPath, out debugInfo))
                return true;

            var debugJsonPath = Path.ChangeExtension(basePath, DEBUG_JSON_EXTENSION);
            return TryLoadUncompressed(debugJsonPath, out debugInfo);
        }

        private static string GetBaseName(string path, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            path = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(suffix)
                && path.EndsWith(suffix, comparison))
            {
                return path.Substring(0, path.Length - suffix.Length);
            }
            return path;
        }

        static bool TryLoadCompressed(string debugInfoPath, [MaybeNullWhen(false)] out NeoDebugInfo debugInfo)
        {
            try
            {
                if (File.Exists(debugInfoPath))
                {
                    using var fileStream = File.OpenRead(debugInfoPath);
                    return TryLoadCompressed(fileStream, out debugInfo);
                }
            }
            catch { }

            debugInfo = default;
            return false;
        }

        internal static bool TryLoadCompressed(Stream stream, [MaybeNullWhen(false)] out NeoDebugInfo debugInfo)
        {
            try
            {
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith(DEBUG_JSON_EXTENSION, StringComparison.OrdinalIgnoreCase))
                    {
                        using var entryStream = entry.Open();
                        debugInfo = Load(entryStream);
                        return true;
                    }
                }
            }
            catch { }

            debugInfo = default;
            return false;
        }

        static bool TryLoadUncompressed(string debugInfoPath, [MaybeNullWhen(false)] out NeoDebugInfo debugInfo)
        {
            try
            {
                if (File.Exists(debugInfoPath))
                {
                    using var fileStream = File.OpenRead(debugInfoPath);
                    debugInfo = Load(fileStream);
                    return true;
                }
            }
            catch { }

            debugInfo = default;
            return false;
        }

        internal static NeoDebugInfo Load(Stream stream)
        {
            using StreamReader reader = new(stream);
            var text = reader.ReadToEnd();
            var json = JToken.Parse(text) ?? throw new InvalidOperationException();
            if (json is not JObject jo) throw new FormatException();
            return FromDebugInfoJson(jo);
        }

        public static NeoDebugInfo FromDebugInfoJson(string json)
        {
            var jsonToken = JToken.Parse(json);
            if (jsonToken is not JObject jobj)
                throw new FormatException("The json must be an object");

            return FromDebugInfoJson(jobj);
        }

        public static NeoDebugInfo FromDebugInfoJson(JObject json)
        {
            if (json["hash"]?.GetString() is not string sHash)
            {
                throw new ArgumentNullException("hash can't be null");
            }

            if (json["documents"] is not JArray jDocs)
            {
                throw new ArgumentNullException("documents must be an array");
            }

            if (json["methods"] is not JArray jMethods)
            {
                throw new ArgumentNullException("methods must be an array");
            }

            var hash = UInt160.TryParse(sHash, out var _hash)
                ? _hash
                : throw new FormatException($"Invalid hash {sHash}");

            var docRoot = json["document-root"]?.GetString();
            docRoot = string.IsNullOrEmpty(docRoot) ? "" : docRoot;

            var documents = jDocs.Select(kvp => kvp?.GetString()!).Where(u => u is not null);
            var methods = jMethods.Select(kvp => MethodFromJson(kvp as JObject));

            // TODO: parse events and static variables

            return new NeoDebugInfo(hash, docRoot, documents.ToList(), methods.ToList());
        }

        static Method MethodFromJson(JObject? json)
        {
            if (json is null)
            {
                throw new ArgumentNullException("Method can't be null");
            }

            if (json["params"] is not JArray jParams)
            {
                throw new ArgumentNullException("params must be an array");
            }

            if (json["sequence-points"] is not JArray jSequence)
            {
                throw new ArgumentNullException("sequence-points must be an array");
            }

            // TODO: parse return, params and variables

            var id = json["id"]?.GetString() ?? throw new ArgumentNullException("method.id can't be null");
            var (@namespace, name) = NameFromJson(json["name"]?.GetString() ?? throw new ArgumentNullException("method.name can't be null"));
            var range = RangeFromJson(json["range"]?.GetString() ?? throw new ArgumentNullException("method.range can't be null"));
            var @params = jParams.Select(kvp => ParamFromJson(kvp?.GetString()));
            var sequencePoints = jSequence.Select(kvp => SequencePointFromJson(kvp?.GetString()));

            return new Method(id, @namespace, name, range, @params.ToList(), sequencePoints.ToList());
        }

        static Parameter ParamFromJson(string? param)
        {
            if (param is null)
            {
                throw new ArgumentNullException("Parameter can't be null");
            }

            var values = param.Split(',');
            if (values.Length == 2 || values.Length == 3)
            {
                var index = values.Length == 3
                    && int.TryParse(values[2], out var _index)
                    && _index >= 0 ? _index : -1;

                return new Parameter(values[0], values[1], index);
            }
            throw new FormatException($"invalid parameter \"{param}\"");
        }

        static (string, string) NameFromJson(string? name)
        {
            if (name is null)
            {
                throw new ArgumentNullException("Name can't be null");
            }

            var values = name.Split(',');
            return values.Length == 2
                ? (values[0], values[1])
                : throw new FormatException($"Invalid name '{name}'");
        }

        static (int, int) RangeFromJson(string? range)
        {
            if (range is null)
            {
                throw new ArgumentNullException("Name can't be null");
            }

            var values = range.Split('-');
            return values.Length == 2
                ? (int.Parse(values[0]), int.Parse(values[1]))
                : throw new FormatException($"Invalid range '{range}'");
        }

        static SequencePoint SequencePointFromJson(string? sequence)
        {
            if (sequence is null)
            {
                throw new ArgumentNullException("Name can't be null");
            }

            var match = spRegex.Match(sequence);
            if (match.Groups.Count != 7)
                throw new FormatException($"Invalid Sequence Point \"{sequence}\"");

            var address = int.Parse(match.Groups[1].Value);
            var document = int.Parse(match.Groups[2].Value);
            var start = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            var end = (int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value));

            return new SequencePoint(address, document, start, end);
        }
    }
}
