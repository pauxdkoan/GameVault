using System.Globalization;

namespace GameVault.Source.Application.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode {  get; }
        public IDictionary<string, string[]>? Errors { get; }
        public ApiException():base() { }
        public ApiException(
               string message,
               int statusCode,
               IDictionary<string, string[]>? errors = null,
               Exception? innerException = null)
               : base(message, innerException)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
