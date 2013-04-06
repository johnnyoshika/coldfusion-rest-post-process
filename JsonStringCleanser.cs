using System;
using System.IO;
using System.Text;
using System.Web;

namespace ColdFusion.RestPostProcess {

    public class JsonStringCleanserModule : IHttpModule {

        void IHttpModule.Dispose() {}

        void IHttpModule.Init(HttpApplication context) {
            context.PostRequestHandlerExecute += context_PostRequestHandlerExecute;
        }

        void context_PostRequestHandlerExecute(object sender, EventArgs e) {
            var httpContext = ((HttpApplication)sender).Context;

            if (httpContext.Response.ContentType == null || !httpContext.Response.ContentType.StartsWith("application/json"))
                return;

            if (httpContext.Response.ContentEncoding != Encoding.UTF8)
                return;

            httpContext.Response.Filter = new UTF8SanitizerStream("\"***string***:", "\"", httpContext.Response.Filter);
        }

    }

}
