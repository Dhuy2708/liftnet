using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.Domain.Interfaces;
using LiftNet.Cloudinary.Contracts;
using LiftNet.Contract.Constants;

namespace LiftNet.Cloudinary.Services.Impl
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary ;
        private readonly ILiftLogger<CloudinaryService> _logger;

        public CloudinaryService(ILiftLogger<CloudinaryService> logger, CloudinaryAppSetting setting)
        {
            _logger = logger;
            var account = new Account(setting.CloudName, setting.ApiKey, setting.ApiSecret);
            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public async Task<string> HostImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File cannot be null or empty.", nameof(file));
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(CoreConstant.MAXIMUM_IMAGE_RESOLUTION.WIDTH)
                                                            .Height(CoreConstant.MAXIMUM_IMAGE_RESOLUTION.HEIGHT)
                                                            .Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        throw new Exception(uploadResult.Error.Message);
                    }

                    return uploadResult.SecureUrl.ToString() ?? String.Empty;
                }
            }
            catch (FileNotFoundException fe)
            {
                _logger.Error($"[CloudinaryService.HostImageAsync] Cloudinary keys file not found, ex {fe}");
            }
            catch (Exception e)
            {
                _logger.Error($"[CloudinaryService.HostImageAsync] {e}");

            }
            return String.Empty;
        }

        public async Task<bool> DeleteImageAsync(List<string> publicIds)
        {
            try
            {
                var deleteResult = await _cloudinary.DeleteResourcesAsync(ResourceType.Image, publicIds.ToArray());

                return deleteResult.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (FileNotFoundException fe)
            {
                _logger.Error($"[CloudinaryService.HostImageAsync] Cloudinary keys file not found, ex {fe}");
            }
            catch (Exception e)
            {
                _logger.Error($"[CloudinaryService.HostImageAsync] {e}");
            }
            return false;
        }

        public async Task<string> HostFileAsync(IFormFile file, string prefix)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File cannot be null or empty.", nameof(file));
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var fileName = $"{prefix}_{Path.GetFileNameWithoutExtension(file.FileName)}{Path.GetExtension(file.FileName)}";

                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(fileName, stream),
                        Folder = "uploads",
                        PublicId = fileName,
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        throw new Exception(uploadResult.Error.Message);
                    }

                    return uploadResult.SecureUrl.ToString() ?? string.Empty;
                }
            }
            catch (Exception e)
            {
                _logger.Error($"[CloudinaryService.UploadFileWithPrefixAsync] {e}");
                return string.Empty;
            }
        }
    }
}
