namespace Neo.Compiler.HIR;

internal sealed record SourceSpan(
    string File,
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn)
{
    public override string ToString()
        => $"{File}:{StartLine}:{StartColumn}-{EndLine}:{EndColumn}";
}

