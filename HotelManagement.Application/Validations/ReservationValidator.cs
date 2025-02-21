using Microsoft.AspNetCore.Http.HttpResults;

public static class ReservationValidator
{
    public static void Validate(CreateReservationDto criteria)
    {
        if (criteria.CheckOutDate <= criteria.CheckInDate)
        {
            throw new ValidationException("Check-out date must be greater than check-in date.");
        }

        if (criteria.CheckInDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ValidationException("Check-in date cannot be in the past.");
        }

        if (criteria.NumberOfGuests <= 0)
        {
            throw new ValidationException("Number of guests must be one person or greater");
        }

        if (!Enum.TryParse<GenderType>(criteria.Guest.Gender, true, out var gender))
        {
            throw new ValidationException("Invalid gender. Allowed values: Male, Female, NonBinary, Other." );
        }

        if (!Enum.TryParse<DocumentType>(criteria.Guest.DocumentType, true, out var documentType))
        {
            throw new ValidationException("Invalid document type. Allowed values: Passport, NationalID, DriverLicense.");
        }
    }
}