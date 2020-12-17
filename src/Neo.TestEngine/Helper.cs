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
using Neo.Cryptography.ECC;
using Neo.IO;
using System.Collections.Generic;
using System.Numerics;
using Array = Neo.VM.Types.Array;
using Boolean = Neo.VM.Types.Boolean;
using Buffer = Neo.VM.Types.Buffer;

namespace Neo.TestingEngine
{
    public static class Helper
    {
        public static JObject ToJson(this TestEngine testEngine)
        {
            var json = new JObject();

            json["vm_state"] = testEngine.State.ToString();
            json["gasconsumed"] = (new BigDecimal(testEngine.GasConsumed, NativeContract.GAS.Decimals)).ToString();
            json["result_stack"] = testEngine.ResultStack.ToJson();

            if (testEngine.ScriptContainer is Transaction tx)
            {
                json["transaction"] = tx.ToSimpleJson();
            }

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
            var jsonStorage = new JArray();
            foreach (var storagePair in storage.Seek())
            {
                var key = new ByteString(storagePair.Key.Key);
                var value = new ByteString(storagePair.Value.Value);

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
            if (parameter.Value is null)
                sb.Emit(OpCode.PUSHNULL);
            else
                switch (parameter.Type)
                {
                    case ContractParameterType.Signature:
                    case ContractParameterType.ByteArray:
                        sb.EmitPush((byte[])parameter.Value);
                        break;
                    case ContractParameterType.Boolean:
                        sb.EmitPush((bool)parameter.Value);
                        break;
                    case ContractParameterType.Integer:
                        if (parameter.Value is BigInteger bi)
                            sb.EmitPush(bi);
                        else
                            sb.EmitPush((BigInteger)typeof(BigInteger).GetConstructor(new[] { parameter.Value.GetType() }).Invoke(new[] { parameter.Value }));
                        break;
                    case ContractParameterType.Hash160:
                        sb.EmitPush((UInt160)parameter.Value);
                        break;
                    case ContractParameterType.Hash256:
                        sb.EmitPush((UInt256)parameter.Value);
                        break;
                    case ContractParameterType.PublicKey:
                        sb.EmitPush((ECPoint)parameter.Value);
                        break;
                    case ContractParameterType.String:
                        sb.EmitPush((string)parameter.Value);
                        break;
                    case ContractParameterType.Array:
                        {
                            IList<ContractParameter> parameters = (IList<ContractParameter>)parameter.Value;
                            for (int i = parameters.Count - 1; i >= 0; i--)
                                sb.EmitPush(parameters[i]);
                            sb.EmitPush(parameters.Count);
                            sb.Emit(OpCode.PACK);
                        }
                        break;
                    case ContractParameterType.Map:
                        {
                            IList<KeyValuePair<ContractParameter, ContractParameter>> parameters = (IList<KeyValuePair<ContractParameter, ContractParameter>>)parameter.Value;
                            sb.Emit(OpCode.NEWMAP);
                            foreach (var (key, value) in parameters)
                            {
                                sb.Emit(OpCode.DUP);
                                sb.EmitPush(key);
                                sb.EmitPush(value);
                                sb.Emit(OpCode.APPEND);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentException();
                }
            return sb;
        }
    }
}

