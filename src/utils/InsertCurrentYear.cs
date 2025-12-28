namespace LicenseGenerator.utils;

public static class InsertCurrentYear
{
    public static string Insert(string body)
    {
        return body.Replace("$currentYear", DateTime.Today.Year.ToString());
    }
}
