using System;

namespace SEP.P724.MediaService.Model
{
    public class MediaModel
    {
        public Guid Id { get; }
        public string FileName { get; set; }
        public DateTime CreationDate { get; } = DateTime.Now;

        public MediaModel()
        {
            this.Id = Guid.NewGuid();
        }
    }
}