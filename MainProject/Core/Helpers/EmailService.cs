using System.Net;
using System.Net.Mail;

public class EmailService
{
    public async Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        string fromMail = "good11day68@gmail.com";
        string fromPassword = "kqnosgwrfumouxfj";

        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromMail);
        message.Subject = subject;
        message.To.Add(new MailAddress(recipientEmail));
        message.Body = $"<html><body> {body} </body></html>";
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587, 
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        smtpClient.Send(message);
    }
}
