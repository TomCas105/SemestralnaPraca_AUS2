using SP_AUS_Database.GUI;
using System.Windows;

namespace SP_AUS_GUI.DetailWindows
{
    /// <summary>
    /// Interaction logic for TestDetailWindow.xaml
    /// </summary>
    public partial class TestDetailWindow : Window
    {
        public TestDetailWindow(PCRTest_GUI test)
        {
            InitializeComponent();
            DataContext = test;
        }
    }
}
