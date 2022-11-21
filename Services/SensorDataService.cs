using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private const string DeleteByIdQuery = "DELETE FROM test WHERE id = @id;";
        private const string UpdateByIdQuery = "UPDATE test SET val = @value WHERE id = @id;";
        private const string GetAllQuery = "SELECT id,val FROM test;";

        private IConfiguration _configuration;
        private ILogger<SensorDataService> _logger;
        private MySqlConnection _conn;

        public SensorDataService(IConfiguration configuration, ILogger<SensorDataService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _conn = new MySqlConnection(_configuration.GetValue<string>("ConnectionString"));
        }
        public IEnumerable<SensorData> GetAll()
        {
            var sensorDataList = new List<SensorData>();
            _conn.Open();
            var cmd = new MySqlCommand(GetAllQuery, _conn);
            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sensorDataList.Add(new SensorData
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetString(1),
                        CreatedTimeStamp = null,
                        LastUpdatedTimeStamp = null,
                    });
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                _conn.Close();
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}", nameof(GetAll));
                throw (new Exception());
            }
            
            _conn.Close();
            return sensorDataList;
        }
        public SensorData GetById(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            SensorData sensorData = null;


            _conn.Open();
            try
            {
                var cmd = new MySqlCommand(GetByIdQuery, _conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    sensorData = new SensorData
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetString(1),
                        CreatedTimeStamp = null,
                        LastUpdatedTimeStamp = null,
                    };
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                _conn.Close();
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}, Id = {id}", nameof(GetById), id);
                throw (new Exception());
            }
            _conn.Close();


            return sensorData;
        }
        public void Add(SensorData sensorData)
        {
            if(sensorData == null)
            {
                throw new ArgumentNullException(nameof(sensorData));
            }
            if(string.IsNullOrWhiteSpace(sensorData.Value))
            {
                throw new ArgumentException(nameof(sensorData.Value));
            }
            sensorData.CreatedTimeStamp = DateTime.UtcNow;
            sensorData.LastUpdatedTimeStamp = null;

            _conn.Open();
            try
            {
                var cmd = new MySqlCommand(InsertQuery, _conn);
                cmd.Parameters.AddWithValue("@value", sensorData.Value);
                sensorData.Id = (int)cmd.ExecuteScalar();
            }
            catch(Exception ex)
            {
                _conn.Close();
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}, Id = {id}, Value ={value}", nameof(Add), sensorData.Id, sensorData.Value);
                throw (new Exception());
            }
            _conn.Close();

            _logger.Log(LogLevel.Information, "SensorData Inserted => Id : {id}, Value : {value}", sensorData.Id, sensorData.Value);
        }

        public int Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            int rows = 0;

            _conn.Open();
            try
            {
                var cmd = new MySqlCommand(DeleteByIdQuery, _conn);
                cmd.Parameters.AddWithValue("@id", id);
                rows = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                _conn.Close();
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}, Id = {id}", nameof(Delete), id);
                throw (new Exception());
            }
            _conn.Close();

            _logger.Log(LogLevel.Information, "SensorData Deleted => Id : {id}", id);
            return rows;
        }

        public int Update(SensorData sensorData)
        {
            if (sensorData == null)
            {
                throw new ArgumentNullException(nameof(sensorData));
            }
            if (sensorData.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sensorData.Id));
            }
            if (string.IsNullOrWhiteSpace(sensorData.Value))
            {
                throw new ArgumentException(nameof(sensorData.Value));
            }
            sensorData.LastUpdatedTimeStamp = DateTime.UtcNow;
            int rows = 0;

            _conn.Open();
            try
            {
                var cmd = new MySqlCommand(UpdateByIdQuery, _conn);
                cmd.Parameters.AddWithValue("@id", sensorData.Id);
                cmd.Parameters.AddWithValue("@value", sensorData.Value);
                rows = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                _conn.Close();
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}, Id = {id}, Value ={value}", nameof(Update), sensorData.Id, sensorData.Value);
                throw (new Exception());
            }
            _conn.Close();

            _logger.Log(LogLevel.Information, "SensorData Updated => Id : {id}, Value : {value}", sensorData.Id, sensorData.Value);
            return rows;
        }
    }
}
