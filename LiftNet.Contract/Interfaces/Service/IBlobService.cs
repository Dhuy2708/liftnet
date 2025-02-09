using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.Service
{
    public interface IBlobService
    {
        // file
        Task<string> UploadFileAsync(IFormFile file, string connectionString, string containerName);
        Task<byte[]> DownloadFileAsync(string fileName, string connectionString, string containerName);
        Task<bool> DeleteFileAsync(string fileName, string connectionString, string containerName);
        Task<List<string>> ListFilesAsync(string connectionString, string containerName);

        // container
        Task<bool> CreateContainerAsync(string connectionString, string containerName);
        Task<bool> DeleteContainerAsync(string connectionString, string containerName);
        Task<List<string>> ListContainersAsync(string connectionString);
    }
}
