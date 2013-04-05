using System;
using System.IO;
using System.Text;
using System.Web;

namespace ColdFusion.RestPostProcess {

    public class JsonCamelCasePropertyConverterModule : IHttpModule {

        void IHttpModule.Dispose() {}

        void IHttpModule.Init(HttpApplication context) {
            context.PostRequestHandlerExecute += context_PostRequestHandlerExecute;
        }

        void context_PostRequestHandlerExecute(object sender, EventArgs e) {
            var httpContext = ((HttpApplication)sender).Context;

            if (!httpContext.Response.ContentType.StartsWith("application/json"))
                return;

            if (httpContext.Response.Status.StartsWith("2"))
                return;

            httpContext.Response.Filter = new JsonCamelCasePropertyConverter(httpContext.Response.Filter, httpContext.Request);
        }

    }

    public class JsonCamelCasePropertyConverter : MemoryStream {
        private readonly Stream _output = null;
        private readonly HttpRequest _request = null;

        public JsonCamelCasePropertyConverter(Stream output, HttpRequest request) {
            _output = output;
            _request = request;
        }

        public override void Write(byte[] buffer, int offset, int count) {

            string content = UTF8Encoding.UTF8.GetString(buffer, offset, count);
            content = content.Replace("\"Message\":", "\"message\":");
            content = content.Replace("\"Type\":", "\"type\":");
            buffer = UTF8Encoding.UTF8.GetBytes(content);
            _output.Write(buffer, 0, buffer.Length);

        }
    }
}
