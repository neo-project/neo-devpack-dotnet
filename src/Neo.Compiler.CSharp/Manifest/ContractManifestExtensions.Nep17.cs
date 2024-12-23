extern alias scfx;
using System.Linq;
using Neo.SmartContract.Manifest;
using scfx::Neo.SmartContract.Framework;
using ContractParameterType = Neo.SmartContract.ContractParameterType;

namespace Neo.Compiler;

internal static partial class ContractManifestExtensions
{
    private static System.Collections.Generic.List<CompilationException>
        CheckNep17Compliant(this ContractManifest manifest)
    {
        System.Collections.Generic.List<CompilationException> errors = [];

        var symbolMethod = manifest.Abi.GetMethod("symbol", 0);
        var decimalsMethod = manifest.Abi.GetMethod("decimals", 0);
        var totalSupplyMethod = manifest.Abi.GetMethod("totalSupply", 0);
        var balanceOfMethod = manifest.Abi.GetMethod("balanceOf", 1);
        var transferMethod = manifest.Abi.GetMethod("transfer", 4);

        // Check symbol method
        if (symbolMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: symbol, it is not found in the ABI"));

        if (symbolMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: symbol, it is not safe, you should add a 'Safe' attribute to the symbol method"));

        if (symbolMethod != null && symbolMethod.ReturnType != ContractParameterType.String)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: symbol, it's return type is not a String"));

        // Check decimals method
        if (decimalsMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: decimals, it is not found in the ABI"));

        if (decimalsMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: decimals, it is not safe, you should add a 'Safe' attribute to the decimals method"));

        if (decimalsMethod != null && decimalsMethod.ReturnType != ContractParameterType.Integer)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: decimals, it's return type is not an Integer"));

        // Check totalSupply method
        if (totalSupplyMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: totalSupply, it is not found in the ABI"));

        if (totalSupplyMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: totalSupply, it is not safe, you should add a 'Safe' attribute to the totalSupply method"));

        if (totalSupplyMethod != null && totalSupplyMethod.ReturnType != ContractParameterType.Integer)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: totalSupply, it's return type is not an Integer"));

        // Check balanceOf method
        if (balanceOfMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: balanceOf, it is not found in the ABI"));

        if (balanceOfMethod is { Safe: false })
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: balanceOf, it is not safe, you should add a 'Safe' attribute to the balanceOf method"));

        if (balanceOfMethod != null && balanceOfMethod.ReturnType != ContractParameterType.Integer)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: balanceOf, it's return type is not an Integer"));

        if (balanceOfMethod != null && balanceOfMethod.Parameters.Length != 1)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: balanceOf, it's parameters length is not 1"));

        if (balanceOfMethod != null && balanceOfMethod.Parameters[0].Type != ContractParameterType.Hash160)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: balanceOf, it's parameter type is not a Hash160"));

        // Check transfer method
        if (transferMethod == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it is not found in the ABI"));

        // Note: transfer method should NOT be safe as per NEP-17 standard
        if (transferMethod != null && transferMethod.Safe)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it should not be marked as Safe"));

        if (transferMethod != null && transferMethod.ReturnType != ContractParameterType.Boolean)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it's return type is not a Boolean"));

        if (transferMethod != null && transferMethod.Parameters.Length != 4)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it's parameters length is not 4"));

        if (transferMethod != null && transferMethod.Parameters.Length == 4)
        {
            if (transferMethod.Parameters[0].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it's first parameter (from) type is not a Hash160"));

            if (transferMethod.Parameters[1].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it's second parameter (to) type is not a Hash160"));

            if (transferMethod.Parameters[2].Type != ContractParameterType.Integer)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it's third parameter (amount) type is not an Integer"));

            if (transferMethod.Parameters[3].Type != ContractParameterType.Any)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep17.ToStandard()} implementation: transfer, it's fourth parameter (data) type is not Any"));
        }

        // Check Transfer event
        var transferEvent = manifest.Abi.Events.FirstOrDefault(e =>
            e.Name == "Transfer");

        if (transferEvent == null)
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: Transfer event is not found in the ABI"));
        else
        {
            if (transferEvent.Parameters.Length != 3)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: Transfer event parameters length is not 3"));

            if (transferEvent.Parameters[0].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: Transfer event first parameter (from) type is not Hash160"));

            if (transferEvent.Parameters[1].Type != ContractParameterType.Hash160)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: Transfer event second parameter (to) type is not Hash160"));

            if (transferEvent.Parameters[2].Type != ContractParameterType.Integer)
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete NEP standard {NepStandard.Nep17.ToStandard()} implementation: Transfer event third parameter (amount) type is not Integer"));
        }

        return errors;
    }
}
