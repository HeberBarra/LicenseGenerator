namespace LicenseGenerator.utils;

public static class ClearTerminalScreen
{
    public static void Clear()
    {
        Console.Clear();
        Console.WriteLine("\e[3J");
        Console.Clear();
    }
}
