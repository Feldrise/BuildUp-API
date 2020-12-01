using BuildUp.API.Models.Users;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMailCredentials _mailCredentials;

        public NotificationService(IMailCredentials mailCredentials)
        {
            _mailCredentials = mailCredentials;
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
    }
}
