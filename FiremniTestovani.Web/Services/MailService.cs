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
using MimeKit.IO;
using Postal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IMailService = FiremniTestovani.Web.Services.Abstraction.IMailService;

namespace FiremniTestovani.Web.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        //private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly ISourceSettingsService _settingsService;

        private static string[] _commonSentFolderNames = { "Sent Messages", "Sent Items", "Sent Mail", "Sent" };

        public MailService(IOptions<MailSettings> mailSettings, IEmailService emailService, IWebHostEnvironment environment, IHttpContextAccessor accessor, LinkGenerator generator, ISourceSettingsService settingsService)
        {
            _mailSettings = mailSettings.Value;
            _emailService = emailService;
            _environment = environment;
            _accessor = accessor;
            _generator = generator;
            _settingsService = settingsService;

            // override the settings if present in db
            if (_settingsService.ContainsKey("MailNotificationsEnabled"))
            {
                _mailSettings.EnableMailNotifications = _settingsService.GetSettingsForCurrentSource<bool>("MailNotificationsEnabled", false);

                if (_mailSettings.EnableMailNotifications)
                {
                    switch (_settingsService.GetSettingsForCurrentSource<string>("MailNotificationsProvider", false))
                    {
                        // TODO
                        case "SMTP":
                            _mailSettings.Mail = _settingsService.GetSettingsForCurrentSource<string>("MailNotifications_SMTP_From", false);
                            _mailSettings.SMTPHost = _settingsService.GetSettingsForCurrentSource<string>("MailNotifications_SMTP_Host", false);
                            _mailSettings.SMTPPort = _settingsService.GetSettingsForCurrentSource<int>("MailNotifications_SMTP_Port", false);
                            _mailSettings.SMTPUser = _settingsService.GetSettingsForCurrentSource<string>("MailNotifications_SMTP_User", false);
                            _mailSettings.SMTPPassword = _settingsService.GetSettingsForCurrentSource<string>("MailNotifications_SMTP_Password", false);
                            break;
                        case "PickupDirectory":
                            _mailSettings.PickupDirectoryPath = _settingsService.GetSettingsForCurrentSource<string>("MailNotifications_PickupDirectory_Path", false);
                            break;
                        default:
                            throw new NotImplementedException($"Method {_settingsService.GetSettingsForCurrentSource<string>("MailNotificationsProvider", false)} for sending emails not implemented.");
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
        public async ValueTask<MimeMessage> GetEmployeeConfirmationEmail(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            // model pro email
            EmployeeBookingConfirmationEmail tempMessageModel = new EmployeeBookingConfirmationEmail(booking);
            System.Net.Mail.MailMessage tempMessage = await _emailService.CreateMailMessageAsync(tempMessageModel);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, booking.Email));
            // add sender
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = tempMessage.Subject;
            string textBody = tempMessage.Body;

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = "-";

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        public async ValueTask<MimeMessage> GetEmployeeTestNegativeEmail(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            // model pro email
            EmployeeTestNegativeEmail tempMessageModel = new EmployeeTestNegativeEmail(booking);
            System.Net.Mail.MailMessage tempMessage = await _emailService.CreateMailMessageAsync(tempMessageModel);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, booking.Email));
            // add sender
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = tempMessage.Subject;
            string textBody = tempMessage.Body;

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = "-";

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        public async ValueTask<MimeMessage> GetEmployeeTestPositiveEmail(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            // model pro email
            EmployeeTestPositiveEmail tempMessageModel = new EmployeeTestPositiveEmail(booking);
            System.Net.Mail.MailMessage tempMessage = await _emailService.CreateMailMessageAsync(tempMessageModel);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, booking.Email));
            // add sender
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = tempMessage.Subject;
            string textBody = tempMessage.Body;

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = "-";

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        public async ValueTask<MimeMessage> GetEmployerTestNegativeEmail(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            // model pro email
            EmployerTestNegativeEmail tempMessageModel = new EmployerTestNegativeEmail(booking);
            System.Net.Mail.MailMessage tempMessage = await _emailService.CreateMailMessageAsync(tempMessageModel);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, booking.Email));
            // add sender
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = tempMessage.Subject;
            string textBody = tempMessage.Body;

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = "-";

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        public async ValueTask<MimeMessage> GetEmployerTestPositiveEmail(ApplicationBooking booking)
        {
            MimeMessage message = new MimeMessage();

            // model pro email
            EmployerTestPositiveEmail tempMessageModel = new EmployerTestPositiveEmail(booking);
            System.Net.Mail.MailMessage tempMessage = await _emailService.CreateMailMessageAsync(tempMessageModel);

            // add recipient
            message.To.Add(new MailboxAddress(booking.Name, booking.Email));
            // add sender
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            // do message přenést z vyrenderované zprávy předmět a tělo
            message.Subject = tempMessage.Subject;
            string textBody = tempMessage.Body;

            BodyBuilder bodyBuilder = new BodyBuilder();

            // textová část
            bodyBuilder.HtmlBody = textBody;
            bodyBuilder.TextBody = "-";

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        #endregion

        #region methods for sending messages

        public async Task SendEmailAsync(MimeMessage message)
        {
            if (!_mailSettings.EnableMailNotifications)
                return;

            string hostSMTP = _mailSettings.SMTPHost;
            int portSMTP = _mailSettings.SMTPPort;
            // SSL - nastaven mód auto => není nutné explicitně uvádět (snad)
            //bool enableSslSMTP = dbSettings.SingleOrDefault(i => i.NazevHodnoty == "SmtpSSL")?.HodnotaBool ?? false;
            string usernameSMTP = _mailSettings.SMTPUser;
            string passwordSMTP = _mailSettings.SMTPPassword;

            string hostIMAP = _mailSettings.IMAPHost;
            int portIMAP = _mailSettings.IMAPPort;
            // SSL - nastaven mód auto => není nutné explicitně uvádět (snad)
            //bool enableSslIMAP = dbSettings.SingleOrDefault(i => i.NazevHodnoty == "ImapSSL")?.HodnotaBool ?? false;
            string usernameIMAP = _mailSettings.IMAPUser;
            string passwordIMAP = _mailSettings.IMAPPassword;

            switch (_mailSettings.MailDelivery)
            {
                case "SMTP":
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

                    // if IMAP enabled, save message to sent folder (as outlook)
                    if (_mailSettings.EnableIMAP)
                    {
                        using (ImapClient client = new ImapClient(new ProtocolLogger(Console.OpenStandardError())))
                        {
                            client.CheckCertificateRevocation = false;

                            await client.ConnectAsync(hostIMAP, portIMAP, SecureSocketOptions.Auto);
                            await client.AuthenticateAsync(usernameIMAP, passwordIMAP);

                            var personal = client.GetFolder(client.PersonalNamespaces[0]);
                            var subfolders = personal.GetSubfolders(false);
                            // first try to see if any of the common sent folders exist
                            var sentFolder = subfolders.FirstOrDefault(x => x.Name == "Sent Items");

                            if (sentFolder == null)
                            {
                                // then see if the sent folder in the policy exists,
                                // but only if the policy folder is not already in the common folder list (since we already checked there)
                                if (!_commonSentFolderNames.Contains("Sent Items"))
                                    sentFolder = subfolders.FirstOrDefault(folder => folder.Name == "Sent Items");

                                // if not, create the folder
                                if (sentFolder == null)
                                    sentFolder = personal.Create("Sent Items", true);
                            }

                            await sentFolder.AppendAsync(message);

                            await client.DisconnectAsync(true);
                        }
                    }

                    break;
                case "PickupDirectory":
                    await SaveToPickupDirectory(message, _mailSettings.PickupDirectoryPath);

                    break;
                default:
                    throw new NotImplementedException($"Method {_mailSettings.MailDelivery} not implemented.");
            }
        }

        public async Task SendEmailAsync(MimeMessage[] messages)
        {
            if (!_mailSettings.EnableMailNotifications)
                return;

            if (messages == null || messages.Length == 0)
                return;

            string hostSMTP = _mailSettings.SMTPHost;
            int portSMTP = _mailSettings.SMTPPort;
            // SSL - nastaven mód auto => není nutné explicitně uvádět (snad)
            //bool enableSslSMTP = dbSettings.SingleOrDefault(i => i.NazevHodnoty == "SmtpSSL")?.HodnotaBool ?? false;
            string usernameSMTP = _mailSettings.SMTPUser;
            string passwordSMTP = _mailSettings.SMTPPassword;

            string hostIMAP = _mailSettings.IMAPHost;
            int portIMAP = _mailSettings.IMAPPort;
            // SSL - nastaven mód auto => není nutné explicitně uvádět (snad)
            //bool enableSslIMAP = dbSettings.SingleOrDefault(i => i.NazevHodnoty == "ImapSSL")?.HodnotaBool ?? false;
            string usernameIMAP = _mailSettings.IMAPUser;
            string passwordIMAP = _mailSettings.IMAPPassword;

            switch (_mailSettings.MailDelivery)
            {
                case "SMTP":
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

                    // if IMAP enabled, save message to sent folder (as outlook)
                    if (_mailSettings.EnableIMAP)
                    {
                        using (ImapClient client = new ImapClient(new ProtocolLogger(Console.OpenStandardError())))
                        {
                            client.CheckCertificateRevocation = false;

                            await client.ConnectAsync(hostIMAP, portIMAP, SecureSocketOptions.Auto);
                            await client.AuthenticateAsync(usernameIMAP, passwordIMAP);

                            var personal = client.GetFolder(client.PersonalNamespaces[0]);
                            var subfolders = personal.GetSubfolders(false);
                            // first try to see if any of the common sent folders exist
                            var sentFolder = subfolders.FirstOrDefault(x => x.Name == "Sent Items");

                            if (sentFolder == null)
                            {
                                // then see if the sent folder in the policy exists,
                                // but only if the policy folder is not already in the common folder list (since we already checked there)
                                if (!_commonSentFolderNames.Contains("Sent Items"))
                                    sentFolder = subfolders.FirstOrDefault(folder => folder.Name == "Sent Items");

                                // if not, create the folder
                                if (sentFolder == null)
                                    sentFolder = personal.Create("Sent Items", true);
                            }

                            foreach (var message in messages)
                                await sentFolder.AppendAsync(message);

                            await client.DisconnectAsync(true);
                        }
                    }
                    break;

                case "PickupDirectory":
                    foreach (var message in messages)
                        await SaveToPickupDirectory(message, _mailSettings.PickupDirectoryPath);

                    break;

                default:
                    throw new NotImplementedException($"Method {_mailSettings.MailDelivery} not implemented.");
            }
        }

        public Task SaveToPickupDirectory(MimeMessage message, string pickupDirectory)
        {
            do
            {
                // Generate a random file name to save the message to.
                var path = Path.Combine(pickupDirectory, Guid.NewGuid().ToString() + ".eml");
                Stream stream;

                try
                {
                    // Attempt to create the new file.
                    stream = File.Open(path, FileMode.CreateNew);
                }
                catch (IOException)
                {
                    // If the file already exists, try again with a new Guid.
                    if (File.Exists(path))
                        continue;

                    // Otherwise, fail immediately since it probably means that there is
                    // no graceful way to recover from this error.
                    throw;
                }

                try
                {
                    using (stream)
                    {
                        // IIS pickup directories expect the message to be "byte-stuffed"
                        // which means that lines beginning with "." need to be escaped
                        // by adding an extra "." to the beginning of the line.
                        //
                        // Use an SmtpDataFilter "byte-stuff" the message as it is written
                        // to the file stream. This is the same process that an SmtpClient
                        // would use when sending the message in a `DATA` command.
                        using (var filtered = new FilteredStream(stream))
                        {
                            filtered.Add(new SmtpDataFilter());

                            // Make sure to write the message in DOS (<CR><LF>) format.
                            var options = FormatOptions.Default.Clone();
                            options.NewLineFormat = NewLineFormat.Dos;

                            message.WriteTo(options, filtered);
                            filtered.Flush();
                            return Task.FromResult(0);
                        }
                    }
                }
                catch
                {
                    // An exception here probably means that the disk is full.
                    //
                    // Delete the file that was created above so that incomplete files are not
                    // left behind for IIS to send accidentally.
                    File.Delete(path);
                    throw;
                }
            } while (true);
        }

        #endregion
    }
}
