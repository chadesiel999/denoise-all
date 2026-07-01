using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScopeX.Hardware.Calibration.Tool.Utilities
{
    internal class DataBaseSqlite : IDisposable
    {
        public static DataBaseSqlite Instance { get; } = new DataBaseSqlite();
        public SQLiteConnection Connection { get; private set; }

        private String DefaultConnectionString { get; set; } = "Data Source=CaliData.db;Version=3;";

        private DataBaseSqlite() { }

        public void Dispose()
        {
            Connection.Close();
            throw new NotImplementedException();
        }

        public Boolean Connect(String connectionString = "")
        {
            if (String.IsNullOrEmpty(connectionString))
                connectionString = Instance.DefaultConnectionString;
            try
            {
                Connection = new SQLiteConnection(connectionString);
                Connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateDb(String dbName)
        {
            throw new NotImplementedException();
        }

        public bool CreateTable(String createTableQuery)
        {
            try
            {
                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, Connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
