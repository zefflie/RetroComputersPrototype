namespace Retro.Emulator;

public class ROMDevice : ROM, IDevice
{
    public uint Address {  get; private set; }

    public ROMDevice(uint address, string filePath, bool unlocked = false, uint? alignToSize = null) : base(filePath, unlocked, alignToSize)
    {
        Address = address;
    }

    public ROMDevice(uint address, byte[] data, bool unlocked = false, uint? alignToSize = null) : base(data, unlocked, alignToSize)
    {
        Address = address;
    }

    public ROMDevice(uint address, uint size, bool unlocked = false) : base(size, unlocked)
    {
        Address = address;
    }

    public bool CheckMemory(uint address) => Address <= address && address < Address + Size;

    public bool CheckPort(byte port) => false;

    public byte ReadPort(byte port) => 0;

    public void WritePort(byte port, byte value) {}

    public void AssignChipset(Chipset chipset) {}

    public void Step() {}

    public new byte Read(uint address)
    {
        return base.Read(address - Address);
    }

    public new void Write(uint address, byte value)
    {
        base.Write(address - Address, value);
    }
}
