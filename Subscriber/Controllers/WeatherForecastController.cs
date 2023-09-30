using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Subscriber.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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


        // =============  Consumer ===================

        [CapSubscribe("place.order.qty.deducted")]
        [NonAction]
        public object DeductProductQty(JsonElement param)
        {
            var orderId = param.GetProperty("OrderId").GetInt32();
            var productId = param.GetProperty("ProductId").GetInt32();
            var qty = param.GetProperty("Qty").GetInt32();

            Console.WriteLine("Add New item");

            //business logic 

            return new { OrderId = orderId, IsSuccess = true };
        }
    }
}