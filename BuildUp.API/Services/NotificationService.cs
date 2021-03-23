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
        private readonly IMongoCollection<BuilderNotification> _builderNotifications;
        private readonly IMongoCollection<Coach> _coachs;
        private readonly IMongoCollection<Builder> _builders;

        readonly IWebHostEnvironment _env;

        private const string OrigineCoach = "coach";
        private const string OrigineBuilder = "builder";

        public NotificationService(IMongoSettings mongoSettings, IMailCredentials mailCredentials, IWebHostEnvironment env)
        {
            _mailCredentials = mailCredentials;
            _env = env;

            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _coachNotifications = database.GetCollection<CoachNotification>("coach_notifications");
            _builderNotifications = database.GetCollection<BuilderNotification>("builder_notifications");
            _coachs = database.GetCollection<Coach>("coachs");
            _builders = database.GetCollection<Builder>("builders");
        }

        public async Task NotifieAccountCreationAsync(RegisterModel registerModel, string password)
        {
            string subject = "Inscription au programme Build Up !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/reception_candidature.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$fullName", registerModel.FirstName);
            message = message.Replace("$builderOrCoach", registerModel.Role == Role.Builder ? "Builder" : "Coach");
            message = message.Replace("$password", password);

            await SendMailAsync(
                registerModel.Role == Role.Coach ? OrigineCoach : OrigineBuilder,
                subject,
                message,
                registerModel.Email
            );

            if (registerModel.Role == Role.Builder)
            {
                await NotifyBuilderCandidating();
            }
            else if (registerModel.Role == Role.Coach)
            {
                await NotifyCoachCandidating();
            }
        }

        // Builders
        public async Task NotifyPreselectionBuilder(string email, string name)
        {
            string subject = "Build Up - Tu as été présélectionné(e) !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/preselection_builder.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                OrigineBuilder,
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
                OrigineBuilder,
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
                OrigineBuilder,
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
                OrigineBuilder,
                subject,
                message,
                email,
                pdfIntegrationPaperPath,
                $"{name}_fiche_integration.pdf"
            );

            await NotifyBuilderIntegrationSucess();
        }
        public async Task NotifyRefusedBuilder(string email, string name)
        {
            string subject = "Build Up - Avis sur entretien de sélection Builder";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/refus_candidature_builder.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                OrigineBuilder,
                subject,
                message,
                email
            );
        }

        // Coach
        public async Task NotifyPreselectionCoach(string email, string name)
        {
            string subject = "Build Up - Tu as été présélectionné(e) !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/preselection_coach.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                OrigineCoach,
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
                OrigineCoach,
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
                OrigineCoach,
                subject,
                message,
                email,
                pdfIntegrationPaperPath,
                $"{name}_fiche_integration.pdf"
            );

            await NotifyCoachIntegrationSucess();
        }

        public async Task NotifyBuilderChoosedCoach(string email)
        {
            string subject = "Build Up - Un Builder attend ta réponse !";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/builder_choosed_coach.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineCoach,
                subject,
                message,
                email
            );
        }

        public async Task NotifyRefusedCoach(string email, string name)
        {
            string subject = "Build Up - Avis sur entretien de sélection Coach";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/refus_candidature_coach.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", name);

            await SendMailAsync(
                OrigineCoach,
                subject,
                message,
                email
            );
        }

        // Build-Ons
        public async Task NotifyBuildOnReturningSubmited(string coachId, string coachMail)
        {
            string subject = "Build Up - Ton Builder a terminé son étape ! ";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/buildon_returning_submited.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineCoach,
                subject,
                message,
                coachMail
            );

            await CreateCoachNotification(coachId, "Votre Builder vient de soumettre une nouvelle étape !");
        }

        public async Task NotifyBuildonStepValidated(string builderId, string builderMail)
        {
            string subject = "Build Up - Ton étape vient d’être validée ! ";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/validation_buildon_step.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineBuilder,
                subject,
                message,
                builderMail
            );

            await CreateBuilderNotification(builderId, "L'étape a été accepté ! On passe à la suite 🎉");
        }

        public async Task NotifyBuildOnReturningRefusedByCoach(string builderMail, string builderName, string reason)
        {
            string subject = "Build Up - Ton Coach n’a pas accepté ton étape ! 😕";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/buildon_step_refused_coach.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", builderName);
            message = message.Replace("$reason", reason);

            await SendMailAsync(
                OrigineBuilder,
                subject,
                message,
                builderMail
            );
        }

        public async Task NotifyBuildOnReturningRefusedByAdmin(string builderMail, string builderName, string reason)
        {
            string subject = "Build Up - Un responsable Builders n’a pas accepté ton étape ! 😕";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/buildon_step_refused_admin.html");

            string message = MessageFromHtmlFile(htmlPath);
            message = message.Replace("$name", builderName);
            message = message.Replace("$reason", reason);

            await SendMailAsync(
                OrigineBuilder,
                subject,
                message,
                builderMail
            );
        }


        // Admin
        public async Task NotifyBuilderCandidating()
        {
            string subject = "Notif - Une nouvelle candidature Builder est disponible ! ";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/admin/candidature_builder.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineBuilder,
                subject,
                message,
                "builder@new-talents.fr"
            );
        }
        
        public async Task NotifyCoachCandidating()
        {
            string subject = "Notif - Une nouvelle candidature Coach est disponible ! ";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/admin/candidature_coach.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineCoach,
                subject,
                message,
                "coach@new-talents.fr"
            );
        }

        public async Task NotifyBuilderIntegrationSucess()
        {
            string subject = "Notif - Un Builder a signé sa fiche d’intégration ! ";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/admin/builder_integration_success.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineBuilder,
                subject,
                message,
                "builder@new-talents.fr"
            );
        }
        public async Task NotifyCoachIntegrationSucess()
        {
            string subject = "Notif - Un Coach a signé sa fiche d’intégration ! ";

            var htmlPath = Path.Combine(_env.ContentRootPath, $"Emails/html/admin/coach_integration_success.html");

            string message = MessageFromHtmlFile(htmlPath);

            await SendMailAsync(
                OrigineCoach,
                subject,
                message,
                "coach@new-talents.fr"
            );
        }

        // News
        public async Task CreateCoachNotification(string coachId, string content)
        {
            CoachNotification notification = new CoachNotification()
            {
                CoachId = coachId,
                Date = DateTime.Now,
                Content = content,

                Seen = false
            };

            await _coachNotifications.InsertOneAsync(notification);
        }

        public async Task<List<CoachNotification>> GetCoachNotificationsAsync(string coachId)
        {
            return await (await _coachNotifications.FindAsync(databaseNotification =>
                databaseNotification.CoachId == coachId &&
                !databaseNotification.Seen,
                new FindOptions<CoachNotification>()
                {
                    Sort = Builders<CoachNotification>.Sort.Descending("Date")
                }
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
                throw new UnauthorizedAccessException("You are not the coach for this notifiction");
            }

            var update = Builders<CoachNotification>.Update
                .Set(databaseNotification => databaseNotification.Seen, true);

            await _coachNotifications.UpdateOneAsync(databaseNotification =>
                databaseNotification.Id == notificationId,
                update
            );
        }

        public async Task CreateBuilderNotification(string builderId, string content)
        {
            BuilderNotification notification = new BuilderNotification()
            {
                BuilderId = builderId,
                Date = DateTime.Now,
                Content = content,

                Seen = false
            };

            await _builderNotifications.InsertOneAsync(notification);
        }

        public async Task<List<BuilderNotification>> GetBuilderNotificationsAsync(string builderId)
        {
            return await (await _builderNotifications.FindAsync(databaseNotification =>
                databaseNotification.BuilderId == builderId && !databaseNotification.Seen,
                new FindOptions<BuilderNotification>()
                {
                    Sort = Builders<BuilderNotification>.Sort.Descending("Date")
                }
            )).ToListAsync();
        }

        public async Task MakeBuilderNotificationReadAsync(string builderId, string notificationId)
        {
            BuilderNotification notification = await GetBuilderNotification(notificationId);

            if (notification == null)
            {
                throw new Exception("The notification seems to not exist anymore");
            }

            if (notification.BuilderId != builderId)
            {
                throw new UnauthorizedAccessException("You are not the builder for this notifiction");
            }

            var update = Builders<BuilderNotification>.Update
                .Set(databaseNotification => databaseNotification.Seen, true);

            await _builderNotifications.UpdateOneAsync(databaseNotification =>
                databaseNotification.Id == notificationId,
                update
            );
        }

        public async Task NotifyAllAsync(string content)
        {
            List<Builder> builders = await (await _builders.FindAsync(dbBuilder => true)).ToListAsync();
            List<Coach> coachs = await (await _coachs.FindAsync(dbCoach => true)).ToListAsync();

            List<BuilderNotification> builderNotifications = new List<BuilderNotification>();
            List<CoachNotification> coachNotifications = new List<CoachNotification>();

            foreach (var builder in builders)
            {
                BuilderNotification notification = new BuilderNotification()
                {
                    BuilderId = builder.Id,
                    Date = DateTime.Now,
                    Content = content,
    
                    Seen = false
                };

                builderNotifications.Add(notification);
            }

            foreach (var coach in coachs)
            {
                CoachNotification notification = new CoachNotification()
                {
                    CoachId = coach.Id,
                    Date = DateTime.Now,
                    Content = content,
    
                    Seen = false
                };

                coachNotifications.Add(notification);
            }

            await _builderNotifications.InsertManyAsync(builderNotifications);
            await _coachNotifications.InsertManyAsync(coachNotifications);
        }

        private async Task SendMailAsync(string origine, string subject, string body, string to, string attachmentPath = "", string attachmentName = "")
        {
            using var client = new SmtpClient(_mailCredentials.Server, _mailCredentials.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    origine == OrigineCoach ? _mailCredentials.CoachUser : _mailCredentials.BuilderUser,
                    origine == OrigineCoach ? _mailCredentials.CoachPassword : _mailCredentials.BuilderPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(origine == OrigineCoach ? _mailCredentials.CoachUser : _mailCredentials.BuilderUser),
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
            var notification = await _coachNotifications.FindAsync(databaseNotification =>
                databaseNotification.Id == notificationId
            );

            return await notification.FirstOrDefaultAsync();
        }

        private async Task<BuilderNotification> GetBuilderNotification(string notificationId)
        {
            var notification = await _builderNotifications.FindAsync(databaseNotification =>
                databaseNotification.Id == notificationId
            );

            return await notification.FirstOrDefaultAsync();
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
