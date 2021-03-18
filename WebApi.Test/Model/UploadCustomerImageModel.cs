using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Test.Helper;

namespace WebApi.Test.Model
{
    public class UploadCustomerImageModel
    {
        public string Description { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] ImageData { get; set; }
    }
}
