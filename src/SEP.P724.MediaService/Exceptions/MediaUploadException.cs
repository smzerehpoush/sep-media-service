using System;

namespace SEP.P724.MediaService.Exceptions
{
    public class MediaUploadException : Exception
    {
        public MediaUploadException(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }
    }
}