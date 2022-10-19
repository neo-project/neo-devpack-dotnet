using System;
using System.IO;

namespace build_tasks
{
    public partial class TestBuild
    {
        internal class TestRootPath : IDisposable
        {
            readonly string Value;
            readonly bool deleteOnDispose;

            public TestRootPath(string root = "")
            {
                deleteOnDispose = string.IsNullOrEmpty(root);
                root = string.IsNullOrEmpty(root)
                    ? Path.GetTempPath()
                    : root;
                Value = Path.Combine(root, Path.GetRandomFileName());
                Directory.CreateDirectory(Value);
            }

            public void Dispose()
            {
                if (deleteOnDispose && Directory.Exists(Value)) Directory.Delete(Value, true);
            }

            public static implicit operator string(TestRootPath p) => p.Value;
        }

    }
}
