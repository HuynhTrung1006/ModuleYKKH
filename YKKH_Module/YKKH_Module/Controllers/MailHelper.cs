using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace YKKH_Module.Controllers
{
    public class MailHelper
    {
        public void SendMail(string toEmail, string subject, string content)
        {
            var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
            var smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]).ToString();
            var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            var fromPassword = ConfigurationManager.AppSettings["FromEmailPassword"];
            var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"];

            bool enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSSL"].ToString());

            string body = content;
            MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(toEmail));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            var smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential(fromEmailAddress, fromPassword);
            smtpClient.Host = smtpHost;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
            smtpClient.Send(message);

        }
    }
}