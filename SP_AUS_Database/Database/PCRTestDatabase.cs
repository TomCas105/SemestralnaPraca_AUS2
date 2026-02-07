using SP_AUS_Database.GUI;
using SP_AUS_Database.Tools;
using SP_AUS_Lib.Structures;

namespace SP_AUS_Database.Database
{
    public class PCRTestDatabase
    {
        private AVLTree<string, Person> personTree_by_PatientNumber; //19
        private AVLTree<int, PCRTest> pcrTestTree_by_TestNumber; //1, 18
        private AVLTree<string, AVLTree<int, PCRTest>> pcrTestTree_by_PatientNumber_TestNumber; //2
        private AVLTree<string, AVLTree<DateOnly, AVLTree<TimeOnly, PCRTest>>> pcrTestTree_by_PatientNumber_TestDate_TestTime; //3
        private AVLTree<int, AVLTree<DateOnly, AVLTree<int, PCRTest>>> pcrTestTree_Positive_by_District_TestDate_TestNumber; //4, 10, 11, 14, 15
        private AVLTree<int, AVLTree<DateOnly, AVLTree<int, PCRTest>>> pcrTestTree_by_District_TestDate_TestNumber; //5
        private AVLTree<int, AVLTree<DateOnly, AVLTree<int, PCRTest>>> pcrTestTree_Positive_by_Region_TestDate_TestNumber; //6, 12, 16
        private AVLTree<int, AVLTree<DateOnly, AVLTree<int, PCRTest>>> pcrTestTree_by_Region_TestDate_TestNumber; //7
        private AVLTree<DateOnly, AVLTree<int, PCRTest>> pcrTestTree_Positive_by_TestDate_TestNumber; //8, 13
        private AVLTree<DateOnly, AVLTree<int, PCRTest>> pcrTestTree_by_TestDate_TestNumber; //9
        private AVLTree<int, AVLTree<DateOnly, AVLTree<int, PCRTest>>> pcrTestTree_by_Workplace_TestDate_TestNumber;//17

        private AVLTree<int, int> districtTree;
        private AVLTree<int, int> regionTree;

        public PCRTestDatabase()
        {
            personTree_by_PatientNumber = new();
            pcrTestTree_by_PatientNumber_TestNumber = new();
            pcrTestTree_by_PatientNumber_TestDate_TestTime = new();
            pcrTestTree_by_TestNumber = new();
            pcrTestTree_by_TestDate_TestNumber = new();
            pcrTestTree_Positive_by_District_TestDate_TestNumber = new();
            pcrTestTree_by_District_TestDate_TestNumber = new();
            pcrTestTree_Positive_by_Region_TestDate_TestNumber = new();
            pcrTestTree_by_Region_TestDate_TestNumber = new();
            pcrTestTree_Positive_by_TestDate_TestNumber = new();
            pcrTestTree_by_Workplace_TestDate_TestNumber = new();
            districtTree = new();
            regionTree = new();
        }

        public List<Person_GUI> GetPersonList()
        {
            List<Person_GUI> _result = new();

            personTree_by_PatientNumber.InOrderTraversal().ForEach(_person => _result.Add(new(_person)));

            return _result;
        }

        public List<PCRTest_GUI> GetPCRTestList()
        {
            List<PCRTest_GUI> _result = new();

            pcrTestTree_by_TestNumber.InOrderTraversal().ForEach(_test => _result.Add(new(_test)));

            return _result;
        }

        public void ClearAll()
        {
            personTree_by_PatientNumber.Clear();
            pcrTestTree_by_PatientNumber_TestNumber.Clear();
            pcrTestTree_by_PatientNumber_TestDate_TestTime.Clear();
            pcrTestTree_by_TestNumber.Clear();
            pcrTestTree_by_TestDate_TestNumber.Clear();
            pcrTestTree_Positive_by_District_TestDate_TestNumber.Clear();
            pcrTestTree_by_District_TestDate_TestNumber.Clear();
            pcrTestTree_Positive_by_Region_TestDate_TestNumber.Clear();
            pcrTestTree_by_Region_TestDate_TestNumber.Clear();
            pcrTestTree_Positive_by_TestDate_TestNumber.Clear();
            pcrTestTree_by_Workplace_TestDate_TestNumber.Clear();
            districtTree.Clear();
            regionTree.Clear();
        }

