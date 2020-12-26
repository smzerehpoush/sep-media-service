using AutoMapper;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Model;

namespace SEP.P724.MediaService.Profiles
{
    public class MediaProfile : Profile
    {
        public MediaProfile()
        {
            CreateMap<MediaModel, MediaDto>();
            CreateMap<MediaDto, MediaModel>();
        }
    }
}