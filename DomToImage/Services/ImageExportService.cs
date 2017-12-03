using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DomToImage.Models;
using DomToImage.ServiceContracts;
using Newtonsoft.Json;

namespace DomToImage.Services
{
    public class ImageExportService : IImageExportService
    {
        private readonly IPhantomJsClient _phantomJsClient;

        public ImageExportService(IPhantomJsClient phantomJsClient)
        {
            _phantomJsClient = phantomJsClient;
        }

        public async Task<ImageResponse> ExportDomToImage(ExportDomToImageRequest request)
        {
            var scriptPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Scripts", "exportDom.js");
            request.Filename = AdjustFilename(request.Filename);
            var options = SerializeRequest(request);
            var result = await _phantomJsClient.ExecuteScript(scriptPath, new [] {options});
            
            return new ImageResponse
            {
                Filename = request.Filename,
                Content = Convert.FromBase64String(result.Output.First())
            };
        }

        private string AdjustFilename(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension) ||
                !extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) &&
                !extension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase))
            {
                filename = $"{filename}.png";
            }

            return filename;
        }

        private string SerializeRequest(ExportDomToImageRequest request)
        {
            var options = JsonConvert.SerializeObject(request,
                GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
            
            return options
                .Replace("\\n", "")
                .Replace("\\t", "")
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}