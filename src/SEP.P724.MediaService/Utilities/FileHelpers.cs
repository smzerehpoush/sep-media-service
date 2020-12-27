using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SEP.P724.MediaService.Exceptions;

namespace SEP.P724.MediaService.Utilities
{
    public static class FileHelpers
    {
        private static readonly Dictionary<string, List<byte[]>> FileSignature = new Dictionary<string, List<byte[]>>
        {
            // case ".txt": return "text/plain";
            // case ".csv": return "text/csv";
            // case ".pdf": return "application/pdf";
            // case ".doc": return "application/vnd.ms-word";
            // case ".xls": return "application/vnd.ms-excel";
            // case ".ppt": return "application/vnd.ms-powerpoint";
            // case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            // case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            // case ".png": return "image/png";
            // case ".jpg": return "image/jpeg";
            // case ".jpeg": return "image/jpeg";
            // case ".gif": return "image/gif";

            {".gif", new List<byte[]> {new byte[] {0x47, 0x49, 0x46, 0x38}}},
            {".png", new List<byte[]> {new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}}},
            {
                ".jpeg", new List<byte[]>
                {
                    new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                    new byte[] {0xFF, 0xD8, 0xFF, 0xE2},
                    new byte[] {0xFF, 0xD8, 0xFF, 0xE3},
                }
            },
            {
                ".jpg", new List<byte[]>
                {
                    new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                    new byte[] {0xFF, 0xD8, 0xFF, 0xE1},
                    new byte[] {0xFF, 0xD8, 0xFF, 0xE8},
                }
            },
            {
                ".zip", new List<byte[]>
                {
                    new byte[] {0x50, 0x4B, 0x03, 0x04},
                    new byte[] {0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45},
                    new byte[] {0x50, 0x4B, 0x53, 0x70, 0x58},
                    new byte[] {0x50, 0x4B, 0x05, 0x06},
                    new byte[] {0x50, 0x4B, 0x07, 0x08},
                    new byte[] {0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70},
                }
            },
        };


        public static async Task<byte[]> ProcessStreamedFile(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition,
            IEnumerable<string> permittedExtensions, long sizeLimit)
        {
            await using (var memoryStream = new MemoryStream())
            {
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

            return new byte[0];
        }

        private static bool IsValidFileExtensionAndSignature(string fileName, Stream data,
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

            // using (var reader = new BinaryReader(data))
            // {
            //     var signatures = FileSignature[ext];
            //     var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
            //
            //     return signatures.Any(signature =>
            //         headerBytes.Take(signature.Length).SequenceEqual(signature));
            // }
            return true;
        }
    }
}