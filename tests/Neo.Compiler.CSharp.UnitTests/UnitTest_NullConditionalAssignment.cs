using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullConditionalAssignment
{
    [TestMethod]
    public void AssignChild_returns_one_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignChild(true));
    }

    [TestMethod]
    public void AssignChild_returns_zero_when_receiver_null()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(0, contract.AssignChild(false));
    }

    [TestMethod]
    public void AssignSibling_handles_field_targets()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignSibling(true));
        Assert.AreEqual(0, contract.AssignSibling(false));
    }

    [TestMethod]
    public void AssignStatic_handles_static_receivers()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignStatic(true));
        Assert.AreEqual(0, contract.AssignStatic(false));
    }

    [TestMethod]
    public void AssignGrandChild_requires_deep_receiver()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignGrandChild(true, true));
        Assert.AreEqual(0, contract.AssignGrandChild(true, false));
        Assert.AreEqual(0, contract.AssignGrandChild(false, false));
    }

    [TestMethod]
    public void AssignSiblingFromOther_returns_value_when_both_receivers_exist()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignSiblingFromOther(true, true));
        Assert.AreEqual(0, contract.AssignSiblingFromOther(true, false));
        Assert.AreEqual(0, contract.AssignSiblingFromOther(false, true));
    }

    [TestMethod]
    public void AssignChildSideEffects_evaluates_right_side_only_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignChildSideEffects(true));
        Assert.AreEqual(0, contract.AssignChildSideEffects(false));
    }

    [TestMethod]
    public void AssignElement_sets_array_element_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(7, contract.AssignElement(true));
        Assert.AreEqual(0, contract.AssignElement(false));
    }

    [TestMethod]
    public void AssignElementSideEffects_evaluates_right_side_only_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignElementSideEffects(true));
        Assert.AreEqual(0, contract.AssignElementSideEffects(false));
    }

    [TestMethod]
    public void AssignElementIndexAndValueSideEffects_evaluates_only_after_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(27, contract.AssignElementIndexAndValueSideEffects(true));
        Assert.AreEqual(0, contract.AssignElementIndexAndValueSideEffects(false));
    }

    [TestMethod]
    public void AssignNestedElement_requires_all_receivers()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(9, contract.AssignNestedElement(true, true));
        Assert.AreEqual(0, contract.AssignNestedElement(true, false));
        Assert.AreEqual(0, contract.AssignNestedElement(false, true));
    }

    [TestMethod]
    public void AssignMatrixElement_supports_multidimensional_arrays()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(5, contract.AssignMatrixElement(true));
        Assert.AreEqual(0, contract.AssignMatrixElement(false));
    }

    [TestMethod]
    public void AddChildValue_supports_field_compound_assignment()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(7, contract.AddChildValue(true));
        Assert.AreEqual(0, contract.AddChildValue(false));
    }

    [TestMethod]
    public void AddChildCount_supports_property_compound_assignment()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(7, contract.AddChildCount(true));
        Assert.AreEqual(0, contract.AddChildCount(false));
    }

    [TestMethod]
    public void AddElement_supports_element_compound_assignment()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(7, contract.AddElement(true));
        Assert.AreEqual(0, contract.AddElement(false));
    }

    [TestMethod]
    public void AddElementSideEffects_evaluates_right_side_only_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AddElementSideEffects(true));
        Assert.AreEqual(0, contract.AddElementSideEffects(false));
    }

    [TestMethod]
    public void AddElementIndexAndValueSideEffects_evaluates_only_after_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(30, contract.AddElementIndexAndValueSideEffects(true));
        Assert.AreEqual(0, contract.AddElementIndexAndValueSideEffects(false));
    }

    [TestMethod]
    public void CoalesceChild_assigns_property_only_when_receiver_and_current_value_require_it()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(11, contract.CoalesceChild(true, false));
        Assert.AreEqual(1, contract.CoalesceChild(true, true));
        Assert.AreEqual(0, contract.CoalesceChild(false, false));
    }

    [TestMethod]
    public void CoalesceSibling_assigns_field_only_when_receiver_and_current_value_require_it()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(11, contract.CoalesceSibling(true, false));
        Assert.AreEqual(1, contract.CoalesceSibling(true, true));
        Assert.AreEqual(0, contract.CoalesceSibling(false, false));
    }

    [TestMethod]
    public void CoalesceCount_assigns_nullable_property_only_when_current_value_is_null()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(17, contract.CoalesceCount(true, false));
        Assert.AreEqual(3, contract.CoalesceCount(true, true));
        Assert.AreEqual(0, contract.CoalesceCount(false, false));
    }

    [TestMethod]
    public void CoalesceValue_assigns_nullable_field_only_when_current_value_is_null()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(17, contract.CoalesceValue(true, false));
        Assert.AreEqual(3, contract.CoalesceValue(true, true));
        Assert.AreEqual(0, contract.CoalesceValue(false, false));
    }

    [TestMethod]
    public void CoalesceElement_assigns_array_element_only_when_current_value_is_null()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(17, contract.CoalesceElement(true, false));
        Assert.AreEqual(3, contract.CoalesceElement(true, true));
        Assert.AreEqual(0, contract.CoalesceElement(false, false));
    }

    [TestMethod]
    public void CoalesceElementIndexAndValueSideEffects_evaluates_value_only_when_assignment_runs()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(27, contract.CoalesceElementIndexAndValueSideEffects(true, false));
        Assert.AreEqual(13, contract.CoalesceElementIndexAndValueSideEffects(true, true));
        Assert.AreEqual(0, contract.CoalesceElementIndexAndValueSideEffects(false, false));
    }

    [TestMethod]
    public void CoalesceNestedElement_requires_all_receivers_before_reading_or_assigning()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(17, contract.CoalesceNestedElement(true, true, false));
        Assert.AreEqual(3, contract.CoalesceNestedElement(true, true, true));
        Assert.AreEqual(0, contract.CoalesceNestedElement(true, false, false));
        Assert.AreEqual(0, contract.CoalesceNestedElement(false, true, false));
    }

    [TestMethod]
    public void AssignIndexer_sets_indexer_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(7, contract.AssignIndexer(true));
        Assert.AreEqual(0, contract.AssignIndexer(false));
    }

    [TestMethod]
    public void AssignIndexerIndexSideEffects_evaluates_index_only_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignIndexerIndexSideEffects(true));
        Assert.AreEqual(0, contract.AssignIndexerIndexSideEffects(false));
    }

    [TestMethod]
    public void AddIndexer_supports_indexer_compound_assignment()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(7, contract.AddIndexer(true));
        Assert.AreEqual(0, contract.AddIndexer(false));
    }

    [TestMethod]
    public void CoalesceIndexer_assigns_indexer_only_when_current_value_is_null()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(17, contract.CoalesceIndexer(true, false));
        Assert.AreEqual(3, contract.CoalesceIndexer(true, true));
        Assert.AreEqual(0, contract.CoalesceIndexer(false, false));
    }

    [TestMethod]
    public void NullConditionalAssignments_compile_through_lowering()
    {
        Syntax.Helper.AssertClassCompilationSucceeds("""
            private class Node
            {
                public Node? Child { get; set; }
                public Node? Sibling;
                public static Node? StaticChild { get; set; }
                public int Count { get; set; }
                public int Value;
                public int[]? Values;
                public Node? MaybeChild { get; set; }
                public Node? MaybeSibling;
                public int? MaybeCount { get; set; }
                public int? MaybeValue;
                public int?[]? MaybeValues;
            }

            private class IndexerBox
            {
                private readonly int[] _values = new[] { 3 };

                public int this[int index]
                {
                    get => _values[index];
                    set => _values[index] = value;
                }
            }

            private static int SideEffects;

            private static Node CreateNode()
            {
                SideEffects += 1;
                return new Node();
            }

            private static int CreateValue()
            {
                SideEffects += 1;
                return 7;
            }

            public static int CompileNullConditionalAssignments(bool createRoot, bool createChild)
            {
                SideEffects = 0;
                Node? node = createRoot
                    ? new Node
                    {
                        Child = createChild ? new Node { Values = new[] { 1 } } : null,
                        Values = new[] { 3 }
                    }
                    : null;

                var assignedChild = node?.Child = CreateNode();
                node?.Sibling = CreateNode();
                Node.StaticChild = node;
                Node.StaticChild?.Child = CreateNode();
                var assignedGrandChild = node?.Child?.Child = CreateNode();
                node?.Value += 4;
                node?.Count += 4;
                var assignedElement = node?.Values?[0] = CreateValue();
                node?.Values?[0] += CreateValue();
                var coalescedChild = node?.MaybeChild ??= CreateNode();
                node?.MaybeSibling ??= CreateNode();
                node?.MaybeCount ??= CreateValue();
                node?.MaybeValue ??= CreateValue();
                var coalescedElement = node?.MaybeValues?[0] ??= CreateValue();
                int[,]? matrix = createRoot ? new int[1, 1] : null;
                matrix?[0, 0] = 5;
                matrix?[0, 0] += CreateValue();
                IndexerBox? box = createRoot ? new IndexerBox() : null;
                var assignedIndexer = box?[0] = CreateValue();
                box?[0] += CreateValue();

                return SideEffects
                    + (assignedChild is null ? 0 : 1)
                    + (assignedGrandChild is null ? 0 : 1)
                    + (assignedElement is null ? 0 : 1)
                    + (coalescedChild is null ? 0 : 1)
                    + (coalescedElement is null ? 0 : 1)
                    + (assignedIndexer is null ? 0 : 1);
            }
            """,
            "Expected null-conditional assignment lowering forms to compile.");
    }
}
