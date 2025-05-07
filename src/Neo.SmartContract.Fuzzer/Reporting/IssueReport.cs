using Neo.SmartContract.Fuzzer.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Neo.SmartContract.Fuzzer.Reporting
{
    /// <summary>
    /// Represents a report of an issue found during fuzzing.
    /// </summary>
    public class IssueReport
    {
        /// <summary>
        /// Gets or sets the unique identifier of the issue.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the type of the issue.
        /// </summary>
        public string IssueType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the severity of the issue.
        /// </summary>
        public IssueSeverity Severity { get; set; } = IssueSeverity.Medium;

        /// <summary>
        /// Gets or sets the description of the issue.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the method name where the issue was found.
        /// </summary>
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the test case that triggered the issue.
        /// </summary>
        [JsonIgnore]
        public TestCase? TestCase { get; set; }

        /// <summary>
        /// Gets or sets the serialized test case for JSON serialization.
        /// </summary>
        public string? SerializedTestCase
        {
            get => TestCase?.ToJson();
            set
            {
                if (value != null)
                {
                    TestCase = TestCase.FromJson(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the execution result that triggered the issue.
        /// </summary>
        [JsonIgnore]
        public ExecutionResult? ExecutionResult { get; set; }

        /// <summary>
        /// Gets or sets the serialized execution result for JSON serialization.
        /// </summary>
        public string? SerializedExecutionResult
        {
            get => ExecutionResult != null ? System.Text.Json.JsonSerializer.Serialize(ExecutionResult) : null;
            set
            {
                if (value != null)
                {
                    try
                    {
                        ExecutionResult = System.Text.Json.JsonSerializer.Deserialize<ExecutionResult>(value);
                    }
                    catch
                    {
                        // If deserialization fails, leave ExecutionResult as null
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimized test case that still triggers the issue.
        /// </summary>
        [JsonIgnore]
        public TestCase? MinimizedTestCase { get; set; }

        /// <summary>
        /// Gets or sets the serialized minimized test case for JSON serialization.
        /// </summary>
        public string? SerializedMinimizedTestCase
        {
            get => MinimizedTestCase?.ToJson();
            set
            {
                if (value != null)
                {
                    MinimizedTestCase = TestCase.FromJson(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the timestamp when the issue was found.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the exception message if the issue is an exception.
        /// </summary>
        public string? ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets the exception type if the issue is an exception.
        /// </summary>
        public string? ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the gas consumed when the issue was triggered.
        /// </summary>
        public long GasConsumed { get; set; }

        /// <summary>
        /// Gets or sets the VM state when the issue was triggered.
        /// </summary>
        public string? VMState { get; set; }

        /// <summary>
        /// Gets or sets the notifications emitted when the issue was triggered.
        /// </summary>
        public List<NotificationInfo> Notifications { get; set; } = new List<NotificationInfo>();

        /// <summary>
        /// Gets or sets the logs emitted when the issue was triggered.
        /// </summary>
        public List<LogInfo> Logs { get; set; } = new List<LogInfo>();

        /// <summary>
        /// Gets or sets the storage changes when the issue was triggered.
        /// </summary>
        public Dictionary<string, string> StorageChanges { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the remediation suggestions for the issue.
        /// </summary>
        public string? Remediation { get; set; }

        /// <summary>
        /// Gets or sets additional information about the issue.
        /// </summary>
        public Dictionary<string, string> AdditionalInfo { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new instance of the <see cref="IssueReport"/> class from an execution result.
        /// </summary>
        /// <param name="testCase">The test case that triggered the issue.</param>
        /// <param name="result">The execution result.</param>
        /// <param name="issueType">The type of the issue.</param>
        /// <param name="severity">The severity of the issue.</param>
        /// <param name="description">The description of the issue.</param>
        /// <returns>A new instance of the <see cref="IssueReport"/> class.</returns>
        public static IssueReport FromExecutionResult(
            TestCase testCase,
            ExecutionResult result,
            string issueType,
            IssueSeverity severity,
            string description)
        {
            var report = new IssueReport
            {
                IssueType = issueType,
                Severity = severity,
                Description = description,
                MethodName = testCase.MethodName,
                TestCase = testCase.Clone(),
                ExecutionResult = result,
                Timestamp = DateTime.Now,
                GasConsumed = result.FeeConsumed,
                VMState = result.Engine?.State.ToString()
            };

            if (!result.Success)
            {
                report.ExceptionMessage = result.ExceptionMessage;
                report.ExceptionType = result.ExceptionType;
            }

            if (result.Engine?.Notifications != null)
            {
                foreach (var notification in result.Engine.Notifications)
                {
                    report.Notifications.Add(new NotificationInfo
                    {
                        ScriptHash = notification.ScriptHash.ToString(),
                        EventName = notification.EventName,
                        State = notification.State.ToString()
                    });
                }
            }

            if (result.CollectedLogs != null)
            {
                foreach (var log in result.CollectedLogs)
                {
                    report.Logs.Add(new LogInfo
                    {
                        ScriptHash = log.ScriptHash.ToString(),
                        Message = log.Message
                    });
                }
            }

            if (result.StorageChanges != null)
            {
                foreach (var change in result.StorageChanges)
                {
                    report.StorageChanges[Convert.ToBase64String(change.Key)] = Convert.ToBase64String(change.NewValue);
                }
            }

            return report;
        }
    }

    /// <summary>
    /// Represents the severity of an issue.
    /// </summary>
    public enum IssueSeverity
    {
        /// <summary>
        /// Low severity issue.
        /// </summary>
        Low,

        /// <summary>
        /// Medium severity issue.
        /// </summary>
        Medium,

        /// <summary>
        /// High severity issue.
        /// </summary>
        High,

        /// <summary>
        /// Critical severity issue.
        /// </summary>
        Critical
    }

    /// <summary>
    /// Represents a notification emitted during execution.
    /// </summary>
    public class NotificationInfo
    {
        /// <summary>
        /// Gets or sets the script hash that emitted the notification.
        /// </summary>
        public string ScriptHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the state of the notification.
        /// </summary>
        public string State { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a log emitted during execution.
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// Gets or sets the script hash that emitted the log.
        /// </summary>
        public string ScriptHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message of the log.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
