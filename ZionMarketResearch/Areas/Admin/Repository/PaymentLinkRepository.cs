using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using ZionAdmin.Models;
using ZionAdmin.Security;
using ZionMarketResearch.Models;

namespace ZionAdmin.Repository
{
    public class PaymentLinkRepository
    {
        #region Properties
        public int Id { get; set; }
        [Required(ErrorMessage = "Report Url is required.")]
        [MaxLength(500, ErrorMessage = "Report Url should not exceed length 500 characters.")]
        public string ReportUrl { get; set; }
        public int ReportId { get; set; }
        [Required(ErrorMessage = "Valid to is required")]
        public DateTime? ValidTo { get; set; }
        [Required(ErrorMessage = "Single User Price is required.")]
        public decimal SingleUser { get; set; }
        public decimal? MultiUser { get; set; }
        public decimal? CorporateUser { get; set; }
        [MaxLength(1000, ErrorMessage = "Description should not exceed length 1000 characters.")]
        public string Description { get; set; }
        public int UserId { get; set; }
        public string PaymentLink { get; set; }
        public string ReportTitle { get; set; }
        #endregion

        #region Methods
        public static string Create(PaymentLinkRepository payment)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                ObjectParameter out_Link = new ObjectParameter("p_LinkUrl", string.Empty);
                string[] reportUrl = payment.ReportUrl.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                db.Zion_AddPaymentLink(reportUrl[reportUrl.Length - 1], payment.ValidTo, payment.SingleUser, payment.MultiUser, payment.CorporateUser, payment.Description, ((CustomPrincipal)HttpContext.Current.User).UserId, out_Link);
                return out_Link.Value.ToString();
            }
        }

        public static int Update(PaymentLinkRepository payment)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return db.Zion_UpdatePaymentLInk(payment.Id, payment.SingleUser, payment.MultiUser, payment.CorporateUser, payment.ValidTo, payment.Description, ((CustomPrincipal)HttpContext.Current.User).UserId);
            }
        }

        public static IPagedList<PaymentLinkRepository> List(int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from p in db.tblpaymentlinks
                        join r in db.tblreportinformations on p.ReportId equals r.ReportID
                        where p.IsActive == true
                        orderby p.Id
                        select new PaymentLinkRepository
                        {
                            Id = p.Id,
                            PaymentLink = p.PaymentLink,
                            ReportTitle = r.ReportTitle
                        }).ToPagedList(page ?? 1, 10);
            }
        }

        public static PaymentLinkRepository Get(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                return (from p in db.tblpaymentlinks
                        where p.Id == id
                        select new PaymentLinkRepository
                        {
                            Id = p.Id,
                            ReportId = (int)p.ReportId,
                            SingleUser = (decimal)p.SingleUserPrice,
                            MultiUser = p.MultiUserPrice != null ? p.MultiUserPrice : 0,
                            CorporateUser = p.CorporateUserPrice != null ? p.CorporateUserPrice : 0,
                            ValidTo = p.ValidTo
                        }).FirstOrDefault();
            }
        }

        public static int Delete(int id)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var payment = db.tblpaymentlinks.Where(x => x.Id == id).SingleOrDefault();
                payment.IsActive = false;
                payment.DeletedBy = ((CustomPrincipal)HttpContext.Current.User).UserId;
                payment.DeletedDate = DateTime.Now;
                return db.SaveChanges();
            }
        }

        public static IPagedList<PaymentLinkRepository> Search(string searchText, int? page)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                int reportId = 0;
                int.TryParse(searchText, out reportId);
                return (from p in db.tblpaymentlinks
                        join r in db.tblreportinformations on p.ReportId equals r.ReportID
                        where p.PaymentLink.Contains(searchText) || p.ReportId == reportId && p.IsActive == true
                        orderby p.Id
                        select new PaymentLinkRepository
                        {
                            Id = p.Id,
                            PaymentLink = p.PaymentLink,
                            ReportTitle = r.ReportTitle
                        }).ToPagedList(page ?? 1, 10);
            }
        }
        #endregion
    }
}