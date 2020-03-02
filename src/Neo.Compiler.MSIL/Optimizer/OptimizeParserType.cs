using System;

namespace Neo.Compiler.Optimizer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class OptimizeParserAttribute : Attribute
    {
        public Type Type { get; }

        public OptimizeParserAttribute(Type type)
        {
            Type = type;
        }
    }

    public enum OptimizeParserType : byte
    {
        [OptimizeParser(typeof(Parser_DeleteDeadCode))]
        DELETE_DEAD_CODDE = 0x01,

        [OptimizeParser(typeof(Parser_DeleteNop))]
        DELETE_NOP = 0x02,

        [OptimizeParser(typeof(Parser_DeleteUselessJmp))]
        DELETE_USERLESS_JMP = 0x03,

        [OptimizeParser(typeof(Parser_UseShortAddress))]
        USE_SHORT_ADDRESS = 0x04,
    }
}
