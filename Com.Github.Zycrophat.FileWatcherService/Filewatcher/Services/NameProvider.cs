using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services
{
    public class NameProvider
    {

        private readonly ILogger<NameProvider> logger;

        public NameProvider(ILogger<NameProvider> logger)
        {
            this.logger = logger;
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            List<string> names = new List<string> { "Peter", "Hans" };

            await Task.Delay(1000);

            logger.LogInformation("Hello, {names}", names);
            return names;
        }
    }
}
