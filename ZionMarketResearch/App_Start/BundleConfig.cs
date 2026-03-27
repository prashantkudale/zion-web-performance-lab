using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ZionMarketResearch.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            #region MainPage
            //bundles.Add(new StyleBundle( "~/bundles/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"));
            //bundles.Add(new StyleBundle("~/bundles/font", "https://fonts.googleapis.com/css?family=Open+Sans"));
            //bundles.Add(new ScriptBundle("~/bundles/jquery", "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js"));
            //bundles.Add(new ScriptBundle("~/bundles/bootstrapjs", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"));


            bundles.Add(new StyleBundle("~/bundles/career")
                    .Include("~/Scripts/career.js")
                    );

            bundles.Add(new StyleBundle("~/bundles/maincss")
                    .Include("~/css/scroll_Image.css")
                    .Include("~/css/custom.css")
                    .Include("~/css/font-awesome.css")
                    );

            bundles.Add(new ScriptBundle("~/bundles/mainscript")
                .Include("~/js/owl.carousel.js")
                );
            #endregion

            #region Autocomplete Control
            bundles.Add(new StyleBundle("~/bundles/autocompletecss")
                    .Include("~/Content/autocomplete/autocomplete.css")
                    );

            bundles.Add(new ScriptBundle("~/bundles/autocompletejs")
                .Include("~/Content/autocomplete/jquery.autocomplete.js")
                .Include("~/Content/autocomplete/autocomplete.js")
                );
            #endregion

            #region Buynow Control
            bundles.Add(new StyleBundle("~/bundles/buynowcss")
                    .Include("~/css/buynow.css")
                    );
            #endregion


            #region FormValidation
            bundles.Add(new ScriptBundle("~/bundles/form-validation")
                    .Include("~/Scripts/jquery.validate.js")
                    .Include("~/Scripts/jquery.validate.unobtrusive.js")
                    .Include("~/Scripts/jquery.validate.unobtrusive.bootstrap.js")
                    .Include("~/Content/captcha/Captcha.js"));
            #endregion

            bundles.Add(new ScriptBundle("~/bundles/report")
                .Include("~/Content/report/Report.js"));

            bundles.Add(new ScriptBundle("~/bundles/checkoutjs", "https://checkout.razorpay.com/v1/checkout.js")
                .Include("~/Scripts/checkout.js"));
            bundles.Add(new ScriptBundle("~/bundles/razorpayjs")
                .Include("~/Scripts/serializeform.js")
                .Include("~/Scripts/customrazorpay.js"));


            bundles.Add(new StyleBundle("~/customcss")
                .Include("~/Content/css/main.css")
                .Include("~/Content/css/default.css")
                .Include("~/Content/css/custom.css"));

            bundles.Add(new Bundle("~/bundles/faicons")             
                 .Include("~/Content/css/font-awesome.custom.css"));

        }
    }
}