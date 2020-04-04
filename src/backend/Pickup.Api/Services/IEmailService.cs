using Microsoft.Data.SqlClient;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Pickup.Api.Services
{
    public interface IEmailService
    {
        Task Execute(MailMessage mailMessage);

        Task SendAsync(string EmailDisplayName, string Subject, string Body, string From, string To);

        Task SendEmailConfirmationAsync(string Email, string CallbackUrl);

        Task SendPasswordResetAsync(string Email, string CallbackUrl);

        Task SendException(Exception ex);

        Task SendSqlException(SqlException ex);
    }
}
