using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZionMarketResearch.Models
{
    public interface IJobVacancyRepository
    {
        List<VacancyEntity> GetVacancies();
        VacancyEntity GetVacancy(int id);
        object ApplyToJob(JobApplication jobApplication);
    }

    public class VacancyEntity
    {
        public int VacancyId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int? NumberOfVacancies { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string Location { get; set; }
        public decimal? MinYearExperience { get; set; }
        public decimal? MaxYearExperience { get; set; }
        public bool CanFresherApply { get; set; }
        public DateTime Displaydate { get; set; }
        public DateTime? InterviewToDate { get; set; }
        public string InterviewTime { get; set; }
    }

    public class JobApplication
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Job is required")]
        public int JobID { get; set; }
        public DateTime JobApplicationDate { get; set; }
        
        public string FilePath { get; set; }
        [Required(ErrorMessage = "Name is required"), MaxLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required"), MaxLength(100)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mobile is required"), MaxLength(12)]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Captcha is required"), MaxLength(3)]
        public string Captcha { get; set; }

        public System.Web.HttpPostedFileBase File { get; set; }
    }
}
