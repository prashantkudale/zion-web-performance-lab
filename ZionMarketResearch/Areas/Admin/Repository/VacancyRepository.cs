using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionMarketResearch.Models;

namespace ZionMarketResearch.Areas.Admin.Repository
{
    public class VacancyRepository : IVacancyRepository
    {
        #region Constructor
        private readonly DbContext db;
        private readonly DbSet<tbljobvacancy> _vacancy;
        private readonly DbSet<tbljobapplication> _job;
        public VacancyRepository()
        {
            db = new ZionDbEntities();
            _vacancy = db.Set<tbljobvacancy>();
            _job = db.Set<tbljobapplication>();
        }
        #endregion

        public IPagedList<VacancyEntity> GetVacancy(int? page)
        {
            return (from v in _vacancy
                    orderby v.VacancyId descending
                    select new VacancyEntity
                    {
                        JobTitle = v.JobTitle,
                        JobDescription = v.JobTitle,
                        CreadtedDate = (DateTime)v.CreadtedDate,
                        CreatedBy = (int)v.CreatedBy,
                        VacancyId = v.VacancyId,
                        NumberOfVacancies = (int)v.NumberOfVacancies,
                        InterviewDate = (DateTime)v.InterviewDate,
                        TotalJobApplications = v.tbljobapplications.Count
                    }).ToPagedList(page ?? 1, 30);
        }

        public bool InsertVacancy(VacancyEntity jobvacancy)
        {
            if (!IsOfSameTitleInfo(jobvacancy))
            {
                _vacancy.Add(new tbljobvacancy
                {
                    CreadtedDate = DateTime.UtcNow.AddHours(5.50),
                    Displaydate = jobvacancy.Displaydate,
                    JobDescription = jobvacancy.JobDescription,
                    CreatedBy = 1,
                    InterviewDate = jobvacancy.InterviewDate,
                    JobTitle = jobvacancy.JobTitle,
                    NumberOfVacancies = jobvacancy.NumberOfVacancies,
                    Location = jobvacancy.Location,
                    MaxYearExperience = jobvacancy.MaxYearExperience,
                    MinYearExperience = jobvacancy.MinYearExperience,
                    CanFresherApply = jobvacancy.CanFresherApply,
                    VacancyId = jobvacancy.VacancyId,
                    InterviewDateTo = jobvacancy.InterviewDateTo,
                    InterviewTime = jobvacancy.InterviewFromTime + "|" + jobvacancy.InterviewToTime
                });
                return db.SaveChanges() > 0;
            }
            return false;
        }

        public VacancyEntity GetVacancyById(int VacancyId)
        {
            var vc = (from v in _vacancy
                      where v.VacancyId == VacancyId
                      select new VacancyEntity
                      {
                          VacancyId = v.VacancyId,
                          JobTitle = v.JobTitle,
                          JobDescription = v.JobDescription,
                          NumberOfVacancies = (int)v.NumberOfVacancies,
                          InterviewDate = (DateTime)v.InterviewDate,
                          Location = v.Location,
                          MaxYearExperience = (int)v.MaxYearExperience,
                          MinYearExperience = (int)v.MinYearExperience,
                          CanFresherApply = (bool)v.CanFresherApply,
                          InterviewDateTo = (DateTime)v.InterviewDateTo,
                          //InterviewFromTime = v.InterviewTime != null ? v.InterviewTime.Split('|')[0].Trim() : "",
                          //InterviewToTime = v.InterviewTime != null ?  v.InterviewTime.Split('|')[1].Trim() : "",
                          InterviewTime = v.InterviewTime
                      }).FirstOrDefault();
            vc.InterviewFromTime = vc.InterviewTime != null ? vc.InterviewTime.Split('|')[0].Trim() : "";
            vc.InterviewToTime = vc.InterviewTime != null ? vc.InterviewTime.Split('|')[1].Trim() : "";
            return vc;
        }

        public bool RemoveVacancy(int VacancyId)
        {
            var jobeditdata = _vacancy.Where(x => x.VacancyId == VacancyId).FirstOrDefault();
            if (jobeditdata.tbljobapplications.Count == 0)
            {
                _vacancy.Remove(jobeditdata);
                return db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateVacancy(VacancyEntity v)
        {
            var jobVacancy = _vacancy.First(x => x.VacancyId == v.VacancyId);
            jobVacancy.VacancyId = v.VacancyId;
            jobVacancy.JobTitle = v.JobTitle;
            jobVacancy.JobDescription = v.JobDescription;
            jobVacancy.NumberOfVacancies = v.NumberOfVacancies;
            jobVacancy.InterviewDate = v.InterviewDate;
            jobVacancy.Displaydate = v.Displaydate;
            jobVacancy.ModifiedBy = 1;
            jobVacancy.ModifiedDate = DateTime.Now;
            jobVacancy.CanFresherApply = v.CanFresherApply;
            jobVacancy.Location = v.Location;
            jobVacancy.MaxYearExperience = v.MaxYearExperience;
            jobVacancy.MinYearExperience = v.MinYearExperience;
            jobVacancy.InterviewDateTo = v.InterviewDateTo;
            jobVacancy.InterviewTime = v.InterviewFromTime + "|" + v.InterviewToTime;
            return db.SaveChanges() > 0;
        }

        public bool IsOfSameTitleInfo(VacancyEntity job)
        {
            return _vacancy.Count(x => x.JobTitle == job.JobTitle && x.InterviewDate == job.InterviewDate) > 0;
        }

        public IPagedList<VacancyEntity> Search(string title, int? page)
        {
            return (from v in _vacancy
                    where v.JobTitle.Contains(title)
                    select new VacancyEntity
                    {
                        JobTitle = v.JobTitle,
                        JobDescription = v.JobTitle,
                        CreadtedDate = (DateTime)v.CreadtedDate,
                        CreatedBy = (int)v.CreatedBy,
                        VacancyId = v.VacancyId,
                        NumberOfVacancies = (int)v.NumberOfVacancies,
                        InterviewDate = (DateTime)v.InterviewDate
                    }).ToPagedList(page ?? 1, 30);
        }

        public List<JobApplication> GetJobApplications(int jobId)
        {
            return (from ja in _job
                    where ja.JobID == jobId
                    select new JobApplication
                    {
                        Name = ja.Name,
                        Email = ja.Email,
                        Mobile = ja.Phone,
                        FilePath = ja.FilePath,
                        JobApplicationDate = (DateTime)ja.DateOfApplication
                    }).ToList();
        }
    }

    public class VacancyEntity
    {
        public int VacancyId { get; set; }
        [Required(ErrorMessage = "Job Title is required"), StringLength(300, ErrorMessage = "Job Tilte should not be greater than 300")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Job Description is required")]
        [AllowHtml]
        public string JobDescription { get; set; }
        [Required(ErrorMessage = "Number Of Vacancy Required.")]
        public int NumberOfVacancies { get; set; }

        public DateTime? InterviewDate { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Minimum Year Experience is required")]
        public decimal? MinYearExperience { get; set; }

        [Required(ErrorMessage = "Maximum Year Experience is required")]
        public decimal? MaxYearExperience { get; set; }

        public DateTime? InterviewDateTo { get; set; }

        public string InterviewFromTime { get; set; }

        public string InterviewToTime { get; set; }

        public string InterviewTime { get; set; }

        public bool CanFresherApply { get; set; }

        public DateTime Displaydate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreadtedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int TotalJobApplications { get; set; }
    }
}