using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Utilities
{
    internal class CaliLogInfo
    {
        private static string tableName = "CaliLogInfos";
        public long InfoId { get; set; }
        public long ExecuteId { get; set; }
        public String InfoType { get; set; }
        public DateTime DateTime { get; set; }
        public string FuncName { get; set; }

        public string Content { get; set; }

        public Boolean Insert()
        {
            CheckConfig();
            string insertQuery = $"INSERT INTO {tableName} " +
                $"({nameof(ExecuteId)}, {nameof(InfoType)}, " +
                $"{nameof(DateTime)}, {nameof(FuncName)}, {nameof(Content)}) VALUES " +
                $"(@{nameof(ExecuteId)}, @{nameof(InfoType)},@{nameof(DateTime)}, @{nameof(FuncName)}, @{nameof(Content)})";
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, DataBaseSqlite.Instance.Connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(ExecuteId)}", ExecuteId);
                    command.Parameters.AddWithValue($"@{nameof(InfoType)}", InfoType);
                    command.Parameters.AddWithValue($"@{nameof(DateTime)}", DateTime);
                    command.Parameters.AddWithValue($"@{nameof(FuncName)}", FuncName);
                    command.Parameters.AddWithValue($"@{nameof(Content)}", Content);
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void CheckConfig()
        {
            if (DataBaseSqlite.Instance.Connection == null)
            {
                DataBaseSqlite.Instance.Connect();
            }
            DataBaseSqlite.Instance.CreateTable(GenerateCreateTableQuery());
        }

        public static List<CaliLogInfo> SelectInfosFromDb(long executeId)
        {
            var retInfos = new List<CaliLogInfo>();
            string insertQuery = $"SELECT * FROM {tableName} WHERE " +
            $"{nameof(ExecuteId)} = {executeId}";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, DataBaseSqlite.Instance.Connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        long temp = 0;
                        while (reader.Read())
                        {
                            CaliLogInfo caliLogInfo = new CaliLogInfo();
                            caliLogInfo.InfoId = Int64.TryParse(reader[$"{nameof(InfoId)}"].ToString(), out temp) ? temp : 0;
                            caliLogInfo.ExecuteId = Int64.TryParse(reader[$"{nameof(ExecuteId)}"].ToString(), out temp) ? temp : 0;
                            caliLogInfo.InfoType = reader[$"{nameof(InfoType)}"].ToString();
                            caliLogInfo.DateTime = DateTime.Parse(reader[$"{nameof(DateTime)}"].ToString());
                            caliLogInfo.FuncName = reader[$"FuncName"].ToString();
                            caliLogInfo.Content = reader[$"Content"].ToString();
                            retInfos.Add(caliLogInfo);
                        }
                    }
                }
            }
            catch { }

            return retInfos;
        }

        public static String GenerateCreateTableQuery()
        {
            return @$"CREATE TABLE IF NOT EXISTS {tableName} ("
                    + @$"{nameof(InfoId)} BIGINT AUTO_INCREMENT PRIMARY KEY, {nameof(ExecuteId)} BIGINT,"
                    + @$"{nameof(InfoType)} VARCHAR(50),"
                    + @$"{nameof(DateTime)} DATETIME,"
                    + @$"{nameof(FuncName)} VARCHAR(200),"
                    + @$"{nameof(Content)} VARCHAR(500));";
        }
    }
}
