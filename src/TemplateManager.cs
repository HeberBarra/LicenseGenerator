using Microsoft.Data.Sqlite;

namespace LicenseGenerator;

public class TemplateManager
{
    private string DatabaseFile { get; set; }
    private SqliteConnection Connection { get; set; }
    
    public TemplateManager(string databaseFile)
    {
        DatabaseFile = databaseFile;
        CreateDatabaseFile();
        Connection = new SqliteConnection($"Data Source ={DatabaseFile}");
        Connection.Open();
        CreateTable();
    }
    
    private void CreateDatabaseFile() 
    {
        if (File.Exists(DatabaseFile)) 
            return;

        FileStream file = File.Create(DatabaseFile);
        file.Close();
    }
    
    private void CreateTable()
    {
        SqliteCommand tableCommand = Connection.CreateCommand();
        tableCommand.CommandText = 
            """
            CREATE TABLE IF NOT EXISTS tbTemplate (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name VARCHAR(30) NOT NULL,
                body TEXT NOT NULL
            )     
            """;
        tableCommand.ExecuteNonQuery();
    }
    
    public void AddTemplate(Template template)
    {
        SqliteCommand insertCommand = Connection.CreateCommand();
        insertCommand.CommandText = "INSERT INTO tbTemplate(name, body) VALUES ($name, $body)";
        insertCommand.Parameters.AddWithValue("$name", template.Name);
        insertCommand.Parameters.AddWithValue("$body", template.Body);
        insertCommand.ExecuteNonQuery();
    }
    
    public List<Template> GetEntries()
    {
        SqliteCommand listCommand = Connection.CreateCommand(); 
        listCommand.CommandText = "SELECT id, name FROM tbTemplate";
        using SqliteDataReader reader = listCommand.ExecuteReader();

        List<Template> templates = [];
        while (reader.Read())
        {
            Template template = new Template
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1)
            };
            templates.Add(template);
        }

        return templates;
    }
    
    public Template? SearchById(long? id)
    {
        SqliteCommand searchCommand = Connection.CreateCommand();
        searchCommand.CommandText = "SELECT id,name,body FROM tbTemplate WHERE id = $id";

        searchCommand.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = searchCommand.ExecuteReader();

        Template? template = null;
        if (reader.Read())
        {

            template = new Template
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1),
                Body = reader.GetString(2)
            };
        }

        return template;
    }
    
    public void UpdateEntry(Template template)
    {
        SqliteCommand updateCommand = Connection.CreateCommand();
        updateCommand.CommandText = "UPDATE tbTemplate SET name = $name, body = $body WHERE id = $id";
        
        updateCommand.Parameters.AddWithValue("$id", template.Id);
        updateCommand.Parameters.AddWithValue("$name", template.Name);
        updateCommand.Parameters.AddWithValue("$body", template.Body);
        
        updateCommand.ExecuteNonQuery();
    }
    
    public void DeleteEntry(long? id)
    {
        SqliteCommand deleteCommand = Connection.CreateCommand();
        deleteCommand.CommandText = "DELETE FROM tbTemplate WHERE id = $id";
        deleteCommand.Parameters.AddWithValue("$id", id);
        deleteCommand.ExecuteNonQuery();
    }
    
    public void CloseConnection() 
    {
        Connection.Close();
    }
    
}