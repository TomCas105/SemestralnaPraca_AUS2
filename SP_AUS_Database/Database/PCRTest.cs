namespace SP_AUS_Database.Database
{
    public record PCRTest
    {
        public int TestNumber { get; set; } = 0;
        public string PatientNumber { get; set; } = "";
        public DateOnly TestDate { get; set; } = default;
        public TimeOnly TestTime { get; set; } = default;
        public int Workplace { get; set; } = 0; //kód pracoviska
        public int District { get; set; } = 0; //kód okresu
        public int Region { get; set; } = 0; //kód kraja
        public bool TestResult { get; set; } = false;
        public double TestValue { get; set; } = 0;
        public string Comment { get; set; } = "";
    }
}
