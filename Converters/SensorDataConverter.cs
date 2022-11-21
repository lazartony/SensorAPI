using SensorAPI.Contracts;
using SensorAPI.Models;

namespace SensorAPI.Converters
{
    public static class SensorDataConverter
    {
        public static SensorData ToModel(this AddSensorDataContract contract)
        {
            var model = new SensorData
            {
                Value = contract.Value
            };
            return model;
        }

        public static SensorData ToModel(this UpdateSensorDataContract contract, int id)
        {
            var model = new SensorData
            {
                Id = id,
                Value = contract.Value
            };
            return model;
        }

        public static SensorDataInfoContract ToContract(this SensorData model)
        {
            var contract = new SensorDataInfoContract
            {
                Id = model.Id,
                Value = model.Value
            };
            return contract;
        }
    }
}
