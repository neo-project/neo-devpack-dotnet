using Neo.SmartContract.Framework.Native;
namespace Neo.SmartContract.Framework;

public static class ByteStringExtension
{
    /// <summary>
    ///  Denotes whether provided character is a number.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is number</returns>
    public static bool IsNumber(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 48 or > 57)
                return false;
        }
        return true;
    }

    /// <summary>
    ///  Denotes whether provided character is a lowercase letter.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is Alpha character</returns>
    public static bool IsLowerAlphabet(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 97 or > 122)
                return false;
        }
        return true;
    }

    /// <summary>
    ///  Denotes whether provided character is a lowercase letter.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is Alpha character</returns>
    public static bool IsUpperAlphabet(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 65 or > 90)
                return false;
        }
        return true;
    }

    /// <summary>
    ///  Denotes whether provided character is a lowercase letter.
    /// </summary>
    /// <param name="byteString">Input to check</param>
    /// <returns>True if is Alpha character</returns>
    public static bool IsAlphabet(this ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (!((value >= 65 && value <= 90) || (value >= 97 && value <= 122)))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns the index of the first occurrence of a given value in an array.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>Index where it is located or -1</returns>
    public static int IndexOf(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind);
    }

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>True if start with</returns>
    public static bool StartWith(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) == 0;
    }

    /// <summary>
    /// Determines whether the end of this string instance matches a specified string.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>True if ends with</returns>
    public static bool EndsWith(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) + byteToFind.Length == byteString.Length;
    }

    /// <summary>
    /// Checks if the <see cref="ByteString"/> contains the given <see cref="ByteString"/>.
    /// </summary>
    /// <param name="byteString"><see cref="ByteString"/> to search.</param>
    /// <param name="byteToFind"><see cref="ByteString"/> to be searched.</param>
    /// <returns></returns>
    public static bool Contains(this ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) != -1;
    }
}
