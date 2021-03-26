using Microsoft.CodeAnalysis;
using Neo.SmartContract.Manifest;
using System.Linq;

namespace Neo.Compiler
{
    class AbiEvent
    {
        public readonly string Name;
        public readonly ContractParameterDefinition[] Parameters;

        public virtual ISymbol Symbol { get; }

        protected AbiEvent(ISymbol symbol, string name, ContractParameterDefinition[] parameters)
        {
            Symbol = symbol;
            Name = name;
            Parameters = parameters;
        }

        public AbiEvent(IEventSymbol symbol)
            : this(symbol, symbol.GetDisplayName(), ((INamedTypeSymbol)symbol.Type).DelegateInvokeMethod!.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
        }
    }
}
