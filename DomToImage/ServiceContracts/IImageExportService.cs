using System.Threading.Tasks;
using DomToImage.Models;

namespace DomToImage.ServiceContracts
{
    public interface IImageExportService
    {
        Task<ImageResponse> ExportDomToImage(ExportDomToImageRequest request);
    }
}