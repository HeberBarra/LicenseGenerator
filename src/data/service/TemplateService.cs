using LicenseGenerator.domain.mapper;
using LicenseGenerator.domain.model;
using Microsoft.Data.Sqlite;

namespace LicenseGenerator.data.service;

public class TemplateService(string databaseFile) : ITemplateService, IDisposable
{
    private const string TableName = "tbTemplate";
    private string DatabaseFile { get; } = databaseFile;
    private SqliteConnection DatabaseConnection { get; } = new($"Data Source = {databaseFile}");

    private void CreateDatabaseFile()
    {
        if (File.Exists(DatabaseFile)) return;

        FileStream file = File.Create(DatabaseFile);
        file.Close();
    }

    private void CreateTable()
    {
        SqliteCommand createTableCommand = DatabaseConnection.CreateCommand();
        createTableCommand.CommandText =
            $"""
             CREATE TABLE IF NOT EXISTS {TableName} (
                 id INTEGER,
                 name VARCHAR(30) NOT NULL,
                 body TEXT NOT NULL,
                 CONSTRAINT pk_tbTemplate PRIMARY KEY (id)
             )
             """;
        createTableCommand.ExecuteNonQuery();
    }

    public void InitService()
    {
        CreateDatabaseFile();
        DatabaseConnection.Open();
        CreateTable();
    }


    public void Save(Template template)
    {
        SqliteCommand insertCommand = DatabaseConnection.CreateCommand();
        insertCommand.CommandText = $"INSERT INTO {TableName}(name, body) VALUES ($name, $body)";
        insertCommand.Parameters.AddWithValue("$name", template.Name);
        insertCommand.Parameters.AddWithValue("$body", template.Body);
        insertCommand.ExecuteNonQuery();
    }

    public Template? SearchById(int id)
    {
        SqliteCommand searchCommand = DatabaseConnection.CreateCommand();
        searchCommand.CommandText = $"SELECT id, name, body FROM {TableName} WHERE id = $id";
        searchCommand.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = searchCommand.ExecuteReader();

        return !reader.Read() ? null : SqliteTemplateMapper.FromReader(reader);
    }

    public List<Template> List()
    {
        SqliteCommand listCommand = DatabaseConnection.CreateCommand();
        listCommand.CommandText = $"SELECT id, name, body FROM {TableName}";

        using SqliteDataReader reader = listCommand.ExecuteReader();
        List<Template> templates = [];

        while (reader.Read()) templates.Add(SqliteTemplateMapper.FromReader(reader));

        return templates;
    }

    public void Update(Template template)
    {
        SqliteCommand updateCommand = DatabaseConnection.CreateCommand();
        updateCommand.CommandText = $"UPDATE {TableName} SET name = $name, body = $body WHERE id = $id";
        updateCommand.Parameters.AddWithValue("$id", template.Id);
        updateCommand.Parameters.AddWithValue("$name", template.Name);
        updateCommand.Parameters.AddWithValue("$body", template.Body);
        updateCommand.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        SqliteCommand deleteCommand = DatabaseConnection.CreateCommand();
        deleteCommand.CommandText = $"DELETE FROM {TableName} WHERE id = $id";
        deleteCommand.Parameters.AddWithValue("$id", id);
        deleteCommand.ExecuteNonQuery();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        DatabaseConnection.Close();
        DatabaseConnection.Dispose();
    }
}
