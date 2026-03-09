using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using ZionMarketResearch.Log;

namespace ZionMarketResearch.Models
{
    /// <summary>
    /// this class is responsible for sending mails.
    /// </summary>
    [LogException]
    public class SendMail
    {
        /// <summary>
        /// Send mail to Administrator and lead managers.
        /// </summary>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body</param>
        public static void SendMailToZion(string subject, string body, string type)
        {
            using (ZionDbEntities db = new ZionDbEntities())
            {
                var mails = new List<MailUser>();
                //publishers and mail
                var zionMails = (from z in db.tblzionmails
                                 where z.IsEnabled == true
                                 select new MailUser
                                 {
                                     MailId = z.EmailID,
                                     SendingType = 0
                                 }).ToList();

                if (zionMails != null && zionMails.Count() > 0)
                    mails.AddRange(zionMails);
                //admin
                var adminMails = from x in db.tblmails
                                 where x.type == type
                                 select new MailUser
                                 {
                                     MailId = x.EmailId,
                                     SendingType = x.SendingType
                                 };
                if (adminMails != null && adminMails.Count() > 0)
                    mails.AddRange(adminMails);
                if (mails != null && mails.Count() > 0)
                    Send(mails, subject, body);
            }
        }

        /// <summary>
        /// Send mail to single user.
        /// </summary>
        /// <param name="to">User email id</param>
        /// <param name="subject">Mail subject</param>
        /// <param name="body">Mail body</param>
        /// <param name="isHtml">Body text will be HTML or not</param>
        public static void Send(string to, string subject, string body, bool isHtml = true)
        {
            //HostingEnvironment.QueueBackgroundWorkItem(ct =>
            //{
            //    try
            //    {
            //        string _hostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
            //        string _from = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            //        string _user = System.Configuration.ConfigurationManager.AppSettings["UserName"];
            //        string _password = System.Configuration.ConfigurationManager.AppSettings["Password"];

            //        MailMessage mail = new MailMessage();
            //        using (SmtpClient SmtpServer = new SmtpClient(_hostName))
            //        {


            //            mail.From = new MailAddress(_from, _from);

            //            #region define to
            //            string[] tos = new string[] { };
            //            if (to.Contains(","))
            //                tos = to.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //            if (tos != null && tos.Count() > 0)
            //            {
            //                foreach (var x in tos)
            //                {
            //                    mail.To.Add(x);
            //                }
            //            }
            //            else
            //            {
            //                mail.To.Add(to);
            //            }
            //            #endregion

            //            mail.Subject = subject;
            //            mail.IsBodyHtml = isHtml;
            //            mail.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
            //            SmtpServer.Port = 587;
            //            SmtpServer.UseDefaultCredentials = false;
            //            SmtpServer.Credentials = new System.Net.NetworkCredential(_user, _password);
            //            SmtpServer.EnableSsl = false;
            //            SmtpServer.Send(mail);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        //TODO: implement resend functionality
            //    }
            //});
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    string _hostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
                    string _from = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
                    string _user = System.Configuration.ConfigurationManager.AppSettings["UserName"];
                    string _password = System.Configuration.ConfigurationManager.AppSettings["Password"];

                    MailMessage mail = new MailMessage();
                    using (SmtpClient SmtpServer = new SmtpClient(_hostName))
                    {


                        mail.From = new MailAddress(_from, _from);

                        #region define to
                        string[] tos = new string[] { };
                        if (to.Contains(","))
                            tos = to.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tos != null && tos.Count() > 0)
                        {
                            foreach (var x in tos)
                            {
                                mail.To.Add(x);
                            }
                        }
                        else
                        {
                            mail.To.Add(to);
                        }
                        #endregion

                        mail.Subject = subject;
                        mail.IsBodyHtml = isHtml;
                        mail.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
                        SmtpServer.Port = 587;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(_user, _password);
                        SmtpServer.EnableSsl = false;
                        SmtpServer.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    //TODO: implement resend functionality
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Send mail to multiple users.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        public static void Send(List<MailUser> to, string subject, string body, bool isHtml = true, string attachmentFile = "")
        {
            //System.Threading.ThreadPool.QueueUserWorkItem(t =>
            //{
            //HostingEnvironment.QueueBackgroundWorkItem(ct =>
            //{
            //    try
            //    {
            //        var _hostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
            //        var _from = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
            //        var _user = System.Configuration.ConfigurationManager.AppSettings["UserName"];
            //        var _password = System.Configuration.ConfigurationManager.AppSettings["Password"];

            //        var _alternateHostName = System.Configuration.ConfigurationManager.AppSettings["AlternateHostName"];
            //        var _alternatePassword = System.Configuration.ConfigurationManager.AppSettings["AlternatePassword"];
            //        var _alternateUserName = System.Configuration.ConfigurationManager.AppSettings["AlternateUser"];

            //        var zionMails = new List<MailUser>();
            //        var differentMails = to.ToList();

            //        #region Send Mail to Zion Market Research Users
            //        if (zionMails.Count > 0)
            //        {
            //            var mail_zoin = GetMailMessage(zionMails, _from, _alternateHostName);
            //            var SmtpServer_zion = GetSMTPClient(new System.Net.NetworkCredential(_alternateUserName, _alternatePassword), _alternateHostName);
            //            mail_zoin.Subject = subject;
            //            mail_zoin.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
            //            mail_zoin.IsBodyHtml = isHtml;
            //            foreach (var m in zionMails)
            //            {
            //                if (m.SendingType == 0)
            //                    mail_zoin.To.Add(m.MailId);
            //            }
            //            if (!string.IsNullOrEmpty(attachmentFile))
            //            {
            //                using (var attachment = new Attachment(attachmentFile))
            //                {
            //                    mail_zoin.Attachments.Add(attachment);
            //                }
            //            }
            //            SmtpServer_zion.Send(mail_zoin);
            //        }
            //        #endregion

            //        #region Send Mail to other users
            //        if (differentMails.Count > 0)
            //        {
            //            var mail = GetMailMessage(differentMails, _from, _hostName);
            //            using (SmtpClient SmtpServer = new SmtpClient(_hostName))
            //            {
            //                SmtpServer.Port = 587;
            //                SmtpServer.UseDefaultCredentials = false;
            //                SmtpServer.Credentials = new System.Net.NetworkCredential(_user, _password);
            //                SmtpServer.EnableSsl = false;

            //                mail.Subject = subject;
            //                mail.IsBodyHtml = isHtml;
            //                foreach (var m in differentMails)
            //                {
            //                    if (m.SendingType == 0)
            //                        mail.To.Add(m.MailId);
            //                }
            //                mail.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
            //                SmtpServer.Send(mail);
            //            }
            //        }
            //        #endregion

            //    }
            //    catch (Exception ex)
            //    {
            //        //TODO: implement resend functionality
            //    }
            //});

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var _hostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
                    var _from = System.Configuration.ConfigurationManager.AppSettings["EmailFrom"];
                    var _user = System.Configuration.ConfigurationManager.AppSettings["UserName"];
                    var _password = System.Configuration.ConfigurationManager.AppSettings["Password"];

                    var _alternateHostName = System.Configuration.ConfigurationManager.AppSettings["AlternateHostName"];
                    var _alternatePassword = System.Configuration.ConfigurationManager.AppSettings["AlternatePassword"];
                    var _alternateUserName = System.Configuration.ConfigurationManager.AppSettings["AlternateUser"];

                    var zionMails = new List<MailUser>();
                    var differentMails = to.ToList();

                    #region Send Mail to Zion Market Research Users
                    if (zionMails.Count > 0)
                    {
                        var mail_zoin = GetMailMessage(zionMails, _from, _alternateHostName);
                        var SmtpServer_zion = GetSMTPClient(new System.Net.NetworkCredential(_alternateUserName, _alternatePassword), _alternateHostName);
                        mail_zoin.Subject = subject;
                        mail_zoin.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
                        mail_zoin.IsBodyHtml = isHtml;
                        foreach (var m in zionMails)
                        {
                            if (m.SendingType == 0)
                                mail_zoin.To.Add(m.MailId);
                        }
                        if (!string.IsNullOrEmpty(attachmentFile))
                        {
                            using (var attachment = new Attachment(attachmentFile))
                            {
                                mail_zoin.Attachments.Add(attachment);
                            }
                        }
                        SmtpServer_zion.Send(mail_zoin);
                    }
                    #endregion

                    #region Send Mail to other users
                    if (differentMails.Count > 0)
                    {
                        var mail = GetMailMessage(differentMails, _from, _hostName);
                        using (SmtpClient SmtpServer = new SmtpClient(_hostName))
                        {
                            SmtpServer.Port = 587;
                            SmtpServer.UseDefaultCredentials = false;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(_user, _password);
                            SmtpServer.EnableSsl = false;

                            mail.Subject = subject;
                            mail.IsBodyHtml = isHtml;
                            foreach (var m in differentMails)
                            {
                                if (m.SendingType == 0)
                                    mail.To.Add(m.MailId);
                            }
                            mail.Body = body + "<br/> Thanks and regards,<br/><br/>" + System.Configuration.ConfigurationManager.AppSettings["Signature"];
                            SmtpServer.Send(mail);
                        }
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    //TODO: implement resend functionality
                }
            }).ConfigureAwait(false);
        }

        static MailMessage GetMailMessage(List<MailUser> user, string _from, string host)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_from, _from)
            };

            foreach (MailUser m in user)
            {
                switch (m.SendingType)
                {
                    case 0:
                        mail.To.Add(m.MailId);
                        break;
                    case 1:
                        mail.CC.Add(m.MailId);
                        break;
                    case 2:
                        mail.Bcc.Add(m.MailId);
                        break;
                    default:
                        break;
                }
            }
            return mail;
        }

        static SmtpClient GetSMTPClient(System.Net.NetworkCredential credentials, string _hostName)
        {
            var SmtpServer = new SmtpClient(_hostName)
            {
                UseDefaultCredentials = false,
                Credentials = credentials,
                EnableSsl = false,
                Port = 587
            };
            return SmtpServer;
        }
    }

    public class MailUser
    {
        public string MailId { get; set; }
        public int SendingType { get; set; }
    }
}
