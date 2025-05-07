using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using TypesSymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression;
using TypesOperator = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator;

namespace Neo.SmartContract.Fuzzer.Tests.ManualTests
{
    /// <summary>
    /// Manual test for the UnifiedConstraintSolver.
    /// This is not an automated test, but a manual test that can be run to verify the solver works.
    /// </summary>
    public class UnifiedConstraintSolverTest
    {
        public static void Main()
        {
            Console.WriteLine("Testing UnifiedConstraintSolver...");

            // Create a solver
            var solver = new DefaultConstraintSolver();

            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            var z = new SymbolicVariable("z", StackItemType.Integer);

            // Create constraints: x > 10, y < 20, z = x + y
            var ten = new ConcreteValue<long>(10);
            var twenty = new ConcreteValue<long>(20);

            var xGt10 = new TypesSymbolicExpression(x, TypesOperator.GreaterThan, ten);
            var yLt20 = new TypesSymbolicExpression(y, TypesOperator.LessThan, twenty);
            var xPlusY = new TypesSymbolicExpression(x, TypesOperator.Add, y);
            var zEqXPlusY = new TypesSymbolicExpression(z, TypesOperator.Equal, xPlusY);

            var constraints = new List<TypesSymbolicExpression> { xGt10, yLt20, zEqXPlusY };

            // Check if the constraints are satisfiable
            bool isSatisfiable = solver.IsSatisfiable(constraints);
            Console.WriteLine($"Constraints are satisfiable: {isSatisfiable}");

            // Solve the constraints
            var solution = solver.Solve(constraints);

            // Print the solution
            Console.WriteLine("Solution:");
            foreach (var entry in solution)
            {
                Console.WriteLine($"{entry.Key} = {entry.Value}");
            }

            // Verify the solution
            if (solution.TryGetValue("x", out var xValue) &&
                solution.TryGetValue("y", out var yValue) &&
                solution.TryGetValue("z", out var zValue))
            {
                long xVal = Convert.ToInt64(xValue);
                long yVal = Convert.ToInt64(yValue);
                long zVal = Convert.ToInt64(zValue);

                bool xConstraintSatisfied = xVal > 10;
                bool yConstraintSatisfied = yVal < 20;
                bool zConstraintSatisfied = zVal == xVal + yVal;

                Console.WriteLine($"x > 10: {xConstraintSatisfied}");
                Console.WriteLine($"y < 20: {yConstraintSatisfied}");
                Console.WriteLine($"z = x + y: {zConstraintSatisfied}");

                bool allConstraintsSatisfied = xConstraintSatisfied && yConstraintSatisfied && zConstraintSatisfied;
                Console.WriteLine($"All constraints satisfied: {allConstraintsSatisfied}");
            }
            else
            {
                Console.WriteLine("Solution does not contain all variables.");
            }

            // Test with contradictory constraints
            var five = new ConcreteValue<long>(5);
            var xLt5 = new TypesSymbolicExpression(x, TypesOperator.LessThan, five);
            var contradictoryConstraints = new List<TypesSymbolicExpression> { xGt10, xLt5 };

            bool contradictorySatisfiable = solver.IsSatisfiable(contradictoryConstraints);
            Console.WriteLine($"Contradictory constraints are satisfiable: {contradictorySatisfiable}");

            // Test simplification
            var xEqX = new TypesSymbolicExpression(x, TypesOperator.Equal, x); // Tautology
            var constraintsWithTautology = new List<TypesSymbolicExpression> { xGt10, xEqX };

            var simplified = solver.Simplify(constraintsWithTautology);

            Console.WriteLine("Simplified constraints:");
            foreach (var constraint in simplified)
            {
                Console.WriteLine($"  {constraint}");
            }

            // Test the object-based methods
            var objConstraints = new List<object> { xGt10, yLt20, zEqXPlusY };
            bool objIsSatisfiable = solver.IsSatisfiable(objConstraints);
            Console.WriteLine($"Object constraints are satisfiable: {objIsSatisfiable}");

            var objSolution = solver.Solve(objConstraints);
            Console.WriteLine("Object solution:");
            foreach (var kvp in objSolution)
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }

            // Test the UpdateConstraints method with objects
            solver.UpdateConstraints(objConstraints);
            Console.WriteLine("Updated object constraints");

            // Test the Simplify method with objects
            var objSimplifiedConstraints = solver.Simplify(objConstraints);
            Console.WriteLine("Simplified object constraints:");
            foreach (var constraint in objSimplifiedConstraints)
            {
                Console.WriteLine($"  {constraint}");
            }

            Console.WriteLine("UnifiedConstraintSolver test completed.");
        }
    }
}
