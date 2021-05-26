using Neo.IO.Caching;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Linq;
using Neo.IO;
using System.Collections.Generic;
using Neo.Persistence;

namespace Neo.TestingEngine
{
    public static class Helper
    {
        public static JObject ToJson(this TestEngine testEngine)
        {
            var json = new JObject();

            json["vm_state"] = testEngine.State.ToString();
            json["gasconsumed"] = (new BigDecimal((decimal)testEngine.GasConsumed, NativeContract.GAS.Decimals)).ToString();
            json["resultstack"] = testEngine.ResultStack.ToJson();

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
            return new JArray(stack.Select(p => p.ToJson()));
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

            JObject json = new JObject();
            json["hash"] = simpleTx.Hash.ToString();
            json["size"] = simpleTx.Size;
            json["signers"] = simpleTx.Signers.Select(p => p.ToJson()).ToArray();
            json["attributes"] = simpleTx.Attributes.Select(p => p.ToJson()).ToArray();
            json["script"] = Convert.ToBase64String(simpleTx.Script);
            json["witnesses"] = simpleTx.Witnesses.Select(p => p.ToJson()).ToArray();
            return json;
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

