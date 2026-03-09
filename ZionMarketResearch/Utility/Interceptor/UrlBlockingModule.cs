using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ZionMarketResearch.Utility.Interceptor
{
    public class UrlBlockingModule : IHttpModule
    {
        private HttpApplication _context;

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            this._context = context;
            context.BeginRequest += new EventHandler(this.Context_BeginRequest);
            context.EndRequest += new EventHandler(this.Context_EndRequest);
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            if (!new Regex("[^0-9A-Za-z/.\\-\\&=_? ]").IsMatch(this._context.Request.Url.AbsolutePath) && !this._context.Request.Url.AbsolutePath.Contains("%C3%"))
                return;
            this._context.Response.Write("404");
            this._context.Response.StatusCode = 404;
        }
    }
}