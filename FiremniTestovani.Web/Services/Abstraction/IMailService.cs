using FiremniTestovani.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services.Abstraction
{
    public interface IMailService
    {
        Task SendEmailAsync(MimeMessage message);

        Task SendEmailAsync(MimeMessage[] messages);

        ValueTask<MimeMessage> GetEmployeeConfirmationEmail(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployeeTestNegativeEmail(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployeeTestPositiveEmail(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployerTestNegativeEmail(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployerTestPositiveEmail(ApplicationBooking booking);
    }
}
