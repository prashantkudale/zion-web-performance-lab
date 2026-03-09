using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZionMarketResearch.Utility
{
    public class OptionAttribute : ValidationAttribute
    {
        public OptionAttribute(string errorMessage) : base(errorMessage)
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            using (ZionMarketResearch.Models.ZionDbEntities db = new Models.ZionDbEntities())
            {
                string v = value.ToString();
                var c = db.tblcountries.Where(x => x.name == v);
                return c.Count() == 0 ? new ValidationResult("Selected Country is not from given options.", new List<string> { "FormRepository.Country" }) : ValidationResult.Success;
            }

        }
    }
}