using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FIT5032_app.Utils
{
    // Contains a method of sending an email to customers.
    public class EmailSender
    {
        
        private const String API_KEY = "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";

        public void Send(String toEmail, String subject, String contents)
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@localhost.com", "Lin's Weight Loss Centre");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }
    }
}