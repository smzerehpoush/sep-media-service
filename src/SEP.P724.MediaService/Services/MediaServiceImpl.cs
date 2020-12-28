using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SEP.P724.MediaService.Configs;
using SEP.P724.MediaService.Context;
using SEP.P724.MediaService.Contract;
using SEP.P724.MediaService.Exceptions;
using SEP.P724.MediaService.Model;
using SEP.P724.MediaService.Utilities;

namespace SEP.P724.MediaService.Services
{
    public class MediaServiceImpl : IMediaService
    {
        private readonly IMapper _mapper;
        private readonly MediaContext _mediaContext;
        private readonly Configuration _configuration;
        private readonly ILogger _logger;

        public MediaServiceImpl(IMapper mapper, MediaContext mediaContext, ILogger<MediaServiceImpl> logger)
        {
            _mapper = mapper;
            _mediaContext = mediaContext;
            _logger = logger;
            _configuration = new Configuration();
        }

        public async Task<Tuple<MediaModel, byte[]>> GetMedia(Guid mediaId)
        {
            _logger.LogInformation($"trying to get media by id [{mediaId}]");
            var mediaModel = await GetMediaById(mediaId);
            IncrementDownloadCount(mediaModel);
            var path = Path.Combine(_configuration.GetPhysicalStorageLocation(), mediaModel.Id.ToString());
            CheckFileExistence(path, mediaModel);
            var bytes = await File.ReadAllBytesAsync(path);
            return new Tuple<MediaModel, byte[]>(mediaModel, bytes);
        }

        private void CheckFileExistence(string path, MediaModel mediaModel)
        {
            if (!File.Exists(path))
            {
                throw new MediaNotFoundException(mediaModel.Id);
            }
        }

        private void IncrementDownloadCount(MediaModel mediaModel)
        {
            mediaModel.DownloadCount = mediaModel.DownloadCount + 1;
            _mediaContext.SaveChangesAsync();
        }

        public async Task<MediaDto> UploadMedia(HttpRequest request)
        {
            CheckContentType(request);

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType),
                new FormOptions().MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();
            MediaModel mediaModel = null!;
            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition != null && contentDisposition.IsFileDisposition())
                {
                    var trustedFileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                    var mimeType = GetMimeTypes(Path.GetExtension(trustedFileName));
                    mediaModel = new MediaModel(trustedFileName, mimeType);
                    var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition,
                        _configuration.GetPermittedExtensions(), _configuration.GetFileSizeLimit());
                    Directory.CreateDirectory(_configuration.GetPhysicalStorageLocation());
                    var fullPath = Path.Combine(_configuration.GetPhysicalStorageLocation(),
                        mediaModel.Id.ToString());

                    await using var targetStream = File.Create(fullPath);
                    await targetStream.WriteAsync(streamedFileContent);
                }

                // Drain any remaining section body that hasn't been consumed and read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            await SaveMediaToDb(mediaModel);
            MediaDto mediaDto = _mapper.Map<MediaDto>(mediaModel);
            mediaDto.DownloadUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/api/v1/media/{mediaModel?.Id}";
            return mediaDto;
        }

        private async Task SaveMediaToDb(MediaModel? mediaModel)
        {
            if (mediaModel == null)
            {
                throw new MediaUploadException("Media Upload Failed.");
            }

            await _mediaContext.Medias.AddAsync(mediaModel);
            await _mediaContext.SaveChangesAsync();
        }

        private static void CheckContentType(HttpRequest request)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
            {
                throw new MediaUploadException("The request couldn't be processed. invalid content type");
            }
        }

        private async Task<MediaModel> GetMediaById(Guid mediaId)
        {
            return await _mediaContext.Medias.FirstOrDefaultAsync(media => media.Id == mediaId);
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
                case ".jpg": return "image/jpg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".svg": return "image/svg+xml";
                case ".mp4": return "video/mp4";
                default: return "application/octet-stream";
            }
        }
    }
}