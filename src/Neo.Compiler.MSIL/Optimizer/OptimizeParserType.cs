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
        DELETE_DEAD_CODE = 0x01,

        [OptimizeParser(typeof(Parser_DeleteNop))]
        DELETE_NOP = 0x02,

        [OptimizeParser(typeof(Parser_DeleteUselessJmp))]
        DELETE_USELESS_JMP = 0x03,

        [OptimizeParser(typeof(Parser_UseShortAddress))]
        USE_SHORT_ADDRESS = 0x04,

        [OptimizeParser(typeof(Parser_DeleteUselessEqual))]
        DELETE_USELESS_EQUAL = 0x05,

        [OptimizeParser(typeof(Parser_DeleteStaticMath))]
        DELETE_STATIC_MATH = 0x06
    }
}
