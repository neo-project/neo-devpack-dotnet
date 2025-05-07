using System;
using System.Collections.Generic;
using System.Linq;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Engine for symbolic execution of Neo smart contracts.
    /// </summary>
    public class SymbolicExecutionEngine : ISymbolicExecutionEngine
    {
        private readonly byte[] _script;
        private readonly IConstraintSolver _solver;
        private readonly List<ISymbolicVulnerabilityDetector> _detectors;
        private readonly int _maxSteps;
        private readonly SymbolicVirtualMachine _vm;

        /// <summary>
        /// Gets the current state of the symbolic execution.
        /// </summary>
        public ISymbolicState CurrentState => _vm.CurrentState;

        /// <summary>
        /// Gets the execution engine limits.
        /// </summary>
        public ExecutionEngineLimits Limits => _vm.Limits;

        /// <summary>
        /// Gets the pending states queue.
        /// </summary>
        public Queue<ISymbolicState> PendingStates => _vm.PendingStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicExecutionEngine"/> class.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="solver">The constraint solver to use.</param>
        /// <param name="detectors">The vulnerability detectors to use.</param>
        /// <param name="initialArguments">The initial arguments to place on the stack.</param>
        /// <param name="maxSteps">The maximum number of steps to execute.</param>
        public SymbolicExecutionEngine(byte[] script, IConstraintSolver solver, IEnumerable<ISymbolicVulnerabilityDetector>? detectors = null, IEnumerable<SymbolicValue>? initialArguments = null, int maxSteps = 1000)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
            _solver = solver ?? throw new ArgumentNullException(nameof(solver));
            _detectors = detectors?.ToList() ?? new List<ISymbolicVulnerabilityDetector>();
            _maxSteps = maxSteps;

            // Create the VM
            var scriptObj = new Script(script);
            var evaluationService = new SimpleEvaluationService();
            var limits = new ExecutionEngineLimits();
            _vm = new SymbolicVirtualMachine(scriptObj, solver, evaluationService, limits);

            // Initialize with arguments if provided
            if (initialArguments != null)
            {
                foreach (var arg in initialArguments)
                {
                    _vm.CurrentState.EvaluationStack.Push(arg);
                }
            }
        }

        /// <summary>
        /// Executes the symbolic engine and returns the result.
        /// </summary>
        /// <returns>The symbolic execution result.</returns>
        public SymbolicExecutionResult Execute()
        {
            return _vm.Execute();
        }

        /// <summary>
        /// Executes a single step in the current state.
        /// </summary>
        public void ExecuteStep()
        {
            _vm.ExecuteStep();
        }

        /// <summary>
        /// Forks the current state with additional constraints.
        /// </summary>
        /// <param name="constraints">The additional constraints.</param>
        /// <returns>A new symbolic state.</returns>
        public ISymbolicState ForkState(IEnumerable<PathConstraint> constraints)
        {
            return _vm.ForkState(constraints);
        }

        /// <summary>
        /// Adds a state to the pending queue.
        /// </summary>
        /// <param name="state">The state to add.</param>
        public void AddPendingState(ISymbolicState state)
        {
            _vm.AddPendingState(state);
        }

        /// <summary>
        /// Logs debug information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogDebug(string message)
        {
            _vm.LogDebug(message);
        }

        /// <summary>
        /// Creates symbolic arguments for a method based on parameter types.
        /// </summary>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>A list of symbolic values.</returns>
        public static List<SymbolicValue> CreateSymbolicArgumentsForMethod(List<string> parameterTypes)
        {
            var result = new List<SymbolicValue>();

            for (int i = 0; i < parameterTypes.Count; i++)
            {
                var paramType = parameterTypes[i];
                var varName = $"arg{i}";

                // Create a symbolic variable based on the parameter type
                var symbolicVar = new SymbolicVariable(varName, SymbolicType.Any);
                result.Add(symbolicVar);
            }

            return result;
        }
    }
}
