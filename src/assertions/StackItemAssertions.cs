using System;
using System.Numerics;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Neo;
using Neo.VM.Types;


namespace Neo.Assertions
{
    public class StackItemAssertions : ReferenceTypeAssertions<StackItem, StackItemAssertions>
    {
        public StackItemAssertions(StackItem subject) : base(subject)
        {
        }

        protected override string Identifier => nameof(StackItem);

        public AndConstraint<StackItemAssertions> BeEquivalentTo(object? expected, string because = "", params object[] becauseArgs)
        {
            if (expected == null)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(Subject.IsNull)
                    .FailWith("Expected {context:StackItem} to be null{reason}, but IsNull returned {0}.", Subject.IsNull);

                return new AndConstraint<StackItemAssertions>(this);
            }
            else
            {
                return expected switch
                {
                    string expectedString => BeEquivalentTo(expectedString, because, becauseArgs),
                    BigInteger expectedInt => BeEquivalentTo(expectedInt, because, becauseArgs),
                    bool expectedBool => BeEquivalentTo(expectedBool, because, becauseArgs),
                    UInt160 expectedHash => BeEquivalentTo(expectedHash, because, becauseArgs),
                    UInt256 expectedHash => BeEquivalentTo(expectedHash, because, becauseArgs),
                    byte[] expectedByteArray => BeEquivalentTo(expectedByteArray.AsSpan(), because, becauseArgs),
                    ReadOnlyMemory<byte> expectedMemory => BeEquivalentTo(expectedMemory.Span, because, becauseArgs),
                    _ => UnsupportedType(expected)
                };
            }

            AndConstraint<StackItemAssertions> UnsupportedType(object expectedObject)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Unknown {context:StackItem} type {0}.", expectedObject.GetType());

                return new AndConstraint<StackItemAssertions>(this);
            }
        }

        public AndConstraint<StackItemAssertions> BeEquivalentTo(string expected, string because = "", params object[] becauseArgs)
        {
            try
            {
                var subject = Subject.GetString();

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(subject == expected)
                    .FailWith("Expected {context:StackItem} to be {0}{reason}, but found {1}.", expected, subject);
            }
            catch (Exception ex)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:StackItem} to support GetString{reason}, but GetString failed with:{0}.", ex.Message);
            }

            return new AndConstraint<StackItemAssertions>(this);
        }

        public AndConstraint<StackItemAssertions> BeEquivalentTo(BigInteger expected, string because = "", params object[] becauseArgs)
        {
            try
            {
                var subject = Subject.GetInteger();

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(subject == expected)
                    .FailWith("Expected {context:StackItem} to be {0}{reason}, but found {1}.", expected, subject);
            }
            catch (Exception ex)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:StackItem} to support GetInteger{reason}, but GetInteger failed with:{0}.", ex.Message);
            }

            return new AndConstraint<StackItemAssertions>(this);
        }

        public AndConstraint<StackItemAssertions> BeEquivalentTo(bool expected, string because = "", params object[] becauseArgs)
        {
            try
            {
                var subject = Subject.GetBoolean();

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(subject == expected)
                    .FailWith("Expected {context:StackItem} to be {0}{reason}, but found {1}.", expected, subject);
            }
            catch (Exception ex)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:StackItem} to support GetBoolean{reason}, but GetBoolean failed with:{0}.", ex.Message);
            }

            return new AndConstraint<StackItemAssertions>(this);
        }

        public AndConstraint<StackItemAssertions> BeTrue(string because = "", params object[] becauseArgs)
            => BeEquivalentTo(true, because, becauseArgs);

        public AndConstraint<StackItemAssertions> BeFalse(string because = "", params object[] becauseArgs)
            => BeEquivalentTo(false, because, becauseArgs);


        public AndConstraint<StackItemAssertions> BeEquivalentTo(UInt160 expected, string because = "", params object[] becauseArgs)
        {
            try
            {
                var span = Subject.GetSpan();

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(span.Length == 20)
                    .FailWith("Expected {context:StackItem} span to be {0}{reason}, but found {1}.", "20 bytes long", span.Length)
                    .Then
                    .Given(() => new UInt160(Subject.GetSpan()))
                    .ForCondition(hash => hash == expected)
                    .FailWith("Expected {context:StackItem} span to be {0}{reason}, but found {1}.", _ => expected, hash => hash);
            }
            catch (Exception ex)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:StackItem} to support GetSpan{reason}, but GetSpan failed with:{0}.", ex.Message);
            }

            return new AndConstraint<StackItemAssertions>(this);
        }

        public AndConstraint<StackItemAssertions> BeEquivalentTo(UInt256 expected, string because = "", params object[] becauseArgs)
        {
            try
            {
                var span = Subject.GetSpan();

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(span.Length == 32)
                    .FailWith("Expected {context:StackItem} span to be {0}{reason}, but found {1}.", "32 bytes long", span.Length)
                    .Then
                    .Given(() => new UInt256(Subject.GetSpan()))
                    .ForCondition(hash => hash == expected)
                    .FailWith("Expected {context:StackItem} to be {0}{reason}, but found {1}.", _ => expected, hash => hash);
            }
            catch (Exception ex)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:StackItem} to support GetSpan{reason}, but GetSpan failed with:{0}.", ex.Message);
            }

            return new AndConstraint<StackItemAssertions>(this);
        }

        public AndConstraint<StackItemAssertions> BeEquivalentTo(ReadOnlySpan<byte> expected, string because = "", params object[] becauseArgs)
        {
            try
            {
                var span = Subject.GetSpan();

                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(span.SequenceEqual(expected))
                    .FailWith("Expected {context:StackItem} to be {0}{reason}, but found {1}.",
                        Convert.ToHexString(expected), Convert.ToHexString(span));
            }
            catch (Exception ex)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:StackItem} to support GetString{reason}, but GetString failed with:{0}.", ex.Message);
            }

            return new AndConstraint<StackItemAssertions>(this);
        }

    }
}
