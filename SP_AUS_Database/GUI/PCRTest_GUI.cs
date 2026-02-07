using SP_AUS_Database.Database;

namespace SP_AUS_Database.GUI
{
    public record PCRTest_GUI
    {
        public int TestNumber { get; }
        public string PatientNumber { get; }
        public DateOnly TestDate { get; }
        public TimeOnly TestTime { get; }
        public int Workplace { get; }
        public int District { get; }
        public int Region { get; }
        public bool TestResult { get; }
        public string TestResultListDisplay { get => TestResult ? "pozitívny" : ""; }
        public string TestResultDetailDisplay { get => TestResult ? "áno" : "nie"; }
        public double TestValue { get; }
        public string TestValueDisplay { get => $"{TestValue:f2}"; }
        public string Comment { get; }

        public PCRTest_GUI(PCRTest copy)
        {
            TestNumber = copy.TestNumber;
            PatientNumber = new string(copy.PatientNumber);
            TestDate = copy.TestDate;
            TestTime = copy.TestTime;
            Workplace = copy.Workplace;
            District = copy.District;
            Region = copy.Region;
            TestResult = copy.TestResult;
            TestValue = copy.TestValue;
            Comment = new string(copy.Comment);
        }
    }
}
