using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Text_Editor
{
    internal class TextDocument
    {
        public string filePath { get; private set; } = "";
        public string text { get; set; } = "";

        public void NewFile()
        {
            if (string.IsNullOrEmpty(text))
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
                File.WriteAllText(filePath, text);
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
                File.WriteAllText(filePath, text);
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
                text = File.ReadAllText(filePath);
            }
        }

        public bool HasUnsavedChanges()
        {
            if (string.IsNullOrEmpty(filePath))
                return !string.IsNullOrEmpty(text);

            if (!File.Exists(filePath))
                return true;

            return File.ReadAllText(filePath) != text;
        }

        public void PasteDateTime(int caretIndex)
        {
            DateTime dateTime = DateTime.Now;
            string fDateTime = dateTime.ToString("HH:mm dd/MM/yyyy");
            int position = caretIndex;
            text = text.Insert(position, fDateTime);
            caretIndex = position + fDateTime.Length;
        }

        public void ResetDocument()
        {
            filePath = "";
            text = "";
        }
    }
}
