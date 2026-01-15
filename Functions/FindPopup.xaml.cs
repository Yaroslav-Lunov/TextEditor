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
    public partial class FindPopup : Window
    {
        private string text;
        private bool matchCase;
        private TextBox textBox;
        private string input;
        MainWindow _owner;

        public FindPopup(MainWindow owner)
        {
            _owner = owner;
            InitializeComponent();
            textBox = owner.textBox;
            text = textBox.Text;
            inputField.Focus();
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            input = inputField.Text;
            matchCase = matchCaseCheckBox.IsChecked ?? false;

            int findIndex = Find(input, matchCase);

            if (findIndex == -1) 
            {
                _owner.Print($"Could not find {input}");
                return;
            }
            FinalSelect(findIndex, input.Length);
            
        }

        private void FinalSelect(int start, int length)
        {
            textBox.Select(start, length);
            textBox.Focus();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private int Find(string what, bool matchCase)
        {
            if (matchCase)
            {
                return textBox.Text.IndexOf(what);
            }
            else
            {
                return textBox.Text.ToLower().IndexOf(what.ToLower());
            }
        }
    }
}
