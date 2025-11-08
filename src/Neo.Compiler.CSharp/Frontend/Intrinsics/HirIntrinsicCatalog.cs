using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

internal static class HirIntrinsicCatalog
{
    private sealed record Descriptor(
        string Category,
        string Name,
        HirEffect Effect,
        HirType ReturnType,
        IReadOnlyList<HirType> Parameters,
        bool IsPure,
        bool Deterministic,
        int? GasCost)
    {
        public bool RequiresToken => (Effect & HirEffect.Memory) != 0 || Effect == HirEffect.Abort;
    }

    private static readonly Dictionary<string, Descriptor> s_intrinsics = new(StringComparer.Ordinal)
    {
        {
            "Neo.SmartContract.Framework.Services.Storage.Get",
            new Descriptor(
                "Storage",
                "Get",
                HirEffect.StorageRead,
                HirType.ByteStringType,
                new HirType[] { new HirInteropHandleType("StorageContext"), HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.Storage.Put",
            new Descriptor(
                "Storage",
                "Put",
                HirEffect.StorageWrite,
                HirType.VoidType,
                new HirType[] { new HirInteropHandleType("StorageContext"), HirType.UnknownType, HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.Runtime.CheckWitness",
            new Descriptor(
                "Runtime",
                "CheckWitness",
                HirEffect.Runtime,
                HirType.BoolType,
                new HirType[] { HirType.ByteStringType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.Runtime.Log",
            new Descriptor(
                "Runtime",
                "Log",
                HirEffect.Runtime,
                HirType.VoidType,
                new HirType[] { HirType.ByteStringType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.Runtime.Notify",
            new Descriptor(
                "Runtime",
                "Notify",
                HirEffect.Runtime,
                HirType.VoidType,
                new HirType[] { HirType.ByteStringType, new HirArrayType(HirType.UnknownType) },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.Storage.Delete",
            new Descriptor(
                "Storage",
                "Delete",
                HirEffect.StorageWrite,
                HirType.VoidType,
                new HirType[] { new HirInteropHandleType("StorageContext"), HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.StorageMap.get_Item",
            new Descriptor(
                "Storage",
                "Get",
                HirEffect.StorageRead,
                HirType.ByteStringType,
                new HirType[] { new HirInteropHandleType("StorageMap"), HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.StorageMap.set_Item",
            new Descriptor(
                "Storage",
                "Put",
                HirEffect.StorageWrite,
                HirType.VoidType,
                new HirType[] { new HirInteropHandleType("StorageMap"), HirType.UnknownType, HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.StorageMap.Get",
            new Descriptor(
                "Storage",
                "Get",
                HirEffect.StorageRead,
                HirType.ByteStringType,
                new HirType[] { new HirInteropHandleType("StorageMap"), HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.StorageMap.GetByteArray",
            new Descriptor(
                "Storage",
                "Get",
                HirEffect.StorageRead,
                HirType.BufferType,
                new HirType[] { new HirInteropHandleType("StorageMap"), HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.StorageMap.Put",
            new Descriptor(
                "Storage",
                "Put",
                HirEffect.StorageWrite,
                HirType.VoidType,
                new HirType[] { new HirInteropHandleType("StorageMap"), HirType.UnknownType, HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.StorageMap.Delete",
            new Descriptor(
                "Storage",
                "Delete",
                HirEffect.StorageWrite,
                HirType.VoidType,
                new HirType[] { new HirInteropHandleType("StorageMap"), HirType.UnknownType },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Services.Contract.Call",
            new Descriptor(
                "Contract",
                "Call",
                HirEffect.Interop,
                HirType.UnknownType,
                new HirType[] { HirType.ByteStringType, HirType.ByteStringType, HirType.IntType, new HirArrayType(HirType.UnknownType) },
                IsPure: false,
                Deterministic: true,
                GasCost: null)
        },
        {
            "Neo.SmartContract.Framework.Native.CryptoLib.Sha256",
            new Descriptor(
                "Crypto",
                "Sha256",
                HirEffect.Crypto,
                HirType.ByteStringType,
                new HirType[] { HirType.ByteStringType },
                IsPure: true,
                Deterministic: true,
                GasCost: 10)
        }
    };

    private static readonly Dictionary<string, Descriptor> s_syscallIntrinsics = new(StringComparer.Ordinal)
    {
        { "System.Storage.Get", s_intrinsics["Neo.SmartContract.Framework.Services.Storage.Get"] },
        { "System.Storage.Put", s_intrinsics["Neo.SmartContract.Framework.Services.Storage.Put"] },
        { "System.Storage.Delete", s_intrinsics["Neo.SmartContract.Framework.Services.Storage.Delete"] },
        { "System.Storage.GetContext", new Descriptor(
            "Storage",
            "GetContext",
            HirEffect.Runtime,
            new HirInteropHandleType("StorageContext"),
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.GetReadOnlyContext", new Descriptor(
            "Storage",
            "GetReadOnlyContext",
            HirEffect.Runtime,
            new HirInteropHandleType("StorageContext"),
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.AsReadOnly", new Descriptor(
            "Storage",
            "AsReadOnly",
            HirEffect.Runtime,
            new HirInteropHandleType("StorageContext"),
            new HirType[] { new HirInteropHandleType("StorageContext") },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.Local.Get", new Descriptor(
            "Storage",
            "Get",
            HirEffect.StorageRead,
            HirType.ByteStringType,
            new HirType[] { HirType.UnknownType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.Local.Put", new Descriptor(
            "Storage",
            "Put",
            HirEffect.StorageWrite,
            HirType.VoidType,
            new HirType[] { HirType.UnknownType, HirType.UnknownType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.Local.Delete", new Descriptor(
            "Storage",
            "Delete",
            HirEffect.StorageWrite,
            HirType.VoidType,
            new HirType[] { HirType.UnknownType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.Find", new Descriptor(
            "Storage",
            "Find",
            HirEffect.StorageRead,
            new HirIteratorType(),
            new HirType[] { new HirInteropHandleType("StorageContext"), HirType.UnknownType, HirType.IntType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Storage.Local.Find", new Descriptor(
            "Storage",
            "Find",
            HirEffect.StorageRead,
            new HirIteratorType(),
            new HirType[] { HirType.UnknownType, HirType.IntType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.CheckWitness", s_intrinsics["Neo.SmartContract.Framework.Services.Runtime.CheckWitness"] },
        { "System.Runtime.Log", s_intrinsics["Neo.SmartContract.Framework.Services.Runtime.Log"] },
        { "System.Runtime.Notify", s_intrinsics["Neo.SmartContract.Framework.Services.Runtime.Notify"] },
        { "System.Runtime.GetTime", new Descriptor(
            "Runtime",
            "GetTime",
            HirEffect.Runtime,
            HirType.IntType,
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.GasLeft", new Descriptor(
            "Runtime",
            "GasLeft",
            HirEffect.Runtime,
            HirType.IntType,
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.GetInvocationCounter", new Descriptor(
            "Runtime",
            "GetInvocationCounter",
            HirEffect.Runtime,
            HirType.IntType,
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.GetAddressVersion", new Descriptor(
            "Runtime",
            "GetAddressVersion",
            HirEffect.Runtime,
            HirType.IntType,
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.GetNotifications", new Descriptor(
            "Runtime",
            "GetNotifications",
            HirEffect.Runtime,
            new HirArrayType(HirType.UnknownType),
            new HirType[] { HirType.UnknownType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.BurnGas", new Descriptor(
            "Runtime",
            "BurnGas",
            HirEffect.Runtime,
            HirType.VoidType,
            new HirType[] { HirType.IntType },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.GetRandom", new Descriptor(
            "Runtime",
            "GetRandom",
            HirEffect.Runtime,
            HirType.IntType,
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.GetNetwork", new Descriptor(
            "Runtime",
            "GetNetwork",
            HirEffect.Runtime,
            HirType.IntType,
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Runtime.CurrentSigners", new Descriptor(
            "Runtime",
            "CurrentSigners",
            HirEffect.Runtime,
            new HirArrayType(new HirInteropHandleType("Signer")),
            Array.Empty<HirType>(),
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Contract.Call", s_intrinsics["Neo.SmartContract.Framework.Services.Contract.Call"] },
        { "System.StorageMap.Get", s_intrinsics["Neo.SmartContract.Framework.Services.StorageMap.Get"] },
        { "System.StorageMap.GetByteArray", s_intrinsics["Neo.SmartContract.Framework.Services.StorageMap.GetByteArray"] },
        { "System.StorageMap.Put", s_intrinsics["Neo.SmartContract.Framework.Services.StorageMap.Put"] },
        { "System.StorageMap.Delete", s_intrinsics["Neo.SmartContract.Framework.Services.StorageMap.Delete"] },
        { "System.Iterator.Next", new Descriptor(
            "Iterator",
            "Next",
            HirEffect.StorageRead,
            HirType.BoolType,
            new HirType[] { new HirIteratorType() },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Iterator.Value", new Descriptor(
            "Iterator",
            "Value",
            HirEffect.StorageRead,
            HirType.UnknownType,
            new HirType[] { new HirIteratorType() },
            IsPure: false,
            Deterministic: true,
            GasCost: null) },
        { "System.Crypto.CheckSig", new Descriptor(
            "Crypto",
            "CheckSig",
            HirEffect.Crypto,
            HirType.BoolType,
            new HirType[] { HirType.ByteStringType, HirType.ByteStringType },
            IsPure: true,
            Deterministic: true,
            GasCost: null) },
        { "System.Crypto.CheckMultisig", new Descriptor(
            "Crypto",
            "CheckMultisig",
            HirEffect.Crypto,
            HirType.BoolType,
            new HirType[] { HirType.UnknownType, HirType.UnknownType },
            IsPure: true,
            Deterministic: true,
            GasCost: null) }
    };

    public static bool TryResolve(string fullyQualifiedMethodName, out HirIntrinsicMetadata metadata)
    {
        if (fullyQualifiedMethodName is null)
            throw new ArgumentNullException(nameof(fullyQualifiedMethodName));

        static string TrimGlobal(string value)
            => value.StartsWith("global::", StringComparison.Ordinal) ? value[8..] : value;

        var key = TrimGlobal(fullyQualifiedMethodName);

        if (s_intrinsics.TryGetValue(key, out var descriptor))
        {
            metadata = new HirIntrinsicMetadata(
                descriptor.Category,
                descriptor.Name,
                descriptor.Effect,
                descriptor.ReturnType,
                descriptor.Parameters,
                descriptor.IsPure,
                descriptor.Deterministic,
                descriptor.RequiresToken,
                descriptor.GasCost);
            return true;
        }

        metadata = default;
        return false;
    }

    public static bool TryResolveSyscall(string syscallName, out HirIntrinsicMetadata metadata)
    {
        if (syscallName is null)
            throw new ArgumentNullException(nameof(syscallName));

        if (s_syscallIntrinsics.TryGetValue(syscallName, out var descriptor))
        {
            metadata = new HirIntrinsicMetadata(
                descriptor.Category,
                descriptor.Name,
                descriptor.Effect,
                descriptor.ReturnType,
                descriptor.Parameters,
                descriptor.IsPure,
                descriptor.Deterministic,
                descriptor.RequiresToken,
                descriptor.GasCost);
            return true;
        }

        metadata = default;
        return false;
    }
}
