using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Reflection;
using System.Text;
using CombineTxt.LookupDictionary;
using System.IO;

namespace CombineTxt.Extensions
{
    public class SqlLookupDictionaryFactory : ILookupDictionaryFactory, IDisposable
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public SqlLookupDictionaryFactory()
        {
            string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            string dbName = "LookupData_" + Guid.NewGuid() + ".sdf";
            _databasePath = Path.Combine(path, dbName);
            
            _connectionString = string.Format("Data Source={0};Persist Security Info=False;", _databasePath);
            SqlCeEngine engine = new SqlCeEngine { LocalConnectionString = _connectionString };
            engine.CreateDatabase();

            SqlCeConnection cn = new SqlCeConnection(_connectionString);
            string sql = @"CREATE TABLE [LookupDictionary](
                     [LookupTableId] [uniqueidentifier] NOT NULL,
                     [LookupTableKey] [nvarchar](250) NOT NULL,
                     [LookupTableData] [nvarchar](4000) NOT NULL)";
            SqlCeCommand cmd = new SqlCeCommand(sql, cn);
            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();
        }

        public SqlLookupDictionaryFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ILookupDictionary CreateLookupDictionary()
        {
            return new SqlLookupDictionary(_connectionString, Guid.NewGuid());
        }

        public void Dispose()
        {
            if(File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
    }
}
