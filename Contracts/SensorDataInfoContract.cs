using System;

namespace SensorAPI.Contracts
{
    public class SensorDataInfoContract
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime? CreatedTimeStamp { get; set; }
        public DateTime? LastUpdatedTimeStamp { get; set; }
    }
}
