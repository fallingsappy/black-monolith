using DDrop.BE.Models;
using DDrop.BL.Calculation.DropletSizeCalculator;
using DDrop.BL.Series;
using DDrop.Utility.ExcelOperations;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.PythonOperations;
using Microsoft.Win32;
using System;
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
using pbu = RFM.RFM_WPFProgressBarUpdate;
using DDrop.BL.DropPhoto;
using DDrop.DAL;
using DDrop.Utility.Mappers;

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
        private readonly IDDropRepository _dDropRepository;
        readonly PythonProvider pythonProvider = new PythonProvider();

        public static readonly DependencyProperty CurrentSeriesProperty = DependencyProperty.Register("CurrentSeries", typeof(Series), typeof(MainWindow));
        public static readonly DependencyProperty CurrentDropPhotoProperty = DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(MainWindow));
        public static readonly DependencyProperty ReferenceImageProperty = DependencyProperty.Register("ReferenceImage", typeof(ImageSource), typeof(MainWindow));
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(User), typeof(MainWindow));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        public MainWindow(ISeriesBL seriesBL, IDropPhotoBL dropPhotoBL, IDDropRepository dDropRepository)
        {
            InitializeComponent();
            _seriesBL = seriesBL;
            _dropPhotoBL = dropPhotoBL;
            _dDropRepository = dDropRepository;
            AppMainWindow.Show();
            Login login = new Login(_dDropRepository, notifier)
            {
                Owner = AppMainWindow
            };
            login.ShowDialog();

            if (login.LoginSucceeded)
            {
                User = login.UserLogin;

                User.UserSeries = DDropDbEntitiesMapper.DbSeriesToSeries(_dDropRepository.GetSeriesByUserId(User.UserId), User);

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
            if (args.PropertyName == nameof(Series.IsChecked))
                RecheckAllSelected();
        }

        private void SeriesDrawerSwap(Series old)
        {
            if (old.ReferencePhotoForSeries?.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Remove(old.ReferencePhotoForSeries.Line);

            if (CurrentSeries.ReferencePhotoForSeries?.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentSeries.ReferencePhotoForSeries.Line);
        }

        private async void AddNewSeries_OnClick(object sender, RoutedEventArgs e)
        {            
            if (User.UserSeries == null)
            {
                User.UserSeries = new ObservableCollection<Series>();
                SeriesDataGrid.ItemsSource = User.UserSeries;
            }

            if (!string.IsNullOrWhiteSpace(OneLineSetterValue.Text))
            {
                Series seriesToAdd = new Series
                {
                    SeriesId = Guid.NewGuid(),
                    Title = OneLineSetterValue.Text,
                    AddedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    CurrentUser = User,
                    CurrentUserId = User.UserId
                };

                try
                {
                    var dbUser = await _dDropRepository.GetUserByLogin(User.Email);

                    await _dDropRepository.CreateSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(seriesToAdd, dbUser));

                    seriesToAdd.PropertyChanged += EntryOnPropertyChanged;

                    User.UserSeries.Add(seriesToAdd);
                    SeriesDataGrid.ItemsSource = User.UserSeries;
                    OneLineSetterValue.Text = "";
                }
                catch (Exception)
                {
                    notifier.ShowError($"Серия {seriesToAdd.Title} не добавлена. Не удалось установить подключение. Проверьте интернет соединение.");
                }               
            }
            else
            {
                notifier.ShowInformation("Введите название серии.");
            }
        }

        private async void SeriesDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (User.UserSeries.Count > 0 && SeriesDataGrid.SelectedItem != null)
            {
                Series old = new Series()
                {
                    CurrentUser = User,
                    CurrentUserId = User.UserId
                };

                if (e.RemovedItems.Count > 0)
                    old = e.RemovedItems[0] as Series;

                CurrentSeries = User.UserSeries[SeriesDataGrid.SelectedIndex];

                if (CurrentSeries.ReferencePhotoForSeries != null)
                {
                    try
                    {
                        CurrentSeries.ReferencePhotoForSeries.Content = await _dDropRepository.GetReferencePhotoContent(CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId);
                    }
                    catch (Exception)
                    {
                        notifier.ShowError("Не удалось загрузить референсный снимок. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                }                   

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
                try
                {
                    var dbUser = await _dDropRepository.GetUserByLogin(User.Email);
                    await _dDropRepository.DeleteSingleSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(User.UserSeries[SeriesDataGrid.SelectedIndex], dbUser));
                    notifier.ShowSuccess($"Серия {User.UserSeries[SeriesDataGrid.SelectedIndex].Title} была удалена.");
                    User.UserSeries.RemoveAt(SeriesDataGrid.SelectedIndex);
                    Photos.ItemsSource = null;
                    ImgPreview.Source = null;
                    SeriesPreviewDataGrid.ItemsSource = null;
                }
                catch (Exception)
                {
                    notifier.ShowError($"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                }                             
            }
        }
        private async void SeriesPreviewDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhoto selectedFile = (DropPhoto)SeriesPreviewDataGrid.SelectedItem;
            if (selectedFile != null)
            {
                try
                {
                    ImgPreview.Source = ImageInterpreter.LoadImage(await _dDropRepository.GetDropPhotoContent(selectedFile.DropPhotoId));
                }
                catch (Exception)
                {
                    notifier.ShowError($"Не удалось загрузить снимок {selectedFile.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }               
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

        private async void DeleteSeriesButton_Click(object sender, RoutedEventArgs e)
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
                            try
                            {
                                if (User.UserSeries[i].IsChecked)
                                {
                                    var dbUser = await _dDropRepository.GetUserByLogin(User.Email);
                                    await _dDropRepository.DeleteSingleSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(User.UserSeries[i], dbUser));

                                    notifier.ShowSuccess($"Серия {User.UserSeries[i].Title} была удалена.");
                                    notifier.ShowSuccess("Выбранные серии были удалены.");

                                    User.UserSeries.RemoveAt(i);
                                }                                   
                            }
                            catch (Exception)
                            {
                                notifier.ShowError($"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                            }
                        }                        
                    }
                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить все серии?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var dbUser = await _dDropRepository.GetUserByLogin(User.Email);
                            foreach (var series in User.UserSeries)
                            {
                                await _dDropRepository.DeleteSingleSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(series, dbUser));
                            }

                            User.UserSeries.Clear();

                            notifier.ShowSuccess("Все серии были удалены.");
                            SeriesPreviewDataGrid.SelectedIndex = -1;
                        }
                        catch (Exception)
                        {
                            notifier.ShowError($"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }                        
                    }
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
                await DDropSerializableEntitiesMapper.ExportSeriesLocalAsync(saveFileDialog.FileName, User);
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

            ObservableCollection<Series> addSeriesViewModel = new ObservableCollection<Series>();

            if (openFileDialog.ShowDialog() == true)
            {
                ProgressBar.IsIndeterminate = true;
                addSeriesViewModel = await DDropSerializableEntitiesMapper.ImportLocalSeriesAsync(openFileDialog.FileName, User);
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
            if (args.PropertyName == nameof(DropPhoto.IsChecked))
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
                        CurrentSeries.DropPhotosSeries = new ObservableCollection<DropPhoto>();
                        Photos.ItemsSource = CurrentSeries.DropPhotosSeries;
                    }

                    var imageForAdding = new DropPhoto()
                    {
                        DropPhotoId = Guid.NewGuid(),
                        Name = openFileDialog.SafeFileNames[i],
                        Path = openFileDialog.FileNames[i],
                        Content = File.ReadAllBytes(openFileDialog.FileNames[i]),
                        AddedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        CurrentSeries = CurrentSeries,
                        CurrentSeriesId = CurrentSeries.SeriesId,
                    };
                    
                    if (ImageValidator.ValidateImage(imageForAdding.Content))
                    {
                        try
                        {
                            imageForAdding.PropertyChanged += PhotosOnPropertyChanged;

                            var dbSeries = _dDropRepository.GetSeriesByUserId(User.UserId);
                            await _dDropRepository.CreateDropPhoto(DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(imageForAdding, dbSeries.FirstOrDefault(x => x.SeriesId == CurrentSeries.SeriesId)));

                            CurrentSeries.DropPhotosSeries.Add(imageForAdding);
                            CurrentSeries.DropPhotosSeries[CurrentSeries.DropPhotosSeries.Count - 1].Drop = new Drop()
                            {
                                DropId = imageForAdding.DropPhotoId,
                                Series = CurrentSeries,
                                DropPhoto = CurrentSeries.DropPhotosSeries[CurrentSeries.DropPhotosSeries.Count - 1]
                            };

                            notifier.ShowSuccess($"Снимок {imageForAdding.Name} добавлен.");
                        }
                        catch (Exception)
                        {
                            notifier.ShowError($"Снимок {imageForAdding.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
                        }                      
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

        private async void Photos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhoto selectedFile = (DropPhoto)Photos.SelectedItem;
            if (selectedFile != null)
            {
                try
                {
                    ImgCurrent.Source = ImageInterpreter.LoadImage(await _dDropRepository.GetDropPhotoContent(selectedFile.DropPhotoId));
                }
                catch (Exception)
                {
                    notifier.ShowError($"Не удалось загрузить снимок {selectedFile.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }                
            }
            else
                ImgCurrent.Source = null;
        }

        private async void DeleteInputPhotos_OnClick(object sender, RoutedEventArgs e)
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
                            {
                                try
                                {
                                    await _dDropRepository.DeleteDropPhoto(CurrentSeries.DropPhotosSeries[i].DropPhotoId);
                                    CurrentSeries.DropPhotosSeries.RemoveAt(i);
                                }
                                catch (Exception)
                                {
                                    notifier.ShowError($"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[i].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                                }
                            }                               
                        }

                        notifier.ShowSuccess("Выбранные снимки удалены.");
                    }
                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Удалить все снимки?", "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        for (int i = CurrentSeries.DropPhotosSeries.Count - 1; i >= 0; i--)
                        {
                            try
                            {
                                await _dDropRepository.DeleteDropPhoto(CurrentSeries.DropPhotosSeries[i].DropPhotoId);
                                CurrentSeries.DropPhotosSeries.RemoveAt(i);
                            }
                            catch (Exception)
                            {
                                notifier.ShowError($"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[i].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                            }
                        }

                        notifier.ShowSuccess("Все снимки удалены");
                    }
                }
            }
            else
            {
                notifier.ShowInformation("Нет снимков для удаления.");
            }
        }

        private async void DeleteSingleInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    await _dDropRepository.DeleteDropPhoto(CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].DropPhotoId);
                    notifier.ShowSuccess($"Снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} удален.");

                    CurrentSeries.DropPhotosSeries.RemoveAt(Photos.SelectedIndex);
                }
                catch (Exception)
                {
                    notifier.ShowError($"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
            }
        }

        private async void EditInputPhotoButton_Click(object sender, RoutedEventArgs e)
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
                    Drop tempDrop = new Drop
                    {
                        DropId = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.DropId,
                        DropPhoto = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.DropPhoto,
                        RadiusInMeters = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.RadiusInMeters,
                        Series = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.Series,
                        VolumeInCubicalMeters = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.VolumeInCubicalMeters,
                        XDiameterInMeters = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.XDiameterInMeters,
                        YDiameterInMeters = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.YDiameterInMeters,
                        ZDiameterInMeters = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Drop.ZDiameterInMeters
                    };

                    try
                    {
                        DropletSizeCalculator.PerformCalculation(
                            Convert.ToInt32(PixelsInMillimeterTextBox.Text), CurrentDropPhoto.XDiameterInPixels,
                            CurrentDropPhoto.YDiameterInPixels, CurrentDropPhoto);

                        var dbSeries = _dDropRepository.GetSeriesByUserId(User.UserId);
                        var dbPhoto = DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(CurrentDropPhoto, dbSeries.FirstOrDefault(x => x.SeriesId == CurrentSeries.SeriesId));

                        await _dDropRepository.UpdatDropPhoto(dbPhoto);

                        notifier.ShowSuccess($"Расчет для снимка {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} выполнен.");
                    }
                    catch (Exception)
                    {
                        CurrentDropPhoto.Drop = tempDrop;
                        notifier.ShowError("Не удалось сохранить результаты расчета. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
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
                {
                    CurrentSeries.IntervalBetweenPhotos = intervalBetweenPhotos;
                }                   
                else
                {
                    CurrentSeries.IntervalBetweenPhotos = 0;
                    notifier.ShowInformation("Некорректное значение для интервала между снимками. Укажите интервал между снимками в секундах.");
                }
            }
        }

        #endregion

        #region Reference Photo

        private async void ChooseReference_OnClick(object sender, RoutedEventArgs e)
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
                    if (CurrentSeries.ReferencePhotoForSeries == null)
                    {
                        CurrentSeries.ReferencePhotoForSeries = new ReferencePhoto();
                    }
                    if (CurrentSeries.ReferencePhotoForSeries.Line != null)
                    {
                        MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                        MainWindowPixelDrawer.PixelsInMillimeter = "";
                        CurrentSeries.ReferencePhotoForSeries.Line = null;
                        CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                    }
                    CurrentSeries.ReferencePhotoForSeries.Name = openFileDialog.SafeFileNames[0];
                    CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId = CurrentSeries.SeriesId;
                    CurrentSeries.ReferencePhotoForSeries.Line = new Line();
                    CurrentSeries.ReferencePhotoForSeries.Content = ImageInterpreter.FileToByteArray(openFileDialog.FileName);

                    try
                    {
                        var dbSeries = _dDropRepository.GetSeriesByUserId(User.UserId);
                        await _dDropRepository.CreateReferencePhoto(DDropDbEntitiesMapper.ReferencePhotoToDbReferencePhoto(CurrentSeries.ReferencePhotoForSeries, dbSeries.FirstOrDefault(x => x.SeriesId == CurrentSeries.SeriesId)));

                        ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);

                        MainWindowPixelDrawer.IsEnabled = false;
                        ChangeReferenceLine.Visibility = Visibility.Visible;
                        SaveReferenceLine.Visibility = Visibility.Hidden;

                        notifier.ShowSuccess($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} добавлен.");
                    }
                    catch (Exception)
                    {
                        notifier.ShowError($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                }
                else
                {
                    notifier.ShowError($"Файл {openFileDialog.FileName} имеет неизвестный формат.");
                }
            }
        }

        private async void DeleteReferencePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentSeries.ReferencePhotoForSeries.Content != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Удалить референсный снимок?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _dDropRepository.DeleteReferencePhoto(CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId);

                        MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                        notifier.ShowSuccess($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} удален.");

                        MainWindowPixelDrawer.PixelsInMillimeter = "";
                        CurrentSeries.ReferencePhotoForSeries.Name = null;
                        CurrentSeries.ReferencePhotoForSeries.Line = null;
                        CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                        CurrentSeries.ReferencePhotoForSeries.Content = null;
                        ReferenceImage = null;
                    }
                    catch (Exception)
                    {
                        notifier.ShowError($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} не удален. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                }
            }
            else
            {
                notifier.ShowInformation("Нет референсного снимка для удаления.");
            }
        }

        private async void SaveReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindowPixelDrawer.IsEnabled = false;
                ChangeReferenceLine.Visibility = Visibility.Visible;
                SaveReferenceLine.Visibility = Visibility.Hidden;

                var dbSeries = _dDropRepository.GetSeriesByUserId(User.UserId);
                var dbReferencePhoto = DDropDbEntitiesMapper.ReferencePhotoToDbReferencePhoto(CurrentSeries.ReferencePhotoForSeries, dbSeries.FirstOrDefault(x => x.SeriesId == CurrentSeries.SeriesId));

                await _dDropRepository.UpdateReferencePhoto(dbReferencePhoto);

                notifier.ShowSuccess("Сохранено новое референсное расстояние.");
            }
            catch (Exception)
            {
                notifier.ShowError($"Не удалось сохранить новое референсное расстояние.");
            }
        }

        private void ChangeReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.ReferencePhotoForSeries != null)
            {
                MainWindowPixelDrawer.IsEnabled = true;
                ChangeReferenceLine.Visibility = Visibility.Hidden;
                SaveReferenceLine.Visibility = Visibility.Visible;
            }
            else
            {
                notifier.ShowInformation("Загрузите референсный снимок.");
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
            Login login = new Login(_dDropRepository, notifier)
            {
                Owner = this
            };
            login.ShowDialog();
            if (login.LoginSucceeded)
            {
                User = login.UserLogin;

                User.UserSeries = DDropDbEntitiesMapper.DbSeriesToSeries(_dDropRepository.GetSeriesByUserId(User.UserId), User);

                SeriesDataGrid.ItemsSource = User.UserSeries;
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
            Account accountMenu = new Account(User, notifier, _dDropRepository);
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
                Login login = new Login(_dDropRepository, notifier)
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
