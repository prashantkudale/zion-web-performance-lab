using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZionMarketResearch.Areas.Admin.Repository
{
    public interface IVacancyRepository
    {
        IPagedList<VacancyEntity> GetVacancy(int? page);
        bool InsertVacancy(VacancyEntity jobvacancy);
        VacancyEntity GetVacancyById(int VacancyId);
        bool RemoveVacancy(int VacancyId);
        bool UpdateVacancy(VacancyEntity jobvacancy);
        IPagedList<VacancyEntity> Search(string title, int? page);
        List<ZionMarketResearch.Models.JobApplication> GetJobApplications(int jobId);
    }
}
