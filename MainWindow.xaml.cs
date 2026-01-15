
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
        private int caretIndex;
        public MainWindow() 
        { 
            InitializeComponent();
            RegisterKeybinds();
        }

        private void RegisterKeybinds()
        {
            var saveCommand = new RelayCommand(SaveFile);
            var saveKeyGesture = new KeyGesture(Key.S, ModifierKeys.Control);
            var saveKeyBinding = new KeyBinding(saveCommand, saveKeyGesture);
            this.InputBindings.Add(saveKeyBinding);

            var openCommand = new RelayCommand(OpenFile);
            var openKeyGesture = new KeyGesture(Key.O, ModifierKeys.Control);
            var openKeyBinding = new KeyBinding(openCommand, openKeyGesture);
            this.InputBindings.Add(openKeyBinding);

            var newCommand = new RelayCommand(NewFile);
            var newKeyGesture = new KeyGesture(Key.N, ModifierKeys.Control);
            var newKeyBinding = new KeyBinding(newCommand, newKeyGesture);
            this.InputBindings.Add(newKeyBinding);

            var newWindowCommand = new RelayCommand(NewWindow);
            var newWindowKeyGesture = new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift);
            var newWindowKeyBinding = new KeyBinding(newWindowCommand, newWindowKeyGesture);
            this.InputBindings.Add(newWindowKeyBinding);

            var saveAsCommand = new RelayCommand(SaveAsFile);
            var saveAsKeyGesture = new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            var saveAsKeyBinding = new KeyBinding(saveAsCommand, saveAsKeyGesture);
            this.InputBindings.Add(saveAsKeyBinding);

            var timeCommand = new RelayCommand(() => 
            { 
                PasteDateTime(textBox.CaretIndex); 
            });
            var timeKeyGesture = new KeyGesture(Key.F5);
            var timeKeyBinding = new KeyBinding(timeCommand, timeKeyGesture);
            this.InputBindings.Add(timeKeyBinding);

            var updateCommand = new RelayCommand(Update);
            var updateKeyGesture = new KeyGesture(Key.Q, ModifierKeys.Control);
            var updateKeyBinding = new KeyBinding(updateCommand, updateKeyGesture);
            this.InputBindings.Add(updateKeyBinding);
        }

        private void New_Click(object sender, RoutedEventArgs e) { NewFile(); }
        private void NewWindow_Click(object sender, RoutedEventArgs e) { NewWindow(); }
        private void Open_Click(object sender, RoutedEventArgs e) { OpenFile(); }
        private void Save_Click(object sender, RoutedEventArgs e) { SaveFile(); }
        private void SaveAs_Click(object sender, RoutedEventArgs e) { SaveAsFile(); }
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
        private void Time_Click(object sender, RoutedEventArgs e) { PasteDateTime(textBox.CaretIndex); }

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
            caretIndex = textBox.CaretIndex;
            int lineIndex = textBox.GetLineIndexFromCharacterIndex(caretIndex);
            int colIndex = caretIndex - textBox.GetCharacterIndexFromLineIndex(lineIndex);

            infoBar.Content = $"Ln {lineIndex}, Col {colIndex}, Text wrapping {textWrapping}, Path {filePath},";
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Update();        
        }

        public void NewFile()
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                ResetDocument();
                return;
            }

            bool hasUnsavedChanges = HasUnsavedChanges();

            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. do you wish to continue?",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            ResetDocument();
        }

        public void SaveFile()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                SaveAsFile();
            }
            else
            {
                File.WriteAllText(filePath, textBox.Text);
            }
        }


        public void SaveAsFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, textBox.Text);
            }
        }


        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.DefaultExt = "txt";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                filePath = openFileDialog.FileName;
                textBox.Text = File.ReadAllText(filePath);
            }
        }

        public bool HasUnsavedChanges()
        {
            if (string.IsNullOrEmpty(filePath))
                return !string.IsNullOrEmpty(textBox.Text);

            if (!File.Exists(filePath))
                return true;

            return File.ReadAllText(filePath) != textBox.Text;
        }


        public void PasteDateTime(int caretIndex)
        {
            DateTime dateTime = DateTime.Now;
            string fDateTime = dateTime.ToString("HH:mm dd/MM/yyyy");
            int position = caretIndex;
            textBox.Text = textBox.Text.Insert(position, fDateTime);
            caretIndex = position + fDateTime.Length;
        }
        public void Update()
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                textBox.Text = File.ReadAllText(filePath);
            }
        }

        public void ResetDocument()
        {
            filePath = "";
            textBox.Text = "";
        }
    }
}
