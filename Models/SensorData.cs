using System;

namespace SensorAPI.Models
{
    public class SensorData
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedTimeStamp { get; set; }
        public DateTime? LastUpdatedTimeStamp { get; set; }
    }
}
