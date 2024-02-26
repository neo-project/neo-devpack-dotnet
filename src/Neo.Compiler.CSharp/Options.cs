// Copyright (C) 2015-2023 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler
{
    public class Options: CompilationOptions
    {
        public enum GenerateArtifactsKind
        {
            None,
            Source,
            Library,
            All
        }

        public string? Output { get; set; }
        public bool Assembly { get; set; }
        public GenerateArtifactsKind GenerateArtifacts { get; set; } = GenerateArtifactsKind.Source;
    }
}
