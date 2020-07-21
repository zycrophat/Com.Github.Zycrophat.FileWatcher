using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services
{
    public class NameProvider
    {

        private readonly ILogger _logger;

        public NameProvider(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            List<string> names = new List<string> { "Peter", "Hans" };

            await Task.Delay(1000);

            _logger.Information("Hello, {@names}", names);
            return names;
        }
    }
}
