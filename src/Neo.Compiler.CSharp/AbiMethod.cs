using Microsoft.CodeAnalysis;
using Neo.SmartContract;

namespace Neo.Compiler
{
    class AbiMethod : AbiEvent
    {
        public IMethodSymbol Symbol;
        public bool Safe;
        public ContractParameterType ReturnType;
    }
}
