﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Contract.DTO;
using SEP.P724.MediaService.Model;

namespace SEP.P724.MediaService.Services
{
    public interface IMediaService
    {
        Task<Tuple<MediaModel, byte[]>> GetMedia(Guid mediaId);
        Task<MediaDto> UploadMedia(HttpRequest request);
        PagedResponse<MediaDto> GetMedias(HttpRequest request, int page, int size);
    }
}