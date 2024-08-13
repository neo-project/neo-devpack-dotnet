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
}
