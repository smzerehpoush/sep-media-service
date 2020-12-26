using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Exceptions;
using SEP.P724.MediaService.Model;
using SEP.P724.MediaService.Utilities;

namespace SEP.P724.MediaService.Services
{
    public class MediaServiceImpl : IMediaService
    {
        private static readonly IEnumerable<string> PermittedExtension = new[] {"*"};
        private static readonly long FileSizeLimit = long.MaxValue;
        private readonly IMapper _mapper;

        public MediaServiceImpl(IMapper mapper)
        {
            _mapper = mapper;
        }

        public MediaDto GetMedia(Guid mediaId)
        {
            var media = new MediaDto() {Id = Guid.NewGuid()};
            return media;
        }

        public object GetMedia()
        {
            var path =
                @"C:\Users\m.zerehpoush\w\SEP.P724.MediaServiceSolution\src\SEP.P724.MediaService\Resources\Files\f424c6d8-145e-41cd-932d-e07c0f6f411f";
            return File.Open(path, FileMode.Open);
        }

        public string UploadMedia(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            Guid fileId = Guid.NewGuid();
            var folderName = Path.Combine("Resources", "Files" /*, new Random().Next(MaxDirectoryCuont).ToString()*/);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(pathToSave);
            var fileName = file.FileName;
            var fullPath = Path.Combine(pathToSave, fileId.ToString());
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fullPath + "\n" + fileId;
        }

        public async Task<MediaDto> UploadMedia(HttpRequest request)
        {
            MediaModel mediaModel = new MediaModel();
            CheckContentType(request);

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType),
                new FormOptions().MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (contentDisposition.IsFileDisposition())
                    {
                        var trustedFileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        mediaModel.FileName = trustedFileName;
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition,
                            PermittedExtension, FileSizeLimit);


                        var trustedFilePath = Path.Combine("Resources", "Files",
                            mediaModel.Id + (Path.GetExtension(trustedFileName) ?? string.Empty));
                        await using var targetStream = File.Create(trustedFilePath);
                        await targetStream.WriteAsync(streamedFileContent);
                    }
                }

                // Drain any remaining section body that hasn't been consumed and read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // todo: validate and persist formModel
            // _logger.LogInformation(mediaModel.Id.ToString());
            return _mapper.Map<MediaDto>(mediaModel);
        }

        private static void CheckContentType(HttpRequest request)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                throw new MediaUploadException("The request couldn't be processed. invalid content type");
            }
        }
    }
}