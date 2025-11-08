namespace Neo.Compiler.LIR;

/// <summary>
/// NeoVM-aligned opcode surface understood by the low-level IR. Each entry maps one-to-one to a concrete NeoVM opcode
/// or syscall and carries stack effect metadata used by schedulers and verifiers.
/// </summary>
internal enum LirOpcode
{
    // Constants
    PUSH0,
    PUSHM1,
    PUSHT,
    PUSHF,
    PUSHNULL,
    PUSHINT,
    PUSHDATA1,
    PUSHDATA2,
    PUSHDATA4,

    // Stack manipulation
    DROP,
    DUP,
    OVER,
    SWAP,
    ROT,
    PICK,
    ROLL,
    REVERSEN,
    NIP,
    TUCK,
    ISNULL,
    CONVERT,
    INITSSLOT,
    INITSLOT,

    // Arithmetic / Bitwise
    ADD,
    SUB,
    MUL,
    DIV,
    MOD,
    NEG,
    ABS,
    SIGN,
    INC,
    DEC,
    SQRT,
    AND,
    OR,
    XOR,
    NOT,
    SHL,
    SHR,
    NUMEQUAL,
    NUMNOTEQUAL,
    GT,
    LT,
    GTE,
    LTE,
    WITHIN,
    MAX,
    MIN,
    POW,
    MODMUL,
    MODPOW,

    // Bytes
    CAT,
    SUBSTR,
    LEFT,
    RIGHT,

    // Containers
    NEWARRAY,
    NEWSTRUCT,
    NEWMAP,
    GETITEM,
    SETITEM,
    REMOVE,
    APPEND,
    PACK,
    PACKSTRUCT,
    UNPACK,
    KEYS,
    VALUES,
    LENGTH,
    HASKEY,
    NEWBUFFER,
    MEMCPY,

    // Static fields
    LDSFLD,
    STSFLD,
    LDARG,
    STARG,
    LDLOC,
    STLOC,

    // Control flow
    JMP,
    JMPIF,
    JMPIFNOT,
    JMPEQ,
    JMPNE,
    JMPGT,
    JMPGE,
    JMPLT,
    JMPLE,
    CALL,
    CALLA,
    CALLT,
    TRY_L,
    ENDTRY_L,
    ENDFINALLY,
    RET,
    ASSERT,

    // System
    SYSCALL,

    // Termination
    ABORT,
    ABORTMSG
}
