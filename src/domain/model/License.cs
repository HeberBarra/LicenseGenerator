namespace LicenseGenerator.domain.model;

public class License(string filename, string path, string body, List<string> authors)
{
    public string Filename { get; set; } = filename;
    public string Path { get; set; } = path;
    public string Body { get; set; } = body;
    public List<string> Authors { get; set; } = authors;
}
