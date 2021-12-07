using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Iterators;
using Neo.SmartContract.Native;
using Neo.VM.Types;
using Newtonsoft.Json;

namespace Neo.Test.Runner
{
    static class JsonWriterExtensions
    {
        public static async Task WriteLogAsync(this JsonWriter writer, LogEventArgs args)
        {
            await writer.WriteStartObjectAsync();
            await writer.WritePropertyNameAsync("contract");
            await writer.WriteValueAsync($"{args.ScriptHash}");
            await writer.WritePropertyNameAsync("message");
            await writer.WriteValueAsync(args.Message);
            await writer.WriteEndObjectAsync();
        }

        public static async Task WriteNotificationAsync(this JsonWriter writer, NotifyEventArgs args, int maxIteratorCount)
        {
            await writer.WriteStartObjectAsync();
            await writer.WritePropertyNameAsync("contract");
            await writer.WriteValueAsync($"{args.ScriptHash}");
            await writer.WritePropertyNameAsync("eventname");
            await writer.WriteValueAsync(args.EventName);
            await writer.WritePropertyNameAsync("state");
            await writer.WriteStackItemAsync(args.State, maxIteratorCount);
            await writer.WriteEndObjectAsync();
        }

        public static async Task WriteStorageAsync(this JsonWriter writer, DataCache snapshot, UInt160 contractHash)
        {
            var contract = NativeContract.ContractManagement.GetContract(snapshot, contractHash);
            if (contract == null) return;

            byte[] keyPrefix = StorageKey.CreateSearchPrefix(contract.Id, default(ReadOnlySpan<byte>));
            await writer.WriteStartObjectAsync();

            await writer.WritePropertyNameAsync("name");
            await writer.WriteValueAsync(contract.Manifest.Name);

            await writer.WritePropertyNameAsync("hash");
            await writer.WriteValueAsync($"{contract.Hash}");

            await writer.WritePropertyNameAsync("values");

            await writer.WriteStartArrayAsync();
            foreach (var kvp in snapshot.Find(keyPrefix))
            {
                await writer.WriteStartObjectAsync();
                await writer.WritePropertyNameAsync("key");
                await writer.WriteValueAsync(Convert.ToBase64String(kvp.Key.Key));
                await writer.WritePropertyNameAsync("value");
                await writer.WriteValueAsync(Convert.ToBase64String(kvp.Value.Value));
                await writer.WriteEndObjectAsync();
            }
            await writer.WriteEndArrayAsync();

            await writer.WriteEndObjectAsync();
        }

        public static async Task WriteStackItemAsync(this JsonWriter writer, StackItem item, int maxIteratorCount, HashSet<StackItem>? context = null)
        {
            await writer.WriteStartObjectAsync();
            await writer.WritePropertyNameAsync("type");
            await writer.WriteValueAsync($"{item.Type}");
            await writer.WritePropertyNameAsync("value");
            switch (item)
            {
                case Neo.VM.Types.Array array:
                    {
                        context ??= new(ReferenceEqualityComparer.Instance);
                        if (!context.Add(array)) throw new InvalidOperationException();
                        await writer.WriteStartArrayAsync();
                        for (int i = 0; i < array.Count; i++)
                        {
                            await writer.WriteStackItemAsync(array[i], maxIteratorCount, context);
                        }
                        await writer.WriteEndArrayAsync();
                        break;
                    }
                case Neo.VM.Types.Boolean _:
                    await writer.WriteValueAsync(item.GetBoolean());
                    break;
                case Neo.VM.Types.Buffer _:
                case ByteString _:
                    {
                        var value = Convert.ToBase64String(item.GetSpan());
                        await writer.WriteValueAsync(value);
                        break;
                    }
                case Neo.VM.Types.Integer _:
                    await writer.WriteValueAsync($"{item.GetInteger()}");
                    break;
                case Map map:
                    {
                        context ??= new(ReferenceEqualityComparer.Instance);
                        if (!context.Add(map)) throw new InvalidOperationException();
                        await writer.WriteStartArrayAsync();
                        foreach (var i in map)
                        {
                            await writer.WriteStartObjectAsync();
                            await writer.WritePropertyNameAsync("key");
                            await writer.WriteStackItemAsync(i.Key, maxIteratorCount, context);
                            await writer.WritePropertyNameAsync("value");
                            await writer.WriteStackItemAsync(i.Value, maxIteratorCount, context);
                            await writer.WriteEndObjectAsync();
                        }
                        await writer.WriteEndArrayAsync();
                        break;
                    }
                case Pointer pointer:
                    await writer.WriteValueAsync(pointer.Position);
                    break;
                case InteropInterface interop
                    when interop.GetInterface<object>() is IIterator iterator:
                    {
                        context ??= new(ReferenceEqualityComparer.Instance);
                        await writer.WriteStartObjectAsync();
                        await writer.WritePropertyNameAsync("iterator");
                        await writer.WriteStartArrayAsync();
                        while (maxIteratorCount-- > 0 && iterator.Next())
                        {
                            await writer.WriteStackItemAsync(iterator.Value(), maxIteratorCount, context);
                        }
                        await writer.WriteEndArrayAsync();
                        await writer.WritePropertyNameAsync("truncated");
                        await writer.WriteValueAsync(iterator.Next());
                        await writer.WriteEndArrayAsync();
                        break;
                    }
            }
            await writer.WriteEndObjectAsync();
        }
    }
}
