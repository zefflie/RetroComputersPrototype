namespace Retro.Emulator;

public class RAM : IMemory
{
    private byte[] Data;

    public uint Size { get; private set; }

    public RAM(uint size)
    {
        Data = new byte[size];
        Size = size;
    }

    public byte Read(uint address) => Data[address];

    public void Write(uint address, byte value) => Data[address] = value;

    public byte[] ReadBytes(uint address, int length)
    {
        if (address + length > Data.Length) throw new ArgumentOutOfRangeException("Address out of memory bounds");
        byte[] result = new byte[length];
        Buffer.BlockCopy(Data, (int)address, result, 0, length);
        return result;
    }

    public void WriteBytes(uint address, byte[] bytes)
    {
        if (address + bytes.Length > Data.Length) throw new ArgumentOutOfRangeException("Address out of memory bounds");
        Buffer.BlockCopy(bytes, 0, Data, (int)address, bytes.Length);
    }
}