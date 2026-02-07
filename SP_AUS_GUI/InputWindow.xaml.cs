using System.Windows;
using System.Windows.Controls;

namespace SP_AUS_GUI
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        (string, string)[] inputFields;
        private TextBox[] inputTextBoxes;

        public string[] InputValues { get; private set; }

        public InputWindow((string, string)[] inputFields)
        {
            InitializeComponent();

            this.inputFields = inputFields;
            InputValues = new string[inputFields.Length];
            inputTextBoxes = new TextBox[inputFields.Length];

            UpdateWindow();
        }

        private void UpdateWindow()
        {
            FieldsPanel.Children.Clear();

            for (int i = 0; i < inputFields.Length; i++)
            {
                TextBlock _textBlock = new TextBlock();
                _textBlock.Text = inputFields[i].Item1 + ":";
                _textBlock.Height = 20;

                TextBox _textBox = new TextBox();
                _textBox.Text = inputFields[i].Item2;
                _textBox.MinWidth = 150;
                _textBox.Height = 20;
                inputTextBoxes[i] = _textBox;

                FieldsPanel.Children.Add(_textBlock);
                FieldsPanel.Children.Add(_textBox);
            }

            MinHeight = FieldsPanel.Children.Count * 20 + 200;
            Height = MinHeight;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < inputTextBoxes.Length; i++)
            {
                InputValues[i] = inputTextBoxes[i].Text;
            }
            Close();
        }
    }
}
