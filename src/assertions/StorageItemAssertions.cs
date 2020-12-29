using System;
using System.Numerics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Neo;
using Neo.Ledger;

namespace Neo.Assertions
{
    public class StorageItemAssertions : ReferenceTypeAssertions<StorageItem, StorageItemAssertions>
    {
        public StorageItemAssertions(StorageItem subject) : base(subject)
        {
        }

        protected override string Identifier => nameof(StorageItem);

        AndConstraint<StorageItemAssertions> Be<T>(T expected, Func<byte[], T> convert, string because = "", params object[] becauseArgs)
            where T : IEquatable<T>
        {
            Execute.Assertion
                .Given(() => convert(Subject.Value))
                .ForCondition(subject => subject.Equals(expected))
                .FailWith("Expected {context:StorageItem} to be {0}{reason}, but was {1}.",
                    _ => expected, subject => subject);

            return new AndConstraint<StorageItemAssertions>(this);
        }

        public AndConstraint<StorageItemAssertions> Be(BigInteger expected, string because = "", params object[] becauseArgs)
            => Be<BigInteger>(expected, bytes => new BigInteger(bytes), because, becauseArgs);

        public AndConstraint<StorageItemAssertions> Be(UInt160 expected, string because = "", params object[] becauseArgs)
            => Be<UInt160>(expected, bytes => new UInt160(bytes), because, becauseArgs);

        public AndConstraint<StorageItemAssertions> Be(UInt256 expected, string because = "", params object[] becauseArgs)
            => Be<UInt256>(expected, bytes => new UInt256(bytes), because, becauseArgs);

        public AndConstraint<StorageItemAssertions> BeConstant(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.IsConstant == true)
                .FailWith("Expected {context:StorageItem} to be constant{reason}, but IsConstant returned {1}.", Subject.IsConstant);

            return new AndConstraint<StorageItemAssertions>(this);
        }

        public AndConstraint<StorageItemAssertions> NotBeConstant(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.IsConstant == false)
                .FailWith("Expected {context:StorageItem} to not be constant{reason}, but IsConstant returned {1}.", Subject.IsConstant);

            return new AndConstraint<StorageItemAssertions>(this);
        }
    }
}