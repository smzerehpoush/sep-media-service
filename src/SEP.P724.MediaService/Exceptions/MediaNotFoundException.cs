using System;

namespace SEP.P724.MediaService.Exceptions
{
    public class MediaNotFoundException : ServiceException
    {
        public MediaNotFoundException(Guid mediaId) : base($"Media By Id [{mediaId.ToString()} Not Found.]")
        {
        }
    }
}