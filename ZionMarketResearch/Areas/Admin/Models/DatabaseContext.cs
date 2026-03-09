using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Areas.Admin.Models
{
    public class DatabaseContext
    {
        public static void PerformAction(Action<ZionDbEntities> action)
        {
            using (var context = new ZionDbEntities())
            {
                //context.Database.CommandTimeout = 2000;
                action?.Invoke(context);
            }
        }
    }
}