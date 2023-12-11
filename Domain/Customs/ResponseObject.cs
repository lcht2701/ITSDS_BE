using System.Net;
using System.Text.Json.Serialization;

namespace Domain.Customs
{
    public class ResponseObject<T>
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        [JsonIgnore]
        public bool Success { get; set; } = false;
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ResponseObject() { }
        public ResponseObject(HttpStatusCode statusCode, bool success, string? message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }
    }
}
