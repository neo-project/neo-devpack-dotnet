using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Evaluation;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    [TestClass]
    public class SymbolicEvaluationTests
    {
        private SymbolicEvaluation _evaluationService;

        [TestInitialize]
        public void Setup()
        {
            _evaluationService = new SymbolicEvaluation();
        }

        [TestMethod]
        public void TestNotNotSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Boolean);
            
            // Apply NOT operation once
            var notExpr = _evaluationService.Not(variable);
            
            // Apply NOT operation again
            var notNotExpr = _evaluationService.Not(notExpr);
            
            // The result should be the original variable (NOT(NOT(x)) = x)
            Assert.AreEqual(variable, notNotExpr);
        }

        [TestMethod]
        public void TestNegateNegateSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // Apply NEGATE operation once
            var negateExpr = _evaluationService.Negate(variable);
            
            // Apply NEGATE operation again
            var negateNegateExpr = _evaluationService.Negate(negateExpr);
            
            // The result should be the original variable (NEGATE(NEGATE(x)) = x)
            Assert.AreEqual(variable, negateNegateExpr);
        }

        [TestMethod]
        public void TestAddZeroSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // Create a concrete zero
            var zero = new ConcreteValue<BigInteger>(BigInteger.Zero);
            
            // x + 0
            var addZeroRight = _evaluationService.Add(variable, zero);
            
            // 0 + x
            var addZeroLeft = _evaluationService.Add(zero, variable);
            
            // Both should simplify to the original variable
            Assert.AreEqual(variable, addZeroRight);
            Assert.AreEqual(variable, addZeroLeft);
        }

        [TestMethod]
        public void TestSubtractZeroSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // Create a concrete zero
            var zero = new ConcreteValue<BigInteger>(BigInteger.Zero);
            
            // x - 0
            var subtractZero = _evaluationService.Subtract(variable, zero);
            
            // Should simplify to the original variable
            Assert.AreEqual(variable, subtractZero);
        }

        [TestMethod]
        public void TestMultiplyOneSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // Create a concrete one
            var one = new ConcreteValue<BigInteger>(BigInteger.One);
            
            // x * 1
            var multiplyOneRight = _evaluationService.Multiply(variable, one);
            
            // 1 * x
            var multiplyOneLeft = _evaluationService.Multiply(one, variable);
            
            // Both should simplify to the original variable
            Assert.AreEqual(variable, multiplyOneRight);
            Assert.AreEqual(variable, multiplyOneLeft);
        }

        [TestMethod]
        public void TestMultiplyZeroSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // Create a concrete zero
            var zero = new ConcreteValue<BigInteger>(BigInteger.Zero);
            
            // x * 0
            var multiplyZeroRight = _evaluationService.Multiply(variable, zero);
            
            // 0 * x
            var multiplyZeroLeft = _evaluationService.Multiply(zero, variable);
            
            // Both should simplify to zero
            Assert.IsTrue(multiplyZeroRight is ConcreteValue<BigInteger>);
            Assert.IsTrue(multiplyZeroLeft is ConcreteValue<BigInteger>);
            
            var rightResult = (ConcreteValue<BigInteger>)multiplyZeroRight;
            var leftResult = (ConcreteValue<BigInteger>)multiplyZeroLeft;
            
            Assert.AreEqual(BigInteger.Zero, rightResult.Value);
            Assert.AreEqual(BigInteger.Zero, leftResult.Value);
        }

        [TestMethod]
        public void TestDivideOneSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // Create a concrete one
            var one = new ConcreteValue<BigInteger>(BigInteger.One);
            
            // x / 1
            var divideOne = _evaluationService.Divide(variable, one);
            
            // Should simplify to the original variable
            Assert.AreEqual(variable, divideOne);
        }

        [TestMethod]
        public void TestBooleanAndTrueSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Boolean);
            
            // Create a concrete true
            var trueValue = new ConcreteValue<bool>(true);
            
            // x && true
            var andTrueRight = _evaluationService.BoolAnd(variable, trueValue);
            
            // true && x
            var andTrueLeft = _evaluationService.BoolAnd(trueValue, variable);
            
            // Both should simplify to the original variable
            Assert.AreEqual(variable, andTrueRight);
            Assert.AreEqual(variable, andTrueLeft);
        }

        [TestMethod]
        public void TestBooleanAndFalseSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Boolean);
            
            // Create a concrete false
            var falseValue = new ConcreteValue<bool>(false);
            
            // x && false
            var andFalseRight = _evaluationService.BoolAnd(variable, falseValue);
            
            // false && x
            var andFalseLeft = _evaluationService.BoolAnd(falseValue, variable);
            
            // Both should simplify to false
            Assert.AreEqual(falseValue, andFalseRight);
            Assert.AreEqual(falseValue, andFalseLeft);
        }

        [TestMethod]
        public void TestBooleanOrTrueSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Boolean);
            
            // Create a concrete true
            var trueValue = new ConcreteValue<bool>(true);
            
            // x || true
            var orTrueRight = _evaluationService.BoolOr(variable, trueValue);
            
            // true || x
            var orTrueLeft = _evaluationService.BoolOr(trueValue, variable);
            
            // Both should simplify to true
            Assert.AreEqual(trueValue, orTrueRight);
            Assert.AreEqual(trueValue, orTrueLeft);
        }

        [TestMethod]
        public void TestBooleanOrFalseSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Boolean);
            
            // Create a concrete false
            var falseValue = new ConcreteValue<bool>(false);
            
            // x || false
            var orFalseRight = _evaluationService.BoolOr(variable, falseValue);
            
            // false || x
            var orFalseLeft = _evaluationService.BoolOr(falseValue, variable);
            
            // Both should simplify to the original variable
            Assert.AreEqual(variable, orFalseRight);
            Assert.AreEqual(variable, orFalseLeft);
        }

        [TestMethod]
        public void TestSubtractSelfSimplification()
        {
            // Create a symbolic variable
            var variable = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            
            // x - x
            var subtractSelf = _evaluationService.Subtract(variable, variable);
            
            // Should simplify to zero
            Assert.IsTrue(subtractSelf is ConcreteValue<BigInteger>);
            var result = (ConcreteValue<BigInteger>)subtractSelf;
            Assert.AreEqual(BigInteger.Zero, result.Value);
        }
    }
}
