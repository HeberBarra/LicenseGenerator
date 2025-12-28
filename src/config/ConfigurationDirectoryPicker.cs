namespace LicenseGenerator.config;

public static class ConfigurationDirectoryPicker
{
    public static string PickConfigurationDirectory()
    {
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string osVersion = Environment.OSVersion.ToString();

        if (osVersion.Contains("linux", StringComparison.CurrentCultureIgnoreCase))
        {
            string? xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            return xdgConfigHome == null
                ? $"{homeDirectory}/.config/{Program.ProgramName}"
                : $"{xdgConfigHome}/{Program.ProgramName}";
        }

        if (osVersion.Contains("mac", StringComparison.CurrentCultureIgnoreCase))
            return $"{homeDirectory}/Library/Preferences/{Program.ProgramName}";

        if (osVersion.Contains("windows", StringComparison.CurrentCultureIgnoreCase))
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return $"{appDataDirectory}/{Program.ProgramName}";
        }

        return $"{homeDirectory}/.config/{Program.ProgramName}";
    }
}
