using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;

namespace AfricanMovies.Email
{
    public class MailService : IMailService
    {
        public async Task SendMail(string fromEmail, List<string> toEmails, string subject, string body)
        {
            var myMessage = new SendGridMessage {From = new MailAddress(fromEmail)};
            var recipients = toEmails;
            myMessage.AddTo(recipients);
            myMessage.Subject = subject;
            myMessage.Html = body;
            myMessage.Text = body;

            var username = Environment.GetEnvironmentVariable("SENDGRID_USER"); //azure_319cea33fc47810d80cdf9feb551e4db@azure.com
            var pswd = Environment.GetEnvironmentVariable("SENDGRID_PASS"); //xtdD6531F828zR4
            var credentials = new NetworkCredential(username, pswd);
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(myMessage);
        }
    }
}