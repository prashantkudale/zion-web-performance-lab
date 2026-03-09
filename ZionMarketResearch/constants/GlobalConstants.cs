using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZionMarketResearch.constants
{
    public static class GlobalConstant
    {
        public enum FormType
        {
            RequestSample = 0,
            BuyingEnquiry = 1,
            ContactUs = 5,
            CustomRequest = 3,
            RequestTOC = 4,
            //CustomizationRequest = 5,
            Search = 6,
            //SearchQuery = 7,
            //BrochureRequest = 8,
            AskToAnalyst = 9,
            RequestBrochure = 10,
            //WithReport = 11,
            RequestDiscount = 12,
            RequestMethodology = 13,
            PopupSample = 14,
            QuickInquiry = 15,
            ServerError = 16,
            NotFound = 17,
            YearEndSale = 18,
            BlackFriday = 19,
            CorporateEmail =20
        }

        public static PagedListRenderOptions GlobalPagedListOption
        {
            get
            {
                return new PagedListRenderOptions
                {
                    LinkToFirstPageFormat = "<<",
                    LinkToLastPageFormat = ">>",
                    LinkToNextPageFormat = "Next",
                    LinkToPreviousPageFormat = "Prev",
                    DisplayLinkToFirstPage = PagedListDisplayMode.Always,
                    DisplayLinkToLastPage = PagedListDisplayMode.Always,
                    DisplayLinkToNextPage = PagedListDisplayMode.Always,
                    DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
                    MaximumPageNumbersToDisplay = 5,
                    FunctionToTransformEachPageLink = (li, a) => { if (a.Attributes.FirstOrDefault(x => x.Key == "rel").Equals(default(KeyValuePair<string, string>))) { a.Attributes.Add("rel", "nofollow"); } else { a.Attributes["rel"] = a.Attributes["rel"] + " nofollow"; } li.InnerHtml = a.ToString(); return li; }
                };
            }
        }
    }
}