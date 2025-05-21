using System;
using Neo.SmartContract.Fuzzer.StaticAnalysis;

namespace Neo.SmartContract.Fuzzer.Feedback
{
    /// <summary>
    /// Represents a feedback item that guides the fuzzing process.
    /// </summary>
    public class FeedbackItem
    {
        /// <summary>
        /// Gets or sets the type of feedback.
        /// </summary>
        public FeedbackType Type { get; set; }

        /// <summary>
        /// Gets or sets the test case related to this feedback.
        /// </summary>
        public TestCase? RelatedTestCase { get; set; }

        /// <summary>
        /// Gets or sets the static analysis hint related to this feedback.
        /// </summary>
        public StaticAnalysisHint? StaticHint { get; set; }

        /// <summary>
        /// Gets or sets the priority of this feedback item.
        /// Higher values indicate higher priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when this feedback was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a description of this feedback item.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Creates a clone of this feedback item.
        /// </summary>
        /// <returns>A new instance of <see cref="FeedbackItem"/> with the same values.</returns>
        public FeedbackItem Clone()
        {
            return new FeedbackItem
            {
                Type = Type,
                RelatedTestCase = RelatedTestCase?.Clone(),
                StaticHint = StaticHint,
                Priority = Priority,
                Timestamp = Timestamp,
                Description = Description
            };
        }
    }
}
