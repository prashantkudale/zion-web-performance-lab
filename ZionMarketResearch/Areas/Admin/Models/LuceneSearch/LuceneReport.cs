using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionAdmin.Models.LuceneSearch
{
    public class LuceneReport
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportUrl { get; set; }
        public string Category { get; set; }
    }
}