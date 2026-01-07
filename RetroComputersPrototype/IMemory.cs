namespace Retro.Emulator;

public interface IMemory
{
    uint Size { get; }
    byte Read(uint address);
    void Write(uint address, byte value);
    byte[] ReadBytes(uint address, int length);
    void WriteBytes(uint address, byte[] bytes);
}