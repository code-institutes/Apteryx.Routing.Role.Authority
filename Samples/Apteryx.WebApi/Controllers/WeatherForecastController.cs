using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Apteryx.Routing.Role.Authority;

namespace Apteryx.WebApi.Controllers
{
    //[Authorize(AuthenticationSchemes = "apteryx")]
    [SwaggerTag("��������")]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "zy1.0")]
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

        //[ApiRoleDescription("A","��ȡ",isMustHave: true)]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        //[ApiRoleDescription("B", "���")]
        public async Task<IActionResult> Post([FromBody] AddTest model)
        {
            return Ok(ApteryxResultApi.Susuccessful());
        }
    }
}