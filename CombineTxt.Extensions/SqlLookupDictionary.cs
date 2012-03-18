using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using CombineTxt.LookupDictionary;

namespace CombineTxt.Extensions
{
    public class SqlLookupDictionary : ILookupDictionary
    {
        private readonly Guid _dictionaryKey;
        private readonly string _connectionString;

        public SqlLookupDictionary(string connectionString, Guid dictionarykey)
        {
            _connectionString = connectionString;
            _dictionaryKey = dictionarykey;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            using (SqlCeConnection cn = new SqlCeConnection(_connectionString))
            {
                SqlCeCommand cmd = new SqlCeCommand("SELECT LookupTableKey, LookupTableData FROM LookupDictionary WHERE LookupTableId = @LookupTableId", cn);
                cmd.Parameters.Add("LookupTableId", _dictionaryKey);
                cn.Open();
                SqlCeDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    var pair = new KeyValuePair<string, List<string>>(
                        reader.GetString(reader.GetOrdinal("LookupTableKey")),
                        reader.GetString(reader.GetOrdinal("LookupTableData")).Split('\r').ToList());
                    yield return pair;
                }
                cn.Close();
            }
        }

        public void Add(string key, string record)
        {
            using (SqlCeConnection cn = new SqlCeConnection(_connectionString))
            {
                SqlCeCommand cmd = new SqlCeCommand("INSERT INTO LookupDictionary VALUES (@LookupTableId, @LookupTableKey, @LookupTableData)", cn);
                cmd.Parameters.Add("LookupTableId", _dictionaryKey);
                cmd.Parameters.Add("LookupTableKey", key);
                cmd.Parameters.Add("LookupTableData", record);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public void Remove(string key)
        {
            using (SqlCeConnection cn = new SqlCeConnection(_connectionString))
            {
                SqlCeCommand cmd = new SqlCeCommand("DELETE FROM LookupDictionary WHERE LookupTableId = @LookupTableId AND LookupTableKey = @LookupTableKey", cn);
                cmd.Parameters.Add("LookupTableId", _dictionaryKey);
                cmd.Parameters.Add("LookupTableKey", key);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public void Clear()
        {
            using (SqlCeConnection cn = new SqlCeConnection(_connectionString))
            {
                SqlCeCommand cmd = new SqlCeCommand("DELETE FROM LookupDictionary WHERE LookupTableId = @LookupTableId", cn);
                cmd.Parameters.Add("LookupTableId", _dictionaryKey);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public bool ContainsKey(string key)
        {
            int count = 0;
            using (SqlCeConnection cn = new SqlCeConnection(_connectionString))
            {
                SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(*) FROM LookupDictionary WHERE LookupTableId = @LookupTableId AND LookupTableKey = @LookupTableKey", cn);
                cmd.Parameters.Add("LookupTableId", _dictionaryKey);
                cmd.Parameters.Add("LookupTableKey", key);
                cn.Open();
                count = (int)cmd.ExecuteScalar();
                cn.Close();
            }

            return count > 0;
        }

        public List<string> this[string key]
        {
            get
            {
                List<string> record = new List<string>();
                using (SqlCeConnection cn = new SqlCeConnection(_connectionString))
                {
                    SqlCeCommand cmd = new SqlCeCommand("SELECT LookupTableData FROM LookupDictionary WHERE LookupTableId = @LookupTableId AND LookupTableKey = @LookupTableKey", cn);
                    cmd.Parameters.Add("LookupTableId", _dictionaryKey);
                    cmd.Parameters.Add("LookupTableKey", key);
                    cn.Open();
                    var reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        record.Add(reader.GetString(reader.GetOrdinal("LookupTableData"))); 
                    }

                    cn.Close();
                }

                return record;
            }
        }
    }
}
