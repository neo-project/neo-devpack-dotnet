// Copyright (C) 2015-2025 The Neo Project.
//
// CompilerLocation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;

namespace Neo.Compiler
{
    /// <summary>
    /// Which file and method in the compiler generates the OpCode
    /// </summary>
    public class CompilerLocation
    {
        public int Line { get; set; }
        public required string Method { get; set; }
        public required string File { get; set; }

        /// <summary>
        /// Convert to Json
        /// </summary>
        /// <returns>Json object</returns>
        public JObject ToJson()
        {
            var compiler = new JObject();
            compiler["file"] = File;
            compiler["line"] = Line;
            compiler["method"] = Method;
            return compiler;
        }
    }
}
