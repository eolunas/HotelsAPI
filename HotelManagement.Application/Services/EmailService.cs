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

    public async Task SendReservationConfirmationAsync(Guest guest, Reservation reservation)
    {
        var subject = "🎉 Your Reservation is Confirmed! 🎉";

        var body = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Reservation Confirmation</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                    text-align: center;
                }}
                .container {{
                    max-width: 600px;
                    margin: 40px auto;
                    background: #ffffff;
                    padding: 20px;
                    border-radius: 15px;
                    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
                }}
                h1 {{
                    color: #333;
                }}
                p {{
                    color: #555;
                    font-size: 16px;
                }}
                .reservation-details {{
                    background-color: #f9f9f9;
                    padding: 15px;
                    border-radius: 10px;
                    margin: 15px 0;
                    text-align: left;
                }}
                .details-label {{
                    font-weight: bold;
                    color: #333;
                }}
                .btn {{
                    display: inline-block;
                    margin-top: 20px;
                    padding: 12px 24px;
                    color: #fff;
                    background-color: #007bff;
                    text-decoration: none;
                    border-radius: 5px;
                    font-size: 16px;
                    font-weight: bold;
                }}
                .btn:hover {{
                    background-color: #0056b3;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>🎉 Reservation Confirmed! 🎉</h1>
                <p>Hello <strong>{guest.FullName}</strong>,</p>
                <p>Your reservation has been successfully confirmed. Here are the details:</p>

                <div class='reservation-details'>
                    <p><span class='details-label'>Reservation ID:</span> {reservation.Id}</p>
                    <p><span class='details-label'>Hotel:</span> {reservation.Hotel?.Name ?? "N/A"}</p>
                    <p><span class='details-label'>Room:</span> {reservation.Room?.RoomType ?? "N/A"}</p>
                    <p><span class='details-label'>Check-In Date:</span> {reservation.CheckInDate}</p>
                    <p><span class='details-label'>Check-Out Date:</span> {reservation.CheckOutDate}</p>
                    <p><span class='details-label'>Nights:</span> {reservation.Nights}</p>
                    <p><span class='details-label'>Guests:</span> {reservation.NumberOfGuests}</p>
                </div>

                <a href='' class='btn'>View Reservation</a>

                <p class='footer'>If you have any questions, please contact us at hotelmanagerapi@gmail.com.</p>
            </div>
        </body>
        </html>";

        await SendEmailAsync(guest.Email, subject, body);
    }
}
