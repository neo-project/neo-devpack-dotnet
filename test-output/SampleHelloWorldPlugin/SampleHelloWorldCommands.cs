// CLI commands for SampleHelloWorld

using System;
using System.Linq;
using System.Numerics;
using Neo;
using Neo.ConsoleService;
using Neo.Cryptography.ECC;
using Neo.SmartContract;
using Neo.VM.Types;

namespace Neo.Plugins.SampleHelloWorldPlugin
{
    public class SampleHelloWorldCommands
    {
        private readonly SampleHelloWorldWrapper _wrapper;

        public SampleHelloWorldCommands(SampleHelloWorldWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        public void Handle(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                string command = args[0].ToLower();
                string[] parameters = args.Skip(1).ToArray();

                switch (command)
                {
                    case "sayhello":
                        HandlesayHello(parameters);
                        break;
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        break;
                    default:
                        ConsoleHelper.Warning($"Unknown command: {command}");
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Error executing command: {ex.Message}");
            }
        }

        private async void HandlesayHello(string[] parameters)
        {
            // sayHello: Safe method
            try
            {
                var result = await _wrapper.sayHelloAsync();
                DisplayResult(result);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"Error calling sayHello: {ex.Message}");
            }
        }

        private void ShowHelp()
        {
            ConsoleHelper.Info("SampleHelloWorld Contract Commands:");
            ConsoleHelper.Info("");
            ConsoleHelper.Info("  sayhello  [SAFE]");
            ConsoleHelper.Info("");
            ConsoleHelper.Info("Contract Hash: 0xef061fe2c2f02e63f00159e99dfd90cbc54ae0d2");
        }

        private object ParseParameter(string value, ContractParameterType type)
        {
            try
            {
                return type switch
                {
                    ContractParameterType.Boolean => bool.Parse(value),
                    ContractParameterType.Integer => BigInteger.Parse(value),
                    ContractParameterType.String => value,
                    ContractParameterType.ByteArray => Convert.FromBase64String(value),
                    ContractParameterType.Hash160 => UInt160.Parse(value),
                    ContractParameterType.Hash256 => UInt256.Parse(value),
                    ContractParameterType.PublicKey => ECPoint.Parse(value, ECCurve.Secp256r1),
                    _ => value
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid parameter value '{value}' for type {type}: {ex.Message}");
            }
        }

        private void DisplayResult(object result)
        {
            if (result == null)
            {
                ConsoleHelper.Info("Result: null");
                return;
            }

            string output = result switch
            {
                BigInteger bi => bi.ToString(),
                byte[] bytes => Convert.ToBase64String(bytes),
                UInt160 hash160 => hash160.ToString(),
                UInt256 hash256 => hash256.ToString(),
                ECPoint point => point.ToString(),
                bool b => b.ToString(),
                string s => s,
                _ => result.ToString()
            };

            ConsoleHelper.Info($"Result: {output}");
        }
    }
}
