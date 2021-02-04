using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Neo;
using Neo.BlockchainToolkit.Persistence;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace NeoTestHarness
{
    using NeoStorage = IReadOnlyDictionary<ReadOnlyMemory<byte>, StorageItem>;

    public static class Extensions
    {
        // TestHarness replacement for Neo.Wallets.Helper.ToAddress that doesn't load protocol settings
        public static string ToAddress(this UInt160 scriptHash, byte addressVersion = (byte)0x35)
        {
            Span<byte> data = stackalloc byte[21];
            data[0] = addressVersion;
            Neo.IO.Helper.ToArray(scriptHash).CopyTo(data[1..]);
            return Neo.Cryptography.Base58.Base58CheckEncode(data);
        }

        // TestHarness replacement for Neo.Wallets.Helper.ToScriptHash that doesn't load protocol settings
        public static UInt160 FromAddress(this string address, byte addressVersion = (byte)0x35)
        {
            byte[] data = Neo.Cryptography.Base58.Base58CheckDecode(address);
            if (data.Length != 21)
                throw new FormatException();
            if (data[0] != addressVersion)
                throw new FormatException();
            return new UInt160(data.AsSpan(1));
        }

        public static VMState ExecuteScript<T>(this TestApplicationEngine engine, params Expression<Action<T>>[] expressions)
            where T : class
        {
            engine.LoadScript<T>(expressions);
            return engine.Execute();
        }

        public static void LoadScript<T>(this TestApplicationEngine engine, params Expression<Action<T>>[] expressions)
            where T : class
        {
            var script = engine.Snapshot.CreateScript<T>(expressions);
            engine.LoadScript(script);
        }

        public static Script CreateScript<T>(this DataCache snapshot, params Expression<Action<T>>[] expressions)
            where T : class
        {
            var scriptHash = snapshot.GetContractAddress<T>();
            using var builder = new ScriptBuilder();
            for (int i = 0; i < expressions.Length; i++)
            {
                var methodCall = (MethodCallExpression)expressions[i].Body;
                var operation = methodCall.Method.Name;

                for (var x = methodCall.Arguments.Count - 1; x >= 0; x--)
                {
                    var obj = Expression.Lambda(methodCall.Arguments[x]).Compile().DynamicInvoke();
                    builder.EmitPush(obj);
                }
                builder.EmitPush(methodCall.Arguments.Count);
                builder.Emit(OpCode.PACK);
                builder.EmitPush(CallFlags.All);
                builder.EmitPush(operation);
                builder.EmitPush(scriptHash);
                builder.EmitSysCall(ApplicationEngine.System_Contract_Call);
            }
            return builder.ToArray();
        }

        public static NeoStorage GetContractStorages<T>(this DataCache snapshot)
            where T : class
        {
            var contract = snapshot.GetContract<T>();
            var prefix = StorageKey.CreateSearchPrefix(contract.Id, default);

            return snapshot.Find(prefix)
                .ToImmutableDictionary(s => (ReadOnlyMemory<byte>)s.Key.Key.AsMemory(), s => s.Value, MemoryEqualityComparer.Instance);
        }

        class MemoryEqualityComparer : IEqualityComparer<ReadOnlyMemory<byte>>
        {
            public static MemoryEqualityComparer Instance = new MemoryEqualityComparer();

            private MemoryEqualityComparer() { }

            public bool Equals([AllowNull] ReadOnlyMemory<byte> x, [AllowNull] ReadOnlyMemory<byte> y) => x.Span.SequenceEqual(y.Span);

            public int GetHashCode([DisallowNull] ReadOnlyMemory<byte> obj)
            {
                unchecked
                {
                    int hash = 17;
                    for (var i = 0; i < obj.Length; i++)
                    {
                        hash = hash * 23 + obj.Span[i].GetHashCode();
                    }
                    return hash;
                }
            }
        }

        public static NeoStorage StorageMap(this NeoStorage storages, string prefix)
            => storages.StorageMap(Utility.StrictUTF8.GetBytes(prefix));

        public static NeoStorage StorageMap(this NeoStorage storages, ReadOnlyMemory<byte> prefix)
            => storages.Where(kvp => kvp.Key.Span.StartsWith(prefix.Span))
                .ToImmutableDictionary(kvp => kvp.Key.Slice(prefix.Length), kvp => kvp.Value, MemoryEqualityComparer.Instance);

        public static bool TryGetValue(this NeoStorage storage, string key, [NotNullWhen(true)] out StorageItem item)
            => storage.TryGetValue(Utility.StrictUTF8.GetBytes(key), out item!);

        public static bool TryGetValue(this NeoStorage storage, UInt160 key, [NotNullWhen(true)] out StorageItem item)
            => storage.TryGetValue(Neo.IO.Helper.ToArray(key), out item!);

        public static bool TryGetValue(this NeoStorage storage, UInt256 key, [NotNullWhen(true)] out StorageItem item)
            => storage.TryGetValue(Neo.IO.Helper.ToArray(key), out item!);

        public static BigInteger ToBigInteger(this StorageItem storageItem)
            => new BigInteger(storageItem.Value);

        public static UInt160 GetContractAddress<T>(this DataCache snapshot)
            where T : class
            => snapshot.GetContract<T>().Hash;

        public static ContractState GetContract<T>(this DataCache snapshot)
            where T : class
        {
            var contractName = GetContractName(typeof(T));

            foreach (var contractState in NativeContract.ContractManagement.ListContracts(snapshot))
            {
                var name = contractState.Id >= 0 ? contractState.Manifest.Name : "Neo.SmartContract.Native." + contractState.Manifest.Name;
                if (string.Equals(contractName, name))
                {
                    return contractState;
                }
            }

            throw new Exception($"couldn't find {contractName} contract");

            static string GetContractName(Type type)
            {
                if (type.IsNested)
                {
                    return GetContractName(type.DeclaringType ?? throw new Exception("reflection IsNested DeclaringType returned null"));
                }

                var attrib = ContractAttribute.GetCustomAttribute(type, typeof(ContractAttribute)) as ContractAttribute;
                return attrib?.Name ?? type.FullName ?? throw new Exception("reflection - FullName returned null");
            }
        }
    }
}
