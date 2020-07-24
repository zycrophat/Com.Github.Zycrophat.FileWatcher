using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Controllers
{
    public class HelloWorldController : Controller
    {
        private readonly ILogger<HelloWorldController> _logger;
        private readonly NameProvider nameProvider;

        public HelloWorldController(ILogger<HelloWorldController> logger, NameProvider nameProvider)
        {
            _logger = logger;
            _logger.LogInformation("{names}", nameProvider.GetNames());
            this.nameProvider = nameProvider;
        }
        // 
        // GET: /HelloWorld/

        public string Index()
        {
            _logger.LogInformation("blabla");
            _logger.LogInformation("{names}", nameProvider.GetNames());
            return "This is my default action...";
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}
