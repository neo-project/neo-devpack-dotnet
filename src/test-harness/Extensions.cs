using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Neo;
using Neo.BlockchainToolkit;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using static Neo.Utility;

namespace NeoTestHarness
{
    using NeoStorage = IReadOnlyDictionary<ReadOnlyMemory<byte>, StorageItem>;

    public static class Extensions
    {
        public static VMState ExecuteScript<T>(this ApplicationEngine engine, params Expression<Action<T>>[] expressions)
            where T : class
        {
            engine.LoadScript<T>(expressions);
            return engine.Execute();
        }

        public static VMState ExecuteScript(this ApplicationEngine engine, Script script)
        {
            engine.LoadScript(script);
            return engine.Execute();
        }

        public static void LoadScript<T>(this ApplicationEngine engine, params Expression<Action<T>>[] expressions)
            where T : class
        {
            var script = engine.Snapshot.CreateScript<T>(expressions);
            engine.LoadScript(script);
        }

        public static Script CreateScript<T>(this DataCache snapshot, params Expression<Action<T>>[] expressions)
            where T : class
        {
            var scriptHash = snapshot.GetContractScriptHash<T>();
            using var builder = new ScriptBuilder();
            for (int i = 0; i < expressions.Length; i++)
            {
                builder.EmitContractCall(scriptHash, expressions[i]);
            }
            return builder.ToArray();
        }

        public static void EmitContractCall<T>(this ScriptBuilder builder, DataCache snapshot, Expression<Action<T>> expression)
            where T : class
        {
            var scriptHash = snapshot.GetContractScriptHash<T>();
            EmitContractCall<T>(builder, scriptHash, expression);
        }

        public static void EmitContractCall<T>(this ScriptBuilder builder, UInt160 scriptHash, Expression<Action<T>> expression)
        {
            var methodCall = (MethodCallExpression)expression.Body;
            var operation = methodCall.Method.Name;

            for (var x = methodCall.Arguments.Count - 1; x >= 0; x--)
            {
                var obj = Expression.Lambda(methodCall.Arguments[x]).Compile().DynamicInvoke();
                var param = ContractParameterParser.ConvertObject(obj);
                builder.EmitPush(param);
            }
            builder.EmitPush(methodCall.Arguments.Count);
            builder.Emit(OpCode.PACK);
            builder.EmitPush(CallFlags.All);
            builder.EmitPush(operation);
            builder.EmitPush(scriptHash);
            builder.EmitSysCall(ApplicationEngine.System_Contract_Call);
        }

        public static NeoStorage GetContractStorages<T>(this DataCache snapshot)
            where T : class
        {
            var contract = snapshot.GetContract<T>();
            var prefix = StorageKey.CreateSearchPrefix(contract.Id, default);

            return snapshot.Find(prefix)
                .ToDictionary(s => s.Key.Key, s => s.Value, MemorySequenceComparer.Default);
        }

        public static NeoStorage StorageMap(this NeoStorage storages, byte prefix)
        {
            byte[]? buffer = null;
            try
            {
                buffer = ArrayPool<byte>.Shared.Rent(1);
                buffer[0] = prefix;
                return storages.StorageMap(buffer.AsMemory(0, 1));
            }
            finally
            {
                if (buffer != null) ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static NeoStorage StorageMap(this NeoStorage storages, string prefix)
        {
            byte[]? buffer = null;
            try
            {
                var count = StrictUTF8.GetByteCount(prefix);
                buffer = ArrayPool<byte>.Shared.Rent(count);
                count = StrictUTF8.GetBytes(prefix, buffer);
                return storages.StorageMap(buffer.AsMemory(0, count));
            }
            finally
            {
                if (buffer != null) ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static NeoStorage StorageMap(this NeoStorage storages, ReadOnlyMemory<byte> prefix)
            => storages.Where(kvp => kvp.Key.Span.StartsWith(prefix.Span))
                .ToDictionary(kvp => kvp.Key.Slice(prefix.Length), kvp => kvp.Value, MemorySequenceComparer.Default);

        public static bool TryGetValue(this NeoStorage storage, string key, [NotNullWhen(true)] out StorageItem item)
            => storage.TryGetValue(StrictUTF8.GetBytes(key), out item!);

        public static bool TryGetValue(this NeoStorage storage, UInt160 key, [NotNullWhen(true)] out StorageItem item)
            => storage.TryGetValue(Neo.IO.Helper.ToArray(key), out item!);

        public static bool TryGetValue(this NeoStorage storage, UInt256 key, [NotNullWhen(true)] out StorageItem item)
            => storage.TryGetValue(Neo.IO.Helper.ToArray(key), out item!);

        public static UInt160 GetContractScriptHash<T>(this DataCache snapshot)
            where T : class
            => snapshot.GetContract<T>().Hash;

        public static ContractState GetContract<T>(this DataCache snapshot)
            where T : class
        {
            var contractName = GetContractName(typeof(T));
            return snapshot.GetContract(contractName);

            static string GetContractName(Type type)
            {
                if (type.IsNested)
                {
                    return GetContractName(type.DeclaringType ?? throw new Exception("reflection IsNested DeclaringType returned null"));
                }

                var contractAttrib = Attribute.GetCustomAttribute(type, typeof(ContractAttribute)) as ContractAttribute;
                if (contractAttrib != null) return contractAttrib.Name;

                var descriptionAttrib = Attribute.GetCustomAttribute(type, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (descriptionAttrib != null) return descriptionAttrib.Description;

                throw new Exception("reflection - FullName returned null");
            }
        }

        public static ContractState GetContract(this DataCache snapshot, string contractName)
        {
            foreach (var contractState in NativeContract.ContractManagement.ListContracts(snapshot))
            {
                var name = contractState.Id >= 0 ? contractState.Manifest.Name : "Neo.SmartContract.Native." + contractState.Manifest.Name;
                if (string.Equals(contractName, name))
                {
                    return contractState;
                }
            }

            throw new Exception($"couldn't find {contractName} contract");
        }

        public static SnapshotCache GetSnapshot(this CheckpointFixture fixture)
        {
            return new SnapshotCache(fixture.CheckpointStore);
        }
    }
}
