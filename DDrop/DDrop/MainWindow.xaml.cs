using DDrop.BE.Models;
using DDrop.BE.Models.Entities;
using DDrop.BL.Calculation.DropletSizeCalculator;
using DDrop.BL.Series;
using DDrop.Utility.ExcelOperations;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.PythonOperations;
using DDrop.Utility.SeriesLocalStorageOperations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.DAL;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using pbu = RFM.RFM_WPFProgressBarUpdate;
using DDrop.BL.DropPhoto;
using System.Data.Entity;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variable Declaration
        private bool _allSelectedSeriesChanging;
        private bool? _allSelectedSeries = false;
        private bool _allSelectedPhotosChanging;
        private bool? _allSelectedPhotos = false;
        private readonly ISeriesBL _seriesBL;
        private readonly IDropPhotoBL _dropPhotoBL;
        readonly PythonProvider pythonProvider = new PythonProvider();
        DDropContext _dDropContext = new DDropContext();

        public static readonly DependencyProperty CurrentSeriesProperty = DependencyProperty.Register("CurrentSeries", typeof(SeriesViewModel), typeof(MainWindow));
        public static readonly DependencyProperty CurrentDropPhotoProperty = DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhotoViewModel), typeof(MainWindow));
        public static readonly DependencyProperty ReferenceImageProperty = DependencyProperty.Register("ReferenceImage", typeof(ImageSource), typeof(MainWindow));
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(UserViewModel), typeof(MainWindow));
        public static readonly DependencyProperty ParticularSeriesIndexProperty = DependencyProperty.Register("ParticularSeriesIndex", typeof(int), typeof(MainWindow));

        private readonly Notifier notifier = new Notifier(cfg =>
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

        #endregion

        #region Properties

        public DropPhotoViewModel CurrentDropPhoto
        {
            get { return (DropPhotoViewModel)GetValue(CurrentDropPhotoProperty); }
            set
            {
                SetValue(CurrentDropPhotoProperty, value);
            }
        }

        public SeriesViewModel CurrentSeries
        {
            get { return (SeriesViewModel)GetValue(CurrentSeriesProperty); }
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

        public UserViewModel User
        {
            get { return (UserViewModel)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public int ParticularSeriesIndex
        {
            get { return (int)GetValue(ParticularSeriesIndexProperty); }
            set { SetValue(ParticularSeriesIndexProperty, value); OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        public MainWindow(ISeriesBL seriesBL, IDropPhotoBL dropPhotoBL)
        {
            InitializeComponent();
            _seriesBL = seriesBL;
            _dropPhotoBL = dropPhotoBL;
            AppMainWindow.Show();
            Login login = new Login(_dDropContext, notifier, _seriesBL)
            {
                Owner = AppMainWindow
            };
            login.ShowDialog();

            if (login.LoginSucceeded)
            {
                User = login.UserLogin;
                User.UserSeries = _seriesBL.ConvertSeriesToSeriesViewModel(_dDropContext.Users.FirstOrDefault(x => x.UserId == User.UserId).UserSeries.Select(x => new Series
                {
                    Title = x.Title,
                    SeriesId = x.SeriesId,
                    AddedDate = x.AddedDate,
                    IntervalBetweenPhotos = x.IntervalBetweenPhotos,
                    ReferencePhotoForSeries = new ReferencePhoto
                    {
                        Name = x.ReferencePhotoForSeries.Name,
                        PixelsInMillimeter = x.ReferencePhotoForSeries.PixelsInMillimeter,
                        ReferencePhotoId = x.ReferencePhotoForSeries.ReferencePhotoId,
                        SimpleLine = x.ReferencePhotoForSeries.SimpleLine
                    } ?? null,
                    DropPhotosSeries = x.DropPhotosSeries.Select(s => new DropPhoto
                    {
                        Name = s.Name,
                        AddedDate = s.AddedDate,
                        Drop = s.Drop,
                        DropPhotoId = s.DropPhotoId,
                        SimpleHorizontalLine = s.SimpleHorizontalLine,
                        SimpleVerticalLine = s.SimpleVerticalLine,
                        XDiameterInPixels = s.XDiameterInPixels,
                        YDiameterInPixels = s.YDiameterInPixels,
                        ZDiameterInPixels = s.ZDiameterInPixels
                    }).ToList()
                }).ToList(), User);
                SeriesDataGrid.ItemsSource = User.UserSeries;
            }

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

        #region Series

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
            if (args.PropertyName == nameof(SeriesViewModel.IsChecked))
                RecheckAllSelected();
        }

        private void SeriesDrawerSwap(SeriesViewModel old)
        {
            if (old.ReferencePhotoForSeries?.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Remove(old.ReferencePhotoForSeries.Line);

            if (CurrentSeries.ReferencePhotoForSeries?.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentSeries.ReferencePhotoForSeries.Line);
        }

        private async void AddNewSeries_OnClick(object sender, RoutedEventArgs e)
        {
            string seriesTitle;

            if (User.UserSeries == null)
            {
                User.UserSeries = new ObservableCollection<SeriesViewModel>();
                SeriesDataGrid.ItemsSource = User.UserSeries;
            }

            if (!string.IsNullOrWhiteSpace(OneLineSetterValue.Text))
            {
                seriesTitle = OneLineSetterValue.Text;

                SeriesViewModel seriesToAdd = new SeriesViewModel(User)
                {
                    SeriesId = Guid.NewGuid(),
                    Title = seriesTitle,                    
                    AddedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    ReferencePhotoForSeries = new ReferencePhotoViewModel
                    {
                        ReferencePhotoId = Guid.NewGuid()
                    }
                };

                seriesToAdd.PropertyChanged += EntryOnPropertyChanged;

                User.UserSeries.Add(seriesToAdd);
                _dDropContext.Users.FirstOrDefault(x => x.UserId == User.UserId).UserSeries.Add(_seriesBL.SingleSeriesViewModelToSingleSeries(seriesToAdd));
                await _dDropContext.SaveChangesAsync();
                SeriesDataGrid.ItemsSource = User.UserSeries;
                OneLineSetterValue.Text = "";
                await _dDropContext.SaveChangesAsync();
                notifier.ShowSuccess($"Серия {seriesToAdd.Title} добавлена.");
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
                SeriesViewModel old = new SeriesViewModel(User);

                if (e.RemovedItems.Count > 0)
                    old = e.RemovedItems[0] as SeriesViewModel;
                CurrentSeries = User.UserSeries[SeriesDataGrid.SelectedIndex];

                SeriesDrawerSwap(old);
                SingleSeries.IsEnabled = true;
                Photos.ItemsSource = null;
                SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                ReferenceImage = null;
                ParticularSeriesIndex = SeriesDataGrid.SelectedIndex;

                if (CurrentSeries?.ReferencePhotoForSeries?.Content != null)
                {
                    ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);
                }
            }
            else
            {
                SingleSeries.IsEnabled = false;
            }
        }

        private async void DeleteSingleSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                notifier.ShowSuccess($"Серия {User.UserSeries[SeriesDataGrid.SelectedIndex].Title} была удалена.");

                using (_dDropContext = new DDropContext())
                {
                    var id = User.UserSeries[SeriesDataGrid.SelectedIndex].SeriesId;
                    var series = await _dDropContext.Series
                        .Include(x => x.DropPhotosSeries)
                        .Include(x => x.ReferencePhotoForSeries)
                        .FirstOrDefaultAsync(x => x.SeriesId == id);
                    if (series != null)
                    {
                        if (series.DropPhotosSeries != null)
                        {
                            foreach (var item in series.DropPhotosSeries)
                            {
                                _dDropContext.Drops.Remove(item.Drop);
                                if (item.SimpleVerticalLine != null)
                                {
                                    _dDropContext.SimpleLines.Remove(item?.SimpleVerticalLine);
                                }

                                if (item.SimpleHorizontalLine != null)
                                {
                                    _dDropContext.SimpleLines.Remove(item?.SimpleHorizontalLine);
                                }                                                              
                            }

                        }

                        if (series.ReferencePhotoForSeries.SimpleLine != null)
                        {
                            _dDropContext.SimpleLines.Remove(series.ReferencePhotoForSeries.SimpleLine);
                        }
                        
                        _dDropContext.ReferencePhotos.Remove(series?.ReferencePhotoForSeries);                        
                    }

                    _dDropContext.Users.FirstOrDefault(x => x.UserId == User.UserId).UserSeries.Remove(_seriesBL.SingleSeriesViewModelToSingleSeries(User.UserSeries[SeriesDataGrid.SelectedIndex]));
                    await _dDropContext.SaveChangesAsync();
                }

                User.UserSeries.RemoveAt(SeriesDataGrid.SelectedIndex);
                Photos.ItemsSource = null;
                ImgPreview.Source = null;
                SeriesPreviewDataGrid.ItemsSource = null;
            }
        }
        private void SeriesPreviewDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhotoViewModel selectedFile = (DropPhotoViewModel)SeriesPreviewDataGrid.SelectedItem;
            if (selectedFile != null)
            {
                ImgPreview.Source = ImageInterpreter.LoadImage(selectedFile.Content);
            }
            else
                ImgPreview.Source = null;
        }

        private void CombinedSeriesPlot_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;

            if (tabItem.IsEnabled && User != null)
            {
                notifier.ShowSuccess("Новый общий график серий построен");
            }
        }

        private void ExportSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Any(x => x.IsChecked))
            {
                if (User.IsAnySelectedSeriesCanDrawPlot)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        AddExtension = true,
                        CheckPathExists = true
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        ExcelOperations.CreateSingleSeriesExcelFile(User, saveFileDialog.FileName);

                        notifier.ShowSuccess($"Файл {saveFileDialog.SafeFileName} успешно сохранен.");
                    }
                }
                else
                {
                    notifier.ShowInformation("Нельзя построить график для выбранных серий.");
                }
            }
            else
            {
                notifier.ShowInformation("Нет выбранных серий.");
            }
        }

        private void DeleteSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Count > 0)
            {
                bool isAnyChecked = User.UserSeries.Any(x => x.IsChecked);

                if (isAnyChecked)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить выбранные серии?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        for (int i = User.UserSeries.Count - 1; i >= 0; i--)
                        {
                            if (User.UserSeries[i].IsChecked)
                                User.UserSeries.RemoveAt(i);
                        }

                        notifier.ShowSuccess("Выбранные серии были удалены.");
                    }
                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить все серии?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                        User.UserSeries.Clear();

                    notifier.ShowSuccess("Все серии были удалены.");
                }

                SeriesPreviewDataGrid.SelectedIndex = -1;
            }
            else
            {
                notifier.ShowInformation("Нет серий для удаления.");
            }
        }

        private void SingleSeriesPlotTabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;

            if (tabItem.IsEnabled && User != null)
            {
                notifier.ShowSuccess("Новый график построен");
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc)
            {
                TabItem item = (TabItem)tc.SelectedItem;

                if (item.Name == "SingleSeries")
                {
                    Photos.ItemsSource = CurrentSeries.DropPhotosSeries;

                    if (!CurrentSeries.CanDrawPlot && SingleSeriesTabControl.SelectedIndex == 2)
                    {
                        SingleSeriesTabControl.SelectedIndex = 0;
                    }
                }
                else if (item.Name == "SeriesManager")
                {
                    if (SeriesDataGrid.SelectedItems.Count > 0 && CurrentSeries?.DropPhotosSeries != null)
                        SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                }
            }
        }

        private async void ExportSeriesLocal_ClickAsync(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "DDrop files (*.ddrops)|*.ddrops|All files (*.*)|*.*",
                AddExtension = true,
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ProgressBar.IsIndeterminate = true;                
                await _seriesBL.ExportSeriesLocalAsync(saveFileDialog.FileName, User);
                ProgressBar.IsIndeterminate = false;
                notifier.ShowSuccess($"{saveFileDialog.SafeFileName} сохранен на диске.");
            }
        }

        private async void ImportLocalSeries_ClickAsync(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "DDrop files (*.ddrops)|*.ddrops|All files (*.*)|*.*",
                Multiselect = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            ObservableCollection<SeriesViewModel> addSeriesViewModel = new ObservableCollection<SeriesViewModel>();

            if (openFileDialog.ShowDialog() == true)
            {
                ProgressBar.IsIndeterminate = true;
                addSeriesViewModel = await _seriesBL.ImportLocalSeriesAsync(openFileDialog.FileName, User);
                ProgressBar.IsIndeterminate = false;
            
                AddSeries addSeries = new AddSeries(addSeriesViewModel);
                addSeries.ShowDialog();

                if (addSeriesViewModel.Count != 0)
                {
                    bool isAnyChecked = addSeriesViewModel.Any(x => x.IsCheckedForAdd);

                    foreach (var seriesViewModel in addSeriesViewModel)
                    {
                        if (isAnyChecked)
                        {
                            if (seriesViewModel.IsCheckedForAdd)
                            {
                                User.UserSeries.Add(seriesViewModel);
                                notifier.ShowSuccess($"Серия {seriesViewModel.Title} добавлена.");
                            }
                        }
                        else
                        {
                            User.UserSeries.Add(seriesViewModel);
                            notifier.ShowSuccess($"Серия {seriesViewModel.Title} добавлена.");
                        }
                    }

                
                    notifier.ShowSuccess($"Все серии успешно добавлены.");
                }
            }
            if (SeriesDataGrid.ItemsSource == null)
                SeriesDataGrid.ItemsSource = User.UserSeries;
            
        }

        #endregion

        #region Drop Photos

        private void AllSelectedPhotosChanged()
        {
            if (CurrentSeries.DropPhotosSeries != null)
            {
                if (_allSelectedPhotosChanging) return;

                try
                {
                    _allSelectedPhotosChanging = true;

                    if (AllSelectedPhotos == true)
                    {
                        foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                            dropPhoto.IsChecked = true;
                    }
                    else if (AllSelectedPhotos == false)
                    {
                        foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                            dropPhoto.IsChecked = false;
                    }
                }
                finally
                {
                    _allSelectedPhotosChanging = false;
                }
            }
            else
            {
                AllSelectedPhotos = false;
            }
        }

        private void RecheckAllSelectedPhotos()
        {
            if (_allSelectedPhotosChanging) return;

            try
            {
                _allSelectedPhotosChanging = true;

                if (CurrentSeries.DropPhotosSeries.All(e => e.IsChecked))
                    AllSelectedPhotos = true;
                else if (CurrentSeries.DropPhotosSeries.All(e => !e.IsChecked))
                    AllSelectedPhotos = false;
                else
                    AllSelectedPhotos = null;
            }
            finally
            {
                _allSelectedPhotosChanging = false;
            }
        }

        public bool? AllSelectedPhotos
        {
            get => _allSelectedPhotos;
            set
            {
                if (value == _allSelectedPhotos) return;
                _allSelectedPhotos = value;

                AllSelectedPhotosChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedPhotos"));
            }
        }

        private void PhotosOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(DropPhotoViewModel.IsChecked))
                RecheckAllSelectedPhotos();
        }

        private async void MenuItemChoosePhotos_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*",
                Multiselect = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var pbuHandle1 = pbu.New(ProgressBar, 0, openFileDialog.FileNames.Length - 1, 0);
                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    pbu.CurValue[pbuHandle1] += 1;
                    if (CurrentSeries.DropPhotosSeries == null)
                    {
                        CurrentSeries.DropPhotosSeries = new ObservableCollection<DropPhotoViewModel>();
                        Photos.ItemsSource = CurrentSeries.DropPhotosSeries;
                    }

                    var imageForAdding = new DropPhotoViewModel
                    {
                        DropPhotoId = Guid.NewGuid(),
                        Name = openFileDialog.SafeFileNames[i],
                        Path = openFileDialog.FileNames[i],
                        Content = File.ReadAllBytes(openFileDialog.FileNames[i]),
                        AddedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    };
                    
                    if (ImageValidator.ValidateImage(imageForAdding.Content))
                    {
                        imageForAdding.PropertyChanged += PhotosOnPropertyChanged;
                        CurrentSeries.DropPhotosSeries.Add(imageForAdding);
                        CurrentSeries.DropPhotosSeries[CurrentSeries.DropPhotosSeries.Count - 1].Drop = new DropViewModel(CurrentSeries, CurrentSeries.DropPhotosSeries[CurrentSeries.DropPhotosSeries.Count - 1])
                        {
                            DropId = Guid.NewGuid()
                        };
                        _dDropContext.Users.FirstOrDefault(x => x.UserId == User.UserId).UserSeries.FirstOrDefault(x => x.SeriesId == CurrentSeries.SeriesId).DropPhotosSeries.Add(_dropPhotoBL.DropPhotoViewModelToDropPhoto(imageForAdding));
                        await _dDropContext.SaveChangesAsync();
                        notifier.ShowSuccess($"Снимок {imageForAdding.Name} добавлен.");                        
                    }
                    else
                    {
                        notifier.ShowError($"Файл {imageForAdding.Name} имеет неизвестный формат.");
                    }
                }

                notifier.ShowSuccess($"Новые снимки успешно добавлены.");
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }
        }

        private void Photos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhotoViewModel selectedFile = (DropPhotoViewModel)Photos.SelectedItem;
            if (selectedFile != null)
            {
                ImgCurrent.Source = ImageInterpreter.LoadImage(selectedFile.Content);
            }
            else
                ImgCurrent.Source = null;
        }

        private void DeleteInputPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                bool isAnyChecked = CurrentSeries.DropPhotosSeries.Any(x => x.IsChecked);

                if (isAnyChecked)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить выбранные снимки?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        for (int i = CurrentSeries.DropPhotosSeries.Count - 1; i >= 0; i--)
                        {
                            if (CurrentSeries.DropPhotosSeries[i].IsChecked)
                                CurrentSeries.DropPhotosSeries.RemoveAt(i);
                        }

                        notifier.ShowSuccess("Выбранные снимки удалены.");
                    }
                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить все снимки?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        CurrentSeries.DropPhotosSeries.Clear();
                        notifier.ShowSuccess("Все снимки удалены");
                    }
                }
            }
            else
            {
                notifier.ShowInformation("Нет снимков для удаления.");
            }
        }

        private void DeleteSingleInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                notifier.ShowSuccess($"Снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} удален.");

                CurrentSeries.DropPhotosSeries.RemoveAt(Photos.SelectedIndex);
            }
        }

        private void EditInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PixelsInMillimeterTextBox.Text) && PixelsInMillimeterTextBox.Text != "0")
            {
                CurrentDropPhoto = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex];

                if (CurrentDropPhoto.HorizontalLine == null)
                    CurrentDropPhoto.HorizontalLine = new Line();

                if (CurrentDropPhoto.VerticalLine == null)
                    CurrentDropPhoto.VerticalLine = new Line();

                ManualEdit manualEdit = new ManualEdit(CurrentDropPhoto);
                manualEdit.ShowDialog();

                if (CurrentDropPhoto.XDiameterInPixels != 0 && CurrentDropPhoto.YDiameterInPixels != 0)
                {
                    DropletSizeCalculator.PerformCalculation(
                        Convert.ToInt32(PixelsInMillimeterTextBox.Text), CurrentDropPhoto.XDiameterInPixels,
                        CurrentDropPhoto.YDiameterInPixels, CurrentSeries, CurrentDropPhoto);

                    notifier.ShowSuccess($"Расчет для снимка {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} выполнен.");
                }
                else
                {
                    notifier.ShowInformation($"Не указан один из диаметров. Расчет для снимка {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} не выполнен.");
                }
            }
            else
            {
                notifier.ShowInformation("Выберите референсное расстояние на референсном снимке.");
            }
        }

        private void IntervalBetweenPhotos_TextChanged(object sender, TextChangedEventArgs e)
        {
            var intervalBetweenPhotosTextBox = sender as TextBox;

            if (CurrentSeries != null)
            {
                if (int.TryParse(intervalBetweenPhotosTextBox?.Text, out int intervalBetweenPhotos))
                    CurrentSeries.IntervalBetweenPhotos = intervalBetweenPhotos;
                else
                {
                    CurrentSeries.IntervalBetweenPhotos = 0;
                    notifier.ShowInformation("Некорректное значение для интервала между снимками. Укажите интервал между снимками в секундах.");
                }
            }
        }

        #endregion

        #region Reference Photo

        private void ChooseReference_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*",
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (ImageValidator.ValidateImage(ImageInterpreter.FileToByteArray(openFileDialog.FileName)))
                {
                    MainWindowPixelDrawer.TwoLineMode = false;
                    if (CurrentSeries.ReferencePhotoForSeries.Line != null)
                    {
                        MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                        MainWindowPixelDrawer.PixelsInMillimeter = "";
                        CurrentSeries.ReferencePhotoForSeries.Line = null;
                        CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                    }
                    CurrentSeries.ReferencePhotoForSeries.Name = openFileDialog.SafeFileNames[0];
                    CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId = Guid.NewGuid();
                    CurrentSeries.ReferencePhotoForSeries.Line = new Line();
                    CurrentSeries.ReferencePhotoForSeries.Content = ImageInterpreter.FileToByteArray(openFileDialog.FileName);
                    ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);

                    notifier.ShowSuccess($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} добавлен.");
                }
                else
                {
                    notifier.ShowError($"Файл {openFileDialog.FileName} имеет неизвестный формат.");
                }
            }
        }

        private void DeleteReferencePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentSeries.ReferencePhotoForSeries.Content != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Удалить референсный снимок?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                    notifier.ShowSuccess($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} удален.");

                    MainWindowPixelDrawer.PixelsInMillimeter = "";
                    CurrentSeries.ReferencePhotoForSeries.Name = null;
                    CurrentSeries.ReferencePhotoForSeries.Line = null;
                    CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                    CurrentSeries.ReferencePhotoForSeries.Content = null;
                    ReferenceImage = null;
                }
            }
            else
            {
                notifier.ShowInformation("Нет референсного снимка для удаления.");
            }
        }

        #endregion

        #region Calculation

        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (PythonMenuItem.IsChecked)
            {
                bool interpreterAndScriptCheck = string.IsNullOrWhiteSpace(Properties.Settings.Default.ScriptToRun) || string.IsNullOrWhiteSpace(Properties.Settings.Default.Interpreter);
                if (!interpreterAndScriptCheck)
                {
                    if (CurrentSeries.DropPhotosSeries.Count > 0)
                    {
                        bool isAnyPhotoChecked = CurrentSeries.DropPhotosSeries.Any(x => x.IsChecked);

                        if (isAnyPhotoChecked)
                        {
                            for (int i = 0; i < CurrentSeries.DropPhotosSeries.Count; i++)
                            {
                                if (CurrentSeries.DropPhotosSeries[i].IsChecked)
                                {
                                    string fs = pythonProvider.RunScript(Properties.Settings.Default.ScriptToRun,
                                        Properties.Settings.Default.Interpreter,
                                        CurrentSeries.DropPhotosSeries[i].Content);
                                    //CurrentSeries.DropPhotosSeries[i] = pythonProvider.RunScript(CurrentSeries.DropPhotosSeries[i].Path, @"C:\Users\FallingsappyPC\Desktop\",
                                    //CurrentSeries.DropPhotosSeries[i], Properties.Settings.Default.ScriptToRun,
                                    //             Properties.Settings.Default.Interpreter);
                                    //DropletSizeCalculator.PerformCalculation(
                                    //    Convert.ToInt32(PixelsInMillimeterTextBox.Text), CurrentSeries.DropPhotosSeries[i].XDiameterInPixels,
                                    //    CurrentSeries.DropPhotosSeries[i].YDiameterInPixels, CurrentSeries, CurrentDropPhoto);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < CurrentSeries.DropPhotosSeries.Count; ++i)
                            {
                                //CurrentSeries.DropPhotosSeries[i] = pythonProvider.RunScript(CurrentSeries.DropPhotosSeries[i].Path, Properties.Settings.Default.SaveTo,
                                //    CurrentSeries.DropPhotosSeries[i], Properties.Settings.Default.ScriptToRun,
                                //                                         Properties.Settings.Default.Interpreter);
                                //DropletSizeCalculator.PerformCalculation(
                                //    Convert.ToInt32(PixelsInMillimeterTextBox.Text), CurrentSeries.DropPhotosSeries[i].XDiameterInPixels,
                                //    CurrentSeries.DropPhotosSeries[i].YDiameterInPixels, CurrentSeries, CurrentDropPhoto);
                            }
                        }

                        notifier.ShowSuccess("Расчет завершен.");
                    }
                    else
                        notifier.ShowInformation("Выберите фотографии для расчета.");
                }
                else
                {
                    notifier.ShowInformation("Выберите интерпритатор python и исполняемый скрипт в меню \"Опции\"");
                }
            }
            else
            {
                notifier.ShowInformation("Эта функция в разработке.");
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

        #endregion

        #region Account

        private void LoginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(_dDropContext, notifier, _seriesBL)
            {
                Owner = this
            };
            login.ShowDialog();
            if (login.LoginSucceeded)
            {
                User = login.UserLogin;

            }
            else
            {
                ShowDialog();
            }
            if (User == null)
            {
                Close();
            }

            if (User.IsLoggedIn)
            {
                AccountMenuItem.Visibility = Visibility.Visible;
                LogInMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Visible;
            }
        }

        private void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Account accountMenu = new Account(User, notifier, _dDropContext);
            accountMenu.ShowDialog();
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Выйти из учетной записи {User.Email}?", "Подтверждение выхода", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (User.UserSeries != null)
                {
                    User.UserSeries.Clear();
                    User = null;
                }

                if (CurrentSeries?.DropPhotosSeries != null)
                {
                    CurrentSeries.DropPhotosSeries.Clear();
                    CurrentSeries = null;
                }

                AccountMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Collapsed;
                LogInMenuItem.Visibility = Visibility.Visible;
                Login login = new Login(_dDropContext, notifier, _seriesBL)
                {
                    Owner = this
                };
                login.ShowDialog();
                if (login.LoginSucceeded)
                {
                    User = login.UserLogin;
                }
                else
                {
                    ShowDialog();
                }


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
        }

        #endregion

        #region Menu

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

        private void AppMainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (User != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Закрыть приложение?", "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    notifier.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            Information information = new Information();
            information.ShowDialog();
        }

        #endregion
    }
}
