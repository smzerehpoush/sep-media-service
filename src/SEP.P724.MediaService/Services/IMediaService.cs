using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SEP.P724.MediaService.Contract;

namespace SEP.P724.MediaService.Services
{
    public interface IMediaService
    {
        MediaDto GetMedia(Guid mediaId);
        object GetMedia();
        string UploadMedia(IFormFile file);
        Task<MediaDto> UploadMedia(HttpRequest request);
    }
}