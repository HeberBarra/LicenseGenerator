namespace LicenseGenerator.utils;

public static class AdjustPath
{
    public static string AddHomeDirectory(string path)
    {
        string userProfileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (path.StartsWith('~')) return path.Replace("~", userProfileDirectory);

        if (path.StartsWith("$HOME")) return path.Replace("$HOME", userProfileDirectory);

        return path;
    }
}
