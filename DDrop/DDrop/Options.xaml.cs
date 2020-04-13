using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.Utility.SeriesLocalStorageOperations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using DDrop.Utility.Mappers;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using ToastNotifications;
using ToastNotifications.Messages;
using pbu = RFM.RFM_WPFProgressBarUpdate;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : INotifyPropertyChanged
    {
        public static readonly DependencyProperty LocalStoredUsersProperty = DependencyProperty.Register("LocalStoredUsers", typeof(LocalStoredUsers), typeof(Options));
        public LocalStoredUsers LocalStoredUsers
        {
            get => (LocalStoredUsers)GetValue(LocalStoredUsersProperty);
            set => SetValue(LocalStoredUsersProperty, value);
        }

        private bool _allSelectedStoredUsersChanging;
        private bool? _allSelectedStoredUsers = false;

        private void AllSelectedChanged()
        {
            if (LocalStoredUsers.Users != null)
            {
                if (_allSelectedStoredUsersChanging) return;

                try
                {
                    _allSelectedStoredUsersChanging = true;

                    if (AllSelectedStoredUsers == true)
                    {
                        foreach (var userSeries in LocalStoredUsers.Users)
                            userSeries.IsChecked = true;
                    }
                    else if (AllSelectedStoredUsers == false)
                    {
                        foreach (var userSeries in LocalStoredUsers.Users)
                            userSeries.IsChecked = false;
                    }
                }
                finally
                {
                    _allSelectedStoredUsersChanging = false;
                }
            }
            else
            {
                AllSelectedStoredUsers = false;
            }
        }

        private void RecheckAllSelected()
        {
            if (_allSelectedStoredUsersChanging) return;

            try
            {
                _allSelectedStoredUsersChanging = true;

                if (LocalStoredUsers.Users.All(e => e.IsChecked))
                    AllSelectedStoredUsers = true;
                else if (LocalStoredUsers.Users.All(e => !e.IsChecked))
                    AllSelectedStoredUsers = false;
                else
                    AllSelectedStoredUsers = null;
            }
            finally
            {
                _allSelectedStoredUsersChanging = false;
            }
        }

        public bool? AllSelectedStoredUsers
        {
            get => _allSelectedStoredUsers;
            set
            {
                if (value == _allSelectedStoredUsers) return;
                _allSelectedStoredUsers = value;

                AllSelectedChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedStoredUsers"));
            }
        }

        private void EntryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(LocalStoredUser.IsChecked))
                RecheckAllSelected();
        }



        public static readonly DependencyProperty UserAutoCalculationTemplatesProperty = DependencyProperty.Register("UserAutoCalculationTemplates", typeof(ObservableCollection<AutoCalculationTemplate>), typeof(Options));
        public ObservableCollection<AutoCalculationTemplate> UserAutoCalculationTemplates
        {
            get => (ObservableCollection<AutoCalculationTemplate>)GetValue(UserAutoCalculationTemplatesProperty);
            set => SetValue(UserAutoCalculationTemplatesProperty, value);
        }

        private bool _allSelectedCalculationTemplatesChanging;
        private bool? _allSelectedCalculationTemplates = false;

        private void AllSelectedCalculationTemplatesChanged()
        {
            if (UserAutoCalculationTemplates != null)
            {
                if (_allSelectedCalculationTemplatesChanging) return;

                try
                {
                    _allSelectedCalculationTemplatesChanging = true;

                    if (AllSelectedCalculationTemplates == true)
                    {
                        foreach (var autoCalculationTemplate in UserAutoCalculationTemplates)
                            autoCalculationTemplate.IsChecked = true;
                    }
                    else if (AllSelectedCalculationTemplates == false)
                    {
                        foreach (var autoCalculationTemplate in UserAutoCalculationTemplates)
                            autoCalculationTemplate.IsChecked = false;
                    }
                }
                finally
                {
                    _allSelectedCalculationTemplatesChanging = false;
                }
            }
            else
            {
                AllSelectedCalculationTemplates = false;
            }
        }

        private void RecheckAllSelectedCalculationTemplates()
        {
            if (_allSelectedCalculationTemplatesChanging) return;

            try
            {
                _allSelectedCalculationTemplatesChanging = true;

                if (UserAutoCalculationTemplates.All(e => e.IsChecked))
                    AllSelectedCalculationTemplates = true;
                else if (UserAutoCalculationTemplates.All(e => !e.IsChecked))
                    AllSelectedCalculationTemplates = false;
                else
                    AllSelectedCalculationTemplates = null;
            }
            finally
            {
                _allSelectedCalculationTemplatesChanging = false;
            }
        }

        public bool? AllSelectedCalculationTemplates
        {
            get => _allSelectedCalculationTemplates;
            set
            {
                if (value == _allSelectedCalculationTemplates) return;
                _allSelectedCalculationTemplates = value;

                AllSelectedCalculationTemplatesChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedCalculationTemplates"));
            }
        }

        private void EntryOnPropertyCalculationTemplatesChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(AutoCalculationTemplate.IsChecked))
                RecheckAllSelectedCalculationTemplates();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        
        public bool ShowLinesOnPreviewIsChanged;
        public bool ShowContourOnPreviewIsChanged;

        private Notifier _notifier;

        public Options(Notifier notifier)
        {
            _notifier = notifier;
            InitializeComponent();
            InitializePaths();
            InitilizeUserTemplates();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.StoredUsers))
            {
                LocalStoredUsers = JsonSerializeProvider.DeserializeFromString<LocalStoredUsers>(Properties.Settings.Default.StoredUsers);

                StoredUsers.ItemsSource = LocalStoredUsers.Users;
            }

            OptionsLoadingComplete();
        }

        private void InitilizeUserTemplates()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AutoCalculationTemplates))
            {
                UserAutoCalculationTemplates = JsonSerializeProvider.DeserializeFromString<ObservableCollection<AutoCalculationTemplate>>(Properties.Settings.Default.AutoCalculationTemplates);
            }
            else
            {
                UserAutoCalculationTemplates = new ObservableCollection<AutoCalculationTemplate>();
            }

            AutoCalculaionTemplates.ItemsSource = UserAutoCalculationTemplates;
        }

        private void InitializePaths()
        {
            ShowLinesOnPreview.IsChecked = Properties.Settings.Default.ShowLinesOnPreview;
            ShowContourOnPreview.IsChecked = Properties.Settings.Default.ShowContourOnPreview;
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
                case OptionsEnum.ShowContourOnPreview:
                    Properties.Settings.Default.ShowContourOnPreview = (bool) value;
                    Properties.Settings.Default.Save();
                    ShowContourOnPreviewIsChanged = true;
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

        private void ShowContourOnPreview_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            UpdateOptions(OptionsEnum.ShowContourOnPreview, checkBox.IsChecked);
        }

        private void ShowContourOnPreview_OnUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            UpdateOptions(OptionsEnum.ShowContourOnPreview, checkBox.IsChecked);
        }

        private void DeleteLocalUser_Click(object sender, RoutedEventArgs e)
        {
            LocalStoredUsers.Users.RemoveAt(StoredUsers.SelectedIndex);

            Properties.Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString(LocalStoredUsers);

            Properties.Settings.Default.Save();
        }

        private void DeleteAllStoredUsers_OnClick(object sender, RoutedEventArgs e)
        {
            var checkedCount = 0;

            MessageBoxResult messageBoxResult =
                MessageBox.Show(checkedCount > 0 ? "Удалить выбранных локальных пользователей?" : "Удалить всех локальныйх пользователей?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                for (int i = LocalStoredUsers.Users.Count - 1; i >= 0; i--)
                {
                    if (checkedCount > 0 && !LocalStoredUsers.Users[i].IsChecked)
                    {
                        continue;
                    }

                    LocalStoredUsers.Users.RemoveAt(i);
                }
            }
        }

        private void AddTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            UserAutoCalculationTemplates.Add(new AutoCalculationTemplate()
            {
                Id = Guid.NewGuid(),
                Title = AutoCalculationTemplateTitle.Text,
                Parameters = new AutoCalculationParameters(),
            });
        }

        private void DeleteSingleTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            UserAutoCalculationTemplates.RemoveAt(AutoCalculaionTemplates.SelectedIndex);
        }

        private void DeleteTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var checkedCount = 0;

            MessageBoxResult messageBoxResult =
                MessageBox.Show(checkedCount > 0 ? "Удалить выбранные шаблоны авторасчета?" : "Удалить все шаблоны авторасчета?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                for (int i = UserAutoCalculationTemplates.Count - 1; i >= 0; i--)
                {
                    if (checkedCount > 0 && !UserAutoCalculationTemplates[i].IsChecked)
                    {
                        continue;
                    }

                    UserAutoCalculationTemplates.RemoveAt(i);
                }
            }
        }

        private async void ImportTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "DDrop files (*.dplate)|*.dplate|All files (*.*)|*.*",
                Multiselect = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                OptionsIsLoading();

                var pbuHandle1 = pbu.New(ProgressBar, 0, openFileDialog.FileNames.Length, 0);

                foreach (var fileName in openFileDialog.FileNames)
                {
                    try
                    {
                        var deserializedTemplate = await JsonSerializeProvider.DeserializeFromFileAsync<AutoCalculationTemplate>(fileName);

                        UserAutoCalculationTemplates.Add(deserializedTemplate);

                        _notifier.ShowSuccess($"Шаблон авторасчета {deserializedTemplate.Title} добавлен.");
                    }
                    catch
                    {
                        _notifier.ShowError(
                            $"Не удалось десериализовать файл {fileName}. Файл не является файлом шаблона или поврежден.");
                    }

                    pbu.CurValue[pbuHandle1] += 1;
                }

                OptionsLoadingComplete();
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }

            if (AutoCalculaionTemplates.ItemsSource == null)
                AutoCalculaionTemplates.ItemsSource = UserAutoCalculationTemplates;
        }

        private async void ExportTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (UserAutoCalculationTemplates.Count > 0)
            {
                VistaFolderBrowserDialog saveFileDialog = new VistaFolderBrowserDialog
                {
                    UseDescriptionForTitle = true,
                    Description = "Выберите папку для сохранения..."
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    OptionsIsLoading();

                    int checkedCount = UserAutoCalculationTemplates.Count(x => x.IsChecked);

                    var pbuHandle1 = pbu.New(ProgressBar, 0, checkedCount > 0 ? checkedCount : UserAutoCalculationTemplates.Count, 0);

                    foreach (var autoCalculationTemplate in UserAutoCalculationTemplates)
                    {
                        if (checkedCount > 0 && !autoCalculationTemplate.IsChecked)
                        {
                            continue;
                        }

                        await JsonSerializeProvider.SerializeToFileAsync(autoCalculationTemplate,
                            $"{saveFileDialog.SelectedPath}\\{autoCalculationTemplate.Title}.dplate");

                        _notifier.ShowSuccess($"файл {autoCalculationTemplate.Title}.dplate сохранен на диске.");

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    OptionsLoadingComplete();
                }
            }
            else
            {
                _notifier.ShowInformation("Нет серий для выгрузки.");
            }
        }
        

        private static readonly Regex _regex = new Regex("^[1-9]+[0-9]*$");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void AutoCalculaionTemplates_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var row = sender as AutoCalculationTemplate;

            if (row != null)
            {
                if (!IsTextAllowed(row.Parameters.Ksize.ToString()) || !IsTextAllowed(row.Parameters.Size1.ToString()) ||
                    !IsTextAllowed(row.Parameters.Size2.ToString()) ||
                    !IsTextAllowed(row.Parameters.Treshold1.ToString()) ||
                    !IsTextAllowed(row.Parameters.Treshold2.ToString()))
                {
                    _notifier.ShowInformation("Введите целое число больше ноля.");
                    e.Cancel = true;
                }
            }
        }

        private void OptionsIsLoading()
        {
            GeneralSettings.IsEnabled = false;
            StoredAccounts.IsEnabled = false;
            AddTemplate.IsEnabled = false;
            AutoCalculationTemplateTitle.IsEnabled = false;
            AutoCalculaionTemplates.IsEnabled = false;
            ExportTemplate.IsEnabled = false;
            ImportTemplate.IsEnabled = false;
            DeleteTemplateButton.IsEnabled = false;
            AutoCalculationTemplateLoading.IsAdornerVisible = true;
        }

        private void OptionsLoadingComplete()
        {
            GeneralSettings.IsEnabled = true;
            StoredAccounts.IsEnabled = true;
            AddTemplate.IsEnabled = true;
            AutoCalculationTemplateTitle.IsEnabled = true;
            AutoCalculaionTemplates.IsEnabled = true;
            ExportTemplate.IsEnabled = true;
            ImportTemplate.IsEnabled = true;
            DeleteTemplateButton.IsEnabled = true;
            AutoCalculationTemplateLoading.IsAdornerVisible = false;
        }
    }
}
