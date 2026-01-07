namespace Retro.Emulator;

// Недоделка! Пока просто проверяет обращения к буферу и обновляет консоль
public class Videoadapter : IDevice
{
    public readonly uint PageSize = 2048;

    public uint Address;

    public Chipset Chipset;

    public Videoadapter(uint address = 0xB000)
    {
        Address = address;
    }

    void IDevice.AssignChipset(Chipset chipset)
    {
        Chipset = chipset;
    }

    bool IDevice.CheckMemory(uint address) => Address <= address && address < Address + PageSize;

    bool IDevice.CheckPort(byte port) => false;

    byte IDevice.Read(uint address) => Chipset.RAM.Read(address);

    byte IDevice.ReadPort(byte port) => 0;
    void IDevice.Step() {}

    void IDevice.Write(uint address, byte value)
    {
        uint pos = address - Address;
        Console.SetCursorPosition((int)(pos % 80), (int)(pos / 80));
        Console.Write((char)value);
        Chipset.RAM.Write(address, value);
    }

    void IDevice.WritePort(byte port, byte value) {}
}
