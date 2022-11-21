using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorAPI.Attributes;
using SensorAPI.Models;
using SensorAPI.Services;
using System.Collections.Generic;
using System.Linq;

namespace SensorAPI.Controllers
{
    [ApiController]
    [Route("sensorData")]
    public class SensorDataController : ControllerBase
    {
        SensorDataService service;
        public SensorDataController()
        {
            service = new SensorDataService();
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<SensorData>> GetAll()
        {
            try
            {
                return service.GetAllData().ToList();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SensorData> Details(int id)
        {
            try
            {
                var sensorData = service.GetData(id);
                if (sensorData == null)
                {
                    return NotFound();
                }
                return sensorData;
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ApiKey]
        public ActionResult Create(SensorData sensorData)
        {
            try
            {
                if (!IsValidAddContract(sensorData))
                {
                    return BadRequest();
                }
                service.AddData(sensorData);
                return CreatedAtAction(nameof(Details), new { id = sensorData.Id }, sensorData);
            }
            catch
            {
                return BadRequest();
            }
        }

        private static bool IsValidAddContract(SensorData sensorData)
        {
            return true;
        }
    }
}
