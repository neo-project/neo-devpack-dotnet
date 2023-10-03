using Neo.SmartContract.Framework.Native;
namespace Neo.SmartContract.Framework;

public class Regex
{
    public static bool NumberOnly(ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 48 or > 57)
                return false;
        }
        return true;
    }

    public static bool AlphabetOnly(ByteString byteString)
    {
        foreach (var value in byteString)
        {
            if (value is < 65 or > 122)
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
    public static int IndexOf(ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind);
    }

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>True if start with</returns>
    public static bool StartWith(ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) == 0;
    }

    /// <summary>
    /// Determines whether the end of this string instance matches a specified string.
    /// </summary>
    /// <param name="byteString">Array where to search.</param>
    /// <param name="byteToFind">Array to search.</param>
    /// <returns>True if ends with</returns>
    public static bool EndsWith(ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) + byteToFind.Length == byteString.Length;
    }

    public static bool Contains(ByteString byteString, ByteString byteToFind)
    {
        return StdLib.MemorySearch(byteString, byteToFind) != -1;
    }
}
