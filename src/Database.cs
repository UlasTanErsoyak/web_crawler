using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using webcrawler.Logs;

namespace webcrawler {
internal class Database {
  private readonly string ConnectionString =
      "server=DESKTOP-V6IUBGJ\\MSSQLSERVER01;database=WebCrawler; " +
      "integrated security=SSPI;persist security info=False; Trusted_Connection=Yes;";
  private static readonly object lockObject = new object();
  Logger logger = new Logger();
  public void SaveURL(string tableName, URL url) {
    try {
      lock (lockObject) {
        using (SqlConnection connection = new SqlConnection(ConnectionString)) {
          connection.Open();
          string urlQuery =
              ($"INSERT INTO {tableName} (URLID, ParentID, Depth, SpiderID, CreatedURLCount, URLAddress, FoundingDate, " +
               $"CrawlingDate, IsFailed) VALUES (@URLID, @ParentID, @Depth, @SpiderID, @CreatedURLCount, @URLAddress, " +
               $"@FoundingDate, @CrawlingDate, @IsFailed)");
          using (SqlCommand cmd = new SqlCommand(urlQuery, connection)) {
            cmd.Parameters.AddWithValue("@URLID", url.URLID);
            cmd.Parameters.AddWithValue("@ParentID", url.ParentID);
            cmd.Parameters.AddWithValue("@Depth", url.Depth);
            cmd.Parameters.AddWithValue("@SpiderID", url.SpiderID);
            cmd.Parameters.AddWithValue("@CreatedURLCount",
                                        url.CreatedURLCount);
            cmd.Parameters.AddWithValue("@URLAddress", url.URLAddress);
            cmd.Parameters.AddWithValue("@FoundingDate", url.FoundingDate);
            cmd.Parameters.AddWithValue("@CrawlingDate", url.CrawlingDate);
            cmd.Parameters.AddWithValue("@IsFailed", url.IsFailed);
            cmd.ExecuteNonQuery();
          }
        }
      }
    } catch (Exception ex) {
      logger.Error(ex);
    }
  }
  public void CreateTable(string tableName) {
    try {
      lock (lockObject) {
        using (SqlConnection connection = new SqlConnection(ConnectionString)) {
          connection.Open();
          string createTableQuery = $@"
                    CREATE TABLE {tableName} (
                    URLID INT PRIMARY KEY,
                    ParentID INT,
                    Depth INT,
                    SpiderID INT,
                    CreatedURLCount INT,
                    URLAddress NVARCHAR(255),
                    FoundingDate DATETIME,
                    CrawlingDate DATETIME,
                    IsFailed BIT)";
          using (SqlCommand cmd =
                     new SqlCommand(createTableQuery, connection)) {
            cmd.ExecuteNonQuery();
          }
        }
      }
    } catch (Exception ex) {
      logger.Error(ex);
    }
  }
  public string SanitizeTableName(string tableName) {
    string sanitizedTableName = Regex.Replace(tableName, @"[^\w]", "");
    if (!char.IsLetter(sanitizedTableName[0]) && sanitizedTableName[0] != '_') {
      sanitizedTableName = '_' + sanitizedTableName;
    }
    string[] reservedKeywords = new string[] {
      "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE",
      "DROP",   "ALTER",  "TABLE",  "INDEX",  "WHERE",
      "FROM",   "INTO",   "AND",    "OR",     "JOIN",
    };

    if (reservedKeywords.Contains(sanitizedTableName.ToUpper())) {
      sanitizedTableName += '_';
    }
    return sanitizedTableName;
  }
  public List<string> GetDatabaseNames() {
    string query = "USE WebCrawler;SELECT TABLE_SCHEMA, TABLE_NAME FROM " +
                   "INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
    List<string> tableNames = new List<string>();
    using (SqlConnection connection = new SqlConnection(ConnectionString)) {
      try {
        connection.Open();

        using (SqlCommand command = new SqlCommand(query, connection)) {
          SqlDataReader reader = command.ExecuteReader();

          while (reader.Read()) {
            string tableName =
                $"{reader["TABLE_SCHEMA"]}.{reader["TABLE_NAME"]}";
            string sanitizedTableName = tableName.StartsWith("dbo.")
                                            ? tableName.Substring(4)
                                            : tableName;
            tableNames.Add(sanitizedTableName);
          }
          reader.Close();
        }
      } catch (Exception ex) {
        logger.Error(ex);
      }
    }
    return tableNames;
  }
  public List<Dictionary<string, object>> GetTable(string tableName) {
    List<Dictionary<string, object>> rows =
        new List<Dictionary<string, object>>();
    try {
      using (SqlConnection connection = new SqlConnection(ConnectionString)) {
        connection.Open();

        string query = $"SELECT * FROM {tableName}";

        using (SqlCommand command = new SqlCommand(query, connection)) {
          using (SqlDataReader reader = command.ExecuteReader()) {
            while (reader.Read()) {
              Dictionary<string, object> row = new Dictionary<string, object>();
              for (int i = 0; i < reader.FieldCount; i++) {
                string columnName = reader.GetName(i);
                object value = reader.GetValue(i);
                row[columnName] = value;
              }
              rows.Add(row);
            }
          }
        }
      }
    } catch (Exception ex) {
      logger.Error(ex);
    }
    return rows;
  }
  public void DeleteTable(string tableName) {
    try {
      lock (lockObject) {
        using (SqlConnection connection = new SqlConnection(ConnectionString)) {
          connection.Open();
          string createTableQuery = $@"DROP TABLE {tableName};";
          using (SqlCommand cmd =
                     new SqlCommand(createTableQuery, connection)) {
            cmd.ExecuteNonQuery();
          }
        }
      }
    } catch (Exception ex) {
      logger.Error(ex);
    }
  }
}
}
