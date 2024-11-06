extern alias scfx;
using System.Linq;
using Neo.SmartContract.Manifest;
using scfx::Neo.SmartContract.Framework;
using ContractParameterType = Neo.SmartContract.ContractParameterType;

namespace Neo.Compiler;

internal static partial class ContractManifestExtensions
{
    private static System.Collections.Generic.List<CompilationException>
        CheckNep24Compliant(this ContractManifest manifest)
    {
        System.Collections.Generic.List<CompilationException> errors = [];

        var royaltyInfoMethod = manifest.Abi.GetMethod("royaltyInfo", 3);

        // Check if method exists
        if (royaltyInfoMethod == null)
        {
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it is not found in the ABI"));
            return errors;
        }

        // Check if method is marked as Safe
        if (royaltyInfoMethod is { Safe: false })
        {
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it is not safe, you should add a 'Safe' attribute to the royaltyInfo method"));
        }

        // Check return type
        if (royaltyInfoMethod.ReturnType != ContractParameterType.Array)
        {
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it's return type is not an Array"));
        }

        // Check parameters length
        if (royaltyInfoMethod.Parameters.Length != 3)
        {
            errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it's parameters length is not 3"));
        }

        // Check individual parameter types if parameters exist
        if (royaltyInfoMethod.Parameters.Length == 3)
        {
            // Check first parameter (tokenId)
            if (royaltyInfoMethod.Parameters[0].Type != ContractParameterType.ByteArray)
            {
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it's first parameter (tokenId) type is not a ByteArray"));
            }

            // Check second parameter (buyer)
            if (royaltyInfoMethod.Parameters[1].Type != ContractParameterType.Hash160)
            {
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it's second parameter (buyer) type is not a Hash160"));
            }

            // Check third parameter (price)
            if (royaltyInfoMethod.Parameters[2].Type != ContractParameterType.Integer)
            {
                errors.Add(new CompilationException(DiagnosticId.IncorrectNEPStandard,
                    $"Incomplete or unsafe NEP standard {NepStandard.Nep24.ToStandard()} implementation: royaltyInfo, it's third parameter (price) type is not an Integer"));
            }
        }

        return errors;
    }
}
