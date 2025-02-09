using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using LiftNet.Contract.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LiftNet.AzureBlob.Services
{
    public class BlobService : IBlobService
    {
        private readonly ILogger<BlobService> _logger;

        public BlobService(ILogger<BlobService> logger)
        {
            _logger = logger;
        }

        private async Task<BlobContainerClient> GetContainerClient(string connectionString, string containerName, bool createIfNotExist = true)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Container name cannot be null or empty.");
            }

            var containerClient = new BlobContainerClient(connectionString, containerName);
            containerClient.CreateIfNotExists();

            return containerClient;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string connectionString, string containerName)
        {
            var containerClient = await GetContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(file.FileName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);
            return blobClient.Uri.ToString();
        }

        public async Task<byte[]> DownloadFileAsync(string fileName, string connectionString, string containerName)
        {
            var containerClient = await GetContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"File '{fileName}' not found in container '{containerName}'.");
            }

            var response = await blobClient.DownloadAsync();
            using var memoryStream = new MemoryStream();
            await response.Value.Content.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<bool> DeleteFileAsync(string fileName, string connectionString, string containerName)
        {
            var containerClient = await GetContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> ListFilesAsync(string connectionString, string containerName)
        {
            var containerClient = await GetContainerClient(connectionString, containerName);
            var blobs = containerClient.GetBlobsAsync();
            var files = new List<string>();
            await foreach (var blob in blobs)
            {
                files.Add(blob.Name);
            }
            return files;
        }

        public async Task<bool> CreateContainerAsync(string connectionString, string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Container name cannot be null or empty.");
            }

            var containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return true;
        }

        public async Task<bool> DeleteContainerAsync(string connectionString, string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Container name cannot be null or empty.");
            }

            var containerClient = new BlobContainerClient(connectionString, containerName);
            return await containerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> ListContainersAsync(string connectionString)
        {
            var serviceClient = new BlobServiceClient(connectionString);
            var containers = serviceClient.GetBlobContainersAsync();
            var containerNames = new List<string>();
            await foreach (var container in containers)
            {
                containerNames.Add(container.Name);
            }
            return containerNames;
        }
    }
}
