using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZionAdmin.Security;
using ZionMarketResearch.Areas.Admin.Repository;

namespace ZionAdmin.Controllers
{
    public class JobVacancyController : Controller
    {
        #region Constructor
        private readonly IVacancyRepository _repo;
        private readonly IJobApplicationRepository _application;
        public JobVacancyController()
        {
            _repo = new VacancyRepository();
            _application = new JobApplicationRespository();
        }
        #endregion


        [ZionAuthorize(Roles = "CreateVacancy")]
        public ActionResult Index(int? id)
        {
            return View(_repo.GetVacancy(id));
        }

        [ZionAuthorize(Roles = "CreateVacancy")]
        public ActionResult Create()
        {
            return View();
        }

        [ZionAuthorize(Roles = "CreateVacancy")]
        [HttpPost]
        public ActionResult Create(VacancyEntity vacancy)
        {
            if (ModelState.IsValid)
            {
                if (_repo.InsertVacancy(vacancy))
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        [ZionAuthorize(Roles = "EditVacancy")]
        public ActionResult Edit(int id)
        {
            return View(_repo.GetVacancyById(id));
        }

        [ZionAuthorize(Roles = "EditVacancy")]
        [HttpPost]
        public ActionResult Edit(VacancyEntity vacancy)
        {
            if (ModelState.IsValid)
            {
                if (_repo.UpdateVacancy(vacancy))
                {
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [ZionAuthorize(Roles = "DeleteVacancy")]
        public ActionResult Delete(int id)
        {
            _repo.RemoveVacancy(id);
            return RedirectToAction("Index");
        }


        [ZionAuthorize(Roles = "CreateVacancy")]
        public ActionResult Search(string title, int? page)
        {
            return View("Index", _repo.Search(title, page));
        }

        [ZionAuthorize(Roles = "CreateVacancy")]
        public ActionResult ListJobApplications(int id)
        {
            return View(_repo.GetJobApplications(id));
        }

        [ZionAuthorize(Roles = "CreateVacancy")]
        public ActionResult ViewResume(string resumePath)
        {
            ViewBag.ResumePath = resumePath;
            return View();
        }

        [ZionAuthorize(Roles = "CreateVacancy")]
        public ActionResult DownloadResume(int applicationId)
        {
            var jobApplication = _application.Get(applicationId);
            return View();
        }


    }
}
