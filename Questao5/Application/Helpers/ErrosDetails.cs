using Newtonsoft.Json;
using System.Net;

namespace Questao5.Application.Helpers
{
    public class ErrorDetails
    {
        public ErrorDetails(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;

            Message = message;
        }

        public ErrorDetails(string message)
        {
            StatusCode = HttpStatusCode.PreconditionFailed;

            Message = message;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string Message { get; private set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }

}
