using System;
using System.Numerics;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Extensions;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Deploy.Shared;

/// <summary>
/// Helper class for common script building operations
/// </summary>
public static class ScriptBuilderHelper
{
    /// <summary>
    /// Emit a parameter array onto the evaluation stack
    /// </summary>
    /// <param name="sb">Script builder instance</param>
    /// <param name="parameters">Parameters to emit</param>
    public static void EmitParameterArray(ScriptBuilder sb, params object?[]? parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        if (parameters.Length > 0)
        {
            // Push parameters in reverse order
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                sb.EmitPush(parameters[i]);
            }
            // Push array length and pack into array
            sb.EmitPush(parameters.Length);
            sb.Emit(OpCode.PACK);
        }
        else
        {
            // Empty array
            sb.Emit(OpCode.NEWARRAY0);
        }
    }

    /// <summary>
    /// Build a contract call script
    /// </summary>
    /// <param name="scriptHash">Contract script hash</param>
    /// <param name="method">Method name</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Contract call script</returns>
    public static byte[] BuildContractCallScript(UInt160 scriptHash, string method, params object?[] parameters)
    {
        using var sb = new ScriptBuilder();
        EmitContractCall(sb, scriptHash, method, parameters);
        return sb.ToArray();
    }

    /// <summary>
    /// Build a contract call script with custom call flags
    /// </summary>
    /// <param name="scriptHash">Contract script hash</param>
    /// <param name="method">Method name</param>
    /// <param name="callFlags">Call flags</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Contract call script</returns>
    public static byte[] BuildContractCallScript(UInt160 scriptHash, string method, CallFlags callFlags, params object?[] parameters)
    {
        using var sb = new ScriptBuilder();
        EmitContractCall(sb, scriptHash, method, callFlags, parameters);
        return sb.ToArray();
    }

    /// <summary>
    /// Build a deployment script for a contract
    /// </summary>
    /// <param name="nefBytes">NEF file bytes</param>
    /// <param name="manifestBytes">Manifest bytes</param>
    /// <param name="initializeMethod">Optional initialization method</param>
    /// <param name="initParams">Initialization parameters</param>
    /// <returns>Deployment script</returns>
    public static byte[] BuildDeploymentScript(byte[] nefBytes, byte[] manifestBytes, string? initializeMethod = null, object?[]? initParams = null)
    {
        using var sb = new ScriptBuilder();

        // Build arguments array for ContractManagement.Deploy
        // Order in array: [nef (byte[]), manifest (string), data (object)]

        // First, push the arguments in reverse order (for PACK)

        // 3. Push deployment data (for _deploy method)
        if (initParams != null && initParams.Length > 0)
        {
            // If single parameter, push it directly
            if (initParams.Length == 1)
            {
                sb.EmitPush(initParams[0]);
            }
            else
            {
                // Multiple parameters need to be packed as array
                EmitParameterArray(sb, initParams);
            }
        }
        else
        {
            // No initialization data
            sb.Emit(OpCode.PUSHNULL);
        }

        // 2. Push manifest as string (ContractManagement expects JSON string)
        var manifestJson = System.Text.Encoding.UTF8.GetString(manifestBytes);
        sb.EmitPush(manifestJson);

        // 1. Push NEF bytes
        sb.EmitPush(nefBytes);

        // Create array with 3 elements
        sb.EmitPush(3);
        sb.Emit(OpCode.PACK);

        // Call ContractManagement.Deploy with the array
        sb.EmitPush(CallFlags.All);
        sb.EmitPush("deploy");
        sb.EmitPush(Neo.SmartContract.Native.NativeContract.ContractManagement.Hash);
        sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

        // If initialization method is specified, call it
        if (!string.IsNullOrEmpty(initializeMethod))
        {
            // The deployed contract hash is on the stack
            sb.Emit(OpCode.DUP); // Duplicate the contract hash

            // Push initialization parameters
            if (initParams != null && initParams.Length > 0)
            {
                EmitParameterArray(sb, initParams);
            }
            else
            {
                sb.Emit(OpCode.NEWARRAY0);
            }

            // Call the initialization method
            sb.EmitPush(initializeMethod);
            sb.Emit(OpCode.ROT); // Move contract hash to proper position
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
        }

        return sb.ToArray();
    }


    private static void EmitContractCall(ScriptBuilder sb, UInt160 scriptHash, string method, params object[] parameters)
    {
        EmitContractCall(sb, scriptHash, method, CallFlags.All, parameters);
    }

    private static void EmitContractCall(ScriptBuilder sb, UInt160 scriptHash, string method, CallFlags callFlags, params object[] parameters)
    {
        // Build parameters array
        if (parameters != null && parameters.Length > 0)
        {
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                sb.EmitPush(parameters[i]);
            }
            sb.EmitPush(parameters.Length);
            sb.Emit(OpCode.PACK);
        }
        else
        {
            sb.Emit(OpCode.NEWARRAY0);
        }

        // Call contract method with correct parameter order
        sb.EmitPush((byte)callFlags);
        sb.EmitPush(method);
        sb.EmitPush(scriptHash);
        sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
    }

}
