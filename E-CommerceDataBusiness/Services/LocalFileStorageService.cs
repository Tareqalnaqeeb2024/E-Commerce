//using E_CommerceDataBusiness.Interfaces;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace E_CommerceDataBusiness.Services
//{
//    public class LocalFileStorageService : IFileStorageService
//    {

//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public LocalFileStorageService(

//            IHttpContextAccessor httpContextAccessor)
//        {

//            _httpContextAccessor = httpContextAccessor;
//        }

//        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory = "images")
//        {
//            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//            var filePath = GetFilePath(fileName, subDirectory);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            return fileName;
//        }

//        public async Task<(Stream FileStream, string ContentType)> GetFileAsync(
//            string fileName, string subDirectory = "images")
//        {
//            var filePath = GetFilePath(fileName, subDirectory);
//            if (!File.Exists(filePath))
//                throw new FileNotFoundException("File not found");

//            var mimeType = GetMimeType(filePath);
//            var fileStream = File.OpenRead(filePath);

//            return (fileStream, mimeType);
//        }

//        public async Task DeleteFileAsync(string fileName, string subDirectory = "images")
//        {
//            var filePath = GetFilePath(fileName, subDirectory);
//            if (File.Exists(filePath))
//            {
//                File.Delete(filePath);
//            }
//        }

//        public string GenerateFileUrl(string fileName, string subDirectory = "images")
//        {
//            var request = _httpContextAccessor.HttpContext?.Request;
//            return request != null
//                ? $"{request.Scheme}://{request.Host}/{subDirectory}/{fileName}"
//                : string.Empty;
//        }

//        private string GetFilePath(string fileName, string subDirectory)
//        {
//            return Path.Combine( "wwwroot/images", subDirectory, fileName);
//        }

//        private string GetMimeType(string filePath)
//        {
//            var extension = Path.GetExtension(filePath).ToLowerInvariant();
//            return extension switch
//            {
//                ".jpg" or ".jpeg" => "image/jpeg",
//                ".png" => "image/png",
//                ".gif" => "image/gif",
//                ".bmp" => "image/bmp",
//                _ => "application/octet-stream"
//            };
//        }
//    }
//}
using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorageService(
          IHostingEnvironment  hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory = "images")
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = GetFilePath(fileName, subDirectory);

            // Create directory if not exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<(Stream FileStream, string ContentType)> GetFileAsync(
            string fileName, string subDirectory = "images")
        {
            var filePath = GetFilePath(fileName, subDirectory);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found");

            var mimeType = GetMimeType(filePath);
            var fileStream = File.OpenRead(filePath);

            return (fileStream, mimeType);
        }

        public async Task DeleteFileAsync(string fileName, string subDirectory = "images")
        {
            var filePath = GetFilePath(fileName, subDirectory);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public string GenerateFileUrl(string fileName, string subDirectory = "images")
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            return request != null
                ? $"{request.Scheme}://{request.Host}/{subDirectory}/{fileName}"
                : string.Empty;
        }

        private string GetFilePath(string fileName, string subDirectory)
        {
            return Path.Combine(
                _hostingEnvironment.WebRootPath,  // Get proper wwwroot path
                subDirectory,
                fileName
            );
        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
