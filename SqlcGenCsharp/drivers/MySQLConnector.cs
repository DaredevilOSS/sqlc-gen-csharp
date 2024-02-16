namespace DefaultNamespace;

using System;
using System.Data;

public static class MySQLConnector
{
    public static Type ColumnType(string columnType, bool notNull)
    {
        if (string.IsNullOrEmpty(columnType))
            return typeof(object);
        
        switch (columnType.ToLower())
        {
            case "bigint":
                return typeof(long);
            case "binary":
            case "bit":
            case "blob":
            case "longblob":
            case "mediumblob":
            case "tinyblob":
            case "varbinary":
                return typeof(byte[]);
            case "char":
            case "date":
            case "datetime":
            case "decimal":
            case "longtext":
            case "mediumtext":
            case "text":
            case "time":
            case "timestamp":
            case "tinytext":
            case "varchar":
                return typeof(string);
            case "double":
            case "float":
                return typeof(double);
            case "int":
            case "mediumint":
            case "smallint":
            case "tinyint":
            case "year":
                return typeof(int);
            case "json":
                return typeof(object); // or a specific class if JSON structure is known
            default:
                throw new NotSupportedException($"Unsupported column type: {columnType}");
        }
    }
}
