namespace Retro.Emulator;

class Program
{
    static void Main(string[] args)
    {
        var rom = new ROMDevice(100, "C:\\Zefflie\\Projects\\RetroComputersPrototype\\test.bin");

        var pc = new Chipset();
        pc.MountDevice(rom);
        pc.MountDevice(new Videoadapter());
        pc.Mainloop();
    }
}


