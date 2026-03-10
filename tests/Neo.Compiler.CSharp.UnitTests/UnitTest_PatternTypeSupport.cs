// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_PatternTypeSupport.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class UnitTest_PatternTypeSupport
{
    [TestMethod]
    public void TypePattern_Int_Compiles()
    {
        Helper.AssertClassCompilationSucceeds(@"
public static bool MatchInt(object value)
{
    return value is int;
}", "Type pattern with int should compile.");
    }

    [TestMethod]
    public void RecursivePattern_EmptyPropertyClause_Compiles()
    {
        Helper.AssertClassCompilationSucceeds(@"
public static bool MatchAnyNonNull(object value)
{
    return value is { };
}", "Recursive pattern '{}' should compile.");
    }

    [TestMethod]
    public void RecursivePattern_PositionalClause_Fails()
    {
        Helper.AssertClassCompilationFails(@"
private readonly record struct Pair(int A, int B);

public static bool MatchPair(Pair pair)
{
    return pair is Pair(1, 2);
}", "Positional recursive pattern should be rejected explicitly.");
    }

    [TestMethod]
    public void RecursivePattern_NonConstantPropertyPattern_Fails()
    {
        Helper.AssertClassCompilationFails(@"
public class NumberHolder
{
    public int Value { get; set; }
}

public static bool MatchGreaterThanZero(NumberHolder value)
{
    return value is { Value: > 0 };
}", "Recursive patterns should reject non-constant property subpatterns.");
    }

    [TestMethod]
    public void RecursivePattern_FieldPattern_Fails()
    {
        Helper.AssertClassCompilationFails(@"
public class FieldHolder
{
    public int Value;
}

public static bool MatchField(FieldHolder value)
{
    return value is { Value: 1 };
}", "Recursive patterns should reject field members and require properties.");
    }
}
