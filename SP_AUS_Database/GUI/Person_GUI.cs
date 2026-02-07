using SP_AUS_Database.Database;

namespace SP_AUS_Database.GUI
{
    public record Person_GUI
    {
        public string FirstName { get; }
        public string LastName { get; }
        public DateOnly DateOfBirth { get; }
        public string PatientNumber { get;}

        public Person_GUI(Person copy)
        {
            FirstName = new string(copy.FirstName);
            LastName = new string(copy.LastName);
            DateOfBirth = copy.DateOfBirth;
            PatientNumber = new string(copy.PatientNumber);
        }
    }
}
