// Copyright (C) 2015-2026 The Neo Project.
//
// KnownFrameworkTypes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    internal sealed class KnownFrameworkTypes
    {
        private const string FrameworkAssemblyName = "Neo.SmartContract.Framework";

        private KnownFrameworkTypes(IAssemblySymbol frameworkAssembly)
        {
            UInt160 = GetTypeByMetadataName(frameworkAssembly, "UInt160");
            UInt256 = GetTypeByMetadataName(frameworkAssembly, "UInt256");
            ECPoint = GetTypeByMetadataName(frameworkAssembly, "ECPoint");
        }

        public INamedTypeSymbol? UInt160 { get; }

        public INamedTypeSymbol? UInt256 { get; }

        public INamedTypeSymbol? ECPoint { get; }

        public static KnownFrameworkTypes? Create(Compilation compilation)
        {
            var frameworkAssembly = compilation.SourceModule.ReferencedAssemblySymbols
                .FirstOrDefault(static assembly => assembly.Identity.Name == FrameworkAssemblyName);

            return frameworkAssembly is null ? null : new KnownFrameworkTypes(frameworkAssembly);
        }

        private static INamedTypeSymbol? GetTypeByMetadataName(IAssemblySymbol frameworkAssembly, string typeName)
        {
            return frameworkAssembly.GetTypeByMetadataName($"{FrameworkAssemblyName}.{typeName}");
        }
    }
}
