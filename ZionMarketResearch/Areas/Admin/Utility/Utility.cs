
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ZionAdmin
{
    public static class Utility
    {
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
        //public static string CreateParameterString(MySqlParameter[] p)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (p.Count() > 0)
        //    {
        //        foreach (var v in p)
        //        {
        //            sb.Append("@" + v.ParameterName + ",");
        //        }
        //    }
        //    return sb.ToString().Remove(sb.ToString().Length - 1, 1);
        //}

        public static void AddToCache(string key, object value)
        {
            if (HttpContext.Current.Cache[key] == null)
                HttpContext.Current.Cache.Add(key, value, null, DateTime.Today.AddHours(5), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
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
            return File.ReadAllText(HttpContext.Current.Server.MapPath("/Util/Template/" + templateName));
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

        public static Dictionary<string, string> UploadFiles(HttpFileCollectionBase files)
        {
            Dictionary<string, string> fileNames = new Dictionary<string, string>();
            string file1 = string.Empty;
            string file2 = string.Empty;

            if (files != null && files.Count > 0)
            {
                HttpFileCollectionWrapper f = (HttpFileCollectionWrapper)files;
                var fName = string.Empty;
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i] != null && !string.IsNullOrEmpty(files[i].FileName))
                    {
                        fName = Guid.NewGuid().ToString() + "." + files[i].FileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        files[i].SaveAs(HttpContext.Current.Server.MapPath("/UploadFiles/") + fName);
                        fileNames.Add(files.GetKey(i), fName);
                    }
                }
            }
            return fileNames;
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(HttpContext.Current.Server.MapPath("/UploadFiles/" + filePath)))
                File.Delete(HttpContext.Current.Server.MapPath("/UploadFiles/" + filePath));
        }

        public static void SendMail(string to, string subject, string body, bool isHtml = true)
        {
            var isZionUser = to.Contains("@zionmarketresearch.com");
            var _hostName = isZionUser ? ConfigurationManager.AppSettings["AlternateHostName"] : System.Configuration.ConfigurationManager.AppSettings["HostName"];
            var _from =  ConfigurationManager.AppSettings["EmailFrom"];
            var _user = isZionUser ? ConfigurationManager.AppSettings["AlternateUser"] : ConfigurationManager.AppSettings["UserName"];
            var _password = isZionUser ? ConfigurationManager.AppSettings["AlternatePassword"] : ConfigurationManager.AppSettings["Password"];


            var mail = new MailMessage();
            using (var SmtpServer = new SmtpClient(_hostName))
            {
                mail.From = new MailAddress(_from, _from);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.IsBodyHtml = isHtml;
                mail.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
                //SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(_user, _password);
                //SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
            }
        }
    }
}