using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ColdFusion.RestPostProcess {

    public class JsonStringCleanserModule : IHttpModule {

        void IHttpModule.Dispose() {}

        void IHttpModule.Init(HttpApplication context) {
            context.PostRequestHandlerExecute += context_PostRequestHandlerExecute;
        }

        void context_PostRequestHandlerExecute(object sender, EventArgs e) {
            var httpContext = ((HttpApplication)sender).Context;

            if (!httpContext.Response.ContentType.StartsWith("application/json"))
                return;

            httpContext.Response.Filter = new JsonStringCleanser(httpContext.Response.Filter, httpContext.Request);
        }

    }

    public class JsonStringCleanser : MemoryStream {
        private readonly Stream _output = null;
        private readonly HttpRequest _request = null;

        public JsonStringCleanser(Stream output, HttpRequest request) {
            _output = output;
            _request = request;
        }

        public override void Write(byte[] buffer, int offset, int count) {

            string content = UTF8Encoding.UTF8.GetString(buffer, offset, count);
            content = content.Replace("\"***string***:", "\"");
            buffer = UTF8Encoding.UTF8.GetBytes(content);
            _output.Write(buffer, 0, buffer.Length);

        }
    }
}
