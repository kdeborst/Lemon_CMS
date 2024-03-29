﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Lemon.Service
{
    public class EmailSender : IEmailSender
    {
        //PROPERTIES
        public EmailOptions Options { get; set; }


        //CONSTRUCTOR
        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }


        //
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        //
        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("kevin.de.borst@gmail.com", "Lemon Restaurant"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            msg.AddTo(new EmailAddress(email));

            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
