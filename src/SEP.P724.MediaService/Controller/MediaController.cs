using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Services;
using ServerSideApp.Filters;

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
        public ActionResult<MediaDto> GetMedia(Guid mediaId)
        {
            var result = _mediaService.GetMedia(mediaId).Result;
            return File(result.Item2, result.Item1.MimeType, result.Item1.FileName);
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