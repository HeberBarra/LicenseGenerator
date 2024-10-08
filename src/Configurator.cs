using Tomlyn;
using Tomlyn.Model;

namespace LicenseGenerator;

public class Configurator
{
    
    private string ConfigDir { get; set; } = ConfigurationFolderPicker.GetDefaultConfigFolder();
    private string ConfigFile { get; set; } = "config.toml";
    public string? DatabaseFile { get; private set;}
    public string? LicenseFilename { get; private set; }

    public void Configure()
    {
        CreateConfiguration();
        WriteDefaultConfiguration();
        ReadConfiguration();
    }
    
    private void CreateConfiguration()
    {
        Directory.CreateDirectory(ConfigDir);

        if (File.Exists($"{ConfigDir}/{ConfigFile}")) return;
        
        FileStream file = File.Create($"{ConfigDir}/{ConfigFile}");
        file.Close();
    }
    
    private void WriteDefaultConfiguration() 
    {
        string defaultConfig = $"template_db_location=\"" + 
                               $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/licenses.db\"\n" + 
                               "license_filename=\"LICENSE\"";
        CreateConfiguration();

        if (new FileInfo($"{ConfigDir}/{ConfigFile}").Length == 0)
        {
            File.WriteAllText($"{ConfigDir}/{ConfigFile}", defaultConfig);
        }
    }

    private void ReadConfiguration()
    {
        string config = $"{ConfigDir}{ConfigFile}";
        string data = File.ReadAllText(config);
        TomlTable toml = Toml.ToModel(data);

        string defaultDbFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/licenses.db";
        DatabaseFile = (string?)toml["template_db_location"] ?? defaultDbFile;
        LicenseFilename = (string?) toml["license_filename"] ?? "LICENSE";
    }
    
    public void ShowConfiguration() 
    {
        Console.WriteLine($"Configuration File = \"{ConfigFile}\"");
        Console.WriteLine($"Database File = \"{DatabaseFile}\"");
    }
}