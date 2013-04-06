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

            if (httpContext.Response.ContentType == null || !httpContext.Response.ContentType.StartsWith("application/json"))
                return;

            if (httpContext.Response.Status == null || httpContext.Response.Status.StartsWith("2"))
                return;

            if (httpContext.Response.ContentEncoding != Encoding.UTF8)
                return;

            httpContext.Response.Filter = new UTF8SanitizerStream("\"Message\":", "\"message\":",
                new UTF8SanitizerStream("\"Type\":", "\"type\":", httpContext.Response.Filter)
            );
        }

    }

}
