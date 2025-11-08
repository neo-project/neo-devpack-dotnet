namespace Neo.Compiler.HIR;

internal sealed record HirSourceInfoAttribute(
    string File,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn) : HirAttribute
{
    public override string ToString()
        => $"{File}:{StartLine}:{StartColumn}-{EndLine}:{EndColumn}";
}
