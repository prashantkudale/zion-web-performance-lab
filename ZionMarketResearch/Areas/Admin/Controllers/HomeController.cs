using System.Web.Mvc;
using ZionAdmin.Repository;
using ZionAdmin.Security;
using ZionMarketResearch.Log;

namespace ZionAdmin.Controllers
{
    [LogException]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [ZionAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            if (userName.Length <= 50 && password.Length <= 50)
            {
                if (UserRepository.Login(userName, password))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Message = "Invalid User Id Or Password.";
            return View();
        }

        public ActionResult Logout()
        {
            UserRepository.Logout();
            return View("Login");
        }

        public ActionResult AskPermission()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AskPermission(string username)
        {
            ViewBag.Message = "Check your email inbox for confirmation. If still you did not got the mail please contact to administrator.";
            UserRepository.AskPermission(username);
            return View();
        }

        public string Approve(string id)
        {
            return UserRepository.Approve(id) ?  "Request has been approved for login." : "If you want to approve another IP address please wait for 30 minutes or the key is not valid.";
        }
    }
}
