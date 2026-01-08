namespace Retro.Emulator;

public class ROM : IMemory
{
    protected byte[] data;
    private bool writableForInit;

    public uint Size => (uint)data.Length;

    // Конструктор с выравниванием размера
    public ROM(string filePath, bool writableForInitialization = false, uint? alignToSize = null)
    {
        var fileData = File.ReadAllBytes(filePath);
        InitializeFromArray(fileData, writableForInitialization, alignToSize);
    }

    // Конструктор с выравниванием из массива
    public ROM(byte[] initialData, bool writableForInitialization = false, uint? alignToSize = null)
    {
        InitializeFromArray(initialData, writableForInitialization, alignToSize);
    }

    // Конструктор для создания пустого ROM определенного размера
    public ROM(uint size, bool writableForInitialization = false)
    {
        data = new byte[size];
        writableForInit = writableForInitialization;
    }

    private void InitializeFromArray(byte[] sourceData, bool writableForInitialization, uint? alignToSize)
    {
        writableForInit = writableForInitialization;

        if (alignToSize.HasValue)
        {
            // Вычисляем размер с выравниванием
            uint alignedSize = alignToSize.Value;
            if (sourceData.Length > alignedSize)
            {
                throw new ArgumentException(
                    $"Source data size ({sourceData.Length}) exceeds aligned size ({alignedSize})");
            }

            data = new byte[alignedSize];
            Buffer.BlockCopy(sourceData, 0, data, 0, sourceData.Length);

            for (int i = sourceData.Length; i < alignedSize; i++)
            {
                data[i] = 0;
            }
        }
        else
        {
            // Без выравнивания - просто копируем
            data = new byte[sourceData.Length];
            Buffer.BlockCopy(sourceData, 0, data, 0, sourceData.Length);
        }
    }

    public byte Read(uint address) => data[address];

    public void Write(uint address, byte value)
    {
        if (!writableForInit)
            throw new InvalidOperationException("ROM is read-only");

        data[address] = value;
    }

    public byte[] ReadBytes(uint address, int length)
    {
        if (address + length > data.Length)
            throw new ArgumentOutOfRangeException(nameof(address));

        byte[] result = new byte[length];
        Buffer.BlockCopy(data, (int)address, result, 0, length);
        return result;
    }

    public void WriteBytes(uint address, byte[] bytes)
    {
        if (!writableForInit)
            throw new InvalidOperationException("ROM is read-only");

        if (address + bytes.Length > data.Length)
            throw new ArgumentOutOfRangeException(nameof(address));

        Buffer.BlockCopy(bytes, 0, data, (int)address, bytes.Length);
    }

    public void Lock() => writableForInit = false;

    // Дополнительные полезные методы
    public ushort ReadWord(uint address)
    {
        return (ushort)(Read(address) | (Read(address + 1) << 8));
    }

    public uint ReadDWord(uint address)
    {
        return (uint)(Read(address) | (Read(address + 1) << 8) |
                      (Read(address + 2) << 16) | (Read(address + 3) << 24));
    }
}