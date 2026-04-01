using System.Net;
using System.Net.Mail;

namespace Filesharing.Helper.Mail;

public interface IMailServices
{
    Task SendMailAsync(EmailBody model);
}

public class MailServices(IConfiguration config, ILogger<MailServices> logger) : IMailServices
{
    public async Task SendMailAsync(EmailBody model)
    {
        try
        {
            var host = config.GetValue<string>("MailSettings:Host");
            var port = config.GetValue<int>("MailSettings:Port");
            var fromEmail = config.GetValue<string>("MailSettings:FromEmail") ?? string.Empty;
            var fromDisplayName = config.GetValue<string>("MailSettings:SenderName") ?? "FileSharing System";
            var password = config.GetValue<string>("MailSettings:Password") ?? string.Empty;

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true // SMTP usually needs SSL
            };

            var message = new MailMessage
            {
                Subject = model.Subject,
                Body = model.Body,
                IsBodyHtml = true,
                From = new MailAddress(fromEmail, fromDisplayName, System.Text.Encoding.UTF8)
            };
            message.To.Add(model.Email);

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", model.Email);
        }
    }
}
