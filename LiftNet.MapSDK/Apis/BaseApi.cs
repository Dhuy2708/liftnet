using LiftNet.Ioc;
using LiftNet.MapSDK.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.MapSDK.Apis
{
    public abstract class BaseApi
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly string _apiKey;

        protected BaseApi(string key, string baseUrl)
        {
            _apiKey = key;
            _baseUrl = baseUrl;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
    }
}
