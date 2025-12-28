using LicenseGenerator.domain.model;
using Microsoft.Data.Sqlite;

namespace LicenseGenerator.domain.mapper;

public static class SqliteTemplateMapper
{
    public static Template FromReader(SqliteDataReader reader)
    {
        return new Template
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Body = reader.GetString(2)
        };
    }
}
