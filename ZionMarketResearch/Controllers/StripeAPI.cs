using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ZionMarketResearch.Controllers
{
    public class StripeAPIController : Controller
    {
        public async System.Threading.Tasks.Task<string> GetToken()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            var token = string.Empty;
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer sk_live_JIOvOpRbHKi8y6uyGlGBywvL");
                //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                var values = new Dictionary<string, string>
                {
                    {"success_url","http://localhost:16687/stripe/success" },
                    {"cancel_url","http://localhost:16687/stripe/error" },
                    {"payment_method_types[]","card" },
                    {"line_items[][name]","T-Shirt" },
                    {"line_items[][description]","T-Shirt" },
                    {"line_items[][amount]",(75 * 100).ToString() },
                    {"line_items[][currency]","inr" },
                    {"line_items[][quantity]","1" }
                };
                using (var content = new System.Net.Http.FormUrlEncodedContent(values))
                {
                    var res = await client.PostAsync("https://api.stripe.com/v1/checkout/sessions", content);
                    var responseString = await res.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<StripeSessionResponse>(responseString).id;
                }
            }
            return token;
        }

        public void Success()
        {

        }

        public void Error()
        {

        }
    }

    public class StripeSessionResponse
    {
        public string id { get; set; }
    }
}