using System;

namespace SEP.P724.MediaService.Exceptions
{
    public class ServiceException : Exception
    {
        protected ServiceException(string? message) : base(message)
        {
        }
    }
}