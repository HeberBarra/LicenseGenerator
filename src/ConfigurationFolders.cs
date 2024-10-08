namespace LicenseGenerator;

public static class ConfigurationFolderPicker
{
    private static readonly string Linux;
    private static readonly string Mac;
    private static readonly string Windows;
    private static readonly string Default;

    static ConfigurationFolderPicker()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string? xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");

        Linux = xdgConfigHome == null
            ? $"{homeFolder}/.config/{Program.ProgramName}/"
            : $"{xdgConfigHome}/{Program.ProgramName}/";

        Mac = $"{homeFolder}/Library/Preferences/{Program.ProgramName}";
        Windows = $"{appData}/{Program.ProgramName}/";
        Default = $"{homeFolder}/.config/{Program.ProgramName}/";
    }

    public static string GetDefaultConfigFolder()
    {
        string osVersion = Environment.OSVersion.ToString();

        if ("linux".Contains(osVersion, StringComparison.CurrentCultureIgnoreCase))
        {
            return Linux;
        } else if (osVersion.Contains("windows", StringComparison.CurrentCultureIgnoreCase))
        {
            return Windows;
        } else if (osVersion.Contains("mac", StringComparison.CurrentCultureIgnoreCase))
        {
            return Mac;
        }
        else
        {
            return Default;
        }
    }
}