using Microsoft.Win32;
using SP_AUS_Database.Database;
using SP_AUS_Database.GUI;
using SP_AUS_Database.Tests;
using SP_AUS_GUI.DetailWindows;
using SP_AUS_GUI.ListWindows;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace SP_AUS_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PCRTestDatabase database;

        public MainWindow()
        {
            InitializeComponent();

            database = new();

            UpdateWindow();
        }

        private void UpdateWindow()
        {
            PersonsListGrid.ItemsSource = null;
            TestsListGrid.ItemsSource = null;

            var _personList = database.GetPersonList();
            PersonsListGrid.ItemsSource = _personList;
            TextBlock_Person.Text = $"Pacienti ({_personList.Count:n0})";

            var _testList = database.GetPCRTestList();
            TestsListGrid.ItemsSource = _testList;
            TextBlock_PCRTest.Text = $"PCR Testy ({_testList.Count:n0})";
        }

        private void GenerateData(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Počet pacientov", "1000"),
                ("Maximálny počet testov na pacienta", "20"),
                ("Pravdepodobnosť pozitívneho testu", "0.25")
            };
            InputWindow _inputWindow = new(_inputFields);
            _inputWindow.ShowDialog();

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _patientCount);
            int.TryParse(_inputValues[1], out int _testCount);
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            double.TryParse(_inputValues[2].Replace(',', '.'), culture, out double _positiveChance);

            DatabaseTests.GenerateData(database, _patientCount, _testCount, _positiveChance);

            UpdateWindow();
        }

        private void ClearDatabase(object sender, RoutedEventArgs e)
        {
            database.ClearAll();

            UpdateWindow();
        }

        private void TestsListGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TestsListGrid.SelectedItem is PCRTest_GUI _test)
            {
                var detailWindow = new TestDetailWindow(_test);
                detailWindow.ShowDialog();
            }
        }

        private void PersonsListGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PersonsListGrid.SelectedItem is Person_GUI _person)
            {
                var detailWindow = new PersonDetailWindow(_person);
                detailWindow.ShowDialog();
            }
        }

        private void ExecuteTask1(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo pacienta", "0123456789"),
                ("Dátum testu (\"DD.MM.YYYY\")", "01.01.2025"),
                ("Čas testu (\"HH:MM:SS\")", "06:00:00"),
                ("Číslo pracoviska", "10101"),
                ("Číslo okresu", "101"),
                ("Číslo kraja", "1"),
                ("Pozitívny test (áno/nie)", "nie"),
                ("Hodnota testu", "0"),
                ("Poznámka", "")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;

            for (int i = 0; i < _inputValues.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(_inputValues[i]))
                {
                    MessageBox.Show($"Nesprávne zadané údaje.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            string _patientNumber = _inputValues[0];
            DateOnly _testDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", out _testDate);
            TimeOnly _testTime = TimeOnly.FromDateTime(DateTime.Now);
            TimeOnly.TryParse(_inputValues[2], out _testTime);
            int.TryParse(_inputValues[3], out int _workplace);
            int.TryParse(_inputValues[4], out int _district);
            int.TryParse(_inputValues[5], out int _region);
            bool _result = _inputValues[6].Trim().ToLower() == "áno";
            double.TryParse(_inputValues[7], out double _testValue);
            string _comment = _inputValues[8];

            if (database.Task1_InsertPcrTest(_patientNumber, _testDate, _testTime, _workplace, _district, _region, _result, _testValue, _comment))
            {
                UpdateWindow();
                MessageBox.Show($"PCR test bol vložený.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"PCR test už je vložený.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteTask2(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo pacienta", "0123456789"),
                ("Číslo testu", "0")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;

            for (int i = 0; i < _inputValues.Length; i++)
            {
                if (string.IsNullOrEmpty(_inputValues[i]))
                {
                    MessageBox.Show($"Nesprávne zadané údaje.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            string _patientNumber = _inputValues[0];
            int.TryParse(_inputValues[1], out int _testNumber);

            var _test = database.Task2_GetTestOfPerson(_patientNumber, _testNumber);

            if (_test != null)
            {
                TestDetailWindow detailWindow = new(_test);
                detailWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show($"Test sa nenašiel.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteTask3(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo pacienta", "0123456789")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            string _patientNumber = _inputValues[0];

            var _tests = database.Task3_GetPersonTests(_patientNumber);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask4(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo okresu", "101"),
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _district);
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[2], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task4_GetPositiveTestsInDistrictBetweenDates(_district, _dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask5(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo okresu", "101"),
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _district);
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[2], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task5_GetTestsInDistrictBetweenDates(_district, _dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask6(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo kraja", "1"),
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _region);
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[2], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task6_GetPositiveTestsInRegionBetweenDates(_region, _dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask7(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo kraja", "1"),
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _region);
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[2], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task7_GetTestsInRegionBetweenDates(_region, _dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask8(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[0], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task8_GetPositiveTestsBetweenDates(_dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask9(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[0], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task9_GetTestsBetweenDates(_dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask10(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo okresu", "101"),
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _district);
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[2], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _persons = database.Task10_GetSickPersonsInDistrictToDate(_district, _date, _sickDays);

            PersonsListWindow _listWindow = new(_persons);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask11(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo okresu", "101"),
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _district);
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[2], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _persons = database.Task11_GetSickPersonsInDistrictToDate_OrderBy_TestValue(_district, _date, _sickDays);

            PersonsListWindow _listWindow = new(_persons);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask12(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo kraja", "1"),
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _region);
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[2], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _persons = database.Task10_GetSickPersonsInDistrictToDate(_region, _date, _sickDays);

            PersonsListWindow _listWindow = new(_persons);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask13(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[1], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[0], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _persons = database.Task13_GetSickPersonsToDate(_date, _sickDays);

            PersonsListWindow _listWindow = new(_persons);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask14(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[1], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[0], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _persons = database.Task14_GetTopSickPersonsPerDistrict(_date, _sickDays);

            PersonsListWindow _listWindow = new(_persons);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask15(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[1], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[0], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _districts = database.Task15_GetDistrictBySickCount(_date, _sickDays);

            DistrictsListWindow _listWindow = new(_districts);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask16(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Dátum (DD.MM.YYYY)", "1.1.2025"),
                ("Počet dní definujúci chorú osobu", "14")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
            int.TryParse(_inputValues[1], out int _sickDays);
            DateOnly.TryParseExact(_inputValues[0], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date);

            var _regions = database.Task16_GetRegionBySickCount(_date, _sickDays);

            RegionsListWindow _listWindow = new(_regions);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask17(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo pracoviska", "10101"),
                ("Dátum od (DD.MM.YYYY)", "1.1.2025"),
                ("Dátum do (DD.MM.YYYY)", "31.12.2025")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;
            int.TryParse(_inputValues[0], out int _workplace);
            DateOnly _dateFrom = DateOnly.FromDateTime(DateTime.Now);
            DateOnly _dateTo = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateFrom);
            DateOnly.TryParseExact(_inputValues[2], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTo);

            var _tests = database.Task17_GetTestsInDistrictBetweenDates(_workplace, _dateFrom, _dateTo);

            TestsListWindow _listWindow = new(_tests);
            _listWindow.ShowDialog();
        }

        private void ExecuteTask18(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo testu", "0")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;

            if (string.IsNullOrEmpty(_inputValues[0]))
            {
                MessageBox.Show($"Nesprávne zadané údaje.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int.TryParse(_inputValues[0], out int _testNumber);

            var _test = database.Task18_GetTestByNumber(_testNumber);

            if (_test != null)
            {
                TestDetailWindow detailWindow = new(_test);
                detailWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show($"Test sa nenašiel.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteTask19(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Krstné meno", "Ján"),
                ("Priezvisko", "Hrach"),
                ("Dátum narodenia (DD.MM.YYYY)", "1.1.2000"),
                ("Číslo pacienta (vygeneruje sa automaticky ak je prázdne)", "")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;

            for (int i = 0; i < _inputValues.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(_inputValues[i]))
                {
                    MessageBox.Show($"Nesprávne zadané údaje.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            string _firstName = _inputValues[0];
            string _lastName = _inputValues[1];
            DateOnly _dateOfBirth = DateOnly.FromDateTime(DateTime.Now);
            DateOnly.TryParseExact(_inputValues[2], "d.M.yyyy", out _dateOfBirth);
            string _patientNumber = _inputValues[3];

            if (database.Task19_InsertPerson(_firstName, _lastName, _dateOfBirth, _patientNumber))
            {
                UpdateWindow();
                MessageBox.Show($"Osoba bola vložená.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Osoba už je vložená.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteTask20(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo testu", "0")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;

            if (string.IsNullOrEmpty(_inputValues[0]))
            {
                MessageBox.Show($"Nesprávne zadané údaje.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int.TryParse(_inputValues[0], out int _testNumber);

            if (database.Task20_DeleteTest(_testNumber))
            {
                UpdateWindow();
                MessageBox.Show($"Test bol odstránený.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Test sa nenašiel.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteTask21(object sender, RoutedEventArgs e)
        {
            (string, string)[] _inputFields = {
                ("Číslo pacienta", "0123456789")
            };

            InputWindow _inputWindow = new(_inputFields);

            if (_inputWindow.ShowDialog() == true)
            {
                return;
            }

            string[] _inputValues = _inputWindow.InputValues;

            if (string.IsNullOrEmpty(_inputValues[0]))
            {
                MessageBox.Show($"Nesprávne zadané údaje.", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string _patientNumber = _inputValues[0];

            if (database.Task21_DeletePerson(_patientNumber))
            {
                UpdateWindow();
                MessageBox.Show($"Test bol odstránený.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Test sa nenašiel.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportPersons(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _dialog = new()
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Vyber súbor so zoznamom osôb"
            };

            if (_dialog.ShowDialog() == true)
            {
                string _filePath = _dialog.FileName;

                database.ImportPersons(_filePath);

                UpdateWindow();
            }
        }

        private void ImportPCRTests(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _dialog = new()
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Vyber súbor so zoznamom PCR testov"
            };

            if (_dialog.ShowDialog() == true)
            {
                string _filePath = _dialog.FileName;

                database.ImportPCRTests(_filePath);

                UpdateWindow();
            }
        }

        private void ExportPersons(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _dialog = new()
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Uložiť zoznam osôb"
            };

            if (_dialog.ShowDialog() == true)
            {
                string _filePath = _dialog.FileName;

                database.ExportPersons(_filePath);

                MessageBox.Show($"Zoznam osôb bol exportovaný do: {_filePath}", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportPCRTests(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _dialog = new()
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Uložiť zoznam PCR testov"
            };

            if (_dialog.ShowDialog() == true)
            {
                string _filePath = _dialog.FileName;

                database.ExportPCRTests(_filePath);

                MessageBox.Show($"Zoznam PCR Testov bol exportovaný do: {_filePath}", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}