using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZionMarketResearch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;

            routes.MapRoute(
               name: "JsonAllNews",
               url: "json-all-news/{page}",
               defaults: new { controller = "News", action = "JsonAllNews" },
               namespaces: new string[] { "ZionMarketResearch.Controllers" });


            routes.MapRoute(
                name: "GetAllParentCategories",
                url: "get-all-parent-categories",
                defaults: new { controller = "Category", action = "GetAllParentCategories" },
                namespaces: new string[] { "ZionMarketResearch.Controllers" });

            routes.MapRoute(
                name: "UpdateReportRoute",
                url: "updatereportdescription",
                defaults: new { controller = "Report", action = "UpdateReportDescription" },
                namespaces: new string[] { "ZionMarketResearch.Controllers" });

            routes.MapRoute(
                name: "GetCategoryUpcomingReportsRoute_Ajax",
                url: "get-category-reports-upcoming/{url}/{page}",
                defaults: new { controller = "Category", action = "CategoryUpcomingReports", page = UrlParameter.Optional },
                namespaces: new string[] { "ZionMarketResearch.Controllers" });

            routes.MapRoute(
                name: "GetCategoryPublishedReportsRoute_Ajax",
                url: "get-category-reports-published/{url}/{page}",
                defaults: new { controller = "Category", action = "CategoryPublishedReports", page = UrlParameter.Optional },
                namespaces: new string[] { "ZionMarketResearch.Controllers" });

            routes.MapRoute(
                name: "GetCategoryReportsRoute_Ajax",
                url: "get-category-reports/{url}/{page}",
                defaults: new { controller = "Category", action = "CategoryReports", page = UrlParameter.Optional },
                namespaces: new string[1] { "ZionMarketResearch.Controllers" });

            routes.MapRoute(
                name: "GetAllUpcomingReportsRoute",
                url: "get-all-upcoming-reports",
                defaults: new { controller = "Report", action = "GetAllUpcomingReports" },
                namespaces: new string[1] { "ZionMarketResearch.Controllers" });

            routes.MapRoute(
               name: "GetAllReportsRoute",
               url: "get-all-reports",
               defaults: new { controller = "Report", action = "GetAllReports" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "StripeSuccess",
               url: "stripe/success",
               defaults: new { controller = "StripeApi", action = "Success" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "StripeError",
               url: "stripe/error",
               defaults: new { controller = "StripeApi", action = "Error" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "StripeToken",
               url: "stripe/gettoken",
               defaults: new { controller = "StripeApi", action = "GetToken" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
              name: "CareerDetail",
              url: "career/detail/{id}",
              defaults: new { controller = "Home", action = "CareerDetail", id = UrlParameter.Optional },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
               name: "Career",
               url: "career",
               defaults: new { controller = "Home", action = "Career" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "DownloadTOCRoute",
               url: "download-toc/{url}",
               defaults: new { controller = "Report", action = "DownloadTOC", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
                name: "Methodology",
                url: "methodology/{url}",
                defaults: new { controller = "Report", action = "Methodology", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "RequestBroucher",
                url: "requestbrochure/{url}",
                defaults: new { controller = "Report", action = "RequestBrochure", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "RequestDiscount",
                url: "requestdiscount/{url}",
                defaults: new { controller = "Report", action = "RequestDiscount", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "ZMRInNews",
                url: "zmr-in-news/{page}",
                defaults: new { controller = "Home", action = "ZMRInNews", page = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "SearchCompany",
                url: "company/{query}",
                defaults: new { controller = "Company", action = "Search", query = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "AskToAnalyst",
                url: "ask-to-analyst/{url}",
                defaults: new { controller = "Report", action = "AskToAnalyst", url = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "BlackFridaySaleRoute",
                url: "blackfriday",
                defaults: new { controller = "Discount", action = "BlackFriday", url = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "YearEndSaleRoute",
                url: "year-end-sale",
                defaults: new { controller = "Discount", action = "YearEnd" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "NewsletterRoute",
                url: "discount/newsletter",
                defaults: new { controller = "Discount", action = "Newsletter", url = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );


            routes.MapRoute(
                name: "PaymentLinkRoute",
                url: "payment/{url}",
                defaults: new { controller = "Report", action = "GetpaymentLinkReport", url = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "RefreshRoute",
                url: "refresh",
                defaults: new { controller = "Captcha", action = "Refresh" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "UnsubscribeRoute",
                url: "unsubscribe/{uniqueid}",
                defaults: new { controller = "Notify", action = "Unsubscribe", uniqueid = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "NotifyRoute",
                url: "notify",
                defaults: new { controller = "Notify", action = "Notify" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );
            routes.MapRoute(
            name: "NotFoundRoute",
            url: "notfound",
            defaults: new { controller = "Home", action = "NotFound", url = UrlParameter.Optional },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
            name: "MethodologyRoute",
            url: "getmethodology/{url}",
            defaults: new { controller = "Report", action = "GetMethodology", url = UrlParameter.Optional },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
            name: "AnalysisRoute",
            url: "market-analysis/{url}",
            defaults: new { controller = "Report", action = "Analysis", url = UrlParameter.Optional },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
            name: "InquiryRoute",
            url: "inquiry/{url}/{reffer}",
            defaults: new { controller = "Report", action = "Inquiry", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
             name: "FeaturedReportsSitemap",
             url: "featured-reports.xml",
             defaults: new { controller = "Sitemap", action = "FeaturedReports" },
             namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
             name: "PublishedReportsSitemap",
             url: "published-reports.xml",
             defaults: new { controller = "Sitemap", action = "PublishedReports", Lang = string.Empty },
             namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
             name: "FrPublishedReportsSitemap",
             url: "fr-published-reports.xml",
             defaults: new { controller = "Sitemap", action = "PublishedReports", Lang = "fr" },
             namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
             name: "DePublishedReportsSitemap",
             url: "de-published-reports.xml",
             defaults: new { controller = "Sitemap", action = "PublishedReports", Lang = "de" },
             namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
             name: "UpcomingReportsSitemap",
             url: "upcoming-reports.xml",
             defaults: new { controller = "Sitemap", action = "UpcomingReports" },
             namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
            name: "ArticleSitemapRoute",
            url: "articles.xml",
            defaults: new { controller = "Sitemap", action = "Articles" },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
            name: "NewsSitemapRoute",
            url: "news.xml",
            defaults: new { controller = "Sitemap", action = "News" },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
           );


            routes.MapRoute(
         name: "AutoCompleteRoute",
         url: "autocomplete/{query}",
         defaults: new { controller = "Report", action = "Autocomplete", query = UrlParameter.Optional },
         namespaces: new[] { "ZionMarketResearch.Controllers" }
     );

            routes.MapRoute(
                name: "PrebookRoute",
                url: "prebook/{reporttype}/{url}",
                defaults: new { controller = "Buynow", action = "Prebook", url = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
          name: "ArticlesRssRoute",
          url: "rss/articles.xml",
          defaults: new { controller = "RSS", action = "Articles" },
          namespaces: new[] { "ZionMarketResearch.Controllers" }
      );

            routes.MapRoute(
           name: "NewsRssRoute",
           url: "rss/news.xml",
           defaults: new { controller = "RSS", action = "News" },
           namespaces: new[] { "ZionMarketResearch.Controllers" }
       );

            routes.MapRoute(
            name: "ArticleRoute",
            url: "article/{url}",
            defaults: new { controller = "Article", action = "Index", url = UrlParameter.Optional },
            namespaces: new[] { "ZionMarketResearch.Controllers" }
        );

            routes.MapRoute(
             name: "AllArticlesRoute",
             url: "all-articles/{page}",
             defaults: new { controller = "Article", action = "AllArticles", page = UrlParameter.Optional },
             namespaces: new[] { "ZionMarketResearch.Controllers" }
         );

            routes.MapRoute(
              name: "SitemapRoute",
              url: "sitemap.xml",
              defaults: new { controller = "Sitemap", action = "MainSitemap" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "PublishedReportRSSRoute",
              url: "rss/published.xml",
              defaults: new { controller = "RSS", action = "PublishedReports" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "UpcmingReportRSSRoute",
              url: "rss/upcoming.xml",
              defaults: new { controller = "RSS", action = "UpcomingReports" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );
            
            routes.MapRoute(
              name: "RequestSampleRoute",
              url: "sample/{url}/{reffer}",
              defaults: new { controller = "Report", action = "Sample", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );
          //  routes.MapRoute(
          //    name: "RrRequestSampleRoute",
          //    url: "fr/sample/{url}/{reffer}",
          //    defaults: new { controller = "Report", action = "Sample", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
          //    namespaces: new[] { "ZionMarketResearch.Controllers" }
          //);


            routes.MapRoute(
              name: "TOCRoute",
              url: "toc/{url}/{reffer}",
              defaults: new { controller = "Report", action = "toc", url = UrlParameter.Optional, reffer = UrlParameter.Optional },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "ProcessRoute",
              url: "process",
              defaults: new { controller = "Buynow", action = "Process" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "ReturnPolicyRoute",
              url: "return-policy",
              defaults: new { controller = "Home", action = "ReturnPolicy" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "FAQRoute",
              url: "faq",
              defaults: new { controller = "Home", action = "FAQ" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "DisclaimerRoute",
              url: "disclaimer",
              defaults: new { controller = "Home", action = "Disclaimer" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
              name: "HowToOrderRoute",
              url: "how-to-order",
              defaults: new { controller = "Home", action = "HowToOrder" },
              namespaces: new[] { "ZionMarketResearch.Controllers" }
          );

            routes.MapRoute(
               name: "PrivacyPolicyRoute",
               url: "privacy-policy",
               defaults: new { controller = "Home", action = "PrivacyPolicy" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "TestimonialRoute",
               url: "testimonials",
               defaults: new { controller = "Home", action = "Testimonials" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "WhyUsRoute",
               url: "why-us",
               defaults: new { controller = "Home", action = "WhyUs" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "TermsRoute",
               url: "terms-and-conditions",
               defaults: new { controller = "Home", action = "TermsNCondition" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "ContactUsRoute",
               url: "contact-us",
               defaults: new { controller = "Home", action = "ContactUs" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "AboutUsRoute",
               url: "about-us",
               defaults: new { controller = "Home", action = "AboutUs" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "CCAvenueProcessRoute",
               url: "ccavenueprocess",
               defaults: new { controller = "Buynow", action = "CCAvenueProcess" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "PayPalProcessRoute",
               url: "paypalprocess",
               defaults: new { controller = "Buynow", action = "PayPalProcess" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "HDFCProcessRoute",
               url: "hdfcprocess",
               defaults: new { controller = "Buynow", action = "HDFCProcess" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "TwoCheckoutProcessRoute",
               url: "twocheckoutprocess",
               defaults: new { controller = "Buynow", action = "TwoCheckoutProcess" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );


            routes.MapRoute(
               name: "RazorPayCreateOrderRoute",
               url: "buynow/CreateOrder",
               defaults: new { controller = "Buynow", action = "CreateOrder" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "RazorPayRazorPaymentSuccessRoute",
               url: "buynow/RazorPaymentSuccess",
               defaults: new { controller = "Buynow", action = "RazorPaymentSuccess" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "TestPaymentRoute",
               url: "buynow/testpayment",
               defaults: new { controller = "Buynow", action = "TestPayment", reporttype = UrlParameter.Optional, url = UrlParameter.Optional },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "CalculateHashRoute",
               url: "buynow/calculatehash",
               defaults: new { controller = "Buynow", action = "CalculateHash" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "PayUResponse",
               url: "buynow/PayuResponse",
               defaults: new { controller = "Buynow", action = "PayuResponse" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "CalculateHashRoute1",
               url: "buynow/calculatehash1",
               defaults: new { controller = "Buynow", action = "CalculateHash1" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "BuynowFormRoute",
               url: "buynow/submit",
               defaults: new { controller = "Buynow", action = "Submit" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "UpcomingRoute",
               url: "upcoming/{url}",
               defaults: new { controller = "Report", action = "Upcoming", url = UrlParameter.Optional },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "AllfeaturedRoute",
               url: "all-featured/{page}",
               defaults: new { controller = "Report", action = "AllFeaturedReports", page = UrlParameter.Optional },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "AllUpcomingRoute",
               url: "all-upcoming/{page}",
               defaults: new { controller = "Report", action = "AllUpcoming", page = UrlParameter.Optional },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );
            routes.MapRoute(
                name: "AllReportsRoute",
                url: "all-reports/{page}",
                defaults: new { controller = "Report", action = "AllReports", page = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "AllNewsRoute",
                url: "all-news/{page}",
                defaults: new { controller = "News", action = "AllNews", page = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            //Ex - /news/global-e-waste-management-market-set-for-rapid
            routes.MapRoute(
                name: "NewsRoute",
                url: "news/{url}",
                defaults: new { controller = "News", action = "Index" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
               name: "PaymentBuynowRoute",
               url: "payment/buynow/{reporttype}/{url}",
               defaults: new { controller = "Buynow", action = "Payment", reporttype = UrlParameter.Optional, url = UrlParameter.Optional },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "StripePayment",
               url: "stripepayment",
               defaults: new { controller = "Buynow", action = "StripePayment" },
               namespaces: new[] { "ZionMarketResearch.Controllers" }
           );

            routes.MapRoute(
                name: "BuynowRoute",
                url: "buynow/{reporttype}/{url}/{reffer}",
                defaults: new { controller = "Buynow", action = "Index", reporttype = UrlParameter.Optional, url = UrlParameter.Optional, reffer = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );
            routes.MapRoute(
                name: "AllCategoryRoute",
                url: "category",
                defaults: new { controller = "Category", action = "Index" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "CategoryRoute",
                url: "category/{url}/{page}",
                defaults: new { controller = "Category", action = "CategoryWithReports", url = UrlParameter.Optional, page = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );
            //Ex - /simplesearch/book
            routes.MapRoute(
                name: "SimpleSearchRoute",
                url: "simplesearch/{q}/{page}",
                defaults: new { controller = "Search", action = "SimpleSearch", q = UrlParameter.Optional, page = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );
            //Ex - /advancesearch/book/2/2
            routes.MapRoute(
                name: "AdvanceSearchRoute",
                url: "advancesearch/{z}/{n}/{i}/{p}",
                defaults: new { controller = "Search", action = "AdvanceSearch", z = UrlParameter.Optional, n = UrlParameter.Optional, i = UrlParameter.Optional, p = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "CustomRoute",
                url: "custom/{id}/{reffer}",
                defaults: new { controller = "Custom", action = "Index", id = UrlParameter.Optional, reffer = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            //Route for Latest Report - Side Bar and Home Page
            routes.MapRoute(
                name: "LatestUpcmingReportRoute",
                url: "report/latestupcomingreports/{isSideBar}",
                defaults: new { controller = "Report", action = "LatestUpcomingReports", isSideBar = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            //Route for Latest Report - Side Bar and Home Page
            routes.MapRoute(
                name: "LatestReportRoute",
                url: "report/latestreport/{isSideBar}",
                defaults: new { controller = "Report", action = "LatestReport", isSideBar = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );
                        
            //Ex - /report/ventilators-medical-devices-pipeline-assessment-2016
            routes.MapRoute(
                name: "ReportRoute",
                url: "report/{url}",
                defaults: new { controller = "Report", action = "Index", Lang = string.Empty, url = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "ThankYouNewsRoute",
                url: "news/thankyou/zn",
                defaults: new { controller = "Home", action = "ThankYou" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            //Ex - /thankyou/4/thank you for contacting us
            routes.MapRoute(
                name: "ThankYouRoute",
                url: "thankyou/{zn}",
                defaults: new { controller = "Home", action = "ThankYou", zn = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

          //  routes.MapRoute(
          //    name: "CorporateEmailIDRoute",
          //    url: "corporateemail/{id}",
          //    defaults: new { controller = "Form", action = "ShowCorporateEmail", id = UrlParameter.Optional},
          //    namespaces: new[] { "ZionMarketResearch.Controllers" }
          //);

            routes.MapRoute(
                name: "CaptchaValidateRoute",
                url: "captcha/validate",
                defaults: new { controller = "Captcha", action = "Validate" },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );

            // routes.MapRoute(
            //    name: "AdminDefault",
            //    url: "admin",
            //    defaults: new { area = "", controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "ZionAdmin.Controllers" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ZionMarketResearch.Controllers" }
            );
        }
    }
}