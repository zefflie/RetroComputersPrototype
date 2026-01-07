namespace Retro.Emulator;

public class I8080
{
    public readonly int Frequency = 2_000_000;

    private byte[] Registers = new byte[8];
    public ushort SP = 0xFFFF;
    public ushort PC = 0;
    public byte IR = 0;
    public bool Halted = false;
    public bool InterruptsEnable = true;

    public byte B { get { return Registers[0]; } set { Registers[0] = value; } }
    public byte C { get { return Registers[1]; } set { Registers[1] = value; } }
    public byte D { get { return Registers[2]; } set { Registers[2] = value; } }
    public byte E { get { return Registers[3]; } set { Registers[3] = value; } }
    public byte H { get { return Registers[4]; } set { Registers[4] = value; } }
    public byte L { get { return Registers[5]; } set { Registers[5] = value; } }

    public byte Flags { get { return Registers[6]; } set { Registers[6] = value; } }

    public byte A { get { return Registers[7]; } set { Registers[7] = value; } }

    public ushort PSW
    {
        get => (ushort)((Registers[7] << 8) | Registers[6]);
        set { Registers[7] = (byte)(value >> 8); Registers[6] = (byte)(value & 0xFF); }
    }

    public ushort BC
    {
        get => (ushort)((Registers[1] << 8) | Registers[0]);
        set { Registers[1] = (byte)(value >> 8); Registers[0] = (byte)(value & 0xFF); }
    }

    public ushort DE
    {
        get => (ushort)((Registers[3] << 8) | Registers[2]);
        set { Registers[3] = (byte)(value >> 8); Registers[2] = (byte)(value & 0xFF); }
    }

    public ushort HL
    {
        get => (ushort)((Registers[5] << 8) | Registers[4]);
        set { Registers[5] = (byte)(value >> 8); Registers[4] = (byte)(value & 0xFF); }
    }

    public byte FlagC
    {
        get => (byte)(Registers[6] & 1);
        set => Registers[6] = (byte)(Registers[6] & 0b11111110 | value & 1);
    }

    public byte FlagP
    {
        get => (byte)((Registers[6] >> 2) & 1);
        set => Registers[6] = (byte)(Registers[6] & 0b11111011 | (value & 1) << 2);
    }

    public byte FlagAC
    {
        get => (byte)((Registers[6] >> 4) & 1);
        set => Registers[6] = (byte)(Registers[6] & 0b11101111 | (value & 1) << 4);
    }

    public byte FlagZ
    {
        get => (byte)((Registers[6] >> 6) & 1);
        set => Registers[6] = (byte)(Registers[6] & 0b10111111 | (value & 1) << 6);
    }

    public byte FlagS
    {
        get => (byte)((Registers[6] >> 7) & 1);
        set => Registers[6] = (byte)(Registers[6] & 0b01111111 | (value & 1) << 7);
    }

    public Chipset Chipset;

    public I8080(Chipset chipset)
    {
        Chipset = chipset;
    }

    public byte ReadByte(uint address) => Chipset.Read(address);

    public void WriteByte(uint address, byte value) => Chipset.Write(address, value);

    public ushort ReadShort(uint address) => (ushort)(Chipset.Read(address + 1) << 8 | Chipset.Read(address));

    public void WriteShort(uint address, ushort value)
    {
        WriteByte(address, (byte)(value & 0xFF));
        WriteByte(address + 1, (byte)(value >> 8));
    }

    public byte ReadSource(byte sss)
    {
        if (sss > 7) throw new ArgumentException("Out of SSS bounds");
        if (sss == 6) return ReadByte(HL);
        else return Registers[sss];
    }

    public void WriteDestination(byte ddd, byte value)
    {
        if (ddd > 7) throw new ArgumentException("Out of DDD bounds");
        if (ddd == 6) WriteByte(HL, value);
        else Registers[ddd] = value;
    }

    public ushort ReadRP(byte rp) {
        return rp switch
        {
            0 => BC,
            1 => DE,
            2 => HL,
            3 => PSW,
            _ => throw new ArgumentException("Out of RP bounds")
        };
    }

    public void WriteRP(byte rp, ushort value)
    {
        switch (rp)
        {
            case 0: BC = value; break;
            case 1: DE = value; break;
            case 2: HL = value; break;
            case 3: PSW = value; break;
            default: throw new ArgumentException("Out of RP bounds");
        }
    }

    public byte FetchByte() => ReadByte(PC++);

    public ushort FetchShort() { 
        var result = ReadShort(PC);
        PC += 2;
        return result;
    }

    public byte ReadPort(byte port) => Chipset.ReadPort(port);

    public void WritePort(byte port, byte value) => Chipset.WritePort(port, value);

    public void UpdateFlags(byte result)
    {
        // S, Z, 0, AC, 0, P, 1, C
        Flags = (byte)(Flags & 0b10001 | 2);
        int bits = 0;
        for (int i = 0; i < 8; i++) bits += ((result >> i) & 1);
        FlagP = (byte)~(bits & 1);
        FlagZ = (byte)(result == 0 ? 1 : 0);
        FlagS = (byte)((result >> 7) & 1);
    }

    public void ALUAdd(byte value, bool use_carry)
    {
        int carry = use_carry ? FlagC : 0;
        int result = A + value + carry;
        FlagC = (byte)(result > 0xFF ? 1 : 0);
        FlagAC = (byte)(((A & 0x0F) + (value & 0x0F) + carry) > 0x0F ? 1 : 0);
        A = (byte)result;
        UpdateFlags(A);
    }

    public void ALUSub(byte value, bool use_carry, bool write_result)
    {
        int carry = use_carry ? FlagC : 0;
        int result = A - value - carry;
        FlagC = (byte)(result < 0 ? 1 : 0);
        FlagAC = (byte)(((A & 0x0F) - (value & 0x0F) - carry) < 0 ? 1 : 0);
        if (write_result) A = (byte)result;
        UpdateFlags((byte)result);
    }

    public void ALUAnd(byte value)
    {
        A = (byte)(A & value);
        FlagC = 0;
        FlagAC = 1;
        UpdateFlags(A);
    }

    public void ALUXor(byte value)
    {
        A = (byte)(A ^ value);
        FlagC = 0;
        FlagAC = 0;
        UpdateFlags(A);
    }

    public void ALUOr(byte value)
    {
        A = (byte)(A | value);
        FlagC = 0;
        FlagAC = 0;
        UpdateFlags(A);
    }

    public void ALUInc(byte ddd)
    {
        byte value = ReadSource(ddd);
        int result = value + 1;
        FlagAC = (byte)(((value & 0x0F) + 1) > 0x0F ? 1 : 0);
        WriteDestination(ddd, (byte)result);
        UpdateFlags((byte)result);
    }

    public void ALUDec(byte ddd)
    {
        byte value = ReadSource(ddd);
        int result = value - 1;
        FlagAC = (byte)(((value & 0x0F) - 1) < 0 ? 1 : 0);
        WriteDestination(ddd, (byte)result);
        UpdateFlags((byte)result);
    }

    public bool CheckCondition(byte ccc)
    {
        return ccc switch
        {
            0 => FlagZ == 0,
            1 => FlagZ == 1,
            2 => FlagC == 0,
            3 => FlagC == 1,
            4 => FlagP == 0,
            5 => FlagP == 1,
            6 => FlagS == 0,
            7 => FlagS == 1,
            _ => throw new IndexOutOfRangeException()
        };
    }

    public int Step()
    {
        if (InterruptsEnable && Chipset.InterruptPending)
        {
            Chipset.InterruptPending = false;
            InterruptsEnable = false;
            Halted = false;

            IR = Chipset.InterruptInstruction;
        }
        else
        {
            if (Halted) return 4;
            IR = FetchByte();
        }

        byte kop1 = (byte)((IR >> 6) & 0b11); // KKxxxxxx
        byte kop2 = (byte)(IR & 0b1111);      // xxxxKKKK
        byte rp = (byte)((IR >> 4) & 0b11);   // xxRPxxxx
        byte ddd = (byte)((IR >> 3) & 0b111); // xxDDDxxx
        byte sss = (byte)(IR & 0b111);        // xxxxxSSS

        int ticks = 0;

        // xxxxxxxx ==========================================================

        var done = true;

        switch (IR)
        {
            // LDA [imm16] => A
            case 0b00111010: ticks = 13; A = ReadByte(FetchShort()); break;
            // LHLD [imm16] => HL
            case 0b00101010: ticks = 16; HL = ReadShort(FetchShort()); break;
            // XCHG HL <=> DE
            case 0b11101011: { ticks = 4; var tmp = HL; HL = DE; DE = tmp; break; }
            // STA A => [imm16]
            case 0b00110010: ticks = 13; WriteByte(FetchShort(), A); break;
            // SHLD HL => [imm16]
            case 0b00100010: ticks = 16; WriteShort(FetchShort(), HL); break;
            // ADI A + [imm8] => A
            case 0b11000110: ticks = 7; ALUAdd(FetchByte(), false); break;
            // ACI A + [imm8] + carry => A
            case 0b11001110: ticks = 7; ALUAdd(FetchByte(), true); break;
            // SUI A - [imm8] => A
            case 0b11010110: ticks = 7; ALUSub(FetchByte(), false, true); break;
            // SBI A - [imm8] - carry => A
            case 0b11011110: ticks = 7; ALUSub(FetchByte(), true, true); break;
            // ANI A & [imm8] => A
            case 0b11100110: ticks = 7; ALUAnd(FetchByte()); break;
            // XRI A ^ [imm8] => A
            case 0b11101110: ticks = 7; ALUXor(FetchByte()); break;
            // ORI A | [imm8] => A
            case 0b11110110: ticks = 7; ALUOr(FetchByte()); break;
            // CPI A - [imm8]
            case 0b11111110: ticks = 7; ALUSub(FetchByte(), false, false); break;
            // RLC A << 1
            case 0b00000111:
                {
                    ticks = 4;
                    FlagC = (byte)(A >> 7);
                    A = (byte)((A << 1) | (A >> 7));
                    UpdateFlags(A);
                    break;
                }
            // RRC A >> 1
            case 0b00001111:
                {
                    ticks = 4;
                    FlagC = (byte)(A & 1);
                    A = (byte)((A >> 1) | ((A & 1) << 7));
                    UpdateFlags(A);
                    break;
                }
            // RAL
            case 0b00010111:
                {
                    ticks = 4;
                    int temp = A >> 7;
                    A = (byte)(A << 1 | FlagC);
                    FlagC = (byte)temp;
                    UpdateFlags(A);
                    break;
                }
            // RAR
            case 0b00011111:
                {
                    ticks = 4;
                    int temp = A & 1;
                    A = (byte)((A >> 1) | FlagC << 7);
                    FlagC = (byte)temp;
                    UpdateFlags(A);
                    break;
                }
            // DAA
            case 0b00100111:
                {
                    ticks = 4;
                    byte correction = 0;
                    byte setAC = 0;
                    byte setC = FlagC;

                    if (FlagAC == 1 || (A & 0x0F) > 9)
                    {
                        correction += 0x06;
                        setAC = 1;
                    }
                    if ((setC == 1) || A > 0x99 || ((A & 0x0F) > 9 && (A >> 4) >= 9))
                    {
                        correction += 0x60;
                        setC = 1;
                    }

                    int result = A + correction;
                    A = (byte)result;

                    FlagAC = setAC;
                    FlagC = setC;
                    UpdateFlags(A);
                    break;
                }
            // CMA
            case 0b00101111: ticks = 4; A = (byte)~A; break;
            // STC
            case 0b00110111: ticks = 4; FlagC = 1; break;
            // CMC
            case 0b00111111: ticks = 4; FlagC = (byte)~FlagC; break;
            // JMP imm16 => PC
            case 0b11000011: ticks = 10; PC = FetchShort(); break;
            // PCHL HL => PC
            case 0b11101001: ticks = 5; PC = HL; break;
            // CALL PC => [--SP] ; imm16 => PC
            case 0b11001101:
                ticks = 17;
                SP -= 2;
                WriteShort(SP, PC);
                PC = FetchShort();
                break;
            // RET [SP++] => PC
            case 0b11001001:
                ticks = 10;
                PC = ReadShort(SP);
                SP += 2;
                break;
            // XTHL HL <=> [--SP]
            case 0b11100011:
                {
                    ticks = 18;
                    ushort temp = HL;
                    HL = ReadShort(SP);
                    WriteShort(SP, temp);
                    break;
                }
            // SPHL HL => SP
            case 0b11111001: ticks = 5; SP = HL; break;
            // IN {imm8} => A
            case 0b11011011: ticks = 10; A = ReadPort(FetchByte()); break;
            // OUT A => {imm8}
            case 0b11010011: ticks = 10; WritePort(FetchByte(), A); break;
            // EI
            case 0b11111011: ticks = 4; InterruptsEnable = true; break;
            // DI
            case 0b11110011: ticks = 4; InterruptsEnable = false; break;
            // HLT
            case 0b01110110: ticks = 7; Halted = true; break;
            // NOP
            case 0b00000000: ticks = 4; break;

            default: done = false; break;
        }

        if (done) return ticks;

        // xxRPxxxx ==========================================================

        // LXI imm16 => rp
        if (kop1 == 0b00 && kop2 == 0b0001) { ticks = 10; WriteRP(rp, FetchShort()); }
        // LDAX [rp] => A
        else if (kop1 == 0b00 && kop2 == 0b1010) { ticks = 7; A = ReadByte(ReadRP(rp)); }
        // STAX A => [rp]
        else if (kop1 == 0b00 && kop2 == 0b0010) { ticks = 7; WriteByte(ReadRP(rp), A); }
        // INX rp + 1 => rp
        else if (kop1 == 0b00 && kop2 == 0b0011) { ticks = 5; WriteRP(rp, (ushort)(ReadRP(rp) + 1)); }
        // DCX rp + 1 => rp
        else if (kop1 == 0b00 && kop2 == 0b1011) { ticks = 5; WriteRP(rp, (ushort)(ReadRP(rp) - 1)); }
        // DAD HL + rp => HL
        else if (kop1 == 0b00 && kop2 == 0b1001)
        {
            ticks = 10;
            int result = HL + ReadRP(rp);
            FlagC = (byte)(result > 0xFFFF ? 1 : 0);
            HL = (ushort)result;
        }
        // PUSH rp => [--SP]
        else if (kop1 == 0b11 && kop2 == 0b0101)
        {
            ticks = 11;
            SP -= 2;
            WriteShort(SP, ReadRP(rp));
            PC = FetchShort();
        }
        // POP [SP++] => rp
        else if (kop1 == 0b11 && kop2 == 0b0001)
        {
            ticks = 10;
            WriteRP(rp, ReadShort(SP));
            SP += 2;
        }

        // xxxxxSSS ==========================================================

        // ADD A + r => A
        // ADD A + [HL] => A
        else if (kop1 == 0b10 && ddd == 0b000) { ticks = sss == 7 ? 7 : 4; ALUAdd(ReadSource(sss), false); }
        // ADC A + r + carry => A
        // ADC A + [HL] + carry => A
        else if (kop1 == 0b10 && ddd == 0b001) { ticks = sss == 7 ? 7 : 4; ALUAdd(ReadSource(sss), true); }
        // SUB A - r => A
        // SUB A - [HL] => A
        else if (kop1 == 0b10 && ddd == 0b010) { ticks = sss == 7 ? 7 : 4; ALUSub(ReadSource(sss), false, true); }
        // SBB A - r - carry => A
        // SBB A - [HL] - carry => A
        else if (kop1 == 0b10 && ddd == 0b011) { ticks = sss == 7 ? 7 : 4; ALUSub(ReadSource(sss), true, true); }
        // ANA A & r => A
        // ANA A & [HL] => A
        else if (kop1 == 0b10 && ddd == 0b100) { ticks = sss == 7 ? 7 : 4; ALUAnd(ReadSource(sss)); }
        // XRA A ^ r => A
        // XRA A ^ [HL] => A
        else if (kop1 == 0b10 && ddd == 0b101) { ticks = sss == 7 ? 7 : 4; ALUXor(ReadSource(sss)); }
        // ORA A | r => A
        // ORA A | [HL] => A
        else if (kop1 == 0b10 && ddd == 0b110) { ticks = sss == 7 ? 7 : 4; ALUOr(ReadSource(sss)); }
        // CMP A - r
        // CMP A - [HL]
        else if (kop1 == 0b10 && ddd == 0b111) { ticks = sss == 7 ? 7 : 4; ALUSub(ReadSource(sss), false, false); }

        // xxDDDxxx ==========================================================

        // MVI imm8 => r
        // MVI imm8 => [HL]
        else if (kop1 == 0b00 && sss == 0b110) { ticks = ddd == 7 ? 10 : 7; WriteDestination(ddd, FetchByte()); }
        // INR r + 1 => r
        // INR [HL] + 1 => [HL]
        else if (kop1 == 0b00 && sss == 0b100) { ticks = ddd == 7 ? 10 : 5; ALUInc(ddd); }
        // DCR r - 1 => r
        // DCR [HL] - 1 => [HL]
        else if (kop1 == 0b00 && sss == 0b101) { ticks = ddd == 7 ? 10 : 5; ALUDec(ddd); }
        // Jcc if cc then PC = imm16
        else if (kop1 == 0b11 && sss == 0b010)
        {
            ticks = 7;
            var addr = FetchShort();
            if (CheckCondition(ddd))
            {
                ticks = 10;
                PC = addr;
            }
        }
        // Ccc if cc then PC => [--SP] ; imm16 => PC
        else if (kop1 == 0b11 && sss == 0b100)
        {
            ticks = 11;
            var addr = FetchShort();
            if (CheckCondition(ddd))
            {
                ticks = 17;
                SP -= 2;
                WriteShort(SP, PC);
                PC = addr;
            }
        }
        // Rcc if cc then [SP++] => PC
        else if (kop1 == 0b11 && sss == 0b000)
        {
            ticks = 5;
            if (CheckCondition(ddd))
            {
                ticks = 11;
                PC = ReadShort(SP);
                SP += 2;
            }
        }
        // RST n
        else if (kop1 == 0b11 && sss == 0b111)
        {
            ticks = 11;
            SP -= 2;
            WriteShort(SP, PC);
            PC = (ushort)(ddd * 8);
        }

        // xxDDDSSS ==========================================================

        // MOV r => r
        // MOV r => [HL]
        // MOV [HL] => r
        else if (kop1 == 0b01)
        {
            ticks = sss == 7 || ddd == 7 ? 7 : 5;
            WriteDestination(ddd, ReadSource(sss));
        }

        return ticks;
    }

    public void Reset()
    {
        Registers = new byte[8];
        SP = 0xFFFF;
        PC = 0;
        IR = 0;
        InterruptsEnable = false;
        Halted = false;
    }
}
