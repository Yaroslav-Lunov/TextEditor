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
    public partial class ReplacePopup : Window
    {
        MainWindow _owner;
        public ReplacePopup(MainWindow owner)
        {
            InitializeComponent();
            _owner = owner;
            select.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            Replace(select.Text, withWhat.Text);
        }

        private void Replace(string select, string withWhat)
        {
            _owner.textBox.Text = _owner.textBox.Text.Replace(select, withWhat);
        }
    }
}
