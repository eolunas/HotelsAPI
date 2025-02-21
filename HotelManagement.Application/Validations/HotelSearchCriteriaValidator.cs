public static class HotelSearchCriteriaValidator
{
    public static void Validate(HotelSearchCriteriaDto criteria)
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
    }
}
