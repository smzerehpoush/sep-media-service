namespace SEP.P724.MediaService.Exceptions
{
    public class MediaUploadException : ServiceException
    {
        public MediaUploadException(string? message) : base(message)
        {
        }
    }
}