using Application.DTOs.Email;
using Common.Responses.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.MailService
{
    public interface IEmailService
    {
        Task<IResponseWrapper> SendAsync(EmailRequest request);
    }
}
