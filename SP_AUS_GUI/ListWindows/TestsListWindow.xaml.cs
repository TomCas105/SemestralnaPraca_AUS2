using SP_AUS_Database.GUI;
using SP_AUS_GUI.DetailWindows;
using System.Windows;
using System.Windows.Input;

namespace SP_AUS_GUI.ListWindows
{
    /// <summary>
    /// Interaction logic for TestsListWindow.xaml
    /// </summary>
    public partial class TestsListWindow : Window
    {
        public List<PCRTest_GUI> Tests { get; set; } = new();

        public TestsListWindow(List<PCRTest_GUI> tests)
        {
            InitializeComponent();

            Tests = tests;
            TextBlockDescription.Text = $"Testy ({Tests.Count})";

            DataContext = this;
        }

        private void TestsListGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TestsListGrid.SelectedItem is PCRTest_GUI _test)
            {
                var detailWindow = new TestDetailWindow(_test);
                detailWindow.ShowDialog();
            }
        }
    }
}
