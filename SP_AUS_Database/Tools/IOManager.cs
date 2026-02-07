using SP_AUS_Database.Database;
using System.Globalization;

namespace SP_AUS_Database.Tools
{
    internal static class IOManager
    {
        public static void SavePersonListToCsv(string filePath, List<Person> list)
        {
            string[] _lines = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var _person = list[i];
                var _line = $"{_person.FirstName};{_person.LastName};{_person.DateOfBirth:dd.MM.yyyy};{_person.PatientNumber};";
                _lines[i] = _line;
            }

            File.WriteAllLines(filePath, _lines);
        }

        public static void SavePCRTestListToCsv(string filePath, List<PCRTest> list)
        {
            string[] _lines = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var _test = list[i];
                var _line = $"{_test.TestNumber};{_test.PatientNumber};{_test.TestDate:dd.MM.yyyy};{_test.TestTime:HH:mm:ss};" +
                    $"{_test.District};{_test.Region};{_test.Workplace};{(_test.TestResult ? "positive" : "")};{_test.TestValue};{_test.Comment};";
                _lines[i] = _line;
            }

            File.WriteAllLines(filePath, _lines);
        }

        public static List<Person> LoadPersonsFromCsv(string filePath)
        {
            var _list = new List<Person>();

            var _lines = File.ReadAllLines(filePath);

            foreach (var _line in _lines)
            {
                if (string.IsNullOrWhiteSpace(_line))
                {
                    continue;
                }

                var _split = _line.Split(';');

                if (_split.Length < 4)
                {
                    continue; // chybný riadok
                }

                var _firstName = _split[0];
                var _lastName = _split[1];
                DateOnly _dateOfBirth = DateOnly.ParseExact(_split[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                var _patientNumber = _split[3];

                _list.Add(new Person
                {
                    FirstName = _firstName,
                    LastName = _lastName,
                    DateOfBirth = _dateOfBirth,
                    PatientNumber = _patientNumber
                });
            }

            return _list;
        }

        public static List<PCRTest> LoadPCRTestsFromCsv(string filePath)
        {
            var list = new List<PCRTest>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var _split = line.Split(';');

                if (_split.Length < 9)
                {
                    continue; // chybný riadok
                }

                int _testNumber = int.Parse(_split[0]);
                var _patientNumber = _split[1];

                DateOnly _testDate = DateOnly.ParseExact(_split[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                TimeOnly _testTime = TimeOnly.ParseExact(_split[3], "HH:mm:ss", CultureInfo.InvariantCulture);

                int _district = int.Parse(_split[4]);
                int _region = int.Parse(_split[5]);
                int _workplace = int.Parse(_split[6]);

                bool _result = _split[7] == "positive";

                double _testValue = 0;

                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";
                double.TryParse(_split[8].Replace(",", "."), out _testValue);

                var _comment = _split[9];

                list.Add(new PCRTest
                {
                    TestNumber = _testNumber,
                    PatientNumber = _patientNumber,
                    TestDate = _testDate,
                    TestTime = _testTime,
                    District = _district,
                    Region = _region,
                    Workplace = _workplace,
                    TestResult = _result,
                    TestValue = _testValue,
                    Comment = ""
                });
            }

            return list;
        }
    }
}
