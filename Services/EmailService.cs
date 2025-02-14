using MailKit.Net.Smtp;
using MimeKit;

namespace hki_2025_registration.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, byte[] attachment, string attachmentName)
        {
            //var emailMessage = new MimeMessage();
            //emailMessage.From.Add(new MailboxAddress("Your App Name", _configuration["EmailSettings:FromEmail"]));
            //emailMessage.To.Add(new MailboxAddress("", toEmail));
            //emailMessage.Subject = subject;

            //// Add Bcc
            //emailMessage.Bcc.Add(new MailboxAddress("", "maaaruf.osl@gmail.com"));

            //var bodyBuilder = new BodyBuilder { HtmlBody = body };
            //bodyBuilder.Attachments.Add(attachmentName, attachment);
            //emailMessage.Body = bodyBuilder.ToMessageBody();

            //using (var client = new SmtpClient())
            //{
            //    await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), true);
            //    await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
            //    await client.SendAsync(emailMessage);
            //    await client.DisconnectAsync(true);
            //}
        }
    }
}
