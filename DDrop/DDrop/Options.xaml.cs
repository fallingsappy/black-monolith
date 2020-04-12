using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.Utility.SeriesLocalStorageOperations;
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
        public bool ShowLinesOnPreviewIsChanged = false;

        public static readonly DependencyProperty LocalStoredUsersProperty = DependencyProperty.Register("LocalStoredUsers", typeof(LocalStoredUsers), typeof(Options));
        public LocalStoredUsers LocalStoredUsers
        {
            get => (LocalStoredUsers)GetValue(LocalStoredUsersProperty);
            set => SetValue(LocalStoredUsersProperty, value);
        }

        public Options()
        {
            InitializeComponent();
            InitializePaths();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.StoredUsers))
            {
                LocalStoredUsers = JsonSerializeProvider.DeserializeFromString<LocalStoredUsers>(Properties.Settings.Default.StoredUsers);

                StoredUsers.ItemsSource = LocalStoredUsers.Users;
            }
        }

        private void InitializePaths()
        {
            ShowLinesOnPreview.IsChecked = Properties.Settings.Default.ShowLinesOnPreview;
        }

        private void ChooseFilePath_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;

            if (openFileDialog.ShowDialog() == true)
            {
                UpdateOptions((OptionsEnum)Enum.Parse(typeof(OptionsEnum), button.Name), openFileDialog.FileName);
            }
        }
        private void ChooseFolderPath_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                UpdateOptions((OptionsEnum)Enum.Parse(typeof(OptionsEnum), button.Name), dialog.FileName);
            }
        }

        private void UpdateOptions(OptionsEnum option, object value)
        {
            switch (option)
            {
                case OptionsEnum.ShowLinesOnPreview:
                    Properties.Settings.Default.ShowLinesOnPreview = (bool)value;
                    Properties.Settings.Default.Save();
                    ShowLinesOnPreviewIsChanged = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        private void ShowLinesOnPreview_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            UpdateOptions(OptionsEnum.ShowLinesOnPreview, checkBox.IsChecked);
        }

        private void ShowLinesOnPreview_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            UpdateOptions(OptionsEnum.ShowLinesOnPreview, checkBox.IsChecked);
        }

        private void DeleteLocalUser_Click(object sender, RoutedEventArgs e)
        {
            LocalStoredUsers.Users.RemoveAt(StoredUsers.SelectedIndex);

            Properties.Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString<LocalStoredUsers>(LocalStoredUsers);

            Properties.Settings.Default.Save();
        }
    }
}
