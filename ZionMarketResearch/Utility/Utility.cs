using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ZionMarketResearch.Util
{
    public static class Utility
    {
        public static string ClientIPAddress { get { return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["Remote_Addr"] != null ? HttpContext.Current.Request.ServerVariables["Remote_Addr"] : HttpContext.Current.Request.UserHostAddress != null && HttpContext.Current.Request.UserHostAddress != string.Empty ? HttpContext.Current.Request.UserHostAddress : "No IP Address"; } }
        static string _view;
        public static string View
        {
            get
            {
                return "_Layout";
            }
            set
            {
                _view = "_LayoutWithoutAnalytics";
            }
        }


        public static object GetCache(string key)
        {
            return HttpContext.Current.Cache[key];
        }

        public static string[] GetUrlFragment()
        {
            return HttpContext.Current.Request.RawUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetUrlFragment(string rawUrl)
        {
            return rawUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        //public static IEnumerable<SelectListItem> Country()
        //{
        //    mvcmrszionmysqldbEntities db = new mvcmrszionmysqldbEntities();
        //    var countries = from c in db.tblcountries
        //                    select new SelectListItem
        //                    {
        //                        Text = c.country_name,
        //                        Value = c.country_alpha3_code
        //                    };
        //    return countries;
        //}

        //public static IEnumerable<SelectListItem> CountryForm()
        //{
        //    mvcmrszionmysqldbEntities db = new mvcmrszionmysqldbEntities();
        //    var countries = from c in db.tblcountries
        //                    select new SelectListItem
        //                    {
        //                        Text = c.country_name,
        //                        Value = c.country_name
        //                    };
        //    return countries;
        //}

        //public static IEnumerable<SelectListItem> ParentCategory()
        //{
        //    mvcmrszionmysqldbEntities db = new mvcmrszionmysqldbEntities();
        //    var category = (from c in db.tblcategories
        //                    where c.ParentCategoryId == 0 || c.ParentCategoryId == null
        //                    select c).ToList();
        //    var selectlistitem = from cats in category
        //                         select new SelectListItem
        //                         {
        //                             Text = cats.CategoryName,
        //                             Value = cats.pkCategoryID.ToString()
        //                         };
        //    return selectlistitem;
        //}

        public static string StripHTML(string source)
        {
            try
            {
                string result;

                if (string.IsNullOrEmpty(source))
                    return string.Empty;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {
                //MessageBox.Show("Error");
                return source;
            }
        }



        public static string ReadTemplate(string templateName)
        {
            return File.ReadAllText(HttpContext.Current.Server.MapPath("/Utility/Template/" + templateName) + ".html");
        }

        public static string ReplaceToken(Dictionary<string, string> tokenValue, string template)
        {
            foreach (var token in tokenValue)
            {
                template = template.Replace(token.Key, token.Value);
            }
            return template;
        }

        public static string GetHtml(string templateName, Dictionary<string, string> tokenValue)
        {
            return ReplaceToken(tokenValue, ReadTemplate(templateName));
        }

        //public static List<string> ZionMail()
        //{
        //    mvcmrszionmysqldbEntities db = new mvcmrszionmysqldbEntities();
        //    var emails = from z in db.tblzionmails where z.IsEnabled == true select z.EmailID;
        //    return emails.ToList();
        //}

        public static string Encryptstring(string PlainText, string Password = "ZION", string Salt = "Zion@25386", string HashAlgorithm = "MD5", int PasswordIterations = 2, string InitialVector = "ghVNHfg544JUdfjdY5BGVbj6784736372", int KeySize = 256)
        {

            if (string.IsNullOrEmpty(PlainText))

                return "";

            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);

            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);

            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);

            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);

            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);

            RijndaelManaged SymmetricKey = new RijndaelManaged();

            SymmetricKey.Mode = CipherMode.ECB;

            byte[] CipherTextBytes = null;

            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {

                using (MemoryStream MemStream = new MemoryStream())
                {

                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                    {

                        CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);

                        CryptoStream.FlushFinalBlock();

                        CipherTextBytes = MemStream.ToArray();

                        MemStream.Close();

                        CryptoStream.Close();

                    }

                }

            }

            SymmetricKey.Clear();

            return Convert.ToBase64String(CipherTextBytes); // returns a encrypted string

        }

        public static string Decryptstring(string CipherText, string Password = "ZION", string Salt = "Zion@25386", string HashAlgorithm = "MD5", int PasswordIterations = 2, string InitialVector = "ghVNHfg544JUdfjdY5BGVbj6784736372", int KeySize = 256)
        {
            try
            {
                if (string.IsNullOrEmpty(CipherText))

                    return "";

                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);

                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);

                byte[] CipherTextBytes = Convert.FromBase64String(CipherText);

                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);

                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);

                RijndaelManaged SymmetricKey = new RijndaelManaged();

                SymmetricKey.Mode = CipherMode.ECB;

                byte[] PlainTextBytes = new byte[CipherTextBytes.Length];

                int ByteCount = 0;

                using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
                {

                    using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                    {

                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                        {



                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);

                            MemStream.Close();

                            CryptoStream.Close();

                        }

                    }

                }

                SymmetricKey.Clear();

                return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount); // returns decrypted string
            }
            catch (Exception ex) { }

            return CipherText;
        }

        public static string GetCurrentCountry(string ipAddress)
        {
            using (WebClient c = new WebClient())
            {
                string countryRes = c.DownloadString("https://ipinfo.io/" + ipAddress + "/country");
                return !string.IsNullOrEmpty(countryRes) ? countryRes.Trim().ToUpper() : string.Empty;
            }
        }

        public static CountryInfo GetCountryInfo(string ipAddress)
        {
            ipAddress = ipAddress == "::1" ? "123.136.169.250" : ipAddress;
            using (WebClient c = new WebClient())
            {
                string countryRes = c.DownloadString("https://usercountry.com/v1.0/json/" + ipAddress + "?token=6f6293a933dae09b31b52eb75499d992c048ec4e851cfbc9");
                return !string.IsNullOrEmpty(countryRes) ? JsonConvert.DeserializeObject<CountryInfo>(countryRes.Trim().Replace("-", "")) : null;
            }
        }

        public static float CurrentRate()
        {
            using (WebClient c = new WebClient())
            {
                string d = c.DownloadString("http://apilayer.net/api/live?access_key=89660efe1fb5602c18218e96c9552c64&currencies=USD,INR&format=1");
                var r = JsonConvert.DeserializeObject<CurrencyRate>(d);
                return r.quotes.USDINR;
            }
        }

        #region Generate UID
        static string[] str = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string GenerateUID()
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 7; i++)
            {
                sb.Append(str[rnd.Next(25)]);
            }
            return string.Concat(sb.ToString(), new Random().Next(100000).ToString());
        }
        #endregion

        public class CurrencyRate
        {
            public bool success { get; set; }
            public string timestamp { get; set; }
            public string source { get; set; }
            public quotes quotes { get; set; }
        }

        public class quotes
        {
            public float USDINR { get; set; }
            public float USDUSD { get; set; }
        }

        public class CountryInfo
        {
            public string status { get; set; }
            public string ip { get; set; }
            public ZCountry country { get; set; }
            public ZRegion region { get; set; }
            public CountryTimeZone timezone { get; set; }
        }

        public class ZCountry
        {
            public string alpha3 { get; set; }
            public string alpha2 { get; set; }
            public string name { get; set; }
            public string phone { get; set; }
        }

        public class ZRegion
        {
            public string city { get; set; }
            public string state { get; set; }
            public string postal { get; set; }
        }

        public class CountryTimeZone
        {
            public string name { get; set; }
            public string CurrentTime { get; set; }
            public string code { get; set; }
            public int? gmt_offset { get; set; }
            public string is_daylight_saving { get; set; }
        }

        public static int IndexOf(this string str, string[] find)
        {
            foreach (var f in find)
            {
                var i = str.IndexOf(f);
                if (i > -1)
                    return i;
            }
            return -1;
        }

    }
}
