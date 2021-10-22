// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Linq;
using Neo.Persistence;
using Neo.SmartContract.Iterators;

namespace Neo.TestingEngine
{
    public static class Helper
    {
        public static JObject ToJson(this TestEngine testEngine)
        {
            var json = new JObject();

            json["vmstate"] = testEngine.State.ToString();
            json["gasconsumed"] = (new BigDecimal((decimal)testEngine.GasConsumedByLastExecution, NativeContract.GAS.Decimals)).ToString();
            json["resultstack"] = testEngine.ResultStack.ToJson();

            json["currentblock"] = testEngine.PersistingBlock.ToSimpleJson();
            if (testEngine.ScriptContainer is Transaction tx)
            {
                json["transaction"] = tx.ToSimpleJson();
            }

            json["storage"] = testEngine.Snapshot.ToJson();
            json["notifications"] = new JArray(testEngine.Notifications.Select(n => n.ToJson()));
            json["error"] = testEngine.State.HasFlag(VMState.FAULT) ? GetExceptionMessage(testEngine.FaultException) : null;

            return json;
        }

        public static JObject ToJson(this EvaluationStack stack)
        {
            JArray jarr = new();
            foreach (var item in stack)
            {
                if (item is InteropInterface interopInterface && interopInterface.GetInterface<object>() is IIterator iterator)
                {
                    var max = 100;
                    JObject json = item.ToJson();
                    JArray array = new();
                    while (max > 0 && iterator.Next())
                    {
                        array.Add(iterator.Value().ToJson());
                        max--;
                    }
                    json["iterator"] = array;
                    json["truncated"] = iterator.Next();
                    jarr.Add(json);
                }
                else
                {
                    jarr.Add(item.ToJson());
                }
            }

            return jarr;
        }

        public static JObject ToJson(this NotifyEventArgs notification)
        {
            var json = new JObject();
            json["eventname"] = notification.EventName;
            json["scripthash"] = notification.ScriptHash.ToString();
            json["value"] = notification.State.ToJson();
            return json;
        }

        public static JObject ToJson(this DataCache storage)
        {
            var jsonStorage = new JArray();
            // had to filter which data should be returned back, since everything is in the storage now
            var newStorage = storage.GetStorageOnly();

            foreach (var (storageKey, storageValue) in newStorage)
            {
                var key = new ByteString(storageKey.Key);
                StackItem value;
                try
                {
                    value = new ByteString(storageValue.Value);
                }
                catch
                {
                    value = StackItem.Null;
                }

                var jsonKey = new JObject();
                jsonKey["id"] = storageKey.Id;
                jsonKey["key"] = key.ToJson();

                var jsonValue = new JObject();
                jsonValue["isconstant"] = false;
                jsonValue["value"] = value.ToJson();

                var storageItem = new JObject();
                storageItem["key"] = jsonKey;
                storageItem["value"] = jsonValue;

                jsonStorage.Add(storageItem);
            }
            return jsonStorage;
        }

        public static JObject ToSimpleJson(this Block block)
        {
            JObject json = new JObject();
            json["hash"] = block.Hash.ToString();
            json["index"] = block.Index;
            json["timestamp"] = block.Timestamp;
            json["transactions"] = new JArray(block.Transactions.Select(tx => tx.ToSimpleJson()));
            return json;
        }

        public static JObject ToSimpleJson(this Transaction tx)
        {
            // build a tx with the mutable fields to have a consistent hash between program input and output
            var simpleTx = new Transaction()
            {
                Signers = tx.Signers,
                Witnesses = tx.Witnesses,
                Attributes = tx.Attributes,
                Script = tx.Script,
                ValidUntilBlock = tx.ValidUntilBlock
            };

            return simpleTx.ToJson(ProtocolSettings.Default);
        }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null) return "Engine faulted.";

            if (exception.InnerException != null)
            {
                return GetExceptionMessage(exception.InnerException);
            }

            return exception.Message;
        }
    }
}

