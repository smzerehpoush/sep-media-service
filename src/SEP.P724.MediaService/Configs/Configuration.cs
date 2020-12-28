using System.Collections.Generic;
using System.IO;

namespace SEP.P724.MediaService.Configs
{
    public class Configuration
    {
        private static readonly IEnumerable<string> PermittedExtensions = new string[] {"*"};
        private const long FileSizeLimit = long.MaxValue;
        private const long MinStreamFileSizeLimitInByte = 2048000L;


        public string GetPhysicalStorageLocation()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Files");
        }

        public IEnumerable<string> GetPermittedExtensions()
        {
            return PermittedExtensions;
        }

        public long GetFileSizeLimit()
        {
            return FileSizeLimit;
        }

        public bool NeedStream(int mediaBytesLength)
        {
            return mediaBytesLength > MinStreamFileSizeLimitInByte;
        }
    }
}