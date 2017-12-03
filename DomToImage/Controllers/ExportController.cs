using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DomToImage.Infrastructure.WebApi;
using DomToImage.Models;
using DomToImage.ServiceContracts;
using DomToImage.Services;

namespace DomToImage.Controllers
{
    [RoutePrefix("api/export")]
    public class ExportController : ApiController
    {
        private readonly IImageExportService _imageExportService;

        public ExportController(IImageExportService imageExportService)
        {
            _imageExportService = imageExportService;
        }

        [HttpPost]
        [Route("dom-to-image")]
        public async Task<IHttpActionResult> ExportDomToImage([FromBody] ExportDomToImageRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _imageExportService.ExportDomToImage(request);
                
                return new FileContentResult(HttpStatusCode.OK, result.Filename, result.Content, this);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
