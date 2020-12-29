using System;

namespace NeoTestHarness
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CheckpointPathAttribute : Attribute
    {
        public string Path { get; private set; } = string.Empty;

        public CheckpointPathAttribute(string path)
        {
            Path = path;
        }
    }
}

