using Neo.SmartContract.Fuzzer.Extensions;
using Neo.SmartContract.Fuzzer.StaticAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Feedback
{
    /// <summary>
    /// Aggregates feedback from various sources to guide the fuzzing process.
    /// </summary>
    public class FeedbackAggregator
    {
        private readonly List<FeedbackItem> _feedbackItems = new List<FeedbackItem>();
        private readonly Dictionary<string, int> _methodCoverage = new Dictionary<string, int>();
        private readonly HashSet<string> _uniqueExceptions = new HashSet<string>();
        private readonly HashSet<string> _uniqueStorageKeys = new HashSet<string>();
        private readonly HashSet<string> _uniqueEvents = new HashSet<string>();
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackAggregator"/> class.
        /// </summary>
        /// <param name="seed">Random seed for prioritization.</param>
        public FeedbackAggregator(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Adds feedback from an execution result.
        /// </summary>
        /// <param name="testCase">The test case that was executed.</param>
        /// <param name="result">The execution result.</param>
        /// <returns>True if the feedback is interesting (adds new coverage or triggers a crash), false otherwise.</returns>
        public bool AddExecutionFeedback(TestCase testCase, ExecutionResult result)
        {
            bool isInteresting = false;

            // Check for crashes
            if (!result.Success)
            {
                string exceptionKey = $"{testCase.MethodName}:{result.Exception?.GetType().Name ?? "Unknown"}";
                if (_uniqueExceptions.Add(exceptionKey))
                {
                    _feedbackItems.Add(new FeedbackItem
                    {
                        Type = FeedbackType.Crash,
                        RelatedTestCase = testCase.Clone(),
                        Priority = 100, // High priority for crashes
                        Timestamp = DateTime.Now,
                        Description = $"Crash in method {testCase.MethodName}: {result.Exception?.Message}"
                    });
                    isInteresting = true;
                }
            }

            // Track method coverage
            if (!_methodCoverage.ContainsKey(testCase.MethodName))
            {
                _methodCoverage[testCase.MethodName] = 1;
                _feedbackItems.Add(new FeedbackItem
                {
                    Type = FeedbackType.NewCoverage,
                    RelatedTestCase = testCase.Clone(),
                    Priority = 80, // High priority for new method coverage
                    Timestamp = DateTime.Now,
                    Description = $"New method coverage: {testCase.MethodName}"
                });
                isInteresting = true;
            }
            else
            {
                _methodCoverage[testCase.MethodName]++;
            }

            // Track storage access
            if (result.StorageChanges != null)
            {
                foreach (var key in result.StorageChanges.Keys)
                {
                    string storageKey = Convert.ToBase64String(key);
                    if (_uniqueStorageKeys.Add(storageKey))
                    {
                        _feedbackItems.Add(new FeedbackItem
                        {
                            Type = FeedbackType.NewCoverage,
                            RelatedTestCase = testCase.Clone(),
                            Priority = 60, // Medium priority for new storage access
                            Timestamp = DateTime.Now,
                            Description = $"New storage key accessed: {storageKey}"
                        });
                        isInteresting = true;
                    }
                }
            }

            // Track events
            if (result.Engine?.Notifications != null)
            {
                foreach (var notification in result.Engine.Notifications)
                {
                    string eventKey = $"{notification.ScriptHash}:{notification.EventName}";
                    if (_uniqueEvents.Add(eventKey))
                    {
                        _feedbackItems.Add(new FeedbackItem
                        {
                            Type = FeedbackType.NewCoverage,
                            RelatedTestCase = testCase.Clone(),
                            Priority = 70, // Medium-high priority for new events
                            Timestamp = DateTime.Now,
                            Description = $"New event triggered: {notification.EventName}"
                        });
                        isInteresting = true;
                    }
                }
            }

            // Track high gas usage
            if (result.FeeConsumed > 10_000_000) // 0.1 GAS threshold
            {
                _feedbackItems.Add(new FeedbackItem
                {
                    Type = FeedbackType.HighGasUsage,
                    RelatedTestCase = testCase.Clone(),
                    Priority = 50, // Medium priority for high gas usage
                    Timestamp = DateTime.Now,
                    Description = $"High gas usage in method {testCase.MethodName}: {result.FeeConsumed}"
                });
                isInteresting = true;
            }

            return isInteresting;
        }

        /// <summary>
        /// Adds feedback from static analysis.
        /// </summary>
        /// <param name="hint">The static analysis hint.</param>
        public void AddStaticAnalysisHint(StaticAnalysisHint hint)
        {
            _feedbackItems.Add(new FeedbackItem
            {
                Type = FeedbackType.StaticHint,
                StaticHint = hint,
                Priority = hint.Priority,
                Timestamp = DateTime.Now,
                Description = hint.Description
            });
        }

        /// <summary>
        /// Gets the next feedback item to guide fuzzing.
        /// </summary>
        /// <returns>The next feedback item, or null if no feedback is available.</returns>
        public FeedbackItem GetNextFeedback()
        {
            if (_feedbackItems.Count == 0)
                return null;

            // Use weighted random selection based on priority
            int totalPriority = _feedbackItems.Sum(f => f.Priority);
            int randomValue = _random.Next(totalPriority);
            int currentSum = 0;

            foreach (var feedback in _feedbackItems.OrderByDescending(f => f.Priority))
            {
                currentSum += feedback.Priority;
                if (randomValue < currentSum)
                    return feedback;
            }

            // Fallback to the highest priority item
            return _feedbackItems.OrderByDescending(f => f.Priority).First();
        }

        /// <summary>
        /// Gets all feedback items.
        /// </summary>
        /// <returns>All feedback items.</returns>
        public IEnumerable<FeedbackItem> GetAllFeedback()
        {
            return _feedbackItems.AsReadOnly();
        }

        /// <summary>
        /// Gets the current coverage statistics.
        /// </summary>
        /// <returns>A dictionary with coverage statistics.</returns>
        public Dictionary<string, object> GetCoverageStatistics()
        {
            return new Dictionary<string, object>
            {
                ["MethodsCovered"] = _methodCoverage.Count,
                ["TotalMethodCalls"] = _methodCoverage.Values.Sum(),
                ["UniqueExceptions"] = _uniqueExceptions.Count,
                ["UniqueStorageKeys"] = _uniqueStorageKeys.Count,
                ["UniqueEvents"] = _uniqueEvents.Count
            };
        }
    }
}
