using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DomToImage.Infrastructure.WebApi
{
    public class FileContentResult : IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _filename;
        private readonly byte[] _content;
        private readonly HttpRequestMessage _request;

        public FileContentResult(HttpStatusCode statusCode, string filename, byte[] content, HttpRequestMessage request)
        {
            _statusCode = statusCode;
            _filename = filename;
            _content = content;
            _request = request;
        }

        public FileContentResult(HttpStatusCode statusCode, string filename, byte[] content, ApiController controller)
        {
            _statusCode = statusCode;
            _filename = filename;
            _content = content;
            _request = controller.Request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var mimeType = MimeMapping.GetMimeMapping(_filename);
            var response = new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new ByteArrayContent(_content),
                RequestMessage = _request
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = _filename
            };

            return Task.FromResult(response);
        }
    }
}