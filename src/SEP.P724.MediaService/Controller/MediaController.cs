using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Services;

namespace SEP.P724.MediaService.Controller
{
    [ApiController]
    [Route("/media")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet("/get/{mediaId}")]
        public ActionResult<MediaDto> GetMedia(Guid mediaId)
        {
            return Ok(_mediaService.GetMedia(mediaId));
        }

        [HttpGet("/file")]
        public ActionResult GetMedia()
        {
            return Ok(_mediaService.GetMedia());
        }

        [HttpPost("/upload")]
        public string UploadMedia([FromForm] IFormFile file)
        {
            using (var sr = new StreamReader(file.OpenReadStream()))
            {
                return _mediaService.UploadMedia(file);
            }
        }
    }
}