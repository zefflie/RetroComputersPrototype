namespace Retro.Emulator;

class Program
{
    static void Main(string[] args)
    {
        var rom = new ROMDevice(100, [
            0b00100001,
            0x00,
            0xB0,

            0b00110110,
            (byte)'H',

            0b01110110,
        ]);

        var pc = new Chipset();
        pc.MountDevice(rom);
        pc.MountDevice(new Videoadapter());
        pc.Mainloop();
    }
}


