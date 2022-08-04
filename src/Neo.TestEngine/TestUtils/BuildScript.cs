// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.Compiler;
using Neo.IO;
using Neo.IO.Json;
using Neo.SmartContract;
using System.Collections.Generic;
using System.IO;

namespace Neo.TestingEngine
{
    public class BuildScript
    {
        public bool Success => !FromCompilation || (Context != null && Context.Success);

        public UInt160? ScriptHash { get; private set; }
        public NefFile Nef { get; protected set; }
        public JObject Manifest { get; protected set; }
        public JObject? DebugInfo { get; protected set; }
        public CompilationContext? Context { get; protected set; }

        private bool FromCompilation { get; set; }

        public BuildScript(NefFile nefFile, JObject manifestJson, UInt160? originHash = null)
        {
            Nef = nefFile;
            Manifest = manifestJson;

            if (originHash is null && nefFile != null)
            {
                originHash = Nef.Script.Span.ToScriptHash();
            }
            ScriptHash = originHash;
        }

        internal static BuildScript Build(List<MetadataReference>? references = null, bool debug = true, params string[] files)
        {
            BuildScript script;
            if (files.Length == 1 && Path.GetExtension(files[0]).ToLowerInvariant() == ".nef")
            {
                var filename = files[0];
                MemoryReader reader = new(File.ReadAllBytes(filename));

                NefFile neffile = new NefFile();
                neffile.Deserialize(ref reader);
                var fileNameManifest = filename.Replace(".nef", ".manifest.json");
                string manifestFile = File.ReadAllText(fileNameManifest);
                script = new BuildScript(neffile, JObject.Parse(manifestFile))
                {
                    FromCompilation = false
                };
            }
            else
            {
                NefFile? nef = null;
                JObject? manifest = null;
                JObject? debuginfo = null;

                var options = new Options
                {
                    AddressVersion = ProtocolSettings.Default.AddressVersion,
                    Debug = debug
                };

                CompilationContext context;
                if (references != null && references.Count > 0)
                {
                    context = CompilationContext.CompileSources(files, references, options);
                }
                else
                {
                    context = CompilationContext.CompileSources(files, options);
                }

                if (context.Success)
                {
                    nef = context.CreateExecutable();
                    manifest = context.CreateManifest();
                    debuginfo = context.CreateDebugInformation();
                }

                script = new BuildScript(nef, manifest)
                {
                    FromCompilation = true,
                    Context = context,
                    DebugInfo = debuginfo
                };
            }

            return script;
        }
    }
}
