using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class SyntaxTests
{
    [TestMethod]
    public void CSharpSyntaxProbe()
    {
        // Basic operations
        Helper.TestCodeBlock("var a = 1+1;");
        Helper.TestCodeBlock("var b = \"string test\";\nb.ToString();");
        Helper.TestCodeBlock("var c = new object();\nc.ToString();");

        // String method tests
        Helper.TestCodeBlock(@"
        string str1 = ""Hello"";
        string str2 = ""World"";
        int result = string.Compare(str1, str2);
        bool contains = str1.Contains(""el"");
        bool endsWith = str1.EndsWith(""lo"");
        int index = str1.IndexOf(""l"");
        string inserted = str1.Insert(5, "" "");
        string joined = string.Join("", "", new string[] { str1, str2 });
        string padded = str1.PadLeft(10);
        string replaced = str1.Replace(""l"", ""L"");
        string[] split = ""a,b,c"".Split(',');
        string sub = str1.Substring(1, 3);
        string lower = str1.ToLower();
        string upper = str1.ToUpper();
        string trimmed = "" Hello "".Trim();
    ");

        // String property tests
        Helper.TestCodeBlock(@"
        string str = ""Test String"";
        int length = str.Length;
        char firstChar = str[0];
    ");

        // Static fields and methods tests
        Helper.TestCodeBlock(@"
        string empty = string.Empty;
        bool isNullOrEmpty = string.IsNullOrEmpty("""");
        bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace("" "");
    ");

        // String interpolation and verbatim string literals
        Helper.TestCodeBlock(@"
        int x = 10;
        string interpolated = $""The value of x is {x}"";
        string verbatim = @""This is a
multiline string"";
    ");

        // String constants and escape sequences
        Helper.TestCodeBlock(@"
        string nullString = null;
        string emptyString = """";
        string whiteSpaceString = ""   "";
        string withEscapes = ""\tHello\nWorld!"";
    ");

        // More complex string operations
        Helper.TestCodeBlock(@"
        string original = ""Hello, World!"";
        string modified = original.Replace(""World"", ""C#"")
                                  .ToUpper()
                                  .Substring(0, 8)
                                  .PadRight(10, '!')
                                  .Trim('!');
        bool startsWith = modified.StartsWith(""HELLO"");
        int lastIndex = modified.LastIndexOf('L');
    ");
    }
}
