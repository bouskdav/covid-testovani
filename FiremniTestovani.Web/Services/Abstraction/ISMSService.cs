using FiremniTestovani.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services.Abstraction
{
    public interface ISMSService
    {
        Task SendMessageAsync(MimeMessage message);

        Task SendMessageAsync(MimeMessage[] messages);

        ValueTask<MimeMessage> GetEmployeeConfirmationMessage(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployeeTestNegativeMessage(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployeeTestPositiveMessage(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployerTestNegativeMessage(ApplicationBooking booking);
        ValueTask<MimeMessage> GetEmployerTestPositiveMessage(ApplicationBooking booking);
    }
}