        public void ImportPersons(string filePath)
        {
            var _list = IOManager.LoadPersonsFromCsv(filePath);
            _list.ForEach(x =>
            personTree_by_PatientNumber.Insert(x.PatientNumber, new Person()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                PatientNumber = x.PatientNumber
            }));
        }

        public void ImportPCRTests(string filePath)
        {
            var _list = IOManager.LoadPCRTestsFromCsv(filePath);
            _list.ForEach(x => InsertTest(new PCRTest()
            {
                TestNumber = x.TestNumber,
                PatientNumber = x.PatientNumber,
                TestDate = x.TestDate,
                TestTime = x.TestTime,
                District = x.District,
                Region = x.Region,
                Workplace = x.Workplace,
                TestResult = x.TestResult,
                TestValue = x.TestValue,
                Comment = x.Comment
            }));
        }

        public void ExportPersons(string filePath)
        {
            IOManager.SavePersonListToCsv(filePath, personTree_by_PatientNumber.LevelOrderTraversal());
        }

        public void ExportPCRTests(string filePath)
        {
            IOManager.SavePCRTestListToCsv(filePath, pcrTestTree_by_TestNumber.LevelOrderTraversal());
        }

        /// <summary>
        /// Vloženie výsledku PCR testu do systému.
        /// </summary>
        public bool Task1_InsertPcrTest(string patientNumber, DateOnly testDate, TimeOnly testTime, int workplace, int district, int region, bool testResult, double testValue, string comment = "")
        {
            // vygenerovanie unikátneho kľúča
            int _key = Random.Shared.Next();
            while (pcrTestTree_by_TestNumber.Find(_key, out _))
            {
                _key = Random.Shared.Next();
            }

            PCRTest test = new PCRTest()
            {
                TestNumber = _key,
                PatientNumber = patientNumber,
                TestDate = testDate,
                TestTime = testTime,
                Workplace = workplace,
                District = district,
                Region = region,
                TestResult = testResult,
                TestValue = testValue,
                Comment = comment
            };

            return InsertTest(test);
        }

        /// <summary>
        /// Vyhľadanie výsledku testu (definovaný kódom PCR testu) pre pacienta (definovaný unikátnym číslom pacienta) a zobrazenie všetkých údajov.
        /// </summary>
        public PCRTest_GUI? Task2_GetTestOfPerson(string patientNumber, int testNumber)
        {
            if (!pcrTestTree_by_PatientNumber_TestNumber.Find(patientNumber, out var _testNumber) || _testNumber == null)
            {
                return null;
            }

            if (_testNumber.Find(testNumber, out var _test) && _test != null)
            {
                return new(_test);
            }

            return null;
        }

        /// <summary>
        /// Výpis všetkých uskutočnených PCR testov pre daného pacienta (definovaný unikátnym číslom pacienta) usporiadaných podľa dátumu a času ich vykonania
        /// </summary>
        public List<PCRTest_GUI> Task3_GetPersonTests(string patientNumber)
        {
            List<PCRTest_GUI> _result = new();


            if (!pcrTestTree_by_PatientNumber_TestDate_TestTime.Find(patientNumber, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            var _dateInOrder = _treeDate.InOrderTraversal();

            foreach (var _treeTime in _dateInOrder)
            {
                _treeTime.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých pozitívnych testov uskutočnených za zadané časové obdobie pre zadaný okres(definovaný kódom okresu).
        /// </summary>
        public List<PCRTest_GUI> Task4_GetPositiveTestsInDistrictBetweenDates(int district, DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            if (!pcrTestTree_Positive_by_District_TestDate_TestNumber.Find(district, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            var _findList = _treeDate.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých testov uskutočnených za zadané časové obdobie pre zadaný okres(definovaný kódom okresu).
        /// </summary>
        public List<PCRTest_GUI> Task5_GetTestsInDistrictBetweenDates(int district, DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            if (!pcrTestTree_by_District_TestDate_TestNumber.Find(district, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            var _findList = _treeDate.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých pozitívnych testov uskutočnených za zadané časové obdobie pre zadaný okres(definovaný kódom okresu).
        /// </summary>
        public List<PCRTest_GUI> Task6_GetPositiveTestsInRegionBetweenDates(int region, DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            if (!pcrTestTree_Positive_by_Region_TestDate_TestNumber.Find(region, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            var _findList = _treeDate.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých testov uskutočnených za zadané časové obdobie pre zadaný okres(definovaný kódom okresu).
        /// </summary>
        public List<PCRTest_GUI> Task7_GetTestsInRegionBetweenDates(int region, DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            if (!pcrTestTree_by_Region_TestDate_TestNumber.Find(region, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            var _findList = _treeDate.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých pozitívnych testov uskutočnených za zadané časové obdobie.
        /// </summary>
        public List<PCRTest_GUI> Task8_GetPositiveTestsBetweenDates(DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            var _findList = pcrTestTree_Positive_by_TestDate_TestNumber.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých testov uskutočnených za zadané časové obdobie.
        /// </summary>
        public List<PCRTest_GUI> Task9_GetTestsBetweenDates(DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            var _findList = pcrTestTree_by_TestDate_TestNumber.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Výpis všetkých pozitívnych testov uskutočnených za zadané časové obdobie pre zadaný okres(definovaný kódom okresu).
        /// </summary>
        public List<Person_GUI> Task10_GetSickPersonsInDistrictToDate(int district, DateOnly date, int sickDays)
        {
            AVLTree<string, Person> _treePerson = new();
            List<Person_GUI> _result = new();

            if (!pcrTestTree_Positive_by_District_TestDate_TestNumber.Find(district, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            DateOnly _dateStart = date.AddDays(-sickDays);
            var _findList = _treeDate.Find(date.AddDays(-sickDays), date);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x =>
                {
                    if (personTree_by_PatientNumber.Find(x.PatientNumber, out var _person) && _person != null)
                    {
                        _treePerson.Insert(_person.PatientNumber, _person);
                    }
                });
            }

            _treePerson.InOrderTraversal().ForEach(x =>
            {
                _result.Add(new(x));
            });

            return _result;
        }

        /// <summary>
        /// Výpis všetkých pozitívnych testov uskutočnených za zadané časové obdobie pre zadaný okres(definovaný kódom okresu),
        /// pričom choré osoby sú usporiadané podľa hodnoty testu
        public List<Person_GUI> Task11_GetSickPersonsInDistrictToDate_OrderBy_TestValue(int district, DateOnly date, int sickDays)
        {
            AVLTree<string, (Person_GUI, double)> _treePerson = new();
            List<Person_GUI> _result = new();

            if (!pcrTestTree_Positive_by_District_TestDate_TestNumber.Find(district, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            DateOnly _dateStart = date.AddDays(-sickDays);
            var _findList = _treeDate.Find(date.AddDays(-sickDays), date);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x =>
                {
                    if (personTree_by_PatientNumber.Find(x.PatientNumber, out var _person) && _person != null)
                    {
                        if (!_treePerson.Find(x.PatientNumber, out var existing))
                        {
                            _treePerson.Insert(x.PatientNumber, (new(_person), x.TestValue));
                        }
                        else if (x.TestValue > existing.Item2)
                        {
                            existing.Item2 = x.TestValue;
                        }
                    }
                });
            }

            _result = _treePerson.InOrderTraversal().OrderBy(x => x.Item2).Select(x => x.Item1).ToList();

            return _result;
        }

        /// <summary>
        /// Výpis všetkých pozitívnych testov uskutočnených za zadané časové obdobie pre zadaný kraj(definovaný kódom okresu).
        /// </summary>
        public List<Person_GUI> Task12_GetSickPersonsInRegionToDate(int region, DateOnly date, int sickDays)
        {
            AVLTree<string, Person> _treePerson = new();
            List<Person_GUI> _result = new();

            if (!pcrTestTree_Positive_by_Region_TestDate_TestNumber.Find(region, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            DateOnly _dateStart = date.AddDays(-sickDays);
            var _findList = _treeDate.Find(date.AddDays(-sickDays), date);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x =>
                {
                    if (personTree_by_PatientNumber.Find(x.PatientNumber, out var _person) && _person != null)
                    {
                        _treePerson.Insert(_person.PatientNumber, _person);
                    }
                });
            }

            _treePerson.InOrderTraversal().ForEach(x =>
            {
                _result.Add(new(x));
            });

            return _result;
        }

        /// <summary>
        /// Výpis chorých osôb k zadanému dátumu, pričom osobu považujeme za chorú X dní od pozitívneho testu (X zadá užívateľ).
        /// </summary>
        public List<Person_GUI> Task13_GetSickPersonsToDate(DateOnly date, int sickDays)
        {
            AVLTree<string, bool> _treePerson = new();
            List<Person_GUI> _result = new();

            DateOnly _dateStart = date.AddDays(-sickDays);
            var _findList = pcrTestTree_Positive_by_TestDate_TestNumber.Find(date.AddDays(-sickDays), date);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x =>
                {
                    if (personTree_by_PatientNumber.Find(x.PatientNumber, out var _person) && _person != null)
                    {
                        if (_treePerson.Insert(_person.PatientNumber, true))
                        {
                            _result.Add(new(_person));
                        }
                    }
                });
            }

            return _result;
        }

        /// <summary>
        /// Výpis jednej chorej osoby k zadanému dátumu, pričom osobu považujeme za chorú X dní od pozitívneho testu (X zadá užívateľ)
        /// z každého okresu, ktorá má najvyššiu hodnotu testu.
        /// </summary>
        public List<Person_GUI> Task14_GetTopSickPersonsPerDistrict(DateOnly date, int sickDays)
        {
            List<Person_GUI> _result = new();

            DateOnly _dateStart = date.AddDays(-sickDays);

            foreach (var _treeDate in pcrTestTree_Positive_by_District_TestDate_TestNumber.InOrderTraversal())
            {
                PCRTest? _topTest = null;

                var _findList = _treeDate.Find(date.AddDays(-sickDays), date);

                foreach (var _treeNumber in _findList)
                {
                    _treeNumber.InOrderTraversal().ForEach(x =>
                    {
                        if (_topTest == null || _topTest.TestValue < x.TestValue)
                        {
                            _topTest = x;
                        }
                    });
                }

                if (_topTest != null && FindPerson(_topTest.PatientNumber, out var _person) && _person != null)
                {
                    _result.Add(new(_person));
                }
            }

            return _result;
        }

        /// <summary>
        /// Výpis okresov usporiadaných podľa počtu chorých osôb k zadanému dátumu, pričom osobu považujeme za chorú X dní od pozitívneho testu (X zadá užívateľ).
        /// </summary>
        public List<Area_GUI> Task15_GetDistrictBySickCount(DateOnly date, int sickDays)
        {
            List<Area_GUI> _result = new();

            foreach (var _district in districtTree.InOrderTraversal())
            {
                AVLTree<string, bool> _regionSickPersons = new();

                if (pcrTestTree_Positive_by_District_TestDate_TestNumber.Find(_district, out var _treeDate) && _treeDate != null)
                {
                    var _findList = _treeDate.Find(date.AddDays(-sickDays), date);

                    foreach (var _treeNumber in _findList)
                    {
                        _treeNumber.InOrderTraversal().ForEach(x =>
                        {
                            _regionSickPersons.Insert(x.PatientNumber, true);
                        });
                    }
                }

                _result.Add(new()
                {
                    ID = _district,
                    SickCount = _regionSickPersons.Count
                });
            }

            return _result.OrderBy(x => x.SickCount).ToList();
        }

        /// <summary>
        /// Výpis krajov usporiadaných podľa počtu chorých osôb k zadanému dátumu, pričom osobu považujeme za chorú X dní od pozitívneho testu (X zadá užívateľ).
        /// </summary>
        public List<Area_GUI> Task16_GetRegionBySickCount(DateOnly date, int sickDays)
        {
            List<Area_GUI> _result = new();

            foreach (var _region in regionTree.InOrderTraversal())
            {
                AVLTree<string, bool> _regionSickPersons = new();

                if (pcrTestTree_Positive_by_Region_TestDate_TestNumber.Find(_region, out var _treeDate) && _treeDate != null)
                {
                    var _findList = _treeDate.Find(date.AddDays(-sickDays), date);

                    foreach (var _treeNumber in _findList)
                    {
                        _treeNumber.InOrderTraversal().ForEach(x =>
                        {
                            _regionSickPersons.Insert(x.PatientNumber, true);
                        });
                    }
                }

                _result.Add(new()
                {
                    ID = _region,
                    SickCount = _regionSickPersons.Count
                });
            }

            return _result.OrderBy(x => x.SickCount).ToList();
        }

        /// <summary>
        /// Výpis všetkých testov uskutočnených za zadané časové obdobie na danom pracovisku (definované kódom pracoviska).
        public List<PCRTest_GUI> Task17_GetTestsInDistrictBetweenDates(int workplace, DateOnly from, DateOnly to)
        {
            List<PCRTest_GUI> _result = new();

            if (!pcrTestTree_by_Workplace_TestDate_TestNumber.Find(workplace, out var _treeDate) || _treeDate == null)
            {
                return _result;
            }

            var _findList = _treeDate.Find(from, to);

            foreach (var _treeNumber in _findList)
            {
                _treeNumber.InOrderTraversal().ForEach(x => _result.Add(new(x)));
            }

            return _result;
        }

        /// <summary>
        /// Vyhľadanie PCR testu podľa jeho kódu.
        /// </summary>
        public PCRTest_GUI? Task18_GetTestByNumber(int testNumber)
        {
            if (pcrTestTree_by_TestNumber.Find(testNumber, out var _test) && _test != null)
            {
                return new(_test);
            }

            return null;
        }

        /// <summary>
        /// Vloženie osoby do systému.
        /// </summary>
        public bool Task19_InsertPerson(string firstName, string lastName, DateOnly dateOfBirth, string patientNumber = "")
        {
            string _patientNumber = patientNumber != "" ? patientNumber : GetPatientNumberFromBirthDate(dateOfBirth);

            Person? _person = new Person()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                PatientNumber = _patientNumber
            };

            if (personTree_by_PatientNumber.Insert(_patientNumber, _person))
            {
                return true;
            }
            else if (personTree_by_PatientNumber.Find(_patientNumber, out _person) && _person != null
                && (string.IsNullOrEmpty(_person.FirstName) || string.IsNullOrEmpty(_person.LastName)))
            {
                _person.FirstName = firstName;
                _person.LastName = lastName;
                _person.DateOfBirth = dateOfBirth;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Trvalé a nevratné vymazanie výsledku PCR testu (napr. po chybnom vložení), test je definovaných svojim kódom.
        /// </summary>
        public bool Task20_DeleteTest(int testNumber)
        {
            if (pcrTestTree_by_TestNumber.Find(testNumber, out var _test) && _test != null)
            {
                return RemoveTest(_test);
            }
            return false;
        }

        /// <summary>
        /// Vymazanie osoby zo systému (definovaná unikátnym číslom pacienta) aj s jej výsledkami PCR testov.
        /// </summary>
        public bool Task21_DeletePerson(string patientNumber)
        {
            if (!personTree_by_PatientNumber.Find(patientNumber, out var _person) || _person == null)
            {
                return false;
            }

            if (!pcrTestTree_by_PatientNumber_TestNumber.Find(patientNumber, out var _treeTest) || _treeTest == null)
            {
                return false;
            }

            _treeTest.InOrderTraversal().ForEach(x =>
            {
                RemoveTest(x);
            });

            return personTree_by_PatientNumber.Delete(patientNumber);
        }

        private bool InsertTest(PCRTest pcrTest)
        {
            if (!FindPerson(pcrTest.PatientNumber, out var _person))
            {
                _person = new Person()
                {
                    DateOfBirth = pcrTest.TestDate,
                    PatientNumber = pcrTest.PatientNumber
                };

                personTree_by_PatientNumber.Insert(pcrTest.PatientNumber, _person);
            }

            string _patientNumber = pcrTest.PatientNumber;
            int _testNumber = pcrTest.TestNumber;
            DateOnly _testDate = pcrTest.TestDate;
            TimeOnly _testTime = pcrTest.TestTime;
            int _district = pcrTest.District;
            int _region = pcrTest.Region;
            int _workplace = pcrTest.Workplace;
            bool _result = pcrTest.TestResult;

            if (!pcrTestTree_by_TestNumber.Insert(_testNumber, pcrTest))
            {
                return false; //test sa už nachádza v databáze
            }

            InsertToNestedTree(pcrTestTree_by_TestDate_TestNumber, _testDate, _testNumber, pcrTest);
            InsertToNestedTree(pcrTestTree_by_PatientNumber_TestNumber, _patientNumber, _testNumber, pcrTest);

            InsertToDoubleNestedTree(pcrTestTree_by_PatientNumber_TestDate_TestTime, _patientNumber, _testDate, _testTime, pcrTest);
            InsertToDoubleNestedTree(pcrTestTree_by_District_TestDate_TestNumber, _district, _testDate, _testNumber, pcrTest);
            InsertToDoubleNestedTree(pcrTestTree_by_Region_TestDate_TestNumber, _region, _testDate, _testNumber, pcrTest);
            InsertToDoubleNestedTree(pcrTestTree_by_Workplace_TestDate_TestNumber, _workplace, _testDate, _testNumber, pcrTest);

            if (_result)
            {
                InsertToNestedTree(pcrTestTree_Positive_by_TestDate_TestNumber, _testDate, _testNumber, pcrTest);
                InsertToDoubleNestedTree(pcrTestTree_Positive_by_District_TestDate_TestNumber, _district, _testDate, _testNumber, pcrTest);
                InsertToDoubleNestedTree(pcrTestTree_Positive_by_Region_TestDate_TestNumber, _region, _testDate, _testNumber, pcrTest);
            }

            districtTree.Insert(_district, _district);
            regionTree.Insert(_region, _region);

            return true;
        }

        private bool RemoveTest(PCRTest pcrTest)
        {
            string _patientNumber = pcrTest.PatientNumber;
            int _testNumber = pcrTest.TestNumber;
            DateOnly _testDate = pcrTest.TestDate;
            TimeOnly _testTime = pcrTest.TestTime;
            int _district = pcrTest.District;
            int _region = pcrTest.Region;
            int _workplace = pcrTest.Workplace;
            bool _result = pcrTest.TestResult;

            if (!pcrTestTree_by_TestNumber.Delete(_testNumber))
            {
                return false; //test sa nenachádza v databáze
            }

            RemoveFromNestedTree(pcrTestTree_by_TestDate_TestNumber, _testDate, _testNumber);
            RemoveFromNestedTree(pcrTestTree_by_PatientNumber_TestNumber, _patientNumber, _testNumber);

            RemoveFromDoubleNestedTree(pcrTestTree_by_PatientNumber_TestDate_TestTime, _patientNumber, _testDate, _testTime);
            RemoveFromDoubleNestedTree(pcrTestTree_by_District_TestDate_TestNumber, _district, _testDate, _testNumber);
            RemoveFromDoubleNestedTree(pcrTestTree_by_Region_TestDate_TestNumber, _region, _testDate, _testNumber);
            RemoveFromDoubleNestedTree(pcrTestTree_by_Workplace_TestDate_TestNumber, _workplace, _testDate, _testNumber);

            if (_result)
            {
                RemoveFromNestedTree(pcrTestTree_Positive_by_TestDate_TestNumber, _testDate, _testNumber);
                RemoveFromDoubleNestedTree(pcrTestTree_Positive_by_District_TestDate_TestNumber, _district, _testDate, _testNumber);
                RemoveFromDoubleNestedTree(pcrTestTree_Positive_by_Region_TestDate_TestNumber, _region, _testDate, _testNumber);
            }

            return true;
        }

        //pomocná metóda pre vkladanie testu do vnoreného stromu
        private void InsertToNestedTree<Key1, Key2>
            (
                AVLTree<Key1, AVLTree<Key2, PCRTest>> tree,
                Key1 k1, Key2 k2, PCRTest pcrTest
            )
            where Key1 : IComparable<Key1>
            where Key2 : IComparable<Key2>
        {
            if (!tree.Find(k1, out var _nested) || _nested == null)
            {
                _nested = new AVLTree<Key2, PCRTest>();
                tree.Insert(k1, _nested);
            }

            _nested.Insert(k2, pcrTest);
        }

        //pomocná metóda pre vkladanie testu do 2x vnoreného stromu
        private void InsertToDoubleNestedTree<Key1, Key2, Key3>
            (
                AVLTree<Key1, AVLTree<Key2, AVLTree<Key3, PCRTest>>> tree,
                Key1 k1, Key2 k2, Key3 k3, PCRTest pcrTest
            )
            where Key1 : IComparable<Key1>
            where Key2 : IComparable<Key2>
            where Key3 : IComparable<Key3>
        {
            if (!tree.Find(k1, out var _nested1) || _nested1 == null)
            {
                _nested1 = new AVLTree<Key2, AVLTree<Key3, PCRTest>>();
                tree.Insert(k1, _nested1);
            }

            if (!_nested1.Find(k2, out var _nested2) || _nested2 == null)
            {
                _nested2 = new AVLTree<Key3, PCRTest>();
                _nested1.Insert(k2, _nested2);
            }

            _nested2.Insert(k3, pcrTest);
        }

        //pomocná metóda pre vkladanie testu z vnoreného stromu
        private void RemoveFromNestedTree<Key1, Key2>(AVLTree<Key1, AVLTree<Key2, PCRTest>> tree, Key1 k1, Key2 k2)
            where Key1 : IComparable<Key1>
            where Key2 : IComparable<Key2>
        {
            if (tree.Find(k1, out var _nested) && _nested != null)
            {
                _nested.Delete(k2);
            }
        }

        //pomocná metóda pre vkladanie testu z 2x vnoreného stromu
        private void RemoveFromDoubleNestedTree<Key1, Key2, Key3>(AVLTree<Key1, AVLTree<Key2, AVLTree<Key3, PCRTest>>> tree, Key1 k1, Key2 k2, Key3 k3)
            where Key1 : IComparable<Key1>
            where Key2 : IComparable<Key2>
            where Key3 : IComparable<Key3>
        {
            if (tree.Find(k1, out var _nested) && _nested != null)
            {
                RemoveFromNestedTree(_nested, k2, k3);
            }
        }

        private string GetPatientNumberFromBirthDate(DateOnly dateOfBirth)
        {
            int _year = dateOfBirth.Year;
            int _month = dateOfBirth.Month;
            int _day = dateOfBirth.Day;

            int i = Random.Shared.Next() % 10_000;
            string _birthNumber = $"{_year % 100:00}{_month:00}{_day:00}{i:0000}";

            while (FindPerson(_birthNumber, out _))
            {
                i = Random.Shared.Next() % 10_000;
                _birthNumber = $"{_year % 100:00}{_month:00}{_day:00}{i:0000}";
            }

            return _birthNumber;
        }

        private bool FindPerson(string patientNumber, out Person? person)
        {
            return personTree_by_PatientNumber.Find(patientNumber, out person);
        }
    }
}
