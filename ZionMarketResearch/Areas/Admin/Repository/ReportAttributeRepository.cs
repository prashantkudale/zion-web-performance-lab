using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZionMarketResearch.Areas.Admin.Models;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Areas.Admin.Repository
{
    public class ReportAttributeRepository : IReportAttributeRepository
    {
        public int Delete(int id)
        {
            var result = 0;
            DatabaseContext.PerformAction(action =>
            {
                var en = action.tblreportattributes.FirstOrDefault(x => x.ID == id);
                if (en != null)
                {
                    action.tblreportattributes.Remove(en);
                    result = action.SaveChanges();
                }
            });
            return result;
        }

        public List<tblreportattribute> GetAll()
        {
            var result = new List<tblreportattribute>();
            DatabaseContext.PerformAction(action =>
            {
                result = action.tblreportattributes.ToList();
            });
            return result;
        }

        public tblreportattribute GetById(int id)
        {
            tblreportattribute result = null;
            DatabaseContext.PerformAction(action =>
            {
                result = action.tblreportattributes.FirstOrDefault(x => x.ID == id);
            });
            return result;
        }

        public int Insert(tblreportattribute reportAttribute)
        {
            var result = 0;
            DatabaseContext.PerformAction(action =>
            {
                action.tblreportattributes.Add(reportAttribute);
                result = action.SaveChanges();
            });
            return result;
        }

        public int Update(tblreportattribute reportAttribute)
        {
            var result = 0;
            DatabaseContext.PerformAction(action =>
            {
                var en = action.tblreportattributes.FirstOrDefault(x => x.ID == reportAttribute.ID);
                if (en != null)
                {
                    en.AttributeName = reportAttribute.AttributeName;
                    result = action.SaveChanges();
                }
            });
            return result;
        }
    }
}