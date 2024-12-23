public static class HotelSearchCriteriaValidator
{
    public static void Validate(HotelSearchCriteriaDto criteria)
    {
        if (criteria.CheckOutDate < criteria.CheckInDate)
        {
            throw new ValidationException("Check-out date must be greater than or equal to check-in date.");
        }

        if (criteria.CheckInDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ValidationException("Check-in date cannot be in the past.");
        }
    }
}
