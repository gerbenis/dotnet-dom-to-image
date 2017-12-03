using System.Collections.Generic;
using System.Threading.Tasks;
using DomToImage.Models;

namespace DomToImage.ServiceContracts
{
    public interface IPhantomJsClient
    {
        Task<PhantomJsExecutionResponse> ExecuteScript(string path, IEnumerable<string> args);
    }
}