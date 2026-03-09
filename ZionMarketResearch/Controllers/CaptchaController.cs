//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using ZionMarketResearch.Models;

//namespace ZionMarketResearch.Controllers
//{
//    public class CaptchaController : Controller
//    {
//        //
//        // GET: /Captcha/

//        public ActionResult Index()
//        {
//            return new FileStreamResult(CaptchaRepository.GetCaptchaImage(), "image/png");
//        }

//        public ActionResult Refresh()
//        {
//            return Json(new { image = CaptchaRepository.GetBase64Image() }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public ActionResult Validate(string captcha)
//        {
//            return Json(new { Success = CaptchaRepository.IsCaptchaValid(captcha) });
//        }
//    }
//}
