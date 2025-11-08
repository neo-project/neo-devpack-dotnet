using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal abstract class MirInst : MirValue
{
    protected MirInst(MirType type)
        : base(type)
    {
    }

    internal virtual MirEffect Effect => MirEffect.None;
    internal bool ConsumesMemoryToken { get; private set; }
    internal bool ProducesMemoryToken { get; private set; }
    internal MirValue? TokenInput { get; private set; }
    internal MirMemoryTokenValue? TokenOutput { get; private set; }

    protected MirMemoryTokenValue ProduceMemoryToken()
    {
        ProducesMemoryToken = true;
        TokenInput = null;
        return TokenOutput = new MirMemoryTokenValue();
    }

    internal MirMemoryTokenValue AttachMemoryToken(MirValue token)
    {
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
        TokenInput = token ?? throw new ArgumentNullException(nameof(token));
        return TokenOutput = new MirMemoryTokenValue();
    }

    internal void CopyMemoryTokenStateFrom(MirInst other)
    {
        if (other is null)
            return;

        ConsumesMemoryToken = other.ConsumesMemoryToken;
        ProducesMemoryToken = other.ProducesMemoryToken;
        TokenInput = other.TokenInput;
        TokenOutput = other.TokenOutput;
    }

    internal void ResetMemoryTokenState()
    {
        ConsumesMemoryToken = false;
        ProducesMemoryToken = false;
        TokenInput = null;
        TokenOutput = null;
    }
}
