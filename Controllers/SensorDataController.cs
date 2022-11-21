using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SensorAPI.Attributes;
using SensorAPI.Contracts;
using SensorAPI.Converters;
using SensorAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SensorAPI.Controllers
{
    [ApiController]
    [Route("sensorData")]
    public class SensorDataController : ControllerBase
    {
        private ILogger<SensorDataController> _logger;
        private SensorDataService _service;

        public SensorDataController(ILogger<SensorDataController> logger, SensorDataService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<SensorDataInfoContract>> GetAll()
        {
            try
            {
                return Ok(_service.GetAll().Select(d => d.ToContract()));
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}", nameof(GetAll));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<SensorDataInfoContract> GetById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest();
                }
                var sensorData = _service.GetById(id);
                if (sensorData == null)
                {
                    return NotFound();
                }
                return Ok(sensorData.ToContract());
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}", nameof(GetById));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiKey]
        public ActionResult Create(AddSensorDataContract sensorDataContract)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sensorDataContract.Value))
                {
                    return BadRequest();
                }
                var sensorData = sensorDataContract.ToModel();
                _service.Add(sensorData);
                return CreatedAtAction(nameof(GetById), new { id = sensorData.Id }, sensorData);
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}", nameof(Create));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiKey]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }
                var rows = _service.Delete(id);
                if (rows <= 0)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}", nameof(Delete));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiKey]
        public ActionResult Update(int id, UpdateSensorDataContract sensorDataContract)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }
                if (string.IsNullOrWhiteSpace(sensorDataContract.Value))
                {
                    return BadRequest();
                }
                var sensorData = sensorDataContract.ToModel(id);
                int rows = _service.Update(sensorData);
                if (rows <= 0)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Exception at {nameOfMethod}", nameof(Update));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
