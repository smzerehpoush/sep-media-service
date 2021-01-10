using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Contract.DTO;
using SEP.P724.MediaService.Model;
using SEP.P724.MediaService.Services;

namespace SEP.P724.MediaService.Controller
{
    [ApiController]
    [Route("/api/v1/media")]
    public class MediaAdminController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IMapper _mapper;

        public MediaAdminController(IMediaService mediaService, IMapper mapper)
        {
            _mediaService = mediaService;
            _mapper = mapper;
        }

        [HttpGet]
        public PagedResponse<MediaDto> GetMedias(int page = 1, int size = 20)
        {
            return _mapper.Map<PagedResponse<MediaModel>, PagedResponse<MediaDto>>(
                _mediaService.GetMedias(page, size));
        }
    }
}