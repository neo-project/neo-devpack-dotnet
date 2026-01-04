// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Char.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Char : SmartContract.Framework.SmartContract
{
    public static bool TestCharIsDigit(char c)
    {
        return char.IsDigit(c);
    }

    public static bool TestCharIsLetter(char c)
    {
        return char.IsLetter(c);
    }

    public static bool TestCharIsWhiteSpace(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public static bool TestCharIsLetterOrDigit(char c)
    {
        return char.IsLetterOrDigit(c);
    }

    public static bool TestCharIsLower(char c)
    {
        return char.IsLower(c);
    }

    public static char TestCharToLower(char c)
    {
        return char.ToLower(c);
    }

    public static bool TestCharIsUpper(char c)
    {
        return char.IsUpper(c);
    }

    public static char TestCharToUpper(char c)
    {
        return char.ToUpper(c);
    }

    public static int TestCharGetNumericValue(char c)
    {
        return (int)char.GetNumericValue(c);
    }

    public static bool TestCharIsPunctuation(char c)
    {
        return char.IsPunctuation(c);
    }

    public static bool TestCharIsSymbol(char c)
    {
        return char.IsSymbol(c);
    }

    public static bool TestCharIsControl(char c)
    {
        return char.IsControl(c);
    }

    public static bool TestCharIsSurrogate(char c)
    {
        return char.IsSurrogate(c);
    }

    public static bool TestCharIsHighSurrogate(char c)
    {
        return char.IsHighSurrogate(c);
    }

    public static bool TestCharIsLowSurrogate(char c)
    {
        return char.IsLowSurrogate(c);
    }

    public static bool TestCharIsBetween(char c, char lower, char upper)
    {
        return char.IsBetween(c, lower, upper);
    }

    public static char TestCharParse(string value)
    {
        return char.Parse(value);
    }

    public static (bool, char) TestCharTryParse(string value)
    {
        bool success = char.TryParse(value, out char result);
        return (success, result);
    }
}
