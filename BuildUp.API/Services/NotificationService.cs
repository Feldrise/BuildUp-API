using BuildUp.API.Models.Users;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using BuildUp.API.Entities;
using BuildUp.API.Entities.Notification;
using MongoDB.Driver;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Mime;

namespace BuildUp.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMailCredentials _mailCredentials;

        private readonly IMongoCollection<CoachNotification> _coachNotifications;

        readonly IWebHostEnvironment _env;

        public NotificationService(IMongoSettings mongoSettings, IMailCredentials mailCredentials, IWebHostEnvironment env)
        {
            _mailCredentials = mailCredentials;
            _env = env;

            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _coachNotifications = database.GetCollection<CoachNotification>("coach_notifications");
        }

        public async Task NotifieAccountCreationAsync(RegisterModel registerModel, string password)
        {
            string subject = "Inscription au program Build-up !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/reception_candidature.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$fullName", registerModel.FirstName);
            message = message.Replace("$builderOrCoach", registerModel.Role == Role.Builder ? "Builder" : "Coach");
            message = message.Replace("$password", password);

            await SendMailAsync(
                subject,
                message,
                registerModel.Email
            );
        }

        // Builders
        public async Task NotifyPreselectionBuilder(string email, string name)
        {
            string subject = "Programme Build Up - Tu as été présélectionné(e) !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/preselection_builder.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                subject,
                message,
                email
            );
        }

        public async Task NotifyAdminMeetingValidatedBuilder(string email, string name)
        {
            string subject = "Build Up - Avis sur entretien de sélection Builder";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/validate_admin_meeting_builder.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                subject,
                message,
                email
            );
        }

        public async Task NotifyCoachAcceptBuilder(string email, string name)
        {
            string subject = "Build Up - Un Coach t’a accepté !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/validate_coach_builder.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                subject,
                message,
                email
            );
        }

        public async Task NotifySignedIntegrationPaperBuilder(string builderId, string email, string name)
        {
            string subject = "Bienvenue dans le programme Build Up by NTF !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/signature_builder.html");
            var pdfIntegrationPaperPath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/builders/{builderId}.pdf");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                subject,
                message,
                email,
                pdfIntegrationPaperPath,
                $"{name}_fiche_integration.pdf"
            );
        }

        // Coach
        public async Task NotifyPreselectionCoach(string email, string name)
        {
            string subject = "Programme Build Up - Tu as été présélectionné(e) !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/preselection_coach.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                subject,
                message,
                email
            );
        }

        public async Task NotifyAcceptationCoach(string email)
        {
            string subject = "Build Up - Avis sur entretien de sélection Coach";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/validation_coach.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                subject,
                message,
                email
            );
        }

        public async Task NotifySignedIntegrationPaperCoach(string coachId, string email, string name)
        {
            string subject = "Bienvenue dans le programme Build Up by NTF !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/signature_coach.html");
            var pdfIntegrationPaperPath = Path.Combine(_env.ContentRootPath, $"wwwroot/pdf/coachs/{coachId}.pdf");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                subject,
                message,
                email,
                pdfIntegrationPaperPath,
                $"{name}_fiche_integration.pdf"
            );
        }

        public async Task NotifyBuilderChoosedCoach(string email)
        {
            string subject = "Build Up - Un Builder attend ta réponse !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/builder_choosed_coach.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                subject,
                message,
                email
            );
        }

        public async Task<List<CoachNotification>> GetCoachNotificationsAsync(string coachId)
        {
            return await (await _coachNotifications.FindAsync(databaseRequest =>
                databaseRequest.CoachId == coachId &&
                !databaseRequest.Seen
            )).ToListAsync();
        }

        public async Task MakeCoachNotificationReadAsync(string coachId, string notificationId)
        {
            CoachNotification notification = await GetCoachNotification(notificationId);

            if (notification == null)
            {
                throw new Exception("The notification seems to not exist anymore");
            }

            if (notification.CoachId != coachId)
            {
                throw new Exception("You are not the coach for this notifiction");
            }

            var update = Builders<CoachNotification>.Update
                .Set(databaseNotification => databaseNotification.Seen, true);

            await _coachNotifications.UpdateOneAsync(databaseNotification =>
                databaseNotification.Id == notificationId,
                update
            );
        }

        private async Task SendMailAsync(string subject, string body, string to, string attachmentPath = "", string attachmentName = "")
        {
            using var client = new SmtpClient(_mailCredentials.Server, _mailCredentials.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailCredentials.User, _mailCredentials.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(_mailCredentials.User),
                To = { to },
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                
            };

            if (!string.IsNullOrWhiteSpace(attachmentPath) && !string.IsNullOrWhiteSpace(attachmentName))
            {
                // Create  the file attachment for this email message.
                Attachment data = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                // Add time stamp information for the file.
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(attachmentPath);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachmentPath);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(attachmentPath);

                // Change the attachement name
                data.Name = attachmentName;
                
                // Add the file attachment to this email message.
                mailMessage.Attachments.Add(data);
            }

            await client.SendMailAsync(mailMessage);
        }

        private async Task<CoachNotification> GetCoachNotification(string notificationId)
        {
            var request = await _coachNotifications.FindAsync(databaseNotification =>
                databaseNotification.Id == notificationId
            );

            return await request.FirstOrDefaultAsync();
        }

        private string MessageFromHtmlFile(string path)
        {
            string message = "";

            using (StreamReader reader = File.OpenText(path))
            {
                message = reader.ReadToEnd();
            }

            return message;
        }

    }
}
