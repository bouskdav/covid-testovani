using FiremniTestovani.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services.Abstraction
{
    public interface INotificationService
    {
        Task NotifyBookingCreated(ApplicationBooking applicationBooking);

        Task NotifyTestResultNegative(ApplicationBooking applicationBooking, bool notifyEmployee, bool notifyEmployer);

        Task NotifyTestResultPositive(ApplicationBooking applicationBooking, bool notifyEmployee, bool notifyEmployer);
    }
}
