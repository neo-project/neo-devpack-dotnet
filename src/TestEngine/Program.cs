using Neo.IO.Json;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.TestingEngine
{
    public class Program
    {
        static int Main(string[] args)
        {
            JObject result = Run(args);
            Console.WriteLine(result);
            if (!result.ContainsProperty("vm_state"))
            {
                return -1;
            }
            return 0;
        }

        public static JObject Run(string[] args)
        {
            if (args.Length == 1)
            {
                // verifies if the parameter is a json string
                try
                {
                    var input = JObject.Parse(args[0]);
                    return RunWithJson(input);
                }
                catch
                {
                    // if the first input is not a json, verifies if the arguments are: nef path, method name, method args
                }

            }

            JObject result;
            if (args.Length >= 2)
            {
                string arg2 = "";
                if (args.Length == 3)
                {
                    arg2 = args[2];
                }
                else if (args.Length > 3)
                {
                    arg2 = $"[{string.Join(",", args.Skip(2))}]";
                }
                result = RunWithMethodName(args[0], args[1], arg2);
            }
            else
            {
                result = BuildJsonException(
                    "One or more arguments are missing\n" +
                    "Expected arguments: <nef path> <method name> <method arguments as json>"
                );
            }

            return result;
        }

        /// <summary>
        /// Runs a nef script given a method name and its arguments
        /// </summary>
        /// <param name="path">Absolute path of the script</param>
        /// <param name="methodName">The name of the targeted method</param>
        /// <param name="jsonParams">Json string representing the arguments of the method</param>
        /// <returns>Returns a json with the engine state after executing the script</returns>
        public static JObject RunWithMethodName(string path, string methodName, string jsonParams)
        {
            try
            {
                JArray parameters = new JArray();
                if (jsonParams.Length > 0)
                {
                    var json = JObject.Parse(jsonParams);
                    if (json is JArray array)
                    {
                        parameters = array;
                    }
                    else
                    {
                        parameters.Insert(0, json);
                    }
                }

                return Run(path, methodName, parameters);
            }
            catch (Exception e)
            {
                return BuildJsonException(e.Message);
            }
        }

        /// <summary>
        /// Runs a nef script given a json with test engine fake values for testing.
        /// </summary>
        /// <param name="json">json object with fake values and execution arguments</param>
        /// <returns>Returns a json with the engine state after executing the script</returns>
        public static JObject RunWithJson(JObject json)
        {
            var missigFieldMessage = "Missing field: '{0}'";
            if (!json.ContainsProperty("path"))
            {
                return BuildJsonException(string.Format(missigFieldMessage, "path"));
            }

            if (!json.ContainsProperty("method"))
            {
                return BuildJsonException(string.Format(missigFieldMessage, "method"));
            }

            if (!json.ContainsProperty("arguments"))
            {
                json["arguments"] = new JArray();
            }

            try
            {
                var path = json["path"].AsString();
                var methodName = json["method"].AsString();
                var parameters = (JArray)json["arguments"];

                return Run(path, methodName, parameters);
            }
            catch (Exception e)
            {
                return BuildJsonException(e.Message);
            }
        }

        /// <summary>
        /// Runs the given method from a nef script
        /// </summary>
        /// <param name="path">Absolute path of the script</param>
        /// <param name="method">The name of the targeted method</param>
        /// <param name="parameters">Arguments of the method</param>
        /// <returns>Returns a json with the engine state after executing the script</returns>
        public static JObject Run(string path, string method, JArray parameters)
        {
            if (!File.Exists(path))
            {
                return BuildJsonException("File doesn't exists");
            }
            if (Path.GetExtension(path).ToLowerInvariant() != ".nef")
            {
                return BuildJsonException("Invalid file. A .nef file required.");
            }

            try
            {
                Engine.Instance.SetTestEngine(path);
                var stackParams = GetStackItemParameters(parameters);
                return Engine.Instance.Run(method, stackParams);
            }
            catch (Exception e)
            {
                return BuildJsonException(e.Message);
            }
        }

        /// <summary>
        /// Converts the data in a json array to an array of StackItem
        /// </summary>
        /// <param name="parameters">json array to be converted</param>
        /// <returns>Returns the built StackItem array</returns>
        private static StackItem[] GetStackItemParameters(JArray parameters)
        {
            var items = new List<StackItem>();
            foreach (JObject param in parameters)
            {
                var success = false;
                if (param.ContainsProperty("value") && param.ContainsProperty("type"))
                {
                    try
                    {
                        items.Add(ContractParameter.FromJson(param).ToStackItem());
                        success = true;
                    }
                    catch (Exception e)
                    {
                        // if something went wrong while reading the json, log the error
                        Console.WriteLine(e);
                    }
                }

                if (!success)
                {
                    // if something went wrong while reading the json, inserts null in this argument position
                    items.Add(StackItem.Null);
                }
            }
            return items.ToArray();
        }

        private static JObject BuildJsonException(string message)
        {
            var json = new JObject();
            json["error"] = message;
            return json;
        }
    }
}
