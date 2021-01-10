using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP.P724.MediaService.Model
{
    public class MediaModel
    {
        [Key] public Guid Id { get; }
        public string FileName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        public string MimeType { get; set; }

        public long DownloadCount { get; set; }

        public MediaModel(string fileName, string mimeType)
        {
            FileName = fileName;
            MimeType = mimeType;
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
            ;
        }
    }
}