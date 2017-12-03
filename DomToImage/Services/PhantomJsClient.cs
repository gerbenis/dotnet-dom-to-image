using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DomToImage.Infrastructure.Exceptions;
using DomToImage.Models;
using DomToImage.ServiceContracts;

namespace DomToImage.Services
{
    public class PhantomJsClient : IPhantomJsClient
    {
        public Task<PhantomJsExecutionResponse> ExecuteScript(string path, IEnumerable<string> args)
        {
            var tcs = new TaskCompletionSource<PhantomJsExecutionResponse>();
            var stdOutEntries = new List<string>();
            var stdErrEntries = new List<string>();

            var phantom = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(HttpRuntime.AppDomainAppPath, "bin", "phantomjs.exe"),
                    Arguments = $"\"{path}\" {string.Join(" ", args.Select(x => $"\"{x}\""))}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            phantom.OutputDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                    stdOutEntries.Add(eventArgs.Data);
            };
            phantom.ErrorDataReceived += (sender, eventArgs) =>
            {
                if (!string.IsNullOrWhiteSpace(eventArgs.Data))
                    stdErrEntries.Add(eventArgs.Data);
            };
            phantom.Exited += (sender, eventArgs) =>
            {
                if (stdErrEntries.Count > 0)
                    tcs.SetException(new PhantomJsException(phantom.ExitCode, stdErrEntries));
                else
                    tcs.SetResult(new PhantomJsExecutionResponse(phantom.ExitCode, stdOutEntries));
                phantom.CancelOutputRead();
                phantom.CancelErrorRead();
                phantom.Dispose();
            };

            phantom.Start();
            phantom.BeginOutputReadLine();
            phantom.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}