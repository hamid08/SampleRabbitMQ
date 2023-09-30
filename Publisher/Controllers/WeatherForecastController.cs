using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Publisher.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ICapPublisher _capBus;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICapPublisher capPublisher)
        {
            _logger = logger;
            _capBus = capPublisher;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[HttpGet(Name = "CapTest")]
        //public async Task<IActionResult> CapTest()
        //{
        //    _capBus.Publish("place.order.qty.deducted",
        //    contentObj: new { OrderId = 1234, ProductId = 23255, Qty = 1 },
        //    callbackName: "place.order.mark.status");


        //    return Ok();
        //}


        [CapSubscribe("place.order.mark.status")]
        [NonAction]
        public void MarkOrderStatus(JsonElement param)
        {
            var orderId = param.GetProperty("OrderId").GetInt32();
            var isSuccess = param.GetProperty("IsSuccess").GetBoolean();

            if (isSuccess)
            {
                // mark order status to succeeded
               
            }
            else
            {
                // mark order status to failed
            }
        }




    }
}