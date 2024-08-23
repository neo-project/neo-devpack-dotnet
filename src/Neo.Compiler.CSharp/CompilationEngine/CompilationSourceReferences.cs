// Copyright (C) 2015-2024 The Neo Project.
//
// CompilationSourceReferences.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler
{
    public class CompilationSourceReferences
    {
        /// <summary>
        /// Packages
        /// </summary>
        public (string packageName, string packageVersion)[]? Packages { get; set; } = null;

        /// <summary>
        /// Projects
        /// </summary>
        public string[]? Projects { get; set; } = null;
    }
}
