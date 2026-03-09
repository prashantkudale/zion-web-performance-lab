using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Areas.Admin.Repository
{
    public class JobApplicationRespository : IJobApplicationRepository
    {
        #region Constructor
        private readonly DbContext _db;
        private readonly DbSet<tbljobapplication> _application;
        public JobApplicationRespository()
        {
            _db = new ZionDbEntities();
            _application = _db.Set<tbljobapplication>();
        }
        #endregion

        public JobApplication Get(int id)
        {
            return _application.Where(x => x.ID == id).Select(x =>
            new JobApplication
            {
                ID = x.ID,
                JobID = (int)x.JobID,
                JobApplicationDate = (DateTime)x.DateOfApplication,
                FilePath = x.FilePath,
                Name = x.Name,
                Mobile = x.Phone
            }).FirstOrDefault();
        }
    }
}