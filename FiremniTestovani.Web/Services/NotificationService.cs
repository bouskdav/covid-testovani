using FiremniTestovani.Models;
using FiremniTestovani.Web.Services.Abstraction;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMailService _mailService;
        private readonly ISMSService _smsService;
        private readonly ISourceSettingsService _settingsService;
        protected readonly ILogger _logger;

        public NotificationService(IMailService mailService, ISMSService smsService, ISourceSettingsService settingsService, ILoggerFactory logger)
        {
            this._mailService = mailService;
            this._smsService = smsService;
            this._settingsService = settingsService;

            _logger = logger.CreateLogger("Notifications");
        }

        public async Task NotifyBookingCreated(ApplicationBooking applicationBooking)
        {
            // emails
            try
            {
                if (_settingsService.GetSettingsForCurrentSource<bool>("Notification_NewBooking_Employee_Email", false, false))
                {
                    // container for emails
                    List<MimeMessage> emails = new List<MimeMessage>();

                    // add confirmation email
                    emails.Add(await _mailService.GetEmployeeConfirmationEmail(applicationBooking));

                    // send emails
                    await _mailService.SendEmailAsync(emails.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotifyBookingCreated (EMAIL) failed for {JsonConvert.SerializeObject(applicationBooking)}.");
            }

            // sms
            try
            {
                if (_settingsService.GetSettingsForCurrentSource<bool>("Notification_NewBooking_Employee_SMS", false, false))
                {
                    // container for sms
                    List<MimeMessage> messages = new List<MimeMessage>();

                    // add confirmation sms
                    messages.Add(await _smsService.GetEmployeeConfirmationMessage(applicationBooking));

                    // send messages
                    await _smsService.SendMessageAsync(messages.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotifyBookingCreated (SMS) failed for {JsonConvert.SerializeObject(applicationBooking)}.");
            }
        }

        public async Task NotifyTestResultNegative(ApplicationBooking applicationBooking, bool notifyEmployee, bool notifyEmployer)
        {
            bool smsNotifyEmployee = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultNegative_Employee_SMS", false, false);
            bool smsNotifyEmployer = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultNegative_Employer_SMS", false, false);
            bool emailNotifyEmployee = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultNegative_Employee_Email", false, false);
            bool emailNotifyEmployer = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultNegative_Employer_Email", false, false);

                // emails
            try
            {
                // container for emails
                List<MimeMessage> emails = new List<MimeMessage>();

                // add email for employee
                if (notifyEmployee && emailNotifyEmployee)
                    emails.Add(await _mailService.GetEmployeeTestNegativeEmail(applicationBooking));
                
                // add email for employer
                if (notifyEmployer && emailNotifyEmployer)
                    emails.Add(await _mailService.GetEmployerTestNegativeEmail(applicationBooking));

                // send emails
                if (emails.Count > 0)
                    await _mailService.SendEmailAsync(emails.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotifyTestResultNegative (EMAIL) failed for {JsonConvert.SerializeObject(applicationBooking)}.");
            }

            // sms
            try
            {
                // container for sms
                List<MimeMessage> messages = new List<MimeMessage>();

                // add sms for employee
                if (notifyEmployee && smsNotifyEmployee)
                    messages.Add(await _smsService.GetEmployeeTestNegativeMessage(applicationBooking));

                // add sms for employer
                if (notifyEmployer && smsNotifyEmployer)
                    messages.Add(await _smsService.GetEmployerTestNegativeMessage(applicationBooking));

                // send messages
                if (messages.Count > 0)
                    await _smsService.SendMessageAsync(messages.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotifyTestResultNegative (SMS) failed for {JsonConvert.SerializeObject(applicationBooking)}.");
            }
        }

        public async Task NotifyTestResultPositive(ApplicationBooking applicationBooking, bool notifyEmployee, bool notifyEmployer)
        {
            bool smsNotifyEmployee = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultPositive_Employee_SMS", false, false);
            bool smsNotifyEmployer = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultPositive_Employer_SMS", false, false);
            bool emailNotifyEmployee = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultPositive_Employee_Email", false, false);
            bool emailNotifyEmployer = _settingsService.GetSettingsForCurrentSource<bool>("Notification_ResultPositive_Employer_Email", false, false);

            // emails
            try
            {
                // container for emails
                List<MimeMessage> emails = new List<MimeMessage>();

                // add email for employee
                if (notifyEmployee && emailNotifyEmployee)
                    emails.Add(await _mailService.GetEmployeeTestPositiveEmail(applicationBooking));

                // add email for employer
                if (notifyEmployer && emailNotifyEmployer)
                    emails.Add(await _mailService.GetEmployerTestPositiveEmail(applicationBooking));

                // send emails
                await _mailService.SendEmailAsync(emails.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotifyTestResultPositive (EMAIL) failed for {JsonConvert.SerializeObject(applicationBooking)}.");
            }

            // sms
            try
            {
                // container for sms
                List<MimeMessage> messages = new List<MimeMessage>();

                // add sms for employee
                if (notifyEmployee && smsNotifyEmployee)
                    messages.Add(await _smsService.GetEmployeeTestPositiveMessage(applicationBooking));

                // add sms for employer
                if (notifyEmployer && smsNotifyEmployer)
                    messages.Add(await _smsService.GetEmployerTestPositiveMessage(applicationBooking));

                // send messages
                await _smsService.SendMessageAsync(messages.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"NotifyTestResultPositive (SMS) failed for {JsonConvert.SerializeObject(applicationBooking)}.");
            }
        }
    }
}
