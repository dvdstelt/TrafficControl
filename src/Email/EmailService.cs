using System.Net;
using System.Net.Mail;
using Email.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorLight;

namespace Email;

public class EmailService(
    ILogger<EmailService> logger,
    IOptions<SmtpSettings> settings,
    RazorLightEngine razorEngine)
{
    private readonly SmtpSettings settings = settings.Value;

    public async Task SendEmailAsync(string to, string subject, string template, object model)
    {
        var htmlBody = await razorEngine.CompileRenderAsync(template, model);

        using var client = new SmtpClient(settings.Host, settings.Port);
        client.EnableSsl = settings.EnableSsl;
        client.Credentials = new NetworkCredential(settings.Username, settings.Password);

        using var message = new MailMessage(settings.FromAddress, to);
        message.Subject = subject;
        message.Body = htmlBody;
        message.IsBodyHtml = true;

        await client.SendMailAsync(message);
        logger.LogInformation("Email sent successfully to {To}", to);
    }
}