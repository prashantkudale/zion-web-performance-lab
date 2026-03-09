//using SRVTextToImage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class CaptchaRepository
    {
        public static MemoryStream GetCaptchaImage()
        {
            CaptchaRandomImage ci = new CaptchaRandomImage();
            HttpContext.Current.Session["Captcha"] = new Random().Next(100, 9999);
            ci.GenerateImage(HttpContext.Current.Session["Captcha"].ToString(), 200, 75);
            MemoryStream stream = new MemoryStream();
            ci.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("_imp", Util.Utility.Encryptstring(HttpContext.Current.Session["Captcha"].ToString())));
            return stream;
        }

        public static string GetBase64Image()
        {
            CaptchaRandomImage ci = new CaptchaRandomImage();
            HttpContext.Current.Session["Captcha"] = new Random().Next(100, 9999);
            ci.GenerateImage(HttpContext.Current.Session["Captcha"].ToString(), 200, 75);
            MemoryStream stream = new MemoryStream();
            ci.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[(int)stream.Length];
            stream.Read(data, 0, data.Length);
            //this works, when multiple reports opening.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("_imp", Util.Utility.Encryptstring(HttpContext.Current.Session["Captcha"].ToString())));
            return Convert.ToBase64String(data);
        }

        public static bool IsCaptchaValid(string captchaText)
        {
            return (HttpContext.Current.Session["Captcha"] != null && HttpContext.Current.Session["Captcha"].ToString() == captchaText) || 
                (!string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["_imp"].Value) && Util.Utility.Decryptstring(HttpContext.Current.Request.Cookies["_imp"].Value) == captchaText);
        }
    }
}