using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.Models
{
    public class CategoryReport
    {
        public int CategryId { get; set; }

        public string Category { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string Description { get; set; }

        public string CategoryUrl { get; set; }

        public string CategoryImage { get; set; }

        public IPagedList<SearchResultView> Reports { get; set; }

        public IPagedList<SearchResultView> PublishedReports { get; set; }

        public IPagedList<SearchResultView> UpcomingReports { get; set; }

        public List<LatestReportView> CategoryReports { get; set; }
    }
}