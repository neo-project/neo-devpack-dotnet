using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.VM;

namespace Neo.Compiler;

/// <summary>
/// Provides higher-level helpers for emitting control-flow opcodes so system methods
/// can be composed without manually wiring jump targets.
/// Typical usage wraps small lambdas that emit the condition or body opcodes, e.g.
/// <code>
/// EmitIf(() => { Dup(); Within(byte.MinValue, byte.MaxValue); Not(); }, () => Throw());
/// EmitWhile(() => { Dup(); Push0(); NumGreaterThan(); }, () => { /* loop body */ });
/// </code>
/// </summary>
internal partial class MethodConvert
{
    /// <summary>
    /// Emits an if/else construct.
    /// The <paramref name="conditionEmitter"/> must push a boolean onto the evaluation stack.
    /// </summary>
    private void EmitIf(Action conditionEmitter, Action thenEmitter, Action? elseEmitter = null)
    {
        ArgumentNullException.ThrowIfNull(conditionEmitter);
        ArgumentNullException.ThrowIfNull(thenEmitter);

        conditionEmitter();

        if (elseEmitter is null)
        {
            JumpTarget noElseTarget = new();
            Jump(OpCode.JMPIFNOT, noElseTarget);
            thenEmitter();
            noElseTarget.Instruction = Nop();
            return;
        }

        JumpTarget thenTarget = new();
        JumpTarget endTarget = new();
        Jump(OpCode.JMPIF, thenTarget);
        elseEmitter();
        Jump(OpCode.JMP, endTarget);
        thenTarget.Instruction = Nop();
        thenEmitter();
        endTarget.Instruction = Nop();
    }

    /// <summary>
    /// Emits a while loop.
    /// The <paramref name="conditionEmitter"/> must push a boolean onto the stack.
    /// </summary>
    private void EmitWhile(Action conditionEmitter, Action bodyEmitter)
    {
        ArgumentNullException.ThrowIfNull(conditionEmitter);
        ArgumentNullException.ThrowIfNull(bodyEmitter);

        JumpTarget conditionTarget = new();
        JumpTarget endTarget = new();
        conditionTarget.Instruction = Nop();
        conditionEmitter();
        Jump(OpCode.JMPIFNOT, endTarget);
        bodyEmitter();
        Jump(OpCode.JMP, conditionTarget);
        endTarget.Instruction = Nop();
    }

    /// <summary>
    /// Emits a for loop (initializer; condition; iterator).
    /// Any null delegate is skipped.
    /// </summary>
    private void EmitFor(Action? initializerEmitter, Action? conditionEmitter, Action? iteratorEmitter, Action bodyEmitter)
    {
        ArgumentNullException.ThrowIfNull(bodyEmitter);

        initializerEmitter?.Invoke();

        JumpTarget conditionTarget = new();
        JumpTarget endTarget = new();
        conditionTarget.Instruction = Nop();

        if (conditionEmitter is not null)
        {
            conditionEmitter();
            Jump(OpCode.JMPIFNOT, endTarget);
        }

        bodyEmitter();
        iteratorEmitter?.Invoke();
        Jump(OpCode.JMP, conditionTarget);
        endTarget.Instruction = Nop();
    }

    /// <summary>
    /// Emits a switch statement. The <paramref name="valueEmitter"/> must push the discriminant value.
    /// Each case body is executed after the switch value has been dropped.
    /// </summary>
    private void EmitSwitch(Action valueEmitter, IReadOnlyList<(BigInteger value, Action body)> cases, Action? defaultBody = null)
    {
        ArgumentNullException.ThrowIfNull(valueEmitter);
        ArgumentNullException.ThrowIfNull(cases);

        valueEmitter();
        if (cases.Count == 0 && defaultBody is null)
        {
            Drop();
            return;
        }

        JumpTarget endTarget = new();
        JumpTarget defaultTarget = new();
        JumpTarget[] caseTargets = new JumpTarget[cases.Count];

        for (int i = 0; i < cases.Count; i++)
        {
            caseTargets[i] = new JumpTarget();
            Dup();
            Push(cases[i].value);
            NumEqual();
            Jump(OpCode.JMPIF, caseTargets[i]);
        }

        Jump(OpCode.JMP, defaultTarget);

        for (int i = 0; i < cases.Count; i++)
        {
            caseTargets[i].Instruction = Drop();
            cases[i].body();
            Jump(OpCode.JMP, endTarget);
        }

        defaultTarget.Instruction = Drop();
        defaultBody?.Invoke();
        endTarget.Instruction = Nop();
    }
}
