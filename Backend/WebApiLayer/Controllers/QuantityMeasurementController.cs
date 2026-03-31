using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebApiLayer.QuantityMeasurementController
{
    [Authorize]
    [ApiController]
    [Route("/api/quantitymeasurement")]
    public class QuantityMeasurementController : ControllerBase
    {
        private IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            _service = service;
        }

        [HttpPost("add")]
        public IActionResult Add(AddRequestDto request)
        {
            try
            {
                var result = _service.Add(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return BadRequest(msg);
            }
        }

        [HttpPost("subtract")]
        public IActionResult Subtract(SubtractRequestDto request)
        {
            try
            {
                var result = _service.Subtract(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return BadRequest(msg);
            }
        }

        [HttpPost("divide")]
        public IActionResult Divide(DivideRequestDto request)
        {
            try
            {
                var result = _service.Divide(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return BadRequest(msg);
            }
        }

        [HttpPost("compare")]
        public IActionResult Compare(ComparisonRequestDto request)
        {
            try
            {
                var result = _service.Compare(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return BadRequest(msg);
            }
        }

        [HttpPost("convert")]
        public IActionResult Convert(ConversionRequestDto request)
        {
            try
            {
                var result = _service.Convert(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return BadRequest(msg);
            }
        }

        
    }
}