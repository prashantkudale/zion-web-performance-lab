using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Extension
{
    public static class StringExtension
    {
        public static int IndexOf(this string str, string[] find)
        {
            for (int index = 0; index < find.Length; ++index)
            {
                if (str.ToLower().IndexOf(find[index].ToLower()) > -1)
                    return str.ToLower().IndexOf(find[index].ToLower());
            }
            return -1;
        }

        public static string ToProperCasing(this string str) => new CultureInfo("en-US", false).TextInfo.ToTitleCase(str);
    }
}