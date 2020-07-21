using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Buffers.Text;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Config
{
    [TypeConverter(typeof(ProtectedValueTypeConverter)), Serializable()]
    public class ProtectedValue
    {

        private static readonly Regex pattern = new Regex(@"^DPAPI\((.*)\)$");
        public string CipherText { get; private set; }
        
        public ProtectedValue(string value)
        {
            CipherText = value;
        }

        public string UnprotectedValue 
        {
            get {
                var matches = pattern.Match(CipherText);
                if (matches.Success)
                {
                    return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(matches.Groups[1].Value), null, DataProtectionScope.CurrentUser));
                }
                return CipherText;
            }
        }

    }
}
