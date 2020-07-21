using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Filesystemwatcher
{
    class EventQueueOverflowException : Exception
    {
        public EventQueueOverflowException()
            : base() { }

        public EventQueueOverflowException(string message)
            : base(message) { }
    }
}
