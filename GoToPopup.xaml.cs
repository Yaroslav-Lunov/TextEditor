using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Text_Editor
{
    public partial class GoToPopup : Window
    {
        MainWindow _owner;
        
        public GoToPopup(MainWindow owner)
        {
            InitializeComponent();
            _owner = owner;
            what.Focus();
        }


        private void Go_Click(object sender, RoutedEventArgs e)
        {
            int lineNumber = Convert.ToInt32(what.Text);

            int success = GoToLine(lineNumber);
            if (success == 0)
            {
                Close();
            }
        }
        private int GoToLine(int lineNumber)
        {
            TextBox textBox = _owner.textBox;
            if (textBox.LineCount > lineNumber - 1 && lineNumber - 1 > -1)
            {
                int lineCharacterIndex = textBox.GetCharacterIndexFromLineIndex(lineNumber - 1);
                textBox.CaretIndex = lineCharacterIndex;
                textBox.Focus();
                return 0;
            }
            else
            {
                _owner.Print("The line is out of range");
                return 1;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) { Close(); }
    }
}
