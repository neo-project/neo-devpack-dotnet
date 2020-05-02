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

    [Flags]
    public enum OptimizeParserType
    {
        NONE = 0x00,

        [OptimizeParser(typeof(Parser_DeleteDeadCode))]
        DELETE_DEAD_CODE = 0x01,

        [OptimizeParser(typeof(Parser_DeleteNop))]
        DELETE_NOP = 0x02,

        [OptimizeParser(typeof(Parser_DeleteUselessJmp))]
        DELETE_USELESS_JMP = 0x04,

        [OptimizeParser(typeof(Parser_UseShortAddress))]
        USE_SHORT_ADDRESS = 0x08,

        [OptimizeParser(typeof(Parser_DeleteUselessEqual))]
        DELETE_USELESS_EQUAL = 0x10,

        [OptimizeParser(typeof(Parser_DeleteConstExecution))]
        DELETE_CONST_EXECUTION = 0x20,

        ALL = DELETE_DEAD_CODE | DELETE_USELESS_JMP | USE_SHORT_ADDRESS | DELETE_USELESS_EQUAL | DELETE_CONST_EXECUTION,
    }
}
