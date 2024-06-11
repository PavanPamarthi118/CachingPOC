using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CachingPOC.Interfaces;
using CachingPOC.Models;

namespace CachingPOC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICacheService _cacheService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("getWeatherForecast")]
        [ResponseCache(Duration = 60)]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var cacheKey = "weatherData";
            var expirationTime = TimeSpan.FromMinutes(5);
            var weatherData = _cacheService.Get<IEnumerable<WeatherForecast>>(cacheKey);

            if (weatherData == null)
            {
                weatherData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                }).ToArray();

                _cacheService.Set(cacheKey, weatherData, expirationTime);
            }

            return weatherData;
        }
    }
}
