using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Extension
{
    public static class Extension
    {
        public static string Join(this System.Collections.Specialized.NameValueCollection collection, Func<string, string> selector, string separator)
        {
            return String.Join(separator, collection.Cast<string>().Select(e => selector(e)));
        }
    }

    public static class SubstringExtension
    {
        public static string ZSubstring(this string str, int from, int to)
        {
            return str.Length >= to ? str.Substring(from, to) : str;
        }
    }
}