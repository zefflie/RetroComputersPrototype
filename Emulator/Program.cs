namespace Retro.Emulator;

class Program
{
    static void Main(string[] args)
    {
        string? fromfile = null;
        string option = "";

        foreach (var arg in args)
        {
            if (arg.StartsWith("-"))
            {
                if (option.Length > 0) throw new ArgumentException($"Option '{option}' requires parameter");
                switch (arg)
                {
                    case "-f": option = arg; break;
                    default: throw new ArgumentException($"Unknown '{arg}' option");
                }

                continue;
            }

            switch (option)
            {
                case "-f": fromfile = arg; break;
            }
        }

        

        var pc = new Chipset();
        pc.MountDevice(new Videoadapter());

        if (fromfile != null)
        {
            var rom = new ROMDevice(0, fromfile);
            pc.MountDevice(rom);
        }

        pc.Mainloop();
    }
}


