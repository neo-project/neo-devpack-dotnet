using Neo;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Caching;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Linq;

namespace TestingEngine
{
    public static class Helper
    {
        public static JObject ToJson(this TestEngine testEngine)
        {
            var json = new JObject();

            json["vm_state"] = testEngine.State.ToString();
            json["gas_consumed"] = (new BigDecimal(testEngine.GasConsumed, NativeContract.GAS.Decimals)).ToString();
            json["result_stack"] = testEngine.ResultStack.ToJson();
            json["storage"] = testEngine.Snapshot.Storages.ToJson();
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
            json["eventName"] = notification.EventName;
            json["scriptHash"] = notification.ScriptHash.ToString();
            json["value"] = notification.State.ToJson();
            return json;
        }

        public static JObject ToJson(this DataCache<StorageKey, StorageItem> storage)
        {
            var storageMap = new Map();
            foreach (var storagePair in storage.Seek())
            {
                var key = new ByteString(storagePair.Key.Key);
                var value = new ByteString(storagePair.Value.Value);
                storageMap[key] = value;
            }
            return storageMap.ToJson()["value"];
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
