using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZionMarketResearch.constants;

namespace ZionMarketResearch.Models
{
    public class CorporateEmailRepository
    {
        [Required]

        public int Id { get; set; }

        [Required]
        public int CrmLeadID { get; set; }

        [Required(ErrorMessage = "Email is required."), RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid E-mail."), StringLength(100, ErrorMessage = "Email should not greater than 100 characters.")]
        [LeadTable(DisplayText = "Email", Order = 3)]        
        public string Email { get; set; }

        public string Submit(CorporateEmailRepository cer, FormRepository fr)
        {

            return GlobalConstant.FormType.CorporateEmail.ToString();
        }
    }
}