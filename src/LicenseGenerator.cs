using LicenseGenerator.domain.model;
using LicenseGenerator.utils;

namespace LicenseGenerator;

public class LicenseGenerator(string filename)
{
    private string Filename { get; set; } = filename;

    public void CreateLicense(License license)
    {
        string path = AdjustPath.AddHomeDirectory(license.Path);
        string absoluteFilename = $"{path}/{license.Filename}";
        string body = InsertAuthors.Insert(license.Authors, license.Body);
        body = InsertCurrentYear.Insert(body);

        if (!File.Exists(absoluteFilename))
            try
            {
                FileStream file = File.Create(absoluteFilename);
                file.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("The specified directory was not found. Could not create license file.");
                return;
            }

        if (File.ReadAllText(absoluteFilename).Length == 0 && !ShouldOverrideFile()) return;

        File.WriteAllText(absoluteFilename, body);
    }

    private static bool ShouldOverrideFile()
    {
        Console.Write("File has contents. Do you wish to override? [y/N] \n> ");
        string? option = Console.ReadLine();

        return option != null && option.ToLower().Contains('y');
    }
}
