using FiremniTestovani.Models;
using FiremniTestovani.Web.Models;
using FiremniTestovani.Web.Models.Emailing;
using FiremniTestovani.Web.Services.Abstraction;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MimeKit;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Services
{
    public class SMSOverSMTPService : ISMSService
    {
        private readonly SMSSettings _smsSettings;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        //private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly ISourceSettingsService _settingsService;

        private static string[] _commonSentFolderNames = { "Sent Messages", "Sent Items", "Sent Mail", "Sent" };

        public SMSOverSMTPService(IOptions<SMSSettings> smsSettings, IEmailService emailService, IWebHostEnvironment environment, IHttpContextAccessor accessor, LinkGenerator generator, ISourceSettingsService settingsService)
        {
            // get settings default values
            _smsSettings = smsSettings.Value;
            _emailService = emailService;
            _environment = environment;
            _accessor = accessor;
            _generator = generator;
            _settingsService = settingsService;

            // override the settings if present in db
            if (_settingsService.ContainsKey("SMSNotificationsEnabled"))
            {
                _smsSettings.EnableSMSNotifications = _settingsService.GetSettingsForCurrentSource<bool>("SMSNotificationsEnabled", false);

                if (_smsSettings.EnableSMSNotifications)
                {
                    switch (_settingsService.GetSettingsForCurrentSource<string>("SMSNotificationsProvider", false))
                    {
                        case "SMTP":
                            _smsSettings.Mail = _settingsService.GetSettingsForCurrentSource<string>("SMSNotifications_SMTP_From", false);
                            _smsSettings.SMTPHost = _settingsService.GetSettingsForCurrentSource<string>("SMSNotifications_SMTP_Host", false);
                            _smsSettings.SMTPPort = _settingsService.GetSettingsForCurrentSource<int>("SMSNotifications_SMTP_Port", false);
                            _smsSettings.SMTPUser = _settingsService.GetSettingsForCurrentSource<string>("SMSNotifications_SMTP_User", false);
                            _smsSettings.SMTPPassword = _settingsService.GetSettingsForCurrentSource<string>("SMSNotifications_SMTP_Password", false);
                            break;
                        default:
                            throw new NotImplementedException($"Method {_settingsService.GetSettingsForCurrentSource<string>("SMSNotificationsProvider", false)} for sending SMS not implemented.");
                    }
                }
            }
        }

        #region generate messages

        /// <summary>
        /// Email for employee with booking confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MimeMessage> GetEmployeeConfirmationMessage(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            string phoneNumber = NormalizePhoneNumber(booking.Phone);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, $"{phoneNumber}@sms.gz"));
            // add sender
            message.From.Add(new MailboxAddress("GZMedia", _smsSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = "Rezervace testování";
            string textBody = $"Dobrý den, potvrzujeme rezervaci na COVID-19 testy na jméno {booking.Name} v {booking.FromExpected?.ToString("HH:mm")} dne {booking.FromExpected?.ToString("dd.MM.yyyy")} v GZ Media.";

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            //bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            return new ValueTask<MimeMessage>(message);
        }

        public ValueTask<MimeMessage> GetEmployeeTestNegativeMessage(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            string phoneNumber = NormalizePhoneNumber(booking.Phone);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, $"{phoneNumber}@sms.gz"));
            // add sender
            message.From.Add(new MailboxAddress("GZMedia", _smsSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = "Výsledek - NEGATIVNÍ";
            string textBody = $"Výsledek COVID-19 testu na jméno {booking.Name} v {booking.FromActual?.ToString("HH:mm")} dne {booking.FromActual?.ToString("dd.MM.yyyy")} v GZ Media je NEGATIVNÍ.";

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            //bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            return new ValueTask<MimeMessage>(message);
        }

        public ValueTask<MimeMessage> GetEmployeeTestPositiveMessage(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            string phoneNumber = NormalizePhoneNumber(booking.Phone);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, $"{phoneNumber}@sms.gz"));
            // add sender
            message.From.Add(new MailboxAddress("GZMedia", _smsSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = "Výsledek - POZITIVNÍ";
            string textBody = $"Výsledek COVID-19 testu na jméno {booking.Name} v {booking.FromActual?.ToString("HH:mm")} dne {booking.FromActual?.ToString("dd.MM.yyyy")} v GZ Media je POZITIVNÍ. Kontaktujte prosím personální oddělení.";

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            //bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            return new ValueTask<MimeMessage>(message);
        }

        public ValueTask<MimeMessage> GetEmployerTestNegativeMessage(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            string phoneNumber = NormalizePhoneNumber(booking.Phone);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, $"{phoneNumber}@sms.gz"));
            // add sender
            message.From.Add(new MailboxAddress("GZMedia", _smsSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = "COVID Negativní";
            string textBody = $"Výsledek COVID-19 testu na jméno {booking.Name} v {booking.FromExpected?.ToString("HH:mm")} dne {booking.FromExpected?.ToString("dd.MM.yyyy")} v GZ Media je NEGATIVNÍ.";

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            //bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            return new ValueTask<MimeMessage>(message);
        }

        public ValueTask<MimeMessage> GetEmployerTestPositiveMessage(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            string phoneNumber = NormalizePhoneNumber(booking.Phone);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, $"{phoneNumber}@sms.gz"));
            // add sender
            message.From.Add(new MailboxAddress("GZMedia", _smsSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = "COVID POZITIVNÍ";
            string textBody = $"POZOR: Výsledek COVID-19 testu {booking.Name} ({booking.EmployeeID}) v {booking.FromExpected?.ToString("HH:mm")} dne {booking.FromExpected?.ToString("dd.MM.yyyy")} v GZ Media je POZITIVNÍ.";

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            //bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = textBody;

            message.Body = bodyBuilder.ToMessageBody();

            return new ValueTask<MimeMessage>(message);
        }

        #endregion

        #region methods for sending messages

        public async Task SendMessageAsync(MimeMessage message)
        {
            if (!_smsSettings.EnableSMSNotifications)
                return;

            string hostSMTP = _smsSettings.SMTPHost;
            int portSMTP = _smsSettings.SMTPPort;
            // SSL - nastaven mód auto => není nutné explicitně uvádět (snad)
            //bool enableSslSMTP = dbSettings.SingleOrDefault(i => i.NazevHodnoty == "SmtpSSL")?.HodnotaBool ?? false;
            string usernameSMTP = _smsSettings.SMTPUser;
            string passwordSMTP = _smsSettings.SMTPPassword;

            using (SmtpClient client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;

                if (portSMTP == 25)
                    await client.ConnectAsync(hostSMTP, portSMTP, SecureSocketOptions.None);
                else
                    await client.ConnectAsync(hostSMTP, portSMTP, SecureSocketOptions.Auto);

                if (!String.IsNullOrEmpty(usernameSMTP))
                    await client.AuthenticateAsync(usernameSMTP, passwordSMTP);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendMessageAsync(MimeMessage[] messages)
        {
            if (!_smsSettings.EnableSMSNotifications)
                return;

            if (messages == null || messages.Length == 0)
                return;

            string hostSMTP = _smsSettings.SMTPHost;
            int portSMTP = _smsSettings.SMTPPort;
            // SSL - nastaven mód auto => není nutné explicitně uvádět (snad)
            //bool enableSslSMTP = dbSettings.SingleOrDefault(i => i.NazevHodnoty == "SmtpSSL")?.HodnotaBool ?? false;
            string usernameSMTP = _smsSettings.SMTPUser;
            string passwordSMTP = _smsSettings.SMTPPassword;


            using (SmtpClient client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;

                if (portSMTP == 25)
                    await client.ConnectAsync(hostSMTP, portSMTP, SecureSocketOptions.None);
                else
                    await client.ConnectAsync(hostSMTP, portSMTP, SecureSocketOptions.Auto);

                if (!String.IsNullOrEmpty(usernameSMTP))
                    await client.AuthenticateAsync(usernameSMTP, passwordSMTP);

                foreach (var message in messages)
                    await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        #endregion

        private string NormalizePhoneNumber(string input)
        {
            string phoneNumber = input.Trim().Replace(" ", "");

            if (phoneNumber.StartsWith("00"))
                phoneNumber = phoneNumber.Substring(2);

            if (phoneNumber.StartsWith("+"))
                phoneNumber = phoneNumber.Replace("+", "");

            if (phoneNumber.Length == 9)
                phoneNumber = "420" + phoneNumber;

            return phoneNumber;
        }
    }
}
