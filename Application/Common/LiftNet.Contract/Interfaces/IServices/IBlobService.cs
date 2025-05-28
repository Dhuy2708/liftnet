using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IBlobService
    {
        Task<BlobContainerClient> GetContainerClient(string containerName, bool createIfNotExist = true);
        // file
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        Task<byte[]> DownloadFileAsync(string fileName, string containerName);
        Task<bool> DeleteFileAsync(string fileName, string containerName);
        Task<List<string>> ListFilesAsync(string containerName);

        // container
        Task<bool> CreateContainerAsync(string containerName);
        Task<bool> DeleteContainerAsync(string containerName);
        Task<List<string>> ListContainersAsync();
    }
}
