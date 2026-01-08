
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Text_Editor
{
    public partial class MainWindow : Window
    {
        private string filePath = "";
        bool textWrapping = true;
        private readonly TextDocument _document = new();
        private int caretIndex;

        public MainWindow() 
        { 
            InitializeComponent();
            RegisterKeybinds();
        }

        private void RegisterKeybinds()
        {
            var saveCommand = new RelayCommand(_document.SaveFile);
            var saveKeyGesture = new KeyGesture(Key.S, ModifierKeys.Control);
            var saveKeyBinding = new KeyBinding(saveCommand, saveKeyGesture);
            this.InputBindings.Add(saveKeyBinding);

            var openCommand = new RelayCommand(_document.OpenFile);
            var openKeyGesture = new KeyGesture(Key.O, ModifierKeys.Control);
            var openKeyBinding = new KeyBinding(openCommand, openKeyGesture);
            this.InputBindings.Add(openKeyBinding);

            var newCommand = new RelayCommand(_document.NewFile);
            var newKeyGesture = new KeyGesture(Key.N, ModifierKeys.Control);
            var newKeyBinding = new KeyBinding(newCommand, newKeyGesture);
            this.InputBindings.Add(newKeyBinding);

            var newWindowCommand = new RelayCommand(NewWindow);
            var newWindowKeyGesture = new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift);
            var newWindowKeyBinding = new KeyBinding(newWindowCommand, newWindowKeyGesture);
            this.InputBindings.Add(newWindowKeyBinding);

            var saveAsCommand = new RelayCommand(_document.SaveAsFile);
            var saveAsKeyGesture = new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            var saveAsKeyBinding = new KeyBinding(saveAsCommand, saveAsKeyGesture);
            this.InputBindings.Add(saveAsKeyBinding);

            var timeCommand = new RelayCommand(() => { _document.PasteDateTime(textBox.CaretIndex); textBox.Text = _document.text; });
            var timeKeyGesture = new KeyGesture(Key.F5);
            var timeKeyBinding = new KeyBinding(timeCommand, timeKeyGesture);
            this.InputBindings.Add(timeKeyBinding);
        }

        private void New_Click(object sender, RoutedEventArgs e) { _document.NewFile(); textBox.Text = _document.text; }
        private void NewWindow_Click(object sender, RoutedEventArgs e) { NewWindow(); }
        private void Open_Click(object sender, RoutedEventArgs e) { _document.OpenFile(); textBox.Text = _document.text; }
        private void Save_Click(object sender, RoutedEventArgs e) {  _document.SaveFile(); }
        private void SaveAs_Click(object sender, RoutedEventArgs e) { _document.text = textBox.Text; _document.SaveAsFile(); }
        private void Exit_Click(object sender, RoutedEventArgs e) { Close(); }
        // -------------------------------------------------------------------
        private void Undo_Click(object sender, RoutedEventArgs e) { textBox.Undo(); }
        private void Cut_Click(object sender, RoutedEventArgs e) { textBox.Cut(); }
        private void Copy_Click(object sender, RoutedEventArgs e) { textBox.Copy(); }
        private void Paste_Click(object sender, RoutedEventArgs e) { textBox.Paste(); }
        private void Select_Click(object sender, RoutedEventArgs e) { textBox.SelectAll(); }
        // -------------------------------------------------------------------
        private void Find_Click(object sender, RoutedEventArgs e) { FindPopup findPopup = new FindPopup(this); findPopup.Show(); }
        private void Replace_Click(object sender, RoutedEventArgs e) { ReplacePopup replacePopup = new ReplacePopup(this); replacePopup.Show(); }
        private void GoTo_Click(object sender, RoutedEventArgs e) { GoToPopup goToPopup = new GoToPopup(this); goToPopup.Show(); }
        private void Time_Click(object sender, RoutedEventArgs e) { _document.PasteDateTime(textBox.CaretIndex); textBox.Text = _document.text; }

        // Functions -----------------------------------------------------

        private void NewWindow()
        {
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
        }

        public void Print(string print)
        {
            MessageBox.Show(
                print,
                "info",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Wrap_Click(object sender, RoutedEventArgs e)
        {
            textWrapping = !textWrapping;
            textBox.TextWrapping = textWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e) {
            _document.text = textBox.Text;
            caretIndex = textBox.CaretIndex;
            int lineIndex = textBox.GetLineIndexFromCharacterIndex(caretIndex);
            int colIndex = caretIndex - textBox.GetCharacterIndexFromLineIndex(lineIndex);

            infoBar.Content = $"Ln {lineIndex}, Col {colIndex}, Text wrapping {textWrapping}, Path {_document.filePath},";
        }
    }
}
