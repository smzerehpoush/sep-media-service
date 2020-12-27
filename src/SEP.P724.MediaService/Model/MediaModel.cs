using System;
using System.ComponentModel.DataAnnotations;

namespace SEP.P724.MediaService.Model
{
    public class MediaModel
    {
        [Key] public Guid Id { get; }
        public string FileName { get; set; }
        public DateTime CreationDate { get; } = DateTime.Now;
        public string MimeType { get; set; }

        public MediaModel(string fileName, string mimeType)
        {
            FileName = fileName;
            MimeType = mimeType;
            Id = Guid.NewGuid();
        }
    }
}