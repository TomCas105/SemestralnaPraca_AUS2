using SP_AUS_Database.GUI;
using SP_AUS_GUI.DetailWindows;
using System.Windows;
using System.Windows.Input;

namespace SP_AUS_GUI.ListWindows
{
    /// <summary>
    /// Interaction logic for PersonsListWindow.xaml
    /// </summary>
    public partial class PersonsListWindow : Window
    {
        public List<Person_GUI> Persons { get; set; } = new();

        public PersonsListWindow(List<Person_GUI> persons)
        {
            InitializeComponent();

            Persons = persons;
            TextBlockDescription.Text = $"Osoby ({Persons.Count})";

            DataContext = this;
        }

        private void PersonsListGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PersonsListGrid.SelectedItem is Person_GUI _person)
            {
                var detailWindow = new PersonDetailWindow(_person);
                detailWindow.ShowDialog();
            }
        }
    }
}
