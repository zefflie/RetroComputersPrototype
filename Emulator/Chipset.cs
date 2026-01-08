namespace Retro.Emulator;

public class Chipset
{
    public RAM RAM;
    public I8080 CPU;
    public List<IDevice> Devices = [];
    public bool InterruptPending = false;
    public byte InterruptInstruction = 0;

    public Chipset(uint ramsize = 0x10000)
    {
        RAM = new RAM(ramsize);
        CPU = new I8080(this);
    }

    public byte Read(uint address)
    {
        foreach (var device in Devices) 
        {   
            if (device.CheckMemory(address)) return device.Read(address);
        }

        return RAM.Read(address);
    }

    public void Write(uint address, byte value)
    {
        foreach (var device in Devices)
        {
            if (device.CheckMemory(address))
            {
                device.Write(address, value);
                return;
            }
        }

        RAM.Write(address, value);
    }

    public byte ReadPort(byte port)
    {
        foreach (var device in Devices)
        {
            if (device.CheckPort(port)) return device.ReadPort(port);
        }

        return 0;
    }

    public void WritePort(byte port, byte value)
    {
        foreach (var device in Devices)
        {
            if (device.CheckPort(port)) device.WritePort(port, value);
        }
    }

    public void Interrupt(byte instruction)
    {
        if (CPU.InterruptsEnable)
        {
            InterruptInstruction = instruction;
            InterruptPending = true;
        }
    }

    public void MountDevice(IDevice device)
    {
        Devices.Add(device);
        device.AssignChipset(this);
    }

    public void Mainloop()
    {
        CPU.Reset();

        while (true)
        {
            int ticks = CPU.Step();
            while (ticks > 0) ticks--;
        }
    }
}