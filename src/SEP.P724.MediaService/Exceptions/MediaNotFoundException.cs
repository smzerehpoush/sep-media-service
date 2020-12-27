using System;

namespace SEP.P724.MediaService.Exceptions
{
    public class MediaNotFoundException : ServiceException
    {
        public string Message { get; }

        public MediaNotFoundException(Guid id)
        {
            this.Message = $"Media By Id [{id} Not Found.]";
        }
    }
}