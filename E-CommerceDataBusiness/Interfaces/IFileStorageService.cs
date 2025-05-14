using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    // E_CommerceDataBusiness/Interfaces/IFileStorageService.cs
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string subDirectory = "images");
        Task<(Stream FileStream, string ContentType)> GetFileAsync(string fileName, string subDirectory = "images");
        Task DeleteFileAsync(string fileName, string subDirectory = "images");
        string GenerateFileUrl(string fileName, string subDirectory = "images");
    }
}
