using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Neo;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Neo.Assertions
{
    public class NotifyEventArgsAssertions : ReferenceTypeAssertions<NotifyEventArgs, NotifyEventArgsAssertions>
    {
        public NotifyEventArgsAssertions(NotifyEventArgs subject) : base(subject)
        {
        }

        protected override string Identifier => nameof(NotifyEventArgs);

        public AndConstraint<NotifyEventArgsAssertions> BeEquivalentTo<T>(Expression<Action<T>> expected, string because = "", params object[] becauseArgs)
        {
            var methodCall = (MethodCallExpression)expected.Body;
            var methodArgs = methodCall.Arguments.Select(a => Expression.Lambda(a).Compile().DynamicInvoke());

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.EventName == methodCall.Method.Name)
                .FailWith("Expected {context:NotifyEventArgs} to have {0}{reason} event name, but found {1}.", methodCall.Method.Name, Subject.EventName)
                .Then
                .ForCondition(Subject.State.Count == methodCall.Arguments.Count)
                .FailWith("Expected {context:NotifyEventArgs} to have {0}{reason} state items, but found {1}.", methodCall.Arguments.Count, Subject.State.Count);

            for (var i = 0; i < methodCall.Arguments.Count; i++)
            {
                var obj = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                Subject.State[i].Should().BeEquivalentTo(obj);
            }

            return new AndConstraint<NotifyEventArgsAssertions>(this);
        }

        public AndConstraint<NotifyEventArgsAssertions> BeSentBy(ContractState expected, string because = "", params object[] becauseArgs)
            => BeSentBy(expected.Hash, because, becauseArgs);

        public AndConstraint<NotifyEventArgsAssertions> BeSentBy(UInt160 expected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.ScriptHash == expected)
                .FailWith("Expected {context:NotifyEventArgs} to be sent by {0}{reason}, but found {1}.", expected, Subject.ScriptHash);

            return new AndConstraint<NotifyEventArgsAssertions>(this);
        }
    }
}
