// Copyright (C) 2015-2021 The Neo Project.
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
    public class Options
    {
        public string? Output { get; set; }
        public string? ContractName { get; set; }
        public bool Debug { get; set; }
        public bool Assembly { get; set; }
        public bool NoOptimize { get; set; }
        public bool NoInline { get; set; }
        public byte AddressVersion { get; set; }
    }
}
