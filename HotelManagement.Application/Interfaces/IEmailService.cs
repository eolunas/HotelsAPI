public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string body);
    Task SendReservationConfirmationAsync(string email, long reservationId);
}
