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
using System.Runtime.InteropServices;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.UnsafeValueAccess;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Config
{
    [TypeConverter(typeof(ProtectedValueTypeConverter)), Serializable()]
    public class ProtectedValue
    {

        private static readonly Regex pattern = new Regex(@"^dpapi:(.*)$");
        public string CipherText { get; private set; }
        
        public ProtectedValue(string value)
        {
            CipherText = value;
        }

        public TryOption<string> TryUnprotect(Option<byte[]> entropy = default, DataProtectionScope scope = DataProtectionScope.CurrentUser) => () =>
        {
            var matches = pattern.Match(CipherText);
            var isOSPlatformSupported = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (matches.Success && isOSPlatformSupported)
            {
                return Some(
                    Encoding.UTF8.GetString(
                        ProtectedData.Unprotect(
                            Convert.FromBase64String(matches.Groups[1].Value),
                            entropy.IfNoneUnsafe(() => null),
                            scope
                        )
                    )
               );
            }
            return None;
        };

    }
}
