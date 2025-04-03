using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiftNet.MapSDK.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.MapSDK.Contracts;

namespace LiftNet.MapSDK.Apis.Tests
{
    [TestClass()]
    public class AutocompleteApiTests
    {
        [TestMethod()]
        public async Task GetAutocompleteAsyncTest()
        {
            var key = "...";
            var mapApiKey = new MapApiKey
            {
                Key = key,
            };
            var api = new AutocompleteApi(mapApiKey);
            var result = await api.GetAutocompleteAsync("ly thai", 10.772211, 106.663493);
            Assert.IsNotNull(result);
        }
    }
}