using DDrop.BE.Enums.Options;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
            InitializePaths();
        }

        private void InitializePaths()
        {
            InterpreterTextBox.Text = Properties.Settings.Default.Interpreter;
            ReferenceTextBox.Text = Properties.Settings.Default.Reference;
            SaveToTextBox.Text = Properties.Settings.Default.SaveTo;
            ScriptToRunTextBox.Text = Properties.Settings.Default.ScriptToRun;
        }

        private void ChooseFilePath_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;

            if (openFileDialog.ShowDialog() == true)
            {
                UpdateOptionsPath((OptionsEnum)Enum.Parse(typeof(OptionsEnum), button.Name), openFileDialog.FileName);
            }
        }
        private void ChooseFolderPath_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                UpdateOptionsPath((OptionsEnum)Enum.Parse(typeof(OptionsEnum), button.Name), dialog.FileName);
            }
        }

        private void UpdateOptionsPath(OptionsEnum option, string path)
        {
            switch (option)
            {
                case OptionsEnum.Interpreter:
                    Properties.Settings.Default.Interpreter = path;
                    Properties.Settings.Default.Save();
                    InterpreterTextBox.Text = path;
                    break;
                case OptionsEnum.Reference:
                    Properties.Settings.Default.Reference = path;
                    Properties.Settings.Default.Save();
                    ReferenceTextBox.Text = path;
                    break;
                case OptionsEnum.SaveTo:
                    Properties.Settings.Default.SaveTo = path;
                    Properties.Settings.Default.Save();
                    SaveToTextBox.Text = path;
                    break;
                case OptionsEnum.ScriptToRun:
                    Properties.Settings.Default.ScriptToRun = path;
                    Properties.Settings.Default.Save();
                    ScriptToRunTextBox.Text = path;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }
    }
}
