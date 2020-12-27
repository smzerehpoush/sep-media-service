using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SEP.P724.MediaService.Exceptions;

namespace SEP.P724.MediaService.Utilities
{
    public static class FileHelpers
    {
        public static async Task<byte[]> ProcessStreamedFile(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition,
            IEnumerable<string> permittedExtensions, long sizeLimit)
        {
            await using var memoryStream = new MemoryStream();
            await section.Body.CopyToAsync(memoryStream);

            // Check if the file is empty or exceeds the size limit.
            if (memoryStream.Length == 0)
            {
                throw new MediaUploadException("The file is empty.");
            }
            else if (memoryStream.Length > sizeLimit)
            {
                throw new MediaUploadException($"The file exceeds {sizeLimit}.");
            }
            else if (!IsValidFileExtensionAndSignature(contentDisposition.FileName.Value, memoryStream,
                permittedExtensions))
            {
                throw new MediaUploadException(
                    "The file type isn't permitted or the file's signature doesn't match the file's extension.");
            }
            else
            {
                return memoryStream.ToArray();
            }
        }

        private static bool IsValidFileExtensionAndSignature(string? fileName, Stream? data,
            IEnumerable<string> permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            var extensions = permittedExtensions as string[] ?? permittedExtensions.ToArray();
            if (string.IsNullOrEmpty(ext) || (!extensions.Contains("*") && !extensions.Contains(ext)))
            {
                return false;
            }

            data.Position = 0;

            return true;
        }
    }
}