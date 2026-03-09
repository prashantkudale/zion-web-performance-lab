using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Security;
using ZionMarketResearch.Log;

namespace ZionAdmin.Controllers
{
    [LogException]
    public class UploadController : Controller
    {
        //
        // GET: /Admin/Upload/
        [ZionAuthorize(Roles = "UploadImage")]
        public ActionResult Upload()
        {
            return View();
        }
        [ZionAuthorize(Roles = "UploadImage")]
        public ActionResult Submit()
        {
            string fileName = Server.MapPath("/Content/UploadedImages");
            for (int i = 0; i < Request.Files.Count && Request.Files[i].ContentLength > 0 && (Request.Files[i].FileName.Contains(".png") || Request.Files[i].FileName.Contains(".jpg") || Request.Files[i].FileName.Contains(".jpeg")); i++)
            {
                Request.Files[i].SaveAs(fileName + "/" + Request.Files[i].FileName);
            }
            return View("Upload");
        }
    }
}
