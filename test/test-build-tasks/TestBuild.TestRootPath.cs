using System;
using System.IO;

namespace build_tasks
{
    public partial class TestBuild
    {
        class TestRootPath : IDisposable
        {
            readonly string Value;

            public TestRootPath()
            {
                Value = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(Value);
            }

            public void Dispose()
            {
                if (Directory.Exists(Value)) Directory.Delete(Value, true);
            }

            public static implicit operator string(TestRootPath p) => p.Value;
        }

    }
}
