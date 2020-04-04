using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Pickup.Api.Settings;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Pickup.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _email;
        private readonly IWebHostEnvironment _env;

        public EmailService(IOptions<EmailSettings> email, IWebHostEnvironment env)
        {
            _email = email.Value;
            _env = env;
        }


        public async Task SendAsync(string EmailDisplayName, string Subject, string Body, string From, string To)
        {
            using var mailMessage = new MailMessage();
            PrepareMailMessage(EmailDisplayName, Subject, Body, From, To, mailMessage);
            await Execute(mailMessage);
        }

        public async Task SendEmailConfirmationAsync(string EmailAddress, string CallbackUrl)
        {
            using MailMessage mailMessage = new MailMessage();
            PrepareMailMessage(_email.DisplayName, "Confirm your email", $"Please confirm your email by clicking here: <a href='{CallbackUrl}'>link</a>", _email.From, EmailAddress, mailMessage);
            await Execute(mailMessage);
        }

        public async Task SendPasswordResetAsync(string EmailAddress, string CallbackUrl)
        {
            using var mailMessage = new MailMessage();
            PrepareMailMessage(_email.DisplayName, "Reset your password", $"Please reset your password by clicking here: <a href='{CallbackUrl}'>link</a>", _email.From, EmailAddress, mailMessage);
            await Execute(mailMessage);
        }

        public async Task SendException(Exception ex)
        {
            using var mailMessage = new MailMessage();
            PrepareMailMessage(_email.DisplayName, $"({_env.EnvironmentName}) INTERNAL SERVER ERROR", $"{ex}", _email.From, _email.To, mailMessage);
            await Execute(mailMessage);
        }

        public async Task SendSqlException(SqlException ex)
        {
            using var mailMessage = new MailMessage();
            PrepareMailMessage(_email.DisplayName, $"({_env.EnvironmentName}) SQL ERROR", $"{ex}", _email.From, _email.To, mailMessage);
            await Execute(mailMessage);
        }

        private void PrepareMailMessage(string EmailDisplayName, string Subject, string Body, string From, string To, MailMessage mailMessage)
        {
            mailMessage.From = new MailAddress(From, EmailDisplayName);
            mailMessage.To.Add(To);
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = Subject;
        }

        public async Task Execute(MailMessage mailMessage)
        {

            using SmtpClient client = new SmtpClient(_email.SMTPServer, _email.Port);
            if (!_email.DefaultCredentials)
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_email.UserName, _email.Password);
            }
#if !DEBUG
            await client.SendMailAsync(mailMessage);
#else
            await Task.CompletedTask;
#endif
        }
    }
}
