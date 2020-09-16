using Neo.Compiler;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestEngineExe
{
    class TestEngineExe
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ready!");
            TestEngine testEngine = new TestEngine();

            if (args.Length < 1)
            {
                Console.WriteLine("Missing required args: path and methodName.");
                return;
                //take off then and throw an exception
                //is it possible to return value? Not being void
            }
            string path = args[0];
            if (!File.Exists(path))
            {
                Console.WriteLine("File doesn't exists");
                return;
            }
            if (Path.GetExtension(path).ToLowerInvariant() != ".nef")
            {
                Console.WriteLine("Incorect file. Nef file required.");
                return;
            }

            if (args.Length < 2)
            {
                Console.WriteLine("Missing required args: methodName.");
                return;
            }
            string methodName = args[1];
            var parameters = ConvertParameters(args.Skip(2).ToArray());

            testEngine.AddEntryScript(path);

            var paramCount = GetMethodParametersCount(methodName, testEngine);
            if (paramCount == -1)
            {
                Console.WriteLine("Method doesn't exist.");
                return;
            }
            if (paramCount != parameters.Length)
            {
                Console.WriteLine("Incorrect number of parameters.");
                return;
            }
            var result = testEngine.GetMethod(methodName).Run(parameters);
            Console.WriteLine("Result: " + result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodname"></param>
        /// <param name="testEngine"></param>
        /// <returns>
        /// Returns the number of parameters in the method if the method exists, otherwise it returns -1.
        /// </returns>
        public static int GetMethodParametersCount(string methodname, TestEngine testEngine)
        {
            if (testEngine.ScriptEntry is null) return -1;
            var methods = testEngine.ScriptEntry.finalABI["methods"] as JArray;
            foreach (JObject method in methods)
            {
                if (method["name"].ToString() == methodname) //method exists
                    return (method["parameters"] as JArray).Count;
            }

            return -1;
        }

        static StackItem ToStackItem(string arg)
        {
            if (int.TryParse(arg, out int intParam))
            {
                return intParam;
            }
            else if (bool.TryParse(arg, out bool boolParam))
            {
                return boolParam;
            }
            else
            {
                return arg;
            }
        }

        static StackItem[] ConvertParameters(string[] args)
        {
            List<StackItem> stackParams = new List<StackItem>();

            foreach (string parameter in args)
            {
                stackParams.Add(ToStackItem(parameter));
            }
            return stackParams.ToArray();
        }
    }
}
