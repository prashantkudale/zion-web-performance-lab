using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class ArchiveRepository
    {
        public int? Year { get; set; }

        public string Month { get; set; }

        public int Counts { get; set; }

        public int? MonthNumber { get; set; }
    }
}