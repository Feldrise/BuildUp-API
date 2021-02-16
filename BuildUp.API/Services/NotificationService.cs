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

namespace BuildUp.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMailCredentials _mailCredentials;

        private readonly IMongoCollection<CoachNotification> _coachNotifications;

        public NotificationService(IMongoSettings mongoSettings, IMailCredentials mailCredentials)
        {
            _mailCredentials = mailCredentials;

            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _coachNotifications = database.GetCollection<CoachNotification>("coach_notifications");
        }

        public async Task NotifieAccountCreationAsync(RegisterModel registerModel, string password)
        {
            string subject = "Inscription au program Build-up !";

            string message = "";
            message += $"Bienvenue {registerModel.FirstName} {registerModel.LastName} dans le programme Build-Up !\n";
            message += "Ta candidature va être évalué. Nous t'invitons à télécharger l'application Build-Up (https://new-talents.fr/application-buildup) pour te connecter.\n";
            message += "\n";
            message += "Nous t'avons généré un mot de passe aléatoire. Nous te conseillons de le changer dans les plus bref délais.\n";
            message += $"Voici le mot de passe généré : {password}";

            await SendMailAsync(
                subject,
                message,
                registerModel.Email
            );
        }

        public async Task NotifyPreselectionBuilder(string email, string fullName)
        {
            string subject = "Programme Build Up - Préselection";

            string message = "";
            message += $"Bonjour {fullName},\n\n";
            message += "Nous sommes ravie de t’annoncer que tu as été préselectionné pour participer au programme de coaching personnalisé Build Up 🎉. Je souhaiterais maintenant te convier à un entretien pour apprendre à te connaître et surtout mieux appréhender ta personnalité.\n\n";
            message += "Nous souhaitons voir tout simplement si le programme peut vraiment t’être utile et t’apporter les meilleures solutions possibles. Je te propose de consulter mes disponibilités à ce lien : calendly.com/marc-thomas5608/entretien-sel-builder et de sélectionner le créneau qui te convient.\n\n";
            message += "Tu n’auras plus qu’à te présenter le jour J avec un microphone fonctionnel. C’est un entretien détente, il n’y à rien à préparer. On pourrait reprendre la phrase fétiche de Mcdo, mais “viens comme tu es !” 😉.\n\n";
            message += "À très vite et si tu as des questions entre temps ou que tu souhaites changer de créneau, n’hésite pas à me contacter via ce mail ou directement sur Discord (discord.new-talents.fr).\n\n";
            message += "Amicalement";

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

        private async Task SendMailAsync(string subject, string body, string to)
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
                Body = body
            };

            await client.SendMailAsync(mailMessage);
        }

        private async Task<CoachNotification> GetCoachNotification(string notificationId)
        {
            var request = await _coachNotifications.FindAsync(databaseNotification =>
                databaseNotification.Id == notificationId
            );

            return await request.FirstOrDefaultAsync();
        }

    }
}
