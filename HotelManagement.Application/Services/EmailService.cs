using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient
        {
            Host = _configuration["EmailSettings:Host"],
            Port = int.Parse(_configuration["EmailSettings:Port"]),
            EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]),
            Credentials = new NetworkCredential(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"])
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error sending email: {ex.Message}", ex);
        }
    }

    public async Task SendReservationConfirmationAsync(string email, long reservationId)
    {
        var subject = "Reservation Confirmation";
        var body = $"<h1>Your reservation with ID {reservationId} has been confirmed!</h1>";

        await SendEmailAsync(email, subject, body);
    }
}
