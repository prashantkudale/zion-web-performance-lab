using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Data.Entity;
using System.IO;

namespace ZionMarketResearch.Models
{
    public class JobVacancyRepository : IJobVacancyRepository
    {
        #region Constructor
        private readonly DbContext _db;
        private readonly DbSet<tbljobvacancy> _vacancy;
        private readonly DbSet<tbljobapplication> _application;
        public JobVacancyRepository()
        {
            _db = new ZionDbEntities();
            _vacancy = _db.Set<tbljobvacancy>();
            _application = _db.Set<tbljobapplication>();
        }
        #endregion

        public object ApplyToJob(JobApplication jobApplication)
        {
            string[] allowedExtension = { ".doc", ".docx", ".pdf" };
            if (!allowedExtension.Contains(System.IO.Path.GetExtension(jobApplication.File.FileName).ToLower()))
                return false;

            string uid = Util.Utility.GenerateUID();

            #region Create Directory if not exist
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("/JobApplications")))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/JobApplications"));
            }
            #endregion

            string filePath = HttpContext.Current.Server.MapPath("/JobApplications/" + uid + System.IO.Path.GetExtension(jobApplication.File.FileName));
            jobApplication.File.SaveAs(filePath);
            //applicant can not apply for same job twice
            if (_application.Count(x => x.JobID == jobApplication.JobID && (x.Email == jobApplication.Email || x.Phone == jobApplication.Mobile)) == 0)
            {
                _application.Add(new tbljobapplication
                {
                    JobID = jobApplication.JobID,
                    DateOfApplication = DateTime.UtcNow.AddHours(5.50),
                    Name = jobApplication.Name,
                    Email = jobApplication.Email,
                    Phone = jobApplication.Mobile,
                    FilePath = uid + System.IO.Path.GetExtension(jobApplication.File.FileName)
                });
                ZionMarketResearch.Models.SendMail.Send(new List<MailUser> { new MailUser { MailId = "hr@zionmarketresearch.com", SendingType = 0 }, new MailUser { MailId = "priya.b@zionmarketresearch.com", SendingType = 0 } }, "Resume " + jobApplication.Name, "", true, filePath);
                if (_db.SaveChanges() > 0) return new { Success = true };
                else return new { Success = false, Message = "Something is worng. Please contact to HR." };
            }
            return new { Success = false, Message = "You already applied to the job." };
        }

        
        public List<VacancyEntity> GetVacancies()
        {
            DateTime thresholdDate = DateTime.UtcNow.AddHours(5.50).Date;
            return (from v in _vacancy
                    where (System.Data.Entity.DbFunctions.TruncateTime(v.InterviewDateTo) >= thresholdDate || v.InterviewDate == null)
                    orderby v.VacancyId descending
                    select new VacancyEntity
                    {
                        VacancyId = v.VacancyId,
                        JobTitle = v.JobTitle,
                        Location = v.Location,
                        MaxYearExperience = v.MaxYearExperience,
                        MinYearExperience = v.MinYearExperience,
                        NumberOfVacancies = v.NumberOfVacancies,
                        CanFresherApply = (bool)v.CanFresherApply,
                        InterviewDate = v.InterviewDate,
                        InterviewToDate = v.InterviewDateTo,
                        InterviewTime = v.InterviewTime
                    }).ToList();
        }

        public VacancyEntity GetVacancy(int id)
        {
            DateTime thresholdDate = DateTime.UtcNow.AddHours(5.50).Date;
            var jobs = (from v in _vacancy
                        where v.VacancyId == id && (System.Data.Entity.DbFunctions.TruncateTime(v.InterviewDateTo) >= thresholdDate || v.InterviewDate == null)
                        select new VacancyEntity
                        {
                            VacancyId = v.VacancyId,
                            JobTitle = v.JobTitle,
                            JobDescription = v.JobDescription,
                            NumberOfVacancies = v.NumberOfVacancies,
                            InterviewDate = v.InterviewDate,
                            Location = v.Location,
                            MaxYearExperience = v.MaxYearExperience,
                            MinYearExperience = v.MinYearExperience,
                            CanFresherApply = (bool)v.CanFresherApply,
                            InterviewToDate = v.InterviewDateTo,
                            InterviewTime = v.InterviewTime
                        }).FirstOrDefault();
            return jobs;
        }
    }
}