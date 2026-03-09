using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZionMarketResearch.Models
{
    public class CountryRepository
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        public static List<SelectListItem> GetCountry(string selectedCountryCode = null)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var countries = (from c in db.tblcountries
                                 orderby c.name
                                 select new SelectListItem
                                 {
                                     Text = c.name.Trim() + " (" + c.full_name + ")",
                                     Value = c.name,
                                     Selected = selectedCountryCode != null && c.iso3 == selectedCountryCode ? true : false
                                 }).ToList();
                return countries;
            }
        }
    }
}