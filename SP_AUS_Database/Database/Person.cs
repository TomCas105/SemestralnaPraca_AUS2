namespace SP_AUS_Database.Database
{
    public record Person
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateOnly DateOfBirth { get; set; } = default;
        public string PatientNumber { get; set; } = "";
    }
}
