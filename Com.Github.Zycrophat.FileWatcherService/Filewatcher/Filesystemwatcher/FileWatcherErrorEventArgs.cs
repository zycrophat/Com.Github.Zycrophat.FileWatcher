using System;
using System.ComponentModel;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Filesystemwatcher
{
    public class FileWatcherErrorEventArgs : HandledEventArgs
    {
        public readonly Exception Exception;
        public FileWatcherErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}


