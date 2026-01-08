namespace Retro.Nyassembler.Enums;

public enum Instruction
{
    MOV, MVI,
    LXI, LDA, LHLD, LDAX,
    XCHG,
    STA, SHLD, STAX,

    ADD, ADI,
    ADC, ACI,
    SUB, SUI,
    SBB, SBI,
    ANA, ANI,
    XRA, XRI,
    ORA, ORI,
    CMP, CPI,
    INR, INX,
    DCR, DCX,
    DAD,
    RLC, RRC, RAL, RAR,
    DAA,
    CMA, STC, CMC,

    PCHL,
    JMP, JNZ, JZ, JNC, JC, JPO, JPE, JP, JM,
    CALL, CNZ, CZ, CNC, CC, CPO, CPE, CP, CM,
    RET, RNZ, RZ, RNC, RC, RPO, RPE, RP, RM,
    RST,
    PUSH, POP,
    XTHL, SPHL,
    IN, OUT,
    EI, DI,
    HLT,
    NOP,

    Unknown,
}
