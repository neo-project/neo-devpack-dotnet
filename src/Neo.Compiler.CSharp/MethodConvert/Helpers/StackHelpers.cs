// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Numerics;
using scfx::Neo.SmartContract.Framework;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    #region Instructions

    private SequencePointInserter InsertSequencePoint(SyntaxNodeOrToken? syntax)
    {
        return new SequencePointInserter(Instructions, syntax);
    }

    private SequencePointInserter InsertSequencePoint(SyntaxReference? syntax)
    {
        return new SequencePointInserter(Instructions, syntax);
    }

    private SequencePointInserter InsertSequencePoint(Location? location)
    {
        return new SequencePointInserter(Instructions, location);
    }

    #endregion


    #region LabelsAndTargets

    private JumpTarget AddLabel(ILabelSymbol symbol, bool checkTryStack)
    {
        if (!_labels.TryGetValue(symbol, out JumpTarget? target))
        {
            target = new JumpTarget();
            _labels.Add(symbol, target);
        }
        if (checkTryStack && _tryStack.TryPeek(out ExceptionHandling? result) && result.State != ExceptionHandlingState.Finally)
        {
            result.Labels.Add(symbol);
        }
        return target;
    }

    #endregion

    private void PushSwitchLabels((SwitchLabelSyntax, JumpTarget)[] labels)
    {
        _switchStack.Push(labels);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.SwitchCount++;
    }

    private void PopSwitchLabels()
    {
        _switchStack.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.SwitchCount--;
    }

    private void PushContinueTarget(JumpTarget target)
    {
        _continueTargets.Push(target);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.ContinueTargetCount++;
    }

    private void PopContinueTarget()
    {
        _continueTargets.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.ContinueTargetCount--;
    }

    private void PushBreakTarget(JumpTarget target)
    {
        _breakTargets.Push(target);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.BreakTargetCount++;
    }

    private void PopBreakTarget()
    {
        _breakTargets.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.BreakTargetCount--;
    }

    /// <summary>
    /// Convert a throw expression or throw statement to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the Throw.</param>
    /// <param name="exception">The content of exception</param>
    /// <exception cref="CompilationException">Only a single parameter is supported for exceptions.</exception>
    /// <example>
    /// throw statement:
    /// <code>
    /// if (shapeAmount <= 0)
    /// {
    ///     throw new Exception("Amount of shapes must be positive.");
    /// }
    ///</code>
    /// throw expression:
    /// <code>
    /// string a = null;
    /// var b = a ?? throw new Exception();
    /// </code>
    /// <code>
    /// var first = args.Length >= 1 ? args[0] : throw new Exception();
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-throw-expression">The throw expression</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-catch-statement">Exception-handling statements - throw</seealso>
    private void Throw(SemanticModel model, ExpressionSyntax? exception)
    {
        if (exception is not null)
        {
            var type = model.GetTypeInfo(exception).Type!;
            if (type.IsSubclassOf(nameof(UncatchableException), includeThisClass: true))
            {
                _instructionsBuilder.Abort();
                return;
            }
        }
        switch (exception)
        {
            case ObjectCreationExpressionSyntax expression:
                switch (expression.ArgumentList?.Arguments.Count)
                {
                    case null:
                    case 0:
                        _instructionsBuilder.Push("exception");
                        break;
                    case 1:
                        ConvertExpression(model, expression.ArgumentList.Arguments[0].Expression);
                        break;
                    default:
                        throw new CompilationException(expression, DiagnosticId.MultiplyThrows, "Only a single parameter is supported for exceptions.");
                }
                break;
            case null:
                _instructionsBuilder.LdLoc(_exceptionStack.Peek());
                break;
            default:
                ConvertExpression(model, exception);
                break;
        }
        _instructionsBuilder.Throw();
    }
}
