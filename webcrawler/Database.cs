using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace webcrawler
{
    internal class Database
    {
        private readonly string ConnectionString = "server=DESKTOP-V6IUBGJ\\MSSQLSERVER01;database=WebCrawler; " +
            "integrated security=SSPI;persist security info=False; Trusted_Connection=Yes;";
        private static readonly object lockObject = new object();
        public void SaveURL(string tableName, URL url)
        {
            lock (lockObject)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string urlQuery = ($"INSERT INTO {tableName} (URLID, ParentID, Depth, SpiderID, CreatedURLCount, URLAddress, FoundingDate, " +
                        $"CrawlingDate, IsFailed) VALUES (@URLID, @ParentID, @Depth, @SpiderID, @CreatedURLCount, @URLAddress, " +
                        $"@FoundingDate, @CrawlingDate, @IsFailed)");
                    using (SqlCommand cmd = new SqlCommand(urlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@URLID", url.URLID);
                        cmd.Parameters.AddWithValue("@ParentID", url.ParentID);
                        cmd.Parameters.AddWithValue("@Depth", url.Depth);
                        cmd.Parameters.AddWithValue("@SpiderID", url.SpiderID);
                        cmd.Parameters.AddWithValue("@CreatedURLCount", url.CreatedURLCount);
                        cmd.Parameters.AddWithValue("@URLAddress", url.URLAddress);
                        cmd.Parameters.AddWithValue("@FoundingDate", url.FoundingDate);
                        cmd.Parameters.AddWithValue("@CrawlingDate", url.CrawlingDate);
                        cmd.Parameters.AddWithValue("@IsFailed", url.IsFailed);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public void CreateTable(string tableName)
        {
            lock (lockObject)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
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
                    IsFailed BIT
                )";
                    using (SqlCommand cmd = new SqlCommand(createTableQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public  string SanitizeTableName(string tableName)
        {
            string sanitizedTableName = Regex.Replace(tableName, @"[^\w]", "");
            if (!char.IsLetter(sanitizedTableName[0]) && sanitizedTableName[0] != '_')
            {
                sanitizedTableName = '_' + sanitizedTableName;
            }
            string[] reservedKeywords = new string[]
            {
            "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER",
            "TABLE", "INDEX", "WHERE", "FROM", "INTO", "AND", "OR", "JOIN",
            };

            if (reservedKeywords.Contains(sanitizedTableName.ToUpper()))
            {
                sanitizedTableName += '_';
            }
            return sanitizedTableName;
        }
    }
}
