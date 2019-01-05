using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowerShop.Services
{
    public class EmailSender : IEmailSender
    {
        private string _apiKey;
        public EmailSender(string apiKey)
        {
            this._apiKey = apiKey;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            SendGrid.SendGridClient client = new SendGrid.SendGridClient(_apiKey);
            var msg = new SendGrid.Helpers.Mail.SendGridMessage()
            {
                From = new SendGrid.Helpers.Mail.EmailAddress("admin@flowershop.com", "Flower Shop Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(email));

            msg.TrackingSettings = new SendGrid.Helpers.Mail.TrackingSettings
            {
                ClickTracking = new SendGrid.Helpers.Mail.ClickTracking { Enable = false }
            };

            return client.SendEmailAsync(msg);
        }

    }
}
