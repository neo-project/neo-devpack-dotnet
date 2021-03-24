using Microsoft.CodeAnalysis;
using Neo.SmartContract.Manifest;
using System.Linq;

namespace Neo.Compiler
{
    class AbiEvent
    {
        public readonly string Name;
        public readonly ContractParameterDefinition[] Parameters;

        protected AbiEvent(string name, ContractParameterDefinition[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public AbiEvent(IEventSymbol symbol)
            : this(symbol.GetDisplayName(), ((INamedTypeSymbol)symbol.Type).DelegateInvokeMethod!.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
        }
    }
}
