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
            json["result_stack"] = testEngine.ResultStack.ToJson();

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
            json["eventName"] = notification.EventName;
            json["scripthash"] = notification.ScriptHash.ToString();
            json["value"] = notification.State.ToJson();
            return json;
        }

        public static JObject ToJson(this DataCache storage)
        {
            var jsonStorage = new JArray();
            foreach (var storagePair in storage.Seek())
            {
                var key = new ByteString(storagePair.Key.Key);
                StackItem value;
                try
                {
                    value = new ByteString(storagePair.Value.Value);
                }
                catch
                {
                    value = StackItem.Null;
                }


                var jsonKey = new JObject();
                jsonKey["id"] = storagePair.Key.Id;
                jsonKey["key"] = key.ToJson();

                var jsonValue = new JObject();
                jsonValue["isconstant"] = storagePair.Value.IsConstant;
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
            JObject json = new JObject();
            json["hash"] = tx.Hash.ToString();
            json["size"] = tx.Size;
            json["signers"] = tx.Signers.Select(p => p.ToJson()).ToArray();
            json["attributes"] = tx.Attributes.Select(p => p.ToJson()).ToArray();
            json["script"] = Convert.ToBase64String(tx.Script);
            json["witnesses"] = tx.Witnesses.Select(p => p.ToJson()).ToArray();
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

        public static ScriptBuilder EmitAppCall(this ScriptBuilder sb, UInt160 scriptHash, string operation, params ContractParameter[] args)
        {
            sb.EmitPush(CallFlags.All);
            for (int i = args.Length - 1; i >= 0; i--)
                sb.EmitPush(args[i]);
            sb.EmitPush(args.Length);
            sb.Emit(OpCode.PACK);
            sb.EmitPush(operation);
            sb.EmitPush(scriptHash);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
            return sb;
        }

        public static ScriptBuilder EmitPush(this ScriptBuilder sb, ContractParameter parameter)
        {
            try
            {
                return VM.Helper.EmitPush(sb, parameter);
            }
            catch (ArgumentException)
            {
                if (parameter.Type == ContractParameterType.Map)
                {
                    var parameters = (IList<KeyValuePair<ContractParameter, ContractParameter>>)parameter.Value;
                    sb.Emit(OpCode.NEWMAP);
                    foreach (var p in parameters)
                    {
                        sb.Emit(OpCode.DUP);
                        sb.EmitPush(p.Key);
                        sb.EmitPush(p.Value);
                        sb.Emit(OpCode.APPEND);
                    }
                    return sb;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

