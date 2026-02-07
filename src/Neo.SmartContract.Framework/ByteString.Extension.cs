// Copyright (C) 2015-2026 The Neo Project.
//
// ByteString.Extension.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Native;
namespace Neo.SmartContract.Framework;

public static class ByteStringExtension
{
    /// <summary>
    ///  Denotes whether provided character is a number.
    /// </summary>
    /// <param name="byteString">Input to check. It cannot be null.</param>
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
    /// <param name="byteString">Input to check. It cannot be null.</param>
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
    /// <param name="byteString">Input to check. It cannot be null.</param>
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
    /// <param name="byteString">Input to check. It cannot be null.</param>
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
    /// <param name="byteString">ByteString where to search. It cannot be null.</param>
    /// <param name="toFind">ByteString to search.</param>
    /// <returns>Index where it is located or -1</returns>
    public static int IndexOf(this ByteString byteString, ByteString toFind)
    {
        return StdLib.MemorySearch(byteString, toFind);
    }

    /// <summary>
    /// Returns the index of the last occurrence of a given value in an array.
    /// </summary>
    /// <param name="byteString">ByteString where to search. It cannot be null.</param>
    /// <param name="toFind">ByteString to search. It cannot be null.</param>
    /// <returns>Index where it is located or -1</returns>
    public static int LastIndexOf(this ByteString byteString, ByteString toFind)
    {
        return StdLib.MemorySearch(byteString, toFind, byteString.Length, true);
    }

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.
    /// </summary>
    /// <param name="byteString">ByteString where to search. It cannot be null.</param>
    /// <param name="toFind">ByteString to search. It cannot be null.</param>
    /// <returns>True if start with</returns>
    [Obsolete("Use StartsWith instead.")]
    public static bool StartWith(this ByteString byteString, ByteString toFind)
    {
        return Helper.NumEqual(StdLib.MemorySearch(byteString, toFind), 0);
    }

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string.
    /// </summary>
    /// <param name="byteString">ByteString where to search. It cannot be null.</param>
    /// <param name="toFind">ByteString to search. It cannot be null.</param>
    /// <returns>True if starts with</returns>
    public static bool StartsWith(this ByteString byteString, ByteString toFind)
    {
        return Helper.NumEqual(StdLib.MemorySearch(byteString, toFind), 0);
    }

    /// <summary>
    /// Determines whether the end of this string instance matches a specified string.
    /// </summary>
    /// <param name="byteString">ByteString where to search. It cannot be null.</param>
    /// <param name="toFind">ByteString to search. It cannot be null.</param>
    /// <returns>True if ends with</returns>
    public static bool EndsWith(this ByteString byteString, ByteString toFind)
    {
        if (toFind.Length == 0) return true;
        int startIndex = byteString.Length - toFind.Length;
        if (startIndex < 0) return false;
        return Helper.NumEqual(StdLib.MemorySearch(byteString, toFind, startIndex), startIndex);
    }

    /// <summary>
    /// Determines whether the end of this string instance matches a specified string.
    /// </summary>
    /// <param name="byteString">ByteString where to search. It cannot be null.</param>
    /// <param name="toFind">ByteString to search. It cannot be null.</param>
    /// <returns>True if ends with</returns>
    [Obsolete("Use EndsWith instead.")]
    public static bool EndWith(this ByteString byteString, ByteString toFind)
    {
        return byteString.EndsWith(toFind);
    }

    /// <summary>
    /// Checks if the <see cref="ByteString"/> contains the given <see cref="ByteString"/>.
    /// </summary>
    /// <param name="byteString"><see cref="ByteString"/> to search. It cannot be null.</param>
    /// <param name="toFind"><see cref="ByteString"/> to be searched. It cannot be null.</param>
    /// <returns></returns>
    public static bool Contains(this ByteString byteString, ByteString toFind)
    {
        return StdLib.MemorySearch(byteString, toFind) != -1;
    }

    /// <summary>
    /// Splits a ByteString into an array of ByteString based on a separator.
    /// </summary>
    /// <param name="byteString">ByteString to split. It cannot be null.</param>
    /// <param name="separator">ByteString to split by. It cannot be null.</param>
    /// <param name="removeEmptyEntries">Whether to remove empty entries from the result. The default value is false.</param>
    /// <returns>An array of ByteString.</returns>
    public static ByteString[] Split(this ByteString byteString, ByteString separator, bool removeEmptyEntries = false)
    {
        return StdLib.StringSplit(byteString, separator, removeEmptyEntries);
    }

    /// <summary>
    /// Returns the text element count of the ByteString(not byte count).
    /// For example, "🦆" = 1 text element, "Hello" = 5 text elements.
    /// </summary>
    /// <param name="byteString">ByteString to get the text element count of. It cannot be null.</param>
    /// <returns>The text element count of the ByteString.</returns>
    public static int TextElementCount(this ByteString byteString)
    {
        return StdLib.StrLen(byteString);
    }
}
