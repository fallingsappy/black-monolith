﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.Properties;
using DDrop.Utility.Logger;
using DDrop.Utility.SeriesLocalStorageOperations;
using DDrop.Utility.Validation;
using Microsoft.Win32;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using ToastNotifications;
using ToastNotifications.Messages;
using pbu = RFM.RFM_WPFProgressBarUpdate;

namespace DDrop
{
    /// <summary>
    ///     Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : INotifyPropertyChanged
    {
        public static readonly DependencyProperty LocalStoredUsersProperty =
            DependencyProperty.Register("LocalStoredUsers", typeof(LocalStoredUsers), typeof(Options));


        public static readonly DependencyProperty UserAutoCalculationTemplatesProperty =
            DependencyProperty.Register("UserAutoCalculationTemplates",
                typeof(ObservableCollection<AutoCalculationTemplate>), typeof(Options));

        private bool? _allSelectedCalculationTemplates = false;

        private bool _allSelectedCalculationTemplatesChanging;
        private bool? _allSelectedStoredUsers = false;

        private bool _allSelectedStoredUsersChanging;
        private AutoCalculationTemplate _currentAutoCalculationTemplate;
        private readonly ILogger _logger;
        private readonly Notifier _notifier;
        private readonly User _user;
        public bool ShowContourOnPreviewIsChanged;


        public bool ShowLinesOnPreviewIsChanged;

        public Options(Notifier notifier, ILogger logger, User user)
        {
            _notifier = notifier;
            _logger = logger;
            _user = user;
            InitializeComponent();
            InitializePaths();
            InitilizeUserTemplates();

            if (!string.IsNullOrEmpty(Settings.Default.StoredUsers))
            {
                LocalStoredUsers =
                    JsonSerializeProvider.DeserializeFromString<LocalStoredUsers>(Settings.Default.StoredUsers);

                StoredUsers.ItemsSource = LocalStoredUsers.Users;
            }

            OptionsLoadingComplete();
        }

        public LocalStoredUsers LocalStoredUsers
        {
            get => (LocalStoredUsers) GetValue(LocalStoredUsersProperty);
            set => SetValue(LocalStoredUsersProperty, value);
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

        public ObservableCollection<AutoCalculationTemplate> UserAutoCalculationTemplates
        {
            get => (ObservableCollection<AutoCalculationTemplate>) GetValue(UserAutoCalculationTemplatesProperty);
            set => SetValue(UserAutoCalculationTemplatesProperty, value);
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


        public event PropertyChangedEventHandler PropertyChanged;

        private void AllSelectedChanged()
        {
            if (LocalStoredUsers.Users != null)
            {
                if (_allSelectedStoredUsersChanging) return;

                try
                {
                    _allSelectedStoredUsersChanging = true;

                    if (AllSelectedStoredUsers == true)
                        foreach (var userSeries in LocalStoredUsers.Users)
                            userSeries.IsChecked = true;
                    else if (AllSelectedStoredUsers == false)
                        foreach (var userSeries in LocalStoredUsers.Users)
                            userSeries.IsChecked = false;
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

        private void EntryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(LocalStoredUser.IsChecked))
                RecheckAllSelected();
        }

        private void AllSelectedCalculationTemplatesChanged()
        {
            if (UserAutoCalculationTemplates != null)
            {
                if (_allSelectedCalculationTemplatesChanging) return;

                try
                {
                    _allSelectedCalculationTemplatesChanging = true;

                    if (AllSelectedCalculationTemplates == true)
                        foreach (var autoCalculationTemplate in UserAutoCalculationTemplates)
                            autoCalculationTemplate.IsChecked = true;
                    else if (AllSelectedCalculationTemplates == false)
                        foreach (var autoCalculationTemplate in UserAutoCalculationTemplates)
                            autoCalculationTemplate.IsChecked = false;
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

        private void EntryOnPropertyCalculationTemplatesChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(AutoCalculationTemplate.IsChecked))
                RecheckAllSelectedCalculationTemplates();
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void InitilizeUserTemplates()
        {
            if (!string.IsNullOrEmpty(Settings.Default.AutoCalculationTemplates))
                UserAutoCalculationTemplates =
                    JsonSerializeProvider.DeserializeFromString<ObservableCollection<AutoCalculationTemplate>>(
                        Settings.Default.AutoCalculationTemplates);
            else
                UserAutoCalculationTemplates = new ObservableCollection<AutoCalculationTemplate>();

            AutoCalculaionTemplates.ItemsSource = UserAutoCalculationTemplates;
        }

        private void InitializePaths()
        {
            ShowLinesOnPreview.IsChecked = Settings.Default.ShowLinesOnPreview;
            ShowContourOnPreview.IsChecked = Settings.Default.ShowContourOnPreview;
        }

        private void UpdateOptions(OptionsEnum option, object value)
        {
            switch (option)
            {
                case OptionsEnum.ShowLinesOnPreview:
                    Settings.Default.ShowLinesOnPreview = (bool) value;
                    Settings.Default.Save();
                    ShowLinesOnPreviewIsChanged = true;
                    break;
                case OptionsEnum.ShowContourOnPreview:
                    Settings.Default.ShowContourOnPreview = (bool) value;
                    Settings.Default.Save();
                    ShowContourOnPreviewIsChanged = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        private void ShowLinesOnPreview_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            UpdateOptions(OptionsEnum.ShowLinesOnPreview, checkBox.IsChecked);
        }

        private void ShowLinesOnPreview_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            UpdateOptions(OptionsEnum.ShowLinesOnPreview, checkBox.IsChecked);
        }

        private void ShowContourOnPreview_OnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            UpdateOptions(OptionsEnum.ShowContourOnPreview, checkBox.IsChecked);
        }

        private void ShowContourOnPreview_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            UpdateOptions(OptionsEnum.ShowContourOnPreview, checkBox.IsChecked);
        }

        private void DeleteLocalUser_Click(object sender, RoutedEventArgs e)
        {
            LocalStoredUsers.Users.RemoveAt(StoredUsers.SelectedIndex);

            Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString(LocalStoredUsers);

            Settings.Default.Save();
        }

        private void DeleteAllStoredUsers_OnClick(object sender, RoutedEventArgs e)
        {
            var checkedCount = 0;

            var messageBoxResult =
                MessageBox.Show(
                    checkedCount > 0
                        ? "Удалить выбранных локальных пользователей?"
                        : "Удалить всех локальныйх пользователей?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                for (var i = LocalStoredUsers.Users.Count - 1; i >= 0; i--)
                {
                    if (checkedCount > 0 && !LocalStoredUsers.Users[i].IsChecked) continue;

                    LocalStoredUsers.Users.RemoveAt(i);

                    Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString(LocalStoredUsers);

                    Settings.Default.Save();
                }
        }

        private void AddTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AutoCalculationTemplateTitle.Text))
            {
                UserAutoCalculationTemplates.Add(new AutoCalculationTemplate
                {
                    Id = Guid.NewGuid(),
                    Title = AutoCalculationTemplateTitle.Text,
                    Parameters = new AutoCalculationParameters
                    {
                        Ksize = 1
                    }
                });

                Settings.Default.AutoCalculationTemplates =
                    JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

                Settings.Default.Save();
            }
            else
            {
                _notifier.ShowInformation("Введите название шаблона.");
            }
        }

        private void DeleteSingleTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            UserAutoCalculationTemplates.RemoveAt(AutoCalculaionTemplates.SelectedIndex);

            Settings.Default.AutoCalculationTemplates =
                JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

            Settings.Default.Save();
        }

        private void DeleteTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var checkedCount = 0;

            var messageBoxResult =
                MessageBox.Show(
                    checkedCount > 0 ? "Удалить выбранные шаблоны авторасчета?" : "Удалить все шаблоны авторасчета?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                for (var i = UserAutoCalculationTemplates.Count - 1; i >= 0; i--)
                {
                    if (checkedCount > 0 && !UserAutoCalculationTemplates[i].IsChecked) continue;

                    UserAutoCalculationTemplates.RemoveAt(i);

                    Settings.Default.AutoCalculationTemplates =
                        JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

                    Settings.Default.Save();
                }
        }

        private async void ImportTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
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
                        var deserializedTemplate =
                            await JsonSerializeProvider.DeserializeFromFileAsync<AutoCalculationTemplate>(fileName);

                        if (deserializedTemplate.Parameters != null)
                        {

                            deserializedTemplate.Id = Guid.NewGuid();

                            UserAutoCalculationTemplates.Add(deserializedTemplate);

                            Settings.Default.AutoCalculationTemplates =
                                JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

                            Settings.Default.Save();

                            _logger.LogInfo(new LogEntry
                            {
                                Username = _user.Email,
                                LogCategory = LogCategory.Options,
                                Message = $"Шаблон авторасчета {deserializedTemplate.Title} добавлен."
                            });
                            _notifier.ShowSuccess($"Шаблон авторасчета {deserializedTemplate.Title} добавлен.");
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    catch (InvalidOperationException)
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
                var saveFileDialog = new VistaFolderBrowserDialog
                {
                    UseDescriptionForTitle = true,
                    Description = "Выберите папку для сохранения..."
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    OptionsIsLoading();

                    var checkedCount = UserAutoCalculationTemplates.Count(x => x.IsChecked);

                    var pbuHandle1 = pbu.New(ProgressBar, 0,
                        checkedCount > 0 ? checkedCount : UserAutoCalculationTemplates.Count, 0);

                    foreach (var autoCalculationTemplate in UserAutoCalculationTemplates)
                    {
                        if (checkedCount > 0 && !autoCalculationTemplate.IsChecked) continue;


                        await JsonSerializeProvider.SerializeToFileAsync(autoCalculationTemplate,
                            $"{saveFileDialog.SelectedPath}\\{autoCalculationTemplate.Title}.dplate");

                        _logger.LogInfo(new LogEntry
                        {
                            Username = _user.Email,
                            LogCategory = LogCategory.Options,
                            Message = $"файл {autoCalculationTemplate.Title}.dplate сохранен на диске."
                        });
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

        private void AutoCalculaionTemplates_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditingElement is TextBox t)
                if (!string.IsNullOrWhiteSpace(t.Text))
                {
                    if (ValidationHelper.IsTextAllowed(t.Text))
                    {
                        Settings.Default.AutoCalculationTemplates =
                            JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

                        Settings.Default.Save();
                    }
                    else
                    {
                        _notifier.ShowInformation("Введите целое число больше ноля.");
                        e.Cancel = true;
                    }
                }
        }

        private void AutoCalculationTypeCombo_OnDropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox != null && comboBox.SelectedIndex != -1)
            {
                _currentAutoCalculationTemplate.TemplateType = (CalculationVariants) comboBox.SelectedIndex;

                Settings.Default.AutoCalculationTemplates =
                    JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

                Settings.Default.Save();
            }
        }

        private void AutoCalculaionTemplates_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            if (dataGrid != null && dataGrid.SelectedIndex != -1)
                _currentAutoCalculationTemplate = UserAutoCalculationTemplates[dataGrid.SelectedIndex];
        }

        private void UpDownBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Settings.Default.AutoCalculationTemplates =
                JsonSerializeProvider.SerializeToString(UserAutoCalculationTemplates);

            Settings.Default.Save();
        }
    }
}