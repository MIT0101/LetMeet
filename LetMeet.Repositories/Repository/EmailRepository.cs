using LetMeet.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MimeKit.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;

namespace LetMeet.Repositories.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailRepositorySettings _mailSettings;


        public EmailRepository(IOptions<EmailRepositorySettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        //return secsess 
        //ffail returns Error
        public async Task<(ResultState state, bool isSended)> SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(recipientEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
               
                return (ResultState.Seccess, true);
                
            }
            catch (Exception ex)
            {

                return (ResultState.Error, false);

            }
            


        }
    }
}
