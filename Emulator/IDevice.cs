namespace Retro.Emulator;

public interface IDevice
{
    public bool CheckMemory(uint address);
    public byte Read(uint address);
    public void Write(uint address, byte value);
    public bool CheckPort(byte port);
    public byte ReadPort(byte port);
    public void WritePort(byte port, byte value);
    public void AssignChipset(Chipset chipset);
    public void Step();
}