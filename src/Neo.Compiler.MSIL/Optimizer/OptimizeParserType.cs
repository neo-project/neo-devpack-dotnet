using System;

namespace Neo.Compiler.Optimizer
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class OptimizeParserAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }

        public OptimizeParserAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum OptimizeParserType : byte
    {
        [OptimizeParser("deletedeadcode", typeof(Parser_DeleteDeadCode))]
        DELETE_DEAD_CODDE = 0x01,

        [OptimizeParser("deletenop", typeof(Parser_DeleteNop))]
        DELETE_NOP = 0x02,

        [OptimizeParser("deleteuselessjmp", typeof(Parser_DeleteUselessJmp))]
        DELETE_USERLESS_JMP = 0x03,

        [OptimizeParser("useshortaddress", typeof(Parser_UseShortAddress))]
        USE_SHORT_ADDRESS = 0x04,
    }
}
