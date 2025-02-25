public interface IEmailService
{
    Task SendEmailAsync(List<string> recipients, string subject, string body);
    Task SendReservationConfirmationAsync(List<GuestDto> guests, Reservation reservation);
}
