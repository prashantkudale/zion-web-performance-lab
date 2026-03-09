using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Areas.Admin.Repository
{
    public interface IJobApplicationRepository
    {
        JobApplication Get(int id);
    }
}
