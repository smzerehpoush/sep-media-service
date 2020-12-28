using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Filters;
using SEP.P724.MediaService.Services;

namespace SEP.P724.MediaService.Controller
{
    [ApiController]
    [Route("/api/v1/media")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet("{mediaId}")]
        public ActionResult<MediaDto> DownloadMedia(Guid mediaId)
        {
            var (mediaModel, mediaBytes) = _mediaService.GetMedia(mediaId).Result;
            return File(mediaBytes, mediaModel.MimeType, true);
        }

        [DisableFormValueModelBinding]
        [HttpPost("/upload")]
        public async Task<IActionResult> UploadMedia()
        {
            MediaDto media = await _mediaService.UploadMedia(Request);
            return Created("api/v1/media/upload", media);
        }
    }
}