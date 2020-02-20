using DDrop.BE.Models;
using DDrop.BL.Calculation.DropletSizeCalculator;
using DDrop.Utility.ExcelOperations;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.PythonOperations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _allSelectedSeriesChanging;
        private bool? _allSelectedSeries = false;

        private void AllSelectedChanged()
        {
            if (User.UserSeries != null)
            {
                if (_allSelectedSeriesChanging) return;

                try
                {
                    _allSelectedSeriesChanging = true;

                    if (AllSelectedSeries == true)
                    {
                        foreach (var userSeries in User.UserSeries)
                            userSeries.IsChecked = true;
                    }
                    else if (AllSelectedSeries == false)
                    {
                        foreach (var userSeries in User.UserSeries)
                            userSeries.IsChecked = false;
                    }
                }
                finally
                {
                    _allSelectedSeriesChanging = false;
                }
            }
            else
            {
                AllSelectedSeries = false;
            }
        }

        private void RecheckAllSelected()
        {
            if (_allSelectedSeriesChanging) return;

            try
            {
                _allSelectedSeriesChanging = true;

                if (User.UserSeries.All(e => e.IsChecked))
                    AllSelectedSeries = true;
                else if (User.UserSeries.All(e => !e.IsChecked))
                    AllSelectedSeries = false;
                else
                    AllSelectedSeries = null;
            }
            finally
            {
                _allSelectedSeriesChanging = false;
            }
        }

        public bool? AllSelectedSeries
        {
            get => _allSelectedSeries;
            set
            {
                if (value == _allSelectedSeries) return;
                _allSelectedSeries = value;

                AllSelectedChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedSeries"));
            }
        }

        private void EntryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Series.IsChecked))
                RecheckAllSelected();
        }

        PythonProvider pythonProvider = new PythonProvider();

        public static readonly DependencyProperty CurrentSeriesProperty = DependencyProperty.Register("CurrentSeries", typeof(Series), typeof(MainWindow));
        public static readonly DependencyProperty CurrentDropPhotoProperty = DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(MainWindow));

        public static readonly DependencyProperty ReferenceImageProperty = DependencyProperty.Register("ReferenceImage", typeof(ImageSource), typeof(MainWindow));
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(User), typeof(MainWindow));
        public static readonly DependencyProperty ParticularSeriesIndexProperty = DependencyProperty.Register("ParticularSeriesIndex", typeof(int), typeof(MainWindow));

        public DropPhoto CurrentDropPhoto
        {
            get { return (DropPhoto)GetValue(CurrentDropPhotoProperty); }
            set
            {
                SetValue(CurrentDropPhotoProperty, value);
            }
        }

        public Series CurrentSeries
        {
            get { return (Series)GetValue(CurrentSeriesProperty); }
            set
            {
                SetValue(CurrentSeriesProperty, value);
            }
        }

        public ImageSource ReferenceImage
        {
            get { return (ImageSource)GetValue(ReferenceImageProperty); }
            set
            {
                SetValue(ReferenceImageProperty, value);
            }
        }

        public User User
        {
            get { return (User)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public int ParticularSeriesIndex
        {
            get { return (int)GetValue(ParticularSeriesIndexProperty); }
            set { SetValue(ParticularSeriesIndexProperty, value); OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex")); }
        }

        public MainWindow()
        {
            InitializeComponent();
            AppMainWindow.Show();
            Login login = new Login();
            login.Owner = AppMainWindow;
            login.ShowDialog();
            User = login.UserLogin;

            if (User == null)
            {
                Close();
            }
            else if (User.IsLoggedIn)
            {
                AccountMenuItem.Visibility = Visibility.Visible;
                LogInMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Visible;
            }
        }

        private Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Закрыть приложение?", "Подтверждение выхода", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                Close();
        }

        private void Options_OnClick(object sender, RoutedEventArgs e)
        {
            Options options = new Options();
            options.ShowDialog();
        }

        private void MenuItemChoosePhotos_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool unique = true;

            openFileDialog.Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            openFileDialog.AddExtension = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    if (CurrentSeries.DropPhotosSeries == null)
                    {
                        CurrentSeries.DropPhotosSeries = new ObservableCollection<DropPhoto>();
                        Photos.ItemsSource = CurrentSeries.DropPhotosSeries;
                    }

                    var imageForAdding = new DropPhoto
                    {
                        Name = openFileDialog.SafeFileNames[i],
                        Path = openFileDialog.FileNames[i],
                        Content = File.ReadAllBytes(openFileDialog.FileNames[i]),
                        Drop = new Drop(CurrentSeries)
                    };

                    foreach (var dropImage in CurrentSeries.DropPhotosSeries)
                    {
                        if (dropImage.Path == imageForAdding.Path)
                        {
                            unique = false;
                            notifier.ShowWarning($"Фотография {imageForAdding.Name} уже добавлена.");
                            break;
                        }
                    }

                    if (unique)
                    {
                        CurrentSeries.DropPhotosSeries.Add(imageForAdding);
                    }
                }
            }
        }

        private void ChooseReference_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;
            if (openFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.Reference = openFileDialog.FileName;
                MainWindowPixelDrawer.TwoLineMode = false;
                if (CurrentSeries.ReferencePhotoForSeries.Line != null)
                {
                    MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                    MainWindowPixelDrawer.PixelsInMillimeter = "";
                    CurrentSeries.ReferencePhotoForSeries.Line = null;
                    CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                }
                CurrentSeries.ReferencePhotoForSeries.Line = new Line();
                CurrentSeries.ReferencePhotoForSeries.Content = ImageConverter.FileToByteArray(openFileDialog.FileName);
                ReferenceImage = ImageConverter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);
            }
        }

        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                for (int i = 0; i < CurrentSeries.DropPhotosSeries.Count; ++i)
                {
                    CurrentSeries.DropPhotosSeries[i] = pythonProvider.RunScript(CurrentSeries.DropPhotosSeries[i].Path, Properties.Settings.Default.SaveTo,
                        CurrentSeries.DropPhotosSeries[i], Properties.Settings.Default.ScriptToRun,
                                                             Properties.Settings.Default.Interpreter);
                    CurrentSeries.DropPhotosSeries[i].Drop = DropletSizeCalculator.PerformCalculation(
                        Convert.ToInt32(PixelsInMillimeterTextBox.Text), CurrentSeries.DropPhotosSeries[i].XDiameterInPixels,
                        CurrentSeries.DropPhotosSeries[i].YDiameterInPixels, CurrentSeries);
                }

                notifier.ShowSuccess("Расчет завершен.");
            }
            else
                notifier.ShowInformation("Выберите фотографии для расчета.");
        }

        private void Photos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhoto selectedFile = (DropPhoto)Photos.SelectedItem;
            if (selectedFile != null)
            {
                ImgCurrent.Source = ImageConverter.LoadImage(selectedFile.Content);
            }
            else
                ImgCurrent.Source = null;
        }

        private void DeleteAllInputPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Закончить работу с этими фотографиями?", "Подтверждение", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    CurrentSeries.DropPhotosSeries.Clear();
                    notifier.ShowSuccess("Фотографии выгружены из памяти.");
                }
            }
            else
                notifier.ShowInformation("Нет загруженных фотографий.");
        }

        private void DeleteSingleInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSeries.DropPhotosSeries.RemoveAt(Photos.SelectedIndex);
        }

        private void EditInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PixelsInMillimeterTextBox.Text) && PixelsInMillimeterTextBox.Text != "0")
            {
                CurrentDropPhoto = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex];
                CurrentDropPhoto.HorizontalLine = new List<Line>();
                ManualEdit manualEdit = new ManualEdit(CurrentDropPhoto);
                manualEdit.ShowDialog();
                CurrentDropPhoto.Drop = DropletSizeCalculator.PerformCalculation(
                    Convert.ToInt32(PixelsInMillimeterTextBox.Text), CurrentDropPhoto.XDiameterInPixels,
                    CurrentDropPhoto.YDiameterInPixels, CurrentSeries);
            }
            else
            {
                notifier.ShowInformation("Заполните поле \"Пикселей в миллиметре\".");
            }
        }

        private void AppMainWindow_Closing(object sender, CancelEventArgs e)
        {
            notifier.Dispose();
        }

        private void DeleteReferencePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Удалить референсный снимок?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                MainWindowPixelDrawer.PixelsInMillimeter = "";
                CurrentSeries.ReferencePhotoForSeries.Line = null;
                CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                CurrentSeries.ReferencePhotoForSeries.Content = null;
                ReferenceImage = null;
            }
        }

        private void SeriesDrawerSwap(Series old)
        {
            if (old.ReferencePhotoForSeries?.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Remove(old.ReferencePhotoForSeries.Line);

            if (CurrentSeries.ReferencePhotoForSeries.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentSeries.ReferencePhotoForSeries.Line);
        }

        private void LoginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Owner = this;
            login.ShowDialog();
            login.UserLogin = User;
            if (User.IsLoggedIn)
            {
                AccountMenuItem.Visibility = Visibility.Visible;
                LogInMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Visible;
            }

        }

        private void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Account accountMenu = new Account(User);
            accountMenu.ShowDialog();
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AccountMenuItem.Visibility = Visibility.Collapsed;
            LogOutMenuItem.Visibility = Visibility.Collapsed;
            LogInMenuItem.Visibility = Visibility.Visible;
            Login login = new Login();
            login.Owner = this;
            login.ShowDialog();
            User = login.UserLogin;

            if (User == null)
            {
                Close();
            }
            else
            {
                AccountMenuItem.Visibility = Visibility.Visible;
                LogInMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Visible;
            }
        }

        private void PythonOption_OnClick(object sender, RoutedEventArgs e)
        {
            CSharpMenuItem.IsChecked = false;
        }

        private void CSharpOption_OnClick(object sender, RoutedEventArgs e)
        {
            PythonMenuItem.IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void AddNewSeries_OnClick(object sender, RoutedEventArgs e)
        {
            string seriesTitle;

            if (User.UserSeries == null)
            {
                User.UserSeries = new ObservableCollection<Series>();
                SeriesDataGrid.ItemsSource = User.UserSeries;
            }

            if (!string.IsNullOrWhiteSpace(OneLineSetterValue.Text))
            {
                seriesTitle = OneLineSetterValue.Text;

                Series seriesToAdd = new Series()
                {
                    Title = seriesTitle,
                    ReferencePhotoForSeries = new ReferencePhoto()
                };

                seriesToAdd.PropertyChanged += EntryOnPropertyChanged;

                User.UserSeries.Add(seriesToAdd);
                OneLineSetterValue.Text = "";
            }
            else
            {
                notifier.ShowInformation("Введите название серии.");
            }
        }

        private void SeriesDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (User.UserSeries.Count > 0 && SeriesDataGrid.SelectedItem != null)
            {
                Series old = new Series();

                if (e.RemovedItems.Count > 0)
                    old = e.RemovedItems[0] as Series;
                CurrentSeries = User.UserSeries[SeriesDataGrid.SelectedIndex];

                SeriesDrawerSwap(old);
                SingleSeries.IsEnabled = true;
                Photos.ItemsSource = null;
                SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                ReferenceImage = null;
                ParticularSeriesIndex = SeriesDataGrid.SelectedIndex;

                if (CurrentSeries?.ReferencePhotoForSeries?.Content != null)
                {
                    ReferenceImage = ImageConverter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);
                }
            }
            else
            {
                SingleSeries.IsEnabled = false;
            }
        }

        private void DeleteSingleSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            User.UserSeries.RemoveAt(SeriesDataGrid.SelectedIndex);
            Photos.ItemsSource = null;
            ImgPreview.Source = null;
            SeriesPreviewDataGrid.ItemsSource = null;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tc = sender as TabControl;

            if (tc != null)
            {
                TabItem item = (TabItem)tc.SelectedItem;
                if (item.Name == "SingleSeries")
                {
                    Photos.ItemsSource = CurrentSeries.DropPhotosSeries;
                }
                else if (item.Name == "SeriesManager")
                {
                    if (SeriesDataGrid.SelectedItems.Count > 0 && CurrentSeries?.DropPhotosSeries != null)
                        SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                }
            }
        }

        private void SeriesPreviewDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhoto selectedFile = (DropPhoto)SeriesPreviewDataGrid.SelectedItem;
            if (selectedFile != null)
            {
                ImgPreview.Source = ImageConverter.LoadImage(selectedFile.Content);
            }
            else
                ImgPreview.Source = null;
        }

        private void SeriesNotifier_Click(object sender, RoutedEventArgs e)
        {
            if (!SingleSeries.IsEnabled)
                notifier.ShowInformation("Выберите серию.");
        }

        private void IntervalBetweenPhotos_TextChanged(object sender, TextChangedEventArgs e)
        {
            var intervalBetweenPhotosTextBox = sender as TextBox;

            if (int.TryParse(intervalBetweenPhotosTextBox.Text, out int intervalBetweenPhotos))
                CurrentSeries.IntervalBetweenPhotos = intervalBetweenPhotos;
        }

        private void ExportSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog.AddExtension = true;
                saveFileDialog.CheckPathExists = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExcelOperations.CreateSingleSeriesExcelFile(User, saveFileDialog.FileName);

                    notifier.ShowSuccess($"Файл {saveFileDialog.SafeFileName} успешно сохранен.");
                }
            }
            else
                notifier.ShowInformation("Нет загруженных фотографий.");
        }

        private void DeleteSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries != null)
            {
                bool isAnyChecked = User.UserSeries.Any(x => x.IsChecked);

                if (isAnyChecked)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить выбранные серии?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        for (int i = User.UserSeries.Count - 1; i >=0; i--)
                        {
                            if (User.UserSeries[i].IsChecked)
                                User.UserSeries.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить все серии?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                        User.UserSeries.Clear();
                }
            }
            else
            {
                notifier.ShowInformation("Нет серий для удаления.");
            }
        }

        private void SingleSeriesPlotTabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;

            if (tabItem.IsEnabled)
            {
                notifier.ShowSuccess("Новый график построен");
            }
            else
            {
                notifier.ShowInformation("Для построения графика необходимо указать интервал между снимками и произвести расчет для минимум двух снимков.");
            }
        }
    }
}
