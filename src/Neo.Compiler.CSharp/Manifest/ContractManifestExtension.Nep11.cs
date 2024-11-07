extern alias scfx;
using System.Linq;
using Neo.SmartContract.Manifest;
using scfx::Neo.SmartContract.Framework;
using ContractParameterType = Neo.SmartContract.ContractParameterType;

namespace Neo.Compiler;

internal static partial class ContractManifestExtensions
{
    private static System.Collections.Generic.List<CompilationException>
        CheckNep11Compliant(this ContractManifest manifest)
    {
        System.Collections.Generic.List<CompilationException> errors = [];

        var symbolMethod = manifest.Abi.GetMethod("symbol", 0);
        var decimalsMethod = manifest.Abi.GetMethod("decimals", 0);
        var totalSupplyMethod = manifest.Abi.GetMethod("totalSupply", 0);
        var balanceOfMethod1 = manifest.Abi.GetMethod("balanceOf", 1);
        var balanceOfMethod2 = manifest.Abi.GetMethod("balanceOf", 2);
        var tokensOfMethod = manifest.Abi.GetMethod("tokensOf", 1);
        var ownerOfMethod = manifest.Abi.GetMethod("ownerOf", 1);
        var transferMethod1 = manifest.Abi.GetMethod("transfer", 3);
        var transferMethod2 = manifest.Abi.GetMethod("transfer", 5);

        var symbolValid = symbolMethod != null && symbolMethod.Safe &&
                            symbolMethod.ReturnType == ContractParameterType.String;

        if (symbolMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: symbol, it is not found in the ABI"));

        if (symbolMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: symbol, it is not safe, you should add a 'Safe' attribute to the symbol method"));

        if (symbolMethod != null && symbolMethod.ReturnType != ContractParameterType.String)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: symbol, it's return type is not a string"));

        if (decimalsMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: decimals, it is not found in the ABI"));

        if (decimalsMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: decimals, it is not safe, you should add a 'Safe' attribute to the decimals method"));

        if (decimalsMethod != null && decimalsMethod.ReturnType != ContractParameterType.Integer)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: decimals, it's return type is not an integer"));


        if (totalSupplyMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: totalSupply, it is not found in the ABI"));

        if (totalSupplyMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: totalSupply, it is not safe, you should add a 'Safe' attribute to the totalSupply method"));

        if (totalSupplyMethod != null && totalSupplyMethod.ReturnType != ContractParameterType.Integer)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: totalSupply, it's return type is not an integer"));

        if (balanceOfMethod1 == null && balanceOfMethod2 == null)
        {
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it is not found in the ABI"));
        }

        if (balanceOfMethod1 != null)
        {
            if (balanceOfMethod1 is { Safe: false })
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it is not safe, you should add a 'Safe' attribute to the balanceOf method"));

            if (balanceOfMethod1 != null && balanceOfMethod1.ReturnType != ContractParameterType.Integer)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's return type is not an integer"));

            if (balanceOfMethod1 != null && balanceOfMethod1.Parameters.Length != 1)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's parameters length is not 1"));

            if (balanceOfMethod1 != null && balanceOfMethod1.Parameters[0].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's parameters type is not a Hash160"));
        }

        if (balanceOfMethod2 != null)
        {
            if (balanceOfMethod2 is { Safe: false })
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it is not safe, you should add a 'Safe' attribute to the balanceOf method"));

            if (balanceOfMethod2 != null && balanceOfMethod2.ReturnType != ContractParameterType.Integer)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's return type is not an integer"));

            if (balanceOfMethod2 != null && balanceOfMethod2.Parameters.Length != 2)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's parameters length is not 2"));

            if (balanceOfMethod2 != null && balanceOfMethod2.Parameters[0].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's first parameters type is not a Hash160"));

            if (balanceOfMethod2 != null && balanceOfMethod2.Parameters[1].Type != ContractParameterType.ByteArray)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it's second parameters type is not a ByteArray"));

        }
        // errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
        // $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: balanceOf, it is not found in the ABI"));

        if (tokensOfMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: tokensOf, it is not found in the ABI"));

        if (tokensOfMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: tokensOf, it is not safe, you should add a 'Safe' attribute to the tokensOf method"));

        if (tokensOfMethod != null && tokensOfMethod.ReturnType != ContractParameterType.InteropInterface)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: tokensOf, it's return type is not an InteropInterface"));

        if (tokensOfMethod != null && tokensOfMethod.Parameters.Length != 1)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: tokensOf, it's parameters length is not 1"));

        if (tokensOfMethod != null && tokensOfMethod.Parameters[0].Type != ContractParameterType.Hash160)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: tokensOf, it's parameters type is not a Hash160"));

        if (ownerOfMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it is not found in the ABI"));

        if (ownerOfMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it is not safe, you should add a 'Safe' attribute to the ownerOf method"));

        if (ownerOfMethod != null && ownerOfMethod.ReturnType != ContractParameterType.Hash160)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it's return type is not a Hash160"));

        if (ownerOfMethod != null && ownerOfMethod.Parameters.Length != 1)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it's parameters length is not 1"));

        if (ownerOfMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it is not found in the ABI"));

        if (ownerOfMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it is not safe, you should add a 'Safe' attribute to the ownerOf method"));

        if (ownerOfMethod != null && ownerOfMethod.ReturnType != ContractParameterType.Hash160)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it's return type is not an InteropInterface"));

        if (ownerOfMethod != null && ownerOfMethod.Parameters.Length != 1)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it's parameters length is not 1"));

        if (ownerOfMethod != null && ownerOfMethod.Parameters[0].Type != ContractParameterType.ByteArray)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: ownerOf, it's parameters type is not a ByteArray"));


        if (transferMethod1 == null && transferMethod2 == null)
        {
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it is not found in the ABI"));
        }

        if (transferMethod1 != null)
        {
            if (transferMethod1 is { Safe: false } &&
           transferMethod1.ReturnType != ContractParameterType.Boolean)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it is not safe, you should add a 'Safe' attribute to the transfer method"));

            if (transferMethod1 != null && transferMethod1.ReturnType != ContractParameterType.Boolean)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's return type is not a Boolean"));

            if (transferMethod1 != null && transferMethod1.Parameters.Length != 3)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's parameters length is not 3"));

            if (transferMethod1 != null && transferMethod1.Parameters[0].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's first parameters type is not a Hash160"));

            if (transferMethod1 != null && transferMethod1.Parameters[1].Type != ContractParameterType.ByteArray)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's second parameters type is not a ByteArray"));

            if (transferMethod1 != null && transferMethod1.Parameters[2].Type != ContractParameterType.Any)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's third parameters type is not a Any"));
        }

        if (transferMethod2 != null)
        {
            if (transferMethod2 is { Safe: false } &&
          transferMethod2.ReturnType != ContractParameterType.Boolean)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it is not safe, you should add a 'Safe' attribute to the transfer method"));

            if (transferMethod2 != null && transferMethod2.ReturnType != ContractParameterType.Boolean)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's return type is not a Boolean"));

            if (transferMethod2 != null && transferMethod2.Parameters.Length != 5)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's parameters length is not 5"));

            if (transferMethod2 != null && transferMethod2.Parameters[0].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's first parameters type is not a Hash160"));

            if (transferMethod2 != null && transferMethod2.Parameters[1].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's second parameters type is not a Hash160"));

            if (transferMethod2 != null && transferMethod2.Parameters[2].Type != ContractParameterType.Integer)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's third parameters type is not an Integer"));

            if (transferMethod2 != null && transferMethod2.Parameters[3].Type != ContractParameterType.ByteArray)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's fourth parameters type is not a ByteArray"));

            if (transferMethod2 != null && transferMethod2.Parameters[4].Type != ContractParameterType.Any)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's fifth parameters type is not a Any"));

        }
        // errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
        // $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it is not found in the ABI"));

        // Check Transfer event
        var transferEvent = manifest.Abi.Events.FirstOrDefault(a =>
            a.Name == "Transfer");

        if (transferEvent == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete NEP standard {NepStandard.Nep11.ToStandard()} implementation: Transfer event is not found in the ABI"));

        if (transferEvent != null && transferEvent.Parameters.Length != 4)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's parameters length is not 4"));

        if (transferEvent != null && transferEvent.Parameters[0].Type != ContractParameterType.Hash160)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's first parameters type is not a Hash160"));

        if (transferEvent != null && transferEvent.Parameters[1].Type != ContractParameterType.Hash160)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's second parameters type is not a Hash160"));

        if (transferEvent != null && transferEvent.Parameters[2].Type != ContractParameterType.Integer)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's third parameters type is not an Integer"));

        if (transferEvent != null && transferEvent.Parameters[3].Type != ContractParameterType.ByteArray)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
            $"Incomplete or unsafe NEP standard {NepStandard.Nep11.ToStandard()} implementation: transfer, it's fourth parameters type is not a ByteArray"));

        return errors;
    }
}
