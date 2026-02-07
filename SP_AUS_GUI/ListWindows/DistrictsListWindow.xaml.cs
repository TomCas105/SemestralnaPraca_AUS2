using SP_AUS_Database.GUI;
using System.Windows;

namespace SP_AUS_GUI.ListWindows
{
    /// <summary>
    /// Interaction logic for DistrictsListWindow.xaml
    /// </summary>
    public partial class DistrictsListWindow : Window
    {
        public List<Area_GUI> Areas { get; set; } = new();

        public DistrictsListWindow(List<Area_GUI> areas)
        {
            InitializeComponent();

            Areas = areas;
            TextBlockDescription.Text = $"Okresy ({Areas.Count})";

            DataContext = this;
        }
    }
}
