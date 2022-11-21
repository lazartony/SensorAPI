using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SensorAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SensorAPI.Services
{
    public class SensorDataService
    {
        private const string InsertQuery = "INSERT INTO test (val) VALUES (@value);SELECT MAX(Id) from test;";
        private const string GetByIdQuery = "SELECT id,val FROM test WHERE id = @id;";
        private const string GetAllQuery = "SELECT id,val FROM test;";
        private MySqlConnection conn;

        private IConfiguration _configuration;
        public SensorDataService(IConfiguration configuration)
        {
            _configuration = configuration;
            conn = new MySqlConnection(configuration.GetValue<string>("ConnectionString"));
        }
        public IEnumerable<SensorData> GetAllData()
        {
            var sensorDataList = new List<SensorData>();
            conn.Open();
            var cmd = new MySqlCommand(GetAllQuery, conn);
            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sensorDataList.Add(new SensorData
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetString(1)
                    });
                }
                reader.Close();
            }
            catch
            {
                //TODO
            }
            
            conn.Close();
            return sensorDataList;
        }
        public SensorData GetData(int id)
        {
            conn.Open();
            SensorData sensorData = null;
            var cmd = new MySqlCommand(GetByIdQuery, conn);
            cmd.Parameters.AddWithValue("@id", id);
            try
            {
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    sensorData = new SensorData
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetString(1)
                    };
                }
                reader.Close();
            }
            catch
            {
                //TODO
            }
            conn.Close();
            return sensorData;
        }
        public void AddData(SensorData sensorData)
        {
            conn.Open();
            var cmd = new MySqlCommand(InsertQuery, conn);
            cmd.Parameters.AddWithValue("@value", sensorData.Value);
            try
            {
                sensorData.Id = (int)cmd.ExecuteScalar();

            }
            catch
            {
                //TODO
            }
            conn.Close();
        }
    }
}
