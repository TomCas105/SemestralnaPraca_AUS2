using SP_AUS_Database.GUI;
using System.Windows;

namespace SP_AUS_GUI.DetailWindows
{
    /// <summary>
    /// Interaction logic for PersonDetailWindow.xaml
    /// </summary>
    public partial class PersonDetailWindow : Window
    {
        public PersonDetailWindow(Person_GUI person)
        {
            InitializeComponent();
            DataContext = person;
        }
    }
}
