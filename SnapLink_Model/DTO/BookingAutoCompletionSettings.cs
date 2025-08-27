namespace SnapLink_Model.DTO
{
    public class BookingAutoCompletionSettings
    {
        public int CheckIntervalHours { get; set; } = 1;
        public int DaysAfterBookingEnd { get; set; } = 3;
        public bool Enabled { get; set; } = true;
    }
}
