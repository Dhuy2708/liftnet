using LiftNet.Ioc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Cloudinary.Services
{
    public interface ICloudinaryService
    {
        Task<string> HostImageAsync(IFormFile imageFile);
        Task<bool> DeleteImageAsync(List<string> publicIds);
        Task<string> HostFileAsync(IFormFile file, string prefix);
    }
}
