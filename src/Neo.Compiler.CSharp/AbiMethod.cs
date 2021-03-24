extern alias scfx;

using Microsoft.CodeAnalysis;
using Neo.SmartContract;
using System.Linq;

namespace Neo.Compiler
{
    class AbiMethod : AbiEvent
    {
        public readonly IMethodSymbol Symbol;
        public readonly bool Safe;
        public readonly ContractParameterType ReturnType;

        public AbiMethod(IMethodSymbol symbol)
            : base(symbol.GetDisplayName(true), symbol.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
            Symbol = symbol;
            Safe = symbol.GetAttributes().Any(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.SafeAttribute));
            ReturnType = symbol.ReturnType.GetContractParameterType();
        }
    }
}
