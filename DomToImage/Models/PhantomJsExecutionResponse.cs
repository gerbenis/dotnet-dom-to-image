using System.Collections.Generic;

namespace DomToImage.Models
{
    public class PhantomJsExecutionResponse
    {
        public PhantomJsExecutionResponse(int exitCode, IEnumerable<string> output)
        {
            ExitCode = exitCode;
            Output = output;
        }

        public int ExitCode { get; }
        
        public IEnumerable<string> Output { get; }
    }
}