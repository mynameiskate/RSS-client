using System;
using System.Net;
using System.Net.Http;

namespace RssClient.HttpProtocol
{
    class HttpRequest
    {
        public HttpRequest(string uri)
        {
            _uri = uri;
        }

        public WebRequest Request
        {
            get
            {
                return CreateRequest();
            }
        }

        public void SetTimeOut(int timeOut)
        {
            _timeOutLimit = timeOut;
        }

        private static int _timeOutLimit = 10000;
        private string _uri;
        private HttpMethod _method = HttpMethod.Get;

        private WebRequest CreateRequest()
        {
            string result = String.Empty;
            WebRequest request = WebRequest.Create(_uri);
            request.Method = _method.ToString();
            request.Timeout = _timeOutLimit;
            return request;
        }
    }
}
