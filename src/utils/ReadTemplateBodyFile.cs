namespace LicenseGenerator.utils;

public static class ReadTemplateBodyFile
{
    public static string? Read()
    {
        while (true)
        {
            Console.Write("Template body filepath: \n> ");
            string? filepath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filepath)) return null;

            if (!File.Exists(filepath))
            {
                Console.WriteLine($"Error! File {filepath} doesn't exists. Try again...");
                continue;
            }

            if (File.ReadAllLines(filepath).Length != 0) return string.Join("\n", File.ReadAllLines(filepath));

            Console.WriteLine($"Error! File {filepath} is empty. Try again...");
        }
    }
}
