using SP_AUS_Database.Database;

namespace SP_AUS_Database.Tests
{
    public class DatabaseTests
    {
        public static void GenerateData(PCRTestDatabase database, int personCnt, int testPerPerson, double positiveTestChance)
        {
            if (personCnt > 0)
            {
                for (int i = 0; i < personCnt; i++)
                {
                    bool _isFemale = Random.Shared.Next() % 2 == 0;

                    int _year = Random.Shared.Next(1950, 2020);
                    int _month = Random.Shared.Next() % 12 + 1;
                    int _day = Random.Shared.Next() % DateTime.DaysInMonth(_year, _month) + 1;

                    string _firstName = _isFemale ? GetRandomFemaleName() : GetRandomMaleName();
                    string _lastName = GetRandomLastName(_isFemale);

                    database.Task19_InsertPerson(_firstName, _lastName, new(_year, _month, _day));
                }
            }

            if (testPerPerson > 0)
            {
                var _personList = database.GetPersonList();
                foreach (var _person in _personList)
                {
                    int _tests = Random.Shared.Next() % testPerPerson + 1;
                    var _now = DateTime.Now;

                    for (int j = 0; j < _tests; j++)
                    {
                        //najviac 1 rok staré testy
                        DateOnly _testDate = DateOnly.FromDateTime(DateTime.Today);
                        _testDate = _testDate.AddDays(-Random.Shared.Next() % 365 + 1);
                        TimeOnly _testTime = new TimeOnly(Random.Shared.Next() % 24, Random.Shared.Next() % 60, Random.Shared.Next() % 60);
                        int _region = Random.Shared.Next() % 10 + 1;
                        int _district = _region * 100 + Random.Shared.Next() % 50 + 1;
                        int _workplace = _district * 100 + Random.Shared.Next() % 5 + 1;
                        bool _testResult = Random.Shared.NextDouble() < positiveTestChance;
                        //ak je test pozitívny, jeho hodnota bude z intervalu <0.1, 1>
                        double _testValue = _testResult ? Random.Shared.NextDouble() * 0.9 + 0.1 : Random.Shared.NextDouble() * 0.1;

                        database.Task1_InsertPcrTest(_person.PatientNumber, _testDate, _testTime, _workplace, _district, _region, _testResult, _testValue);
                    }
                }
            }
        }

        private static readonly List<string> MaleNames = new List<string>
        {
            "Adam", "Adrián", "Alexander", "Andrej", "Anton", "Branislav", "Daniel", "David",
            "Dominik", "Emil", "Erik", "Filip", "František", "Gabriel", "Henrich", "Igor",
            "Ivan", "Jakub", "Ján", "Jozef", "Juraj", "Karol", "Ladislav", "Lukáš", "Marián",
            "Martin", "Matúš", "Michal", "Milan", "Miroslav", "Nikolas", "Ondrej", "Patrik",
            "Peter", "Radovan", "Rastislav", "Richard", "Róbert", "Samuel", "Stanislav",
            "Štefan", "Tomáš", "Viktor", "Viliam", "Vladimír", "Vlastimil", "Zdenko"
        };

        private static readonly List<string> FemaleNames = new List<string>
        {
            "Adela", "Alena", "Alexandra", "Andrea", "Anna", "Barbora", "Beáta", "Blanka",
            "Božena", "Dajana", "Dana", "Daniela", "Denisa", "Dominika", "Diana", "Eliška",
            "Emília", "Eva", "Gabriela", "Helena", "Ivana", "Jana", "Jarmila", "Jaroslava",
            "Júlia", "Kamila", "Katarína", "Kristína", "Laura", "Lenka", "Lívia", "Lucia",
            "Magdaléna", "Mária", "Martina", "Monika", "Natália", "Nina", "Nikola", "Patrícia",
            "Petra", "Renáta", "Romana", "Simona", "Soňa", "Tatiana", "Veronika", "Viktória",
            "Zuzana"
        };

        private static readonly List<string> LastNames = new List<string>
        {
            "Novák", "Horváth", "Kováč", "Farkaš", "Varga",  "Nagy", "Baláž", "Szabó", "Molnár", "Kučera",
            "Polák", "Urban", "Bartoš", "Kollár", "Lukáč", "Šimko", "Dudáš", "Dudešek", "Hruška", "Štefánik",
            "Kováč", "Sýkora", "Kraus", "Petrák", "Hudák", "Kašpar", "Holub", "Blaho", "Černák", "Švec",
            "Bielik", "Cibulka", "Gregor", "Jurík", "Lacko", "Mach", "Pavlík", "Paško", "Richter", "Sedlák",
            "Slanina", "Smetana", "Smolík", "Straka", "Ševčík", "Šoltés", "Tóth", "Urbánek", "Valach",
            "Valent", "Vanek", "Veselý", "Vlach", "Vlček", "Volek", "Vojtek", "Zachar", "Zeman", "Žák",
            "Benko", "Bobák", "Boháč", "Božík", "Branický", "Laš", "Gajdoš", "Halás", "Hanák", "Havran",
            "Hronec", "Janík", "Karas", "Kmeť", "Kočiš", "Kozák", "Král", "Krištof", "Kružliak", "Lipták",
            "Majer", "Mlynár", "Oravec", "Pekár", "Plachý", "Rybár", "Sedláček", "Sloboda", "Sokol", "Šebesta",
            "Ševčík", "Škoda", "Turek", "Válek", "Vodička"
        };

        private static string GetRandomMaleName()
        {
            return MaleNames[Random.Shared.Next() % MaleNames.Count];
        }

        private static string GetRandomFemaleName()
        {
            return FemaleNames[Random.Shared.Next() % FemaleNames.Count];
        }

        private static string GetRandomLastName(bool isFemale = false)
        {
            string _lastName = LastNames[Random.Shared.Next() % LastNames.Count];

            if (isFemale)
            {
                if (_lastName.EndsWith("ý"))
                {
                    _lastName = _lastName.Substring(0, _lastName.Length - 1) + "á";
                }
                else if (_lastName.EndsWith("a"))
                {
                    _lastName = _lastName.Substring(0, _lastName.Length - 1) + "ová";
                }
                else if (_lastName.EndsWith("ec"))
                {
                    _lastName = _lastName.Substring(0, _lastName.Length - 2) + "cová";
                }
                else if (_lastName.EndsWith("ek"))
                {
                    _lastName = _lastName.Substring(0, _lastName.Length - 2) + "ková";
                }
                else if (_lastName.EndsWith("o"))
                {
                    _lastName += "vá";
                }
                else
                {
                    _lastName += "ová";
                }
            }

            return _lastName;
        }
    }
}
