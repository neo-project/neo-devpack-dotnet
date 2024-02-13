// Copyright (C) 2015-2023 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Neo.Compiler
{
    public class Options
    {
        public string? Output { get; set; }
        public string? BaseName { get; set; }
        public NullableContextOptions Nullable { get; set; }
        public bool Checked { get; set; }
        public bool Debug { get; set; }
        public bool Assembly { get; set; }
        public bool GenerateArtifactLibrary { get; set; }
        public bool NoOptimize { get; set; }
        public bool NoInline { get; set; }
        public byte AddressVersion { get; set; }

        private CSharpParseOptions? parseOptions = null;
        public CSharpParseOptions GetParseOptions()
        {
            if (parseOptions is null)
            {
                List<string> preprocessorSymbols = new();
                if (Debug) preprocessorSymbols.Add("DEBUG");
                parseOptions = new CSharpParseOptions(preprocessorSymbols: preprocessorSymbols);
            }
            return parseOptions;
        }
    }
}
