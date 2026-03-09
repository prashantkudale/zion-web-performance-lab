using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Areas.Admin.Repository
{
    public interface IReportAttributeRepository
    {
        int Insert(tblreportattribute reportAttribute);
        int Update(tblreportattribute reportAttribute);
        int Delete(int id);
        List<tblreportattribute> GetAll();
        tblreportattribute GetById(int id);
    }
}
