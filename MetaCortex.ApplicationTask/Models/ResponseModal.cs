using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MetaCortex.ApplicationTask.Models
{
    public abstract class AbstractResponseModal
    {
        protected string message { get; set; } = null;
        protected bool result { get; set; } = false;
    }
    public class ResponseModal<T> : ResponseModal
    {
        public T Data { get; set; }
    }
    public class ResponseModal : AbstractResponseModal
    {
        protected HttpStatusCode? statusCode { get; set; }
        public HttpStatusCode StatusCode
        {
            get { return statusCode.HasValue ? statusCode.Value : (!string.IsNullOrEmpty(ErrorException) ? HttpStatusCode.InternalServerError : HttpStatusCode.OK); }
            set { statusCode = value; }
        }
        public string ErrorException { get; set; }
        public Exception Exception { set { ErrorException = value.Message; } }
        public string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(message)) return message;
                else
                {
                    if (StatusCode == HttpStatusCode.OK) return "Successful.";
                    else return $"Error: {StatusCode.ToString()}, code: {(int)StatusCode}";
                }
            }
            set => message = value;
        }
        public bool HasMessage { get => !string.IsNullOrEmpty(Message); }
    }
}