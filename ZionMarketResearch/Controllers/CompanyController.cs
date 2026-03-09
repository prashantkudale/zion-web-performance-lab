using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string query)
        {
            List<AutoComplete> ac = new List<AutoComplete>();
            //try
            //{
            if (query.Length < 4)
                return null;
            RestClient client = new RestClient();
            var request = new RestRequest("https://autocomplete.clearbit.com/v1/companies/suggest?query=" + query, Method.GET);

            var response = client.Execute(request);
            string dt = response.Content;
            var res = JsonConvert.DeserializeObject<CompanySearch>(dt);
            ac = (from c in res.company
                  select new AutoComplete
                  {
                      value = c.name,
                      data = c.name,
                      ImageUrl = c.logo
                  }).ToList();
            return Json(new { suggestions = ac }, JsonRequestBehavior.AllowGet);
        }

    }

    public class CompanySearch
    {
        public List<Company> company { get; set; }
    }

    public class Company
    {
        public string name { get; set; }
        public string domain { get; set; }
        public string logo { get; set; }
    }

}
