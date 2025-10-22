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
    private void EmitIf(Action conditionEmitter, Action thenEmitter, Action? elseEmitter = null, bool fallThroughElse = false)
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
        JumpTarget? endTarget = fallThroughElse ? null : new JumpTarget();
        Jump(OpCode.JMPIF, thenTarget);
        elseEmitter();
        if (endTarget is not null)
            Jump(OpCode.JMP, endTarget);
        thenTarget.Instruction = Nop();
        thenEmitter();
        if (endTarget is not null)
            endTarget.Instruction = Nop();
    }

    /// <summary>
    /// Emits a conditional jump that executes <paramref name="conditionEmitter"/> and jumps if it evaluates true.
    /// </summary>
    private void EmitJumpIf(Action conditionEmitter, JumpTarget trueTarget)
    {
        ArgumentNullException.ThrowIfNull(conditionEmitter);
        ArgumentNullException.ThrowIfNull(trueTarget);
        conditionEmitter();
        Jump(OpCode.JMPIF, trueTarget);
    }

    /// <summary>
    /// Emits a conditional jump that executes <paramref name="conditionEmitter"/> and jumps if it evaluates false.
    /// </summary>
    private void EmitJumpIfNot(Action conditionEmitter, JumpTarget falseTarget)
    {
        ArgumentNullException.ThrowIfNull(conditionEmitter);
        ArgumentNullException.ThrowIfNull(falseTarget);
        conditionEmitter();
        Jump(OpCode.JMPIFNOT, falseTarget);
    }

    /// <summary>
    /// Emits a jump that uses the boolean already on the stack.
    /// </summary>
    private void JumpIfTrue(JumpTarget target) => Jump(OpCode.JMPIF, target);

    /// <summary>
    /// Emits a jump that uses the boolean already on the stack.
    /// </summary>
    private void JumpIfFalse(JumpTarget target) => Jump(OpCode.JMPIFNOT, target);

    /// <summary>
    /// Emits an unconditional jump.
    /// </summary>
    private void JumpAlways(JumpTarget target) => Jump(OpCode.JMP, target);

    /// <summary>
    /// Emits a long jump that uses the boolean already on the stack.
    /// </summary>
    private void JumpIfTrueLong(JumpTarget target) => Jump(OpCode.JMPIF_L, target);

    /// <summary>
    /// Emits a long jump that uses the boolean already on the stack.
    /// </summary>
    private void JumpIfFalseLong(JumpTarget target) => Jump(OpCode.JMPIFNOT_L, target);

    /// <summary>
    /// Emits an unconditional long jump.
    /// </summary>
    private void JumpAlwaysLong(JumpTarget target) => Jump(OpCode.JMP_L, target);

    /// <summary>
    /// Emits a while loop with optional loop control callbacks.
    /// The <paramref name="conditionEmitter"/> must push a boolean onto the stack.
    /// </summary>
    private void EmitWhile(Action conditionEmitter, Action<LoopScope> bodyEmitter)
    {
        ArgumentNullException.ThrowIfNull(conditionEmitter);
        ArgumentNullException.ThrowIfNull(bodyEmitter);

        JumpTarget conditionTarget = new();
        JumpTarget endTarget = new();
        conditionTarget.Instruction = Nop();
        conditionEmitter();
        Jump(OpCode.JMPIFNOT, endTarget);
        LoopScope scope = new(this, conditionTarget, endTarget);
        bodyEmitter(scope);
        Jump(OpCode.JMP, conditionTarget);
        endTarget.Instruction = Nop();
    }

    private void EmitWhile(Action conditionEmitter, Action bodyEmitter)
        => EmitWhile(conditionEmitter, _ => bodyEmitter());

    /// <summary>
    /// Emits a for loop (initializer; condition; iterator).
    /// Any null delegate is skipped.
    /// </summary>
    private void EmitFor(Action? initializerEmitter, Action? conditionEmitter, Action? iteratorEmitter, Action<LoopScope> bodyEmitter)
    {
        ArgumentNullException.ThrowIfNull(bodyEmitter);

        initializerEmitter?.Invoke();

        JumpTarget conditionTarget = new();
        JumpTarget iteratorTarget = new();
        JumpTarget endTarget = new();
        conditionTarget.Instruction = Nop();

        if (conditionEmitter is not null)
        {
            conditionEmitter();
            Jump(OpCode.JMPIFNOT, endTarget);
        }

        LoopScope scope = new(this, iteratorTarget, endTarget);
        bodyEmitter(scope);
        iteratorTarget.Instruction = Nop();
        iteratorEmitter?.Invoke();
        Jump(OpCode.JMP, conditionTarget);
        endTarget.Instruction = Nop();
    }

    private void EmitFor(Action? initializerEmitter, Action? conditionEmitter, Action? iteratorEmitter, Action bodyEmitter)
        => EmitFor(initializerEmitter, conditionEmitter, iteratorEmitter, _ => bodyEmitter());

    /// <summary>
    /// Emits a do/while loop (body executes once before condition check).
    /// </summary>
    private void EmitDoWhile(Action<LoopScope> bodyEmitter, Action conditionEmitter)
    {
        ArgumentNullException.ThrowIfNull(bodyEmitter);
        ArgumentNullException.ThrowIfNull(conditionEmitter);

        JumpTarget bodyTarget = new();
        JumpTarget conditionTarget = new();
        JumpTarget endTarget = new();

        bodyTarget.Instruction = Nop();
        LoopScope scope = new(this, conditionTarget, endTarget);
        bodyEmitter(scope);
        conditionTarget.Instruction = Nop();
        conditionEmitter();
        Jump(OpCode.JMPIF, bodyTarget);
        endTarget.Instruction = Nop();
    }

    private void EmitDoWhile(Action bodyEmitter, Action conditionEmitter)
        => EmitDoWhile(_ => bodyEmitter(), conditionEmitter);

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

    private readonly struct LoopScope
    {
        private readonly MethodConvert emitter;
        private readonly JumpTarget continueTarget;
        private readonly JumpTarget breakTarget;

        public LoopScope(MethodConvert emitter, JumpTarget continueTarget, JumpTarget breakTarget)
        {
            this.emitter = emitter;
            this.continueTarget = continueTarget;
            this.breakTarget = breakTarget;
        }

        public void Continue() => emitter.Jump(OpCode.JMP, continueTarget);
        public void Break() => emitter.Jump(OpCode.JMP, breakTarget);
    }
}
