using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using SEP.P724.MediaService.Contract;

namespace SEP.P724.MediaService.Services
{
    public class MediaServiceImpl : IMediaService
    {
        private static int MaxDirectoryCuont = 100;

        public MediaDto GetMedia(Guid mediaId)
        {
            var media = new MediaDto() {Id = Guid.NewGuid()};
            return media;
        }

        public object GetMedia()
        {
            var path =
                @"C:\Users\m.zerehpoush\w\SEP.P724.MediaServiceSolution\src\SEP.P724.MediaService\Resources\Files\f424c6d8-145e-41cd-932d-e07c0f6f411f";
            return File.Open(path,FileMode.Open);
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
    }
}