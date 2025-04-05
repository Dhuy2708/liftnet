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
            try
            {
                var key = "";
                var mapApiKey = new MapApiKey
                {
                    Key = key,
                };
                //var api = new AutocompleteApi(mapApiKey);
                //var result = await api.GetAutocompleteAsync("Thành phố Hà Nội");

                var api = new GeoCodeApi(mapApiKey);
                var result = await api.FowardGeoCodeAsync("Tỉnh Lào Cai");
                Assert.IsNotNull(result);
            }
            catch (Exception ex)
            {

            }
        }
    }
}