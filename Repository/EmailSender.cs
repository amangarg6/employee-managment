using Microsoft.Extensions.Options;
using Project_Ecomm_App_1035_Untility;
using System.Net;
using Login_Register.Repository.IRepository;
using Login_Register.DTO_s;
using Azure.Core;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using MimeKit.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Employee_API_JWT_1035.Identity;
using Microsoft.Graph.Models;
using System.Net.Mail;
using EmailSettings = Project_Ecomm_App_1035_Untility.EmailSettings;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Login_Register.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
       
        public EmailSender(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
          

        }
        public void SendEmailAsync(Emaildto emaildto)
        {
            var email = new MimeMessage();


            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:UserNameEmail").Value));

            email.To.Add(MailboxAddress.Parse(emaildto.To));
            email.Subject = emaildto.Subject;
            var template = Directory.GetCurrentDirectory() + "\\Template\\Email.html";
            using (StreamReader stream = new StreamReader(template))
            {
                var mailText = stream.ReadToEnd();
                mailText = mailText.Replace("[senderUserName]", emaildto.SenderuserName)
                    .Replace("[receiverUserName]", emaildto.ReceiveruserName)
                    .Replace("[senderId]", emaildto.senderId)
                    .Replace("[date]", DateTime.UtcNow.ToString())
                    .Replace("[time]", DateTime.Now.ToShortTimeString())
                    .Replace("[reciverId]", emaildto.ReceiverId)
                    .Replace("[phone]", emaildto.Number)
                    .Replace("[email]", emaildto.Email);

                email.Body = new TextPart(TextFormat.Html) { Text = mailText };
            }

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSettings:PrimaryDomain").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:UserNameEmail").Value, _configuration.GetSection("EmailSettings:UserNamePassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }


    }
}

