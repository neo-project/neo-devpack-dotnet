// Copyright (C) 2015-2024 The Neo Project.
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
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Neo.Compiler
{
    public class CompilationOptions
    {
        [Flags]
        public enum OptimizationType : byte
        {
            None = 0,
            Basic = 1,
            Experimental = 2,

            All = Basic | Experimental
        }

        public enum DebugType : byte
        {
            /// <summary>
            /// No debug
            /// </summary>
            None = 0,

            /// <summary>
            /// Strict NEP-19 standard
            /// </summary>
            Strict = 1,

            /// <summary>
            /// It will include Abi and compiler location information
            /// </summary>
            Extended = 2,
        }

        public NullableContextOptions Nullable { get; set; }
        public DebugType Debug { get; set; } = DebugType.None;
        public OptimizationType Optimize { get; set; } = OptimizationType.Basic;
        public bool Checked { get; set; }
        public bool NoInline { get; set; }
        public byte AddressVersion { get; set; } = 0x35;
        public string? BaseName { get; set; }
        public string CompilerVersion { get; set; }
        private CSharpParseOptions? parseOptions = null;

        public CSharpParseOptions GetParseOptions()
        {
            if (parseOptions is null)
            {
                List<string> preprocessorSymbols = new();
                if (Debug != DebugType.None) preprocessorSymbols.Add("DEBUG");
                parseOptions = new CSharpParseOptions(preprocessorSymbols: preprocessorSymbols);
            }
            return parseOptions;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CompilationOptions()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>()!;
            var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!;

            CompilerVersion = $"{titleAttribute.Title} {versionAttribute.InformationalVersion}";
        }
    }
}
