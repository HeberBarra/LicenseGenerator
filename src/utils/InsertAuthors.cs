namespace LicenseGenerator.utils;

public static class InsertAuthors
{
    public static string Insert(List<string> authors, string body)
    {
        string authorsName = string.Join(", ", authors);
        return body.Replace("$author", authorsName);
    }
}
