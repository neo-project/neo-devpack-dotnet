// Copyright (C) 2015-2023 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.Compiler
{
    public class Options : CompilationOptions
    {
        [Flags]
        public enum GenerateArtifactsKind : byte
        {
            None = 0,
            Source = 1,
            Library = 2,

            All = Source | Library
        }

        public string? Output { get; set; }
        public bool Assembly { get; set; }
        public GenerateArtifactsKind GenerateArtifacts { get; set; } = GenerateArtifactsKind.None;
    }
}
