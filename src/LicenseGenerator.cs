namespace LicenseGenerator;

public class LicenseGenerator(string filename)
{
    private string Filename { get; set; } = filename;
    
    public void CreateLicenseFile(string body, string path, List<string> authors) 
    {
        
        if (!path.EndsWith(Filename))
        {
            path += path.EndsWith('/') ? Filename : "/" + Filename;
        }
        
        if (path.StartsWith('~'))
        {
            path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }
        
        if (path.StartsWith("$HOME"))
        {
            path = path.Replace("$HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        if (!File.Exists(path))
        {
            try
            {
                FileStream file = File.Create(path);
                file.Close();
            } catch (DirectoryNotFoundException) {
                Console.WriteLine("The specified directory was not found. Could not create license file.");
                return;
            }
        }

        if (File.ReadAllText(path).Length != 0 && !ShouldOverrideFile()) 
            return;
        
        if (authors.Count != 0)
        {
            string authorsName = string.Join(", ", authors);
            body = body.Replace("$author", authorsName);
        }

        File.WriteAllText(path, body);
    }
    
    private static bool ShouldOverrideFile() 
    {
        Console.Write("File has contents. Do you wish to override? [y/N] \n> ");
        string? option = Console.ReadLine();

        return option != null && option.ToLower().Contains('y');
    }
    
}