using Tomlyn;
using Tomlyn.Model;

namespace LicenseGenerator.config;

public class Configurator(string configurationDirectory)
{
    private const string ConfigFile = "config.toml";

    public string DatabaseFile { get; private set; } = $"{configurationDirectory}/licenses.db";

    public string LicenseFilename { get; private set; } = "LICENSE";

    private void CreateConfiguration()
    {
        Directory.CreateDirectory(configurationDirectory);

        if (File.Exists($"{configurationDirectory}/{ConfigFile}")) return;

        FileStream file = File.Create($"{configurationDirectory}/{ConfigFile}");
        file.Close();

        if (new FileInfo($"{configurationDirectory}/{ConfigFile}").Length == 0)
        {
            string defaultConfiguration = $"license_filename=\"LICENSE\"";
            File.WriteAllText($"{configurationDirectory}/{ConfigFile}", defaultConfiguration);
        }
    }

    private void ReadConfiguration()
    {
        string configurationData = File.ReadAllText($"{configurationDirectory}/{ConfigFile}");
        TomlTable tomlTable = Toml.ToModel(configurationData);
        string? newLicenseFilename = (string?)tomlTable["license_filename"];

        if (newLicenseFilename == null) return;

        LicenseFilename = newLicenseFilename;
    }

    public void Configure()
    {
        CreateConfiguration();
        ReadConfiguration();
    }

    public void ShowConfiguration()
    {
        Console.WriteLine($"License Filename = \"{LicenseFilename}\"");
    }
}
