using Neo.SmartContract.Fuzzer.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Fuzzer.Minimization
{
    /// <summary>
    /// Provides common predicates for test case minimization.
    /// </summary>
    public static class MinimizationPredicates
    {
        /// <summary>
        /// Creates a predicate that checks if the execution fails.
        /// </summary>
        /// <returns>A predicate that returns true if the execution fails.</returns>
        public static Predicate<ExecutionResult> FailsExecution()
        {
            return result => !result.Success;
        }

        /// <summary>
        /// Creates a predicate that checks if the execution fails with a specific exception.
        /// </summary>
        /// <param name="exceptionType">The type of exception to check for.</param>
        /// <returns>A predicate that returns true if the execution fails with the specified exception.</returns>
        public static Predicate<ExecutionResult> FailsWithException(Type exceptionType)
        {
            return result => !result.Success && result.Exception?.GetType() == exceptionType;
        }

        /// <summary>
        /// Creates a predicate that checks if the execution fails with an exception matching a specific message pattern.
        /// </summary>
        /// <param name="messagePattern">The regex pattern to match against the exception message.</param>
        /// <returns>A predicate that returns true if the execution fails with an exception matching the pattern.</returns>
        public static Predicate<ExecutionResult> FailsWithExceptionMessage(string messagePattern)
        {
            var regex = new Regex(messagePattern);
            return result => !result.Success && result.Exception != null && regex.IsMatch(result.Exception.Message);
        }

        /// <summary>
        /// Creates a predicate that checks if the execution consumes more than a specific amount of gas.
        /// </summary>
        /// <param name="gasThreshold">The gas threshold.</param>
        /// <returns>A predicate that returns true if the execution consumes more than the threshold.</returns>
        public static Predicate<ExecutionResult> ConsumesMoreGasThan(long gasThreshold)
        {
            return result => result.FeeConsumed > gasThreshold;
        }

        /// <summary>
        /// Creates a predicate that checks if the execution emits a specific notification.
        /// </summary>
        /// <param name="eventName">The name of the event to check for.</param>
        /// <returns>A predicate that returns true if the execution emits the specified notification.</returns>
        public static Predicate<ExecutionResult> EmitsNotification(string eventName)
        {
            return result => result.Engine?.Notifications?.Any(n => n.EventName == eventName) == true;
        }

        /// <summary>
        /// Creates a predicate that checks if the execution returns a specific value.
        /// </summary>
        /// <param name="expectedValue">The expected return value.</param>
        /// <returns>A predicate that returns true if the execution returns the expected value.</returns>
        public static Predicate<ExecutionResult> ReturnsValue(object expectedValue)
        {
            return result => result.Success && result.ReturnValue != null && result.ReturnValue.ToString() == expectedValue.ToString();
        }

        /// <summary>
        /// Creates a predicate that checks if the execution modifies a specific storage key.
        /// </summary>
        /// <param name="key">The storage key to check.</param>
        /// <returns>A predicate that returns true if the execution modifies the specified storage key.</returns>
        public static Predicate<ExecutionResult> ModifiesStorage(byte[] key)
        {
            return result => result.StorageChanges?.ContainsKey(key) == true;
        }

        /// <summary>
        /// Creates a predicate that combines multiple predicates with a logical AND.
        /// </summary>
        /// <param name="predicates">The predicates to combine.</param>
        /// <returns>A predicate that returns true if all the specified predicates return true.</returns>
        public static Predicate<ExecutionResult> And(params Predicate<ExecutionResult>[] predicates)
        {
            return result => predicates.All(p => p(result));
        }

        /// <summary>
        /// Creates a predicate that combines multiple predicates with a logical OR.
        /// </summary>
        /// <param name="predicates">The predicates to combine.</param>
        /// <returns>A predicate that returns true if any of the specified predicates return true.</returns>
        public static Predicate<ExecutionResult> Or(params Predicate<ExecutionResult>[] predicates)
        {
            return result => predicates.Any(p => p(result));
        }

        /// <summary>
        /// Creates a predicate that negates another predicate.
        /// </summary>
        /// <param name="predicate">The predicate to negate.</param>
        /// <returns>A predicate that returns true if the specified predicate returns false.</returns>
        public static Predicate<ExecutionResult> Not(Predicate<ExecutionResult> predicate)
        {
            return result => !predicate(result);
        }
    }
}
