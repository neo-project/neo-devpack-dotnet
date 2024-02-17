using Neo.SmartContract.Manifest;
using System;
using System.Linq;

namespace Neo.SmartContract.Testing.Extensions
{
    public static class StandardExtensions
    {
        /// <summary>
        /// Is Nep17 contract
        /// </summary>
        /// <param name="manifest">Manifest</param>
        /// <returns>True if NEP-17</returns>
        public static bool IsNep17(this ContractManifest manifest)
        {
            return manifest.SupportedStandards.Contains("NEP-17");
        }

        /// <summary>
        /// Is Ownable
        /// </summary>
        /// <param name="manifest">Manifest</param>
        /// <returns>True if is Ownable</returns>
        public static bool IsOwnable(this ContractManifest manifest)
        {
            return
                    manifest.Abi.Methods
                        .Any(u => u.Name == "getOwner" && u.Safe && u.Parameters.Length == 0) &&
                    manifest.Abi.Methods
                        .Any(u => u.Name == "setOwner" && !u.Safe && u.Parameters.Length == 1 && u.Parameters[0].Type == ContractParameterType.Hash160) &&
                    manifest.Abi.Events
                        .Any(u => u.Name == "SetOwner" && u.Parameters.Length == 2 &&
                        u.Parameters[0].Type == ContractParameterType.Hash160 &&
                        u.Parameters[1].Type == ContractParameterType.Hash160);
        }

        /// <summary>
        /// Is Verificable
        /// </summary>
        /// <param name="manifest">Manifest</param>
        /// <returns>True if is Verificable</returns>
        public static bool IsVerificable(this ContractManifest manifest)
        {
            return manifest.Abi.Methods.Any(u => u.Name == "verify" && u.Safe && u.Parameters.Length == 0);
        }
    }
}
