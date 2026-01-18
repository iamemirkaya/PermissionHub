using Application.DTOs.Email;
using Application.Services.MailService;
using Common.Requests.Settings;
using Common.Responses.Wrappers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.MailService
{
    public class EmailService : IEmailService
    {
        public MailSettings _mailSettings { get; }

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task<IResponseWrapper> SendAsync(EmailRequest request)
        {
            try
            {

                var email = new MimeMessage();

                email.Sender = new MailboxAddress(_mailSettings.DisplayName, request.From ?? _mailSettings.EmailFrom);

                email.To.Add(MailboxAddress.Parse(request.To));

                email.Subject = request.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = request.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return await ResponseWrapper.SuccessAsync("Mail başarıyla gönderildi.");

            }
            catch (System.Exception ex)
            {
                return await ResponseWrapper.FailAsync($"Mail gönderilemedi: {ex.Message}");
            }
        }
    }
}
