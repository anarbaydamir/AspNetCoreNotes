using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Test.Configuration;
using WebApi.Test.Model;

namespace WebApi.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly CarConfiguration carConfiguration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IOptions<CarConfiguration> configurationSingletion, CarConfiguration carConfiguration,
            IOptionsSnapshot<CarConfiguration> carConfigurationTransient)
        {
            _logger = logger;
            this.carConfiguration = carConfigurationTransient.Value;
        }

        [HttpGet]
        public CarConfiguration Get()
        {
            SetPaginationHeader(1, 10, 100, 2000);
            return carConfiguration;

        }

        [HttpPost]
        [HttpPost("{customerId}/images")]
        public FileContentResult UploadCustomerImage([FromBody] UploadCustomerImageModel model)
        {
            //Depending on if you want the byte array or a memory stream, you can use the below. 
            //THIS IS NO LONGER NEEDED AS OUR MODEL NOW HAS A BYTE ARRAY
            //var imageDataByteArray = Convert.FromBase64String(model.ImageData);

            //When creating a stream, you need to reset the position, without it you will see that you always write files with a 0 byte length. 
            var imageDataStream = new MemoryStream(model.ImageData);
            imageDataStream.Position = 0;
            using (FileStream fstream = new FileStream(@"E:/personal/WebApi.Test/WebApi.Test/" +
                    "salam.png", FileMode.Create))
            {
                //memStream.WriteTo(fstream);
                imageDataStream.WriteTo(fstream);
            }


            //Go and do something with the actual data.
            //_customerImageService.Upload([...])

            //For the purpose of the demo, we return a file so we can ensure it was uploaded correctly. 
            //But otherwise you can just return a 204 etc. 
            return File(model.ImageData, "image/png");
        }

        private void SetPaginationHeader(int pageSize,int pageNumber,int pageCount,int totalRecords)
        {
            HttpContext.Response.Headers.Add("PageNumber", pageNumber.ToString());
            HttpContext.Response.Headers.Add("PageSize", pageSize.ToString());
            HttpContext.Response.Headers.Add("PageCount", pageCount.ToString());
            HttpContext.Response.Headers.Add("TotalRecords", totalRecords.ToString());
        }
        
    }
}
