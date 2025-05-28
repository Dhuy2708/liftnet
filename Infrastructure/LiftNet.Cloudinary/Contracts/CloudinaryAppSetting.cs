using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiftNet.Cloudinary.Contracts
{
    public class CloudinaryAppSetting
    {
        public string CloudName
        {
            get; set;
        } = String.Empty;
        public string ApiKey
        {
            get; set;
        } = String.Empty;
        public string ApiSecret
        {
            get; set;
        } = String.Empty;
    }
}
