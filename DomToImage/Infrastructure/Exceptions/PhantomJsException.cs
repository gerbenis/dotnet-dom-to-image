using System;
using System.Collections.Generic;

namespace DomToImage.Infrastructure.Exceptions
{
    public class PhantomJsException : Exception
    {
        public PhantomJsException(int exitCode, IList<string> errors) : base(string.Join(" ", errors))
        {
            Errors = errors;
            ExitCode = exitCode;
        }

        public PhantomJsException(int exitCode, IList<string> errors, Exception innerException)
            : base(string.Join(" ", errors), innerException)
        {
            Errors = errors;
            ExitCode = exitCode;
        }

        public int ExitCode { get; }

        public IList<string> Errors { get; }
    }
}