using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class CategoryReportView
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int ReportCount { get; set; }

        public string CategoryUrl { get; set; }
    }
}