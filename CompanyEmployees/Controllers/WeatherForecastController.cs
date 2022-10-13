using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        ////private ILoggerManager _logger;
        ////public WeatherForecastController(ILoggerManager logger)
        ////{
        ////    _logger = logger;
        ////}

        ////[HttpGet]
        ////public IEnumerable<string> Get()
        ////{
        ////    _logger.LogInfo("Here is info message from our values controller.");
        ////    _logger.LogDebug("Here is debug message from our values controller.");
        ////    _logger.LogWarn("Here is warn message from our values controller.");
        ////    _logger.LogError("Here is an error message from our values controller.");
        ////    return new string[] { "value1", "value2" };
        ////}
        private readonly IRepositoryManager _repository;
        public WeatherForecastController(IRepositoryManager repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _repository.Company.ToString();
            _repository.Employee.ToString();
            return new string[] { "value1", "value2" };
        }

    }

}
