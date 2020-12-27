using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
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
        private readonly MediaServiceContext _mediaServiceContext;

        public MediaServiceImpl(IMapper mapper, MediaServiceContext mediaServiceContext)
        {
            _mapper = mapper;
            _mediaServiceContext = mediaServiceContext;
        }

        public async Task<Tuple<MediaModel, byte[]>> GetMedia(Guid mediaId)
        {
            var mediaModel = await _mediaServiceContext.Medias.FirstOrDefaultAsync(media => media.Id == mediaId);
            if (mediaModel == null)
            {
                throw new MediaNotFoundException(mediaId);
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Files",
                mediaModel.Id.ToString());
            var bytes = await File.ReadAllBytesAsync(path);
            return new Tuple<MediaModel, byte[]>(mediaModel, bytes);
        }

        private static string GetMimeTypes(string ext)
        {
            switch (ext)
            {
                case ".txt": return "text/plain";
                case ".csv": return "text/csv";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/vnd.ms-word";
                case ".xls": return "application/vnd.ms-excel";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                default: return "application/octet-stream";
            }
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
                        mediaModel.MimeType = GetMimeTypes(Path.GetExtension(trustedFileName));
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition,
                            PermittedExtension, FileSizeLimit);


                        var folderName = Path.Combine("Resources", "Files");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        Directory.CreateDirectory(pathToSave);
                        var fullPath = Path.Combine(pathToSave,
                            mediaModel.Id.ToString());

                        await using var targetStream = File.Create(fullPath);
                        await targetStream.WriteAsync(streamedFileContent);
                    }
                }

                // Drain any remaining section body that hasn't been consumed and read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            await _mediaServiceContext.Medias.AddAsync(mediaModel);
            await _mediaServiceContext.SaveChangesAsync();
            MediaDto mediaDto = _mapper.Map<MediaDto>(mediaModel);
            mediaDto.DownloadUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/api/v1/media/{mediaModel.Id}";
            return mediaDto;
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