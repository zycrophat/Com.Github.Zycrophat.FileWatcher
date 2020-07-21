using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Controllers
{
    public class HelloWorldController : Controller
    {
        private readonly ILogger _logger;
        private readonly NameProvider _nameProvider;

        public HelloWorldController(ILogger logger, NameProvider nameProvider)
        {
            _logger = logger;
            _logger.Information("{names}", nameProvider.GetNames());
            _nameProvider = nameProvider;
        }
        // 
        // GET: /HelloWorld/

        public string Index()
        {
            _logger.Information("blabla");
            _logger.Information("{names}", _nameProvider.GetNames());
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
