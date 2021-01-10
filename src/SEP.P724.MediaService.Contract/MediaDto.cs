using System;

namespace SEP.P724.MediaService.Contract
{
    public class MediaDto
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
        
        public string MimeType { get; set; }
    }
}