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
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;
using System.Threading;
using DDrop.BL.ImageProcessing.CSharp;
using DDrop.BL.ImageProcessing.Python;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Variable Declaration
        private bool _allSelectedSeriesChanging;
        private bool? _allSelectedSeries = false;
        private bool _allSelectedPhotosChanging;
        private bool? _allSelectedPhotos = false;
        private Line HorizontalLineSeriesPreview;
        private Line VerticalLineSeriesPreview;
        private Line HorizontalLinePhotosPreview;
        private Line VerticalLinePhotosPreview;
        private readonly ISeriesBL _seriesBL;
        private readonly IDropPhotoBL _dropPhotoBL;
        private readonly IDDropRepository _dDropRepository;
        private readonly IDropletImageProcessor _dropletImageProcessor;
        private readonly IPythonProvider _pythonProvider;

        private DropPhoto CurrentSeriesPreviewPhoto = new DropPhoto();

        public static readonly DependencyProperty CurrentSeriesProperty = DependencyProperty.Register("CurrentSeries", typeof(Series), typeof(MainWindow));
        public static readonly DependencyProperty CurrentDropPhotoProperty = DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(MainWindow));
        public static readonly DependencyProperty ReferenceImageProperty = DependencyProperty.Register("ReferenceImage", typeof(ImageSource), typeof(MainWindow));
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(User), typeof(MainWindow));
        public static readonly DependencyProperty ParticularSeriesIndexProperty = DependencyProperty.Register("ParticularSeriesIndex", typeof(int), typeof(MainWindow));

        private readonly Notifier _notifier = new Notifier(cfg =>
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
            get => (DropPhoto)GetValue(CurrentDropPhotoProperty);
            set => SetValue(CurrentDropPhotoProperty, value);
        }

        public Series CurrentSeries
        {
            get => (Series)GetValue(CurrentSeriesProperty);
            set => SetValue(CurrentSeriesProperty, value);
        }

        public ImageSource ReferenceImage
        {
            get => (ImageSource)GetValue(ReferenceImageProperty);
            set => SetValue(ReferenceImageProperty, value);
        }

        public User User
        {
            get => (User)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public int ParticularSeriesIndex
        {
            get => (int)GetValue(ParticularSeriesIndexProperty);
            set { SetValue(ParticularSeriesIndexProperty, value); OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        public MainWindow(ISeriesBL seriesBL, IDropPhotoBL dropPhotoBL, IDDropRepository dDropRepository, IDropletImageProcessor dropletImageProcessor, IPythonProvider pythonProvider)
        {
            InitializeComponent();
            _seriesBL = seriesBL;
            _dropPhotoBL = dropPhotoBL;
            _dDropRepository = dDropRepository;
            _dropletImageProcessor = dropletImageProcessor;
            _pythonProvider = pythonProvider;

            AppMainWindow.Show();

            ProgressBar.IsIndeterminate = true;


            Login login = new Login(_dDropRepository, _notifier)
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

            ProgressBar.IsIndeterminate = false;
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

        private async void AddNewSeries_OnClick(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries == null)
            {
                User.UserSeries = new ObservableCollection<Series>();
                SeriesDataGrid.ItemsSource = User.UserSeries;
            }

            if (!string.IsNullOrWhiteSpace(OneLineSetterValue.Text))
            {
                SeriesWindowLoading();
                SeriesManagerIsLoading();

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
                    var userEmail = User.Email;
                    var dbUser = await Task.Run(() =>_dDropRepository.GetUserByLogin(userEmail));

                    await Task.Run(() => _dDropRepository.CreateSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(seriesToAdd, dbUser)));

                    seriesToAdd.PropertyChanged += EntryOnPropertyChanged;

                    User.UserSeries.Add(seriesToAdd);
                    SeriesDataGrid.ItemsSource = User.UserSeries;
                    OneLineSetterValue.Text = "";

                    _notifier.ShowSuccess($"Добавлена новая серия {seriesToAdd.Title}");
                }
                catch (Exception)
                {
                    _notifier.ShowError($"Серия {seriesToAdd.Title} не добавлена. Не удалось установить подключение. Проверьте интернет соединение.");
                }

                SeriesWindowLoading();
                SeriesManagerLoadingComplete();
            }
            else
            {
                _notifier.ShowInformation("Введите название серии.");
            }
        }

        private async void SeriesDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (User.UserSeries.Count > 0 && SeriesDataGrid.SelectedItem != null)
            {
                PreviewCanvas.Children.Remove(HorizontalLineSeriesPreview);
                PreviewCanvas.Children.Remove(VerticalLineSeriesPreview);
                PhotosPreviewCanvas.Children.Remove(HorizontalLinePhotosPreview);
                PhotosPreviewCanvas.Children.Remove(VerticalLinePhotosPreview);

                SingleSeries.IsEnabled = false;
                ProgressBar.IsIndeterminate = true;
                SeriesManagerIsLoading(false);

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
                        var referencePhotoId = CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId;
                        CurrentSeries.ReferencePhotoForSeries.Content = await Task.Run(() => _dDropRepository.GetReferencePhotoContent(referencePhotoId));
                    }
                    catch (Exception)
                    {
                        _notifier.ShowError("Не удалось загрузить референсный снимок. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                }                   

                SeriesDrawerSwap(old);
                Photos.ItemsSource = null;
                SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                ReferenceImage = null;
                ParticularSeriesIndex = SeriesDataGrid.SelectedIndex;

                if (CurrentSeries?.ReferencePhotoForSeries?.Content != null)
                {
                    ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);
                }

                SeriesManagerLoadingComplete(false);
                SingleSeries.IsEnabled = true;
                ProgressBar.IsIndeterminate = false;
            }
            else
            {
                SingleSeries.IsEnabled = false;
            }
        }

        private void SeriesDrawerSwap(Series old)
        {
            if (old.ReferencePhotoForSeries?.Line != null)
                MainWindowPixelDrawer.CanDrawing.Children.Remove(old.ReferencePhotoForSeries.Line);

            if (CurrentSeries.ReferencePhotoForSeries?.Line != null)
            {
                MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);
                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentSeries.ReferencePhotoForSeries.Line);
            }
        }

        private CancellationTokenSource _tokenSource;

        private async void SeriesPreviewDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropPhoto selectedPhoto = (DropPhoto)SeriesPreviewDataGrid.SelectedItem;
            if (selectedPhoto != null)
            {
                CurrentSeriesPreviewPhoto = selectedPhoto;

                PreviewCanvas.Children.Clear();

                PreviewWindowLoading();

                try
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Cancel();
                    }

                    _tokenSource = new CancellationTokenSource();

                    ImgPreview.Source = null;

                    ProgressBar.IsIndeterminate = true;

                    CurrentSeriesPreviewPhoto.Content = await Task.Run(() =>
                        _dDropRepository.GetDropPhotoContent(CurrentSeriesPreviewPhoto.DropPhotoId, _tokenSource.Token));

                    ImgPreview.Source = ImageInterpreter.LoadImage(CurrentSeriesPreviewPhoto.Content);

                    PrepareLines(CurrentSeriesPreviewPhoto, out HorizontalLineSeriesPreview, out VerticalLineSeriesPreview);

                }
                catch (OperationCanceledException ex)
                {
                    
                }
                catch (Exception)
                {
                    _notifier.ShowError($"Не удалось загрузить снимок {CurrentSeriesPreviewPhoto.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }

                PreviewWindowLoading();

                PreviewCanvas.Children.Clear();
                PreviewCanvas.Children.Add(ImgPreview);
                if (HorizontalLineSeriesPreview != null)
                    PreviewCanvas.Children.Add(HorizontalLineSeriesPreview);
                if (VerticalLineSeriesPreview != null)
                    PreviewCanvas.Children.Add(VerticalLineSeriesPreview);
                
                ProgressBar.IsIndeterminate = false;
            }
            else
                ImgPreview.Source = null;
        }

        private void PreviewWindowLoading()
        {
            PreviewLoading.IsAdornerVisible = !PreviewLoading.IsAdornerVisible;
        }

        private void CombinedSeriesPlot_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;

            if (tabItem.IsEnabled && User != null)
            {
                _notifier.ShowSuccess("Новый общий график серий построен");
            }
        }

        private void ExportSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Any(x => x.IsChecked))
            {
                if (User.IsAnySelectedSeriesCanDrawPlot)
                {
                    SeriesManagerIsLoading();
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        AddExtension = true,
                        CheckPathExists = true
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        ExcelOperations.CreateSingleSeriesExcelFile(User, saveFileDialog.FileName);

                        _notifier.ShowSuccess($"Файл {saveFileDialog.SafeFileName} успешно сохранен.");
                    }

                    SeriesManagerLoadingComplete();
                }
                else
                {
                    _notifier.ShowInformation("Нельзя построить график для выбранных серий.");
                }
            }
            else
            {
                _notifier.ShowInformation("Нет выбранных серий.");
            }
        }

        private async void DeleteSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Count > 0)
            {
                var checkedCount = User.UserSeries.Count(x => x.IsChecked);

                MessageBoxResult messageBoxResult = MessageBox.Show(checkedCount > 0 ? "Удалить выбранные серии?" : "Удалить все серии?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0, User.UserSeries.Count, 0);
                    SeriesWindowLoading(false);
                    SeriesManagerIsLoading();
                    for (int i = User.UserSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && !User.UserSeries[i].IsChecked)
                            {
                                continue;
                            }

                            var userEmail = User.Email;
                            var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));
                            var userSeries = User.UserSeries[i];
                            await Task.Run(() => _dDropRepository.DeleteSingleSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(userSeries, dbUser)));

                            _notifier.ShowSuccess($"Серия {User.UserSeries[i].Title} была удалена.");

                            User.UserSeries.RemoveAt(i);
                        }
                        catch (Exception)
                        {
                            _notifier.ShowError($"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    SeriesPreviewDataGrid.SelectedIndex = -1;

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    SeriesManagerLoadingComplete();
                    SeriesWindowLoading(false);
                }

            }
            else
            {
                _notifier.ShowInformation("Нет серий для удаления.");
            }
        }

        private async void DeleteSingleSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    SeriesWindowLoading();
                    SeriesManagerIsLoading();

                    var userEmail = User.Email;
                    var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));
                    await Task.Run(() =>
                    {
                        Dispatcher.InvokeAsync(() => _dDropRepository.DeleteSingleSeries(
                                DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(
                                    User.UserSeries[SeriesDataGrid.SelectedIndex], dbUser)));
                    });
                    _notifier.ShowSuccess($"Серия {User.UserSeries[SeriesDataGrid.SelectedIndex].Title} была удалена.");
                    User.UserSeries.RemoveAt(SeriesDataGrid.SelectedIndex);
                    Photos.ItemsSource = null;
                    ImgPreview.Source = null;
                    SeriesPreviewDataGrid.ItemsSource = null;
                    SeriesPreviewDataGrid.SelectedIndex = -1;
                }
                catch (Exception)
                {
                    _notifier.ShowError($"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                }

                SeriesManagerLoadingComplete();
                SeriesWindowLoading();
            }
        }

        private void SingleSeriesPlotTabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;

            if (tabItem.IsEnabled && User != null)
            {
                _notifier.ShowSuccess("Новый график построен");
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
            if (User.UserSeries.Count > 0)
            {
                VistaFolderBrowserDialog saveFileDialog = new VistaFolderBrowserDialog
                {
                    UseDescriptionForTitle = true,
                    Description = "Выберите папку для сохранения..."
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    SeriesWindowLoading(false);
                    SeriesManagerIsLoading();

                    int checkedCount = User.UserSeries.Count(x => x.IsChecked);

                    var pbuHandle1 = pbu.New(ProgressBar, 0, checkedCount > 0 ? checkedCount : User.UserSeries.Count, 0);

                    foreach (var series in User.UserSeries)
                    {
                        if (checkedCount > 0 && !series.IsChecked)
                        {
                            continue;
                        }

                        try
                        {
                            var fullDbSeries = await Task.Run(() => _dDropRepository.GetFullDbSeriesForExportById(series.SeriesId));

                            await Task.Run(() => DDropDbEntitiesMapper.ExportSeriesLocalAsync(
                                $"{saveFileDialog.SelectedPath}\\{series.Title}.ddrops", fullDbSeries));
                            _notifier.ShowSuccess($"файл {series.Title}.ddrops сохранен на диске.");
                        }
                        catch
                        {
                            _notifier.ShowError($"Не удалось сохранить серию {series.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }
                        
                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    SeriesManagerLoadingComplete();
                    SeriesWindowLoading(false);
                }
            }
            else
            {
                _notifier.ShowInformation("Нет серий для выгрузки.");
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

            if (openFileDialog.ShowDialog() == true)
            {
                SeriesManagerIsLoading();
                SeriesWindowLoading();

                var pbuHandle1 = pbu.New(ProgressBar, 0, openFileDialog.FileNames.Length, 0);

                var userEmail = User.Email;
                var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));

                foreach (var fileName in openFileDialog.FileNames)
                {
                    try
                    {
                        var dbSerieForAdd = await Task.Run(() => DDropDbEntitiesMapper.ImportLocalSeriesAsync(fileName, dbUser));

                        var deserializedSerie = DDropDbEntitiesMapper.SingleDbSerieToSerie(dbSerieForAdd, User, true);

                        try
                        {
                            await Task.Run(() => _dDropRepository.CreateFullSeries(DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(deserializedSerie, dbUser)));

                            deserializedSerie.ReferencePhotoForSeries.Content = null;
                            foreach (var dropPhoto in deserializedSerie.DropPhotosSeries)
                            {
                                dropPhoto.Content = null;
                            }

                            User.UserSeries.Add(deserializedSerie);
                            _notifier.ShowSuccess($"Серия {dbSerieForAdd.Title} добавлена.");

                            
                        }
                        catch
                        {
                            _notifier.ShowError(
                                $"Не удалось сохранить серию серию {dbSerieForAdd.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }
                    }
                    catch
                    {
                        _notifier.ShowError(
                            $"Не удалось десериализовать файл {fileName}. Файл не является файлом серии или поврежден.");
                    }

                    pbu.CurValue[pbuHandle1] += 1;


                }

                SeriesManagerLoadingComplete();
                SeriesWindowLoading();
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }

            if (SeriesDataGrid.ItemsSource == null)
                SeriesDataGrid.ItemsSource = User.UserSeries;
        }

        private async void SeriesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SeriesWindowLoading(false);
            SeriesManagerIsLoading();

            var seriesNameCell = e.EditingElement as TextBox;
            try
            {
                if (seriesNameCell != null)
                {
                    var seriesId = CurrentSeries.SeriesId;
                    var text = seriesNameCell.Text;
                    await Task.Run(() => _dDropRepository.UpdateSeriesName(text, seriesId));
                    _notifier.ShowSuccess("Название серии изменено успешно.");
                }
            }
            catch (Exception)
            {
                _notifier.ShowError("Не удалось изменить название серии. Не удалось установить подключение. Проверьте интернет соединение.");
            }

            SeriesWindowLoading(false);
            SeriesManagerLoadingComplete();
        }

        private void SeriesWindowLoading(bool indeterminateLoadingBar = true)
        {
            if (indeterminateLoadingBar)
                ProgressBar.IsIndeterminate = !ProgressBar.IsIndeterminate;
            SeriesLoading.IsAdornerVisible = !SeriesLoading.IsAdornerVisible;
        }

        private void LoadSeriesPreviewPhoto(DropPhoto dropPhoto)
        {
            PreviewCanvas.Children.Clear();

            ProgressBar.IsIndeterminate = true;
            ImgPreview.Source = null;
            CurrentSeriesImageLoadingWindow();

            ImgPreview.Source = ImageInterpreter.LoadImage(CurrentSeriesPreviewPhoto.Content);

            PrepareLines(dropPhoto, out HorizontalLineSeriesPreview, out VerticalLineSeriesPreview);

            ProgressBar.IsIndeterminate = false;
            CurrentSeriesImageLoadingWindow();

            PreviewCanvas.Children.Clear();
            if (ImgPreview != null)
                PreviewCanvas.Children.Add(ImgPreview);
            if (HorizontalLineSeriesPreview != null)
                PreviewCanvas.Children.Add(HorizontalLineSeriesPreview);
            if (VerticalLineSeriesPreview != null)
                PreviewCanvas.Children.Add(VerticalLineSeriesPreview);
        }

        private void EditIntervalBetweenPhotos_Click(object sender, RoutedEventArgs e)
        {
            SaveIntervalBetweenPhotos.Visibility = Visibility.Visible;
            EditIntervalBetweenPhotos.Visibility = Visibility.Hidden;
            IntervalBetweenPhotos.IsEnabled = true;
        }

        private async void SaveIntervalBetweenPhotos_Click(object sender, RoutedEventArgs e)
        {
            SingleSeriesLoading();
            await SaveIntervalBetweenPhotosAsync();
            SingleSeriesLoadingComplete();
        }

        private async Task SaveIntervalBetweenPhotosAsync()
        {
            if (CurrentSeries != null)
            {
                if (int.TryParse(IntervalBetweenPhotos?.Text, out int intervalBetweenPhotos))
                {
                    try
                    {
                        SaveIntervalBetweenPhotos.Visibility = Visibility.Hidden;
                        EditIntervalBetweenPhotos.Visibility = Visibility.Visible;
                        IntervalBetweenPhotos.IsEnabled = false;
                        ProgressBar.IsIndeterminate = true;

                        var seriesId = CurrentSeries.SeriesId;
                        await Task.Run(() => _dDropRepository.UpdateSeriesIntervalBetweenPhotos(intervalBetweenPhotos, seriesId));

                        CurrentSeries.IntervalBetweenPhotos = intervalBetweenPhotos;
                    }
                    catch
                    {
                        _notifier.ShowError("Не удалось сохранить новый временной интервал между снимками. Не удалось установить подключение. Проверьте интернет соединение.");
                        IntervalBetweenPhotos.Text = CurrentSeries.IntervalBetweenPhotos.ToString();
                    }

                    ProgressBar.IsIndeterminate = false;
                }
                else
                {
                    CurrentSeries.IntervalBetweenPhotos = 0;
                    _notifier.ShowInformation("Некорректное значение для интервала между снимками. Укажите интервал между снимками в секундах.");
                }
            }
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
                var pbuHandle1 = pbu.New(ProgressBar, 0, openFileDialog.FileNames.Length, 0);

                SeriesManagerIsLoading();
                CurrentSeriesPhotoContentLoadingWindow();

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
                        CreationDateTime = File.GetCreationTime(openFileDialog.FileNames[i]).ToString(),
                        PhotoOrderInSeries = CurrentSeries.DropPhotosSeries.Count
                    };
                    imageForAdding.Drop = new Drop()
                    {
                        DropId = imageForAdding.DropPhotoId,
                        Series = CurrentSeries,
                        DropPhoto = imageForAdding
                    };

                    imageForAdding.PropertyChanged += PhotosOnPropertyChanged;

                    if (ImageValidator.ValidateImage(imageForAdding.Content))
                    {
                        try
                        {
                            var seriesId = CurrentSeries.SeriesId;
                            await Task.Run(() => _dDropRepository.CreateDropPhoto(DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(imageForAdding, seriesId), seriesId));

                            imageForAdding.Content = null;
                            CurrentSeries.DropPhotosSeries.Add(imageForAdding);
                            
                            _notifier.ShowSuccess($"Снимок {imageForAdding.Name} добавлен.");
                        }
                        catch (Exception)
                        {
                            _notifier.ShowError($"Снимок {imageForAdding.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
                        }                      
                    }
                    else
                    {
                        _notifier.ShowError($"Файл {imageForAdding.Name} имеет неизвестный формат.");
                    }
                }

                SeriesManagerLoadingComplete();
                CurrentSeriesPhotoContentLoadingWindow();
                _notifier.ShowSuccess($"Новые снимки успешно добавлены.");
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
                    PhotosPreviewCanvas.Children.Clear();

                    ProgressBar.IsIndeterminate = true;
                    ImgCurrent.Source = null;
                    CurrentSeriesImageLoadingWindow();
                    SingleSeriesLoading(false);

                    if (e.RemovedItems.Count > 0)
                    {
                        var oldCurrentPhoto = e.RemovedItems[0] as DropPhoto;
                        CurrentSeries.DropPhotosSeries.FirstOrDefault(x => oldCurrentPhoto != null && x.DropPhotoId == oldCurrentPhoto.DropPhotoId)
                            .Content = null;
                    }

                    if (_tokenSource != null)
                    {
                        _tokenSource.Cancel();
                    }

                    _tokenSource = new CancellationTokenSource();

                    CurrentDropPhoto = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex];
                    CurrentDropPhoto.Content = await Task.Run(() => _dDropRepository.GetDropPhotoContent(selectedFile.DropPhotoId, _tokenSource.Token));
                    ImgCurrent.Source = ImageInterpreter.LoadImage(CurrentDropPhoto.Content);

                    PrepareLines(selectedFile, out HorizontalLinePhotosPreview, out VerticalLinePhotosPreview);
                }
                catch (OperationCanceledException ex)
                {

                }
                catch (Exception)
                {
                    _notifier.ShowError($"Не удалось загрузить снимок {selectedFile.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }


                ProgressBar.IsIndeterminate = false;
                CurrentSeriesImageLoadingWindow();
                SingleSeriesLoadingComplete(false);

                PhotosPreviewCanvas.Children.Clear();
                PhotosPreviewCanvas.Children.Add(ImgCurrent);
                if (HorizontalLinePhotosPreview != null)
                    PhotosPreviewCanvas.Children.Add(HorizontalLinePhotosPreview);
                if (VerticalLinePhotosPreview != null)
                    PhotosPreviewCanvas.Children.Add(VerticalLinePhotosPreview);
            }
            else
                ImgCurrent.Source = null;
        }

        private void LoadPreviewPhoto(DropPhoto dropPhoto)
        {
            PhotosPreviewCanvas.Children.Clear();

            ProgressBar.IsIndeterminate = true;
            ImgCurrent.Source = null;
            CurrentSeriesImageLoadingWindow();

            ImgCurrent.Source = ImageInterpreter.LoadImage(CurrentDropPhoto.Content);

            PrepareLines(dropPhoto, out HorizontalLinePhotosPreview, out VerticalLinePhotosPreview);

            ProgressBar.IsIndeterminate = false;
            CurrentSeriesImageLoadingWindow();

            PhotosPreviewCanvas.Children.Clear();
            if(ImgCurrent != null)
                PhotosPreviewCanvas.Children.Add(ImgCurrent);
            if (HorizontalLinePhotosPreview != null)
                PhotosPreviewCanvas.Children.Add(HorizontalLinePhotosPreview);
            if (VerticalLinePhotosPreview != null)
                PhotosPreviewCanvas.Children.Add(VerticalLinePhotosPreview);
        }

        private void CurrentSeriesPhotoContentLoadingWindow()
        {
            CurrentSeriesPhotoContentLoading.IsAdornerVisible = !CurrentSeriesPhotoContentLoading.IsAdornerVisible;
        }

        private void CurrentSeriesImageLoadingWindow()
        {
            CurrentSeriesImageLoading.IsAdornerVisible = !CurrentSeriesImageLoading.IsAdornerVisible;
        }

        private async void DeleteInputPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                int checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);

                MessageBoxResult messageBoxResult = MessageBox.Show(checkedCount > 0 ? "Удалить выбранные снимки?" : "Удалить все снимки?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0, checkedCount > 0 ? checkedCount : CurrentSeries.DropPhotosSeries.Count, 0);
                    SingleSeriesLoading();
                    CurrentSeriesPhotoContentLoadingWindow();

                    for (int i = CurrentSeries.DropPhotosSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && !CurrentSeries.DropPhotosSeries[i].IsChecked)
                            {
                                continue;
                            }

                            var photoForDeleteId = CurrentSeries.DropPhotosSeries[i].DropPhotoId;
                            await Task.Run(() => _dDropRepository.DeleteDropPhoto(photoForDeleteId));

                            CurrentSeries.DropPhotosSeries.Remove(CurrentSeries.DropPhotosSeries[i]);
                            
                        }
                        catch (Exception)
                        {
                            _notifier.ShowError($"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[i].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    CurrentSeriesPhotoContentLoadingWindow();
                    SingleSeriesLoadingComplete();
                }
            }
            else
            {
                _notifier.ShowInformation("Нет снимков для удаления.");
            }
        }

        private async void DeleteSingleInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}?", "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ProgressBar.IsIndeterminate = true;
                SingleSeriesLoading();
                CurrentSeriesPhotoContentLoadingWindow();
                try
                {
                    var photoForDeleteId = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].DropPhotoId;
                    await Task.Run(() => _dDropRepository.DeleteDropPhoto(photoForDeleteId));
                    _notifier.ShowSuccess($"Снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} удален.");

                    CurrentSeries.DropPhotosSeries.RemoveAt(Photos.SelectedIndex);
                }
                catch (Exception)
                {
                    _notifier.ShowError($"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }

                ProgressBar.IsIndeterminate = false;
                CurrentSeriesPhotoContentLoadingWindow();
                SingleSeriesLoadingComplete();
            }
        }

        private async void EditInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.IsIndeterminate = true;
            SingleSeriesLoading();

            if (!string.IsNullOrWhiteSpace(PixelsInMillimeterTextBox.Text) && PixelsInMillimeterTextBox.Text != "0")
            {                
                if (CurrentDropPhoto.HorizontalLine == null)
                    CurrentDropPhoto.HorizontalLine = new Line();

                if (CurrentDropPhoto.VerticalLine == null)
                    CurrentDropPhoto.VerticalLine = new Line();

                if (CurrentDropPhoto.Content == null)
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Cancel();
                    }

                    _tokenSource = new CancellationTokenSource();

                    var currentDropPhotoId = CurrentDropPhoto.DropPhotoId;
                    try
                    {
                        CurrentDropPhoto.Content = await Task.Run(() => _dDropRepository.GetDropPhotoContent(currentDropPhotoId, _tokenSource.Token));
                    }
                    catch (OperationCanceledException ex)
                    {

                    }
                    catch (Exception)
                    {

                    }                   
                }
                
                if (CurrentDropPhoto.Content != null)
                {
                    ManualEdit manualEdit = new ManualEdit(CurrentDropPhoto);
                    manualEdit.ShowDialog();

                    if (manualEdit.reCalculate)
                    {
                        if (CurrentDropPhoto.XDiameterInPixels != 0 && CurrentDropPhoto.YDiameterInPixels != 0)
                        {
                            CurrentSeriesPhotoContentLoadingWindow();
                            CurrentSeriesImageLoadingWindow();
                            SingleSeriesLoading();

                            await CalculateDropParameters(CurrentDropPhoto);

                            CurrentSeriesPhotoContentLoadingWindow();
                            CurrentSeriesImageLoadingWindow();
                        }
                        else
                        {
                            _notifier.ShowInformation($"Не указан один из диаметров. Расчет для снимка {CurrentDropPhoto.Name} не выполнен.");
                        }
                    }
                }
                else
                {
                    _notifier.ShowError("Не удалось загрузить снимок. Не удалось установить подключение. Проверьте интернет соединение.");
                }
            }
            else
            {
                _notifier.ShowInformation("Выберите референсное расстояние на референсном снимке.");
            }

            LoadPreviewPhoto(CurrentDropPhoto);

            ProgressBar.IsIndeterminate = false;
            SingleSeriesLoadingComplete();
        }

        private async Task CalculateDropParameters(DropPhoto dropPhoto)
        {
            Drop tempDrop = new Drop
            {
                DropId = dropPhoto.Drop.DropId,
                DropPhoto = dropPhoto.Drop.DropPhoto,
                RadiusInMeters = dropPhoto.Drop.RadiusInMeters,
                Series = dropPhoto.Drop.Series,
                VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters,
                ZDiameterInMeters = dropPhoto.Drop.ZDiameterInMeters
            };

            try
            {
                DropletSizeCalculator.PerformCalculation(
                    Convert.ToInt32(PixelsInMillimeterTextBox.Text), dropPhoto.XDiameterInPixels,
                    dropPhoto.YDiameterInPixels, dropPhoto);

                if (dropPhoto.Content == null)
                {
                    dropPhoto.Content = await _dDropRepository.GetDropPhotoContent(dropPhoto.DropPhotoId, CancellationToken.None);
                }

                var dbPhoto = DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(dropPhoto, CurrentSeries.SeriesId);

                await Task.Run(() => _dDropRepository.UpdateDropPhoto(dbPhoto));

                if (CurrentDropPhoto != null && dropPhoto.DropPhotoId != CurrentDropPhoto.DropPhotoId)
                {
                    dropPhoto.Content = null;
                }

                _notifier.ShowSuccess($"Расчет для снимка {dropPhoto.Name} выполнен.");
            }
            catch (Exception)
            {
                dropPhoto.Drop = tempDrop;
                _notifier.ShowError("Не удалось сохранить результаты расчета. Не удалось установить подключение. Проверьте интернет соединение.");
            }            
        }

        private async void Photos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SingleSeriesLoading();

            var photoNameCell = e.EditingElement as TextBox;
            try
            {
                if (photoNameCell != null)
                {
                    ProgressBar.IsIndeterminate = true;
                    
                    var currentDropPhotoId = CurrentDropPhoto.DropPhotoId;
                    var text = photoNameCell.Text;
                    await Task.Run(() => _dDropRepository.UpdateDropPhotoName(text, currentDropPhotoId));
                    _notifier.ShowSuccess("Название снимка изменено успешно.");
                }
            }
            catch (Exception)
            {
                _notifier.ShowError("Не удалось изменить название снимка. Не удалось установить подключение. Проверьте интернет соединение.");
            }

            SingleSeriesLoadingComplete();
            ProgressBar.IsIndeterminate = false;
        }

        private async void CreationTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries != null)
            {
                CurrentSeries.UseCreationDateTime = true;

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var seriesId = CurrentSeries.SeriesId;
                    await Task.Run(() => _dDropRepository.UseCreationDateTime(true, seriesId));
                    _notifier.ShowSuccess($"Серия {CurrentSeries.Title} использует время создания снимков. Порядок фотографий будет проигнорирован.");
                }
                catch
                {
                    _notifier.ShowError("Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
                }

                ProgressBar.IsIndeterminate = false;
            }
        }

        private async void CreationTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries != null)
            {
                CurrentSeries.UseCreationDateTime = false;
                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var seriesId = CurrentSeries.SeriesId;
                    await Task.Run(() => _dDropRepository.UseCreationDateTime(false, seriesId));
                    _notifier.ShowSuccess($"Серия {CurrentSeries.Title} использует интервал между снимками.");
                }
                catch
                {
                    _notifier.ShowError("Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
                }

                ProgressBar.IsIndeterminate = false;
            }
        }

        private void EditPhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            EditTable editTableWindow = new EditTable(CurrentSeries, _dDropRepository);

            try
            {
                editTableWindow.ShowDialog();
                //CurrentSeries.DropPhotosSeries.OrderBy(x => x.PhotoOrderInSeries);
                _notifier.ShowSuccess($"Порядок снимков для серии {CurrentSeries.Title} обновлен.");
            }
            catch
            {
                _notifier.ShowError(
                    $"Не удалось обновить порядок снимков для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
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
                ProgressBar.IsIndeterminate = true;
                SingleSeriesLoading();
                ReferencePhotoContentLoadingWindow();

                var referencePhotoContentForAdd = ImageInterpreter.FileToByteArray(openFileDialog.FileName);

                if (ImageValidator.ValidateImage(referencePhotoContentForAdd))
                {
                    ReferencePhoto newReferencePhoto;
                    MainWindowPixelDrawer.TwoLineMode = false;
                    if (CurrentSeries.ReferencePhotoForSeries == null)
                    {
                        CurrentSeries.ReferencePhotoForSeries = new ReferencePhoto();

                        newReferencePhoto = new ReferencePhoto
                        {
                            Content = referencePhotoContentForAdd,
                            Name = openFileDialog.SafeFileNames[0],
                            ReferencePhotoId = CurrentSeries.SeriesId,
                            Line = new Line(),
                        };
                    }
                    else
                    {
                        newReferencePhoto = new ReferencePhoto
                        {
                            Content = referencePhotoContentForAdd,
                            Name = openFileDialog.SafeFileNames[0],
                            ReferencePhotoId = CurrentSeries.SeriesId,
                            Line = new Line()                          
                        };

                        if (CurrentSeries.ReferencePhotoForSeries.SimpleLine != null)
                        {
                            newReferencePhoto.SimpleLine = CurrentSeries.ReferencePhotoForSeries.SimpleLine;
                            newReferencePhoto.SimpleLine.X1 = 0;
                            newReferencePhoto.SimpleLine.X2 = 0;
                            newReferencePhoto.SimpleLine.Y1 = 0;
                            newReferencePhoto.SimpleLine.Y2 = 0;
                            newReferencePhoto.SimpleReferencePhotoLineId = newReferencePhoto.SimpleLine.SimpleLineId;
                        }
                    }

                    try
                    {
                        await Task.Run(() => _dDropRepository.UpdateReferencePhoto(DDropDbEntitiesMapper.ReferencePhotoToDbReferencePhoto(newReferencePhoto)));

                        if (CurrentSeries.ReferencePhotoForSeries.Line != null)
                        {
                            MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                            MainWindowPixelDrawer.PixelsInMillimeter = "";
                            CurrentSeries.ReferencePhotoForSeries.Line = null;
                            CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                        }

                        CurrentSeries.ReferencePhotoForSeries = newReferencePhoto;
                        ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);

                        MainWindowPixelDrawer.IsEnabled = false;
                        ChangeReferenceLine.Visibility = Visibility.Visible;
                        SaveReferenceLine.Visibility = Visibility.Hidden;

                        _notifier.ShowSuccess($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} добавлен.");
                    }
                    catch
                    {
                        _notifier.ShowError($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
                    }

                    ProgressBar.IsIndeterminate = false;
                    SingleSeriesLoadingComplete();
                    ReferencePhotoContentLoadingWindow();
                }
                else
                {
                    _notifier.ShowError($"Файл {openFileDialog.FileName} имеет неизвестный формат.");
                }
            }
        }

        private async void DeleteReferencePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentSeries.ReferencePhotoForSeries?.Content != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Удалить референсный снимок?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SingleSeriesLoading();
                    ReferencePhotoContentLoadingWindow();

                    ProgressBar.IsIndeterminate = true;
                    MainWindowPixelDrawer.IsEnabled = false;
                    ChangeReferenceLine.Visibility = Visibility.Visible;
                    SaveReferenceLine.Visibility = Visibility.Hidden;

                    try
                    {
                        var referencePhotoId = CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId;
                        await Task.Run(() => _dDropRepository.DeleteReferencePhoto(referencePhotoId));

                        MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                        _notifier.ShowSuccess($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} удален.");

                        MainWindowPixelDrawer.PixelsInMillimeter = "";
                        CurrentSeries.ReferencePhotoForSeries.Name = null;
                        CurrentSeries.ReferencePhotoForSeries.Line = null;
                        CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                        CurrentSeries.ReferencePhotoForSeries.Content = null;
                        ReferenceImage = null;
                    }
                    catch
                    {
                        _notifier.ShowError($"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} не удален. Не удалось установить подключение. Проверьте интернет соединение.");
                    }

                    SingleSeriesLoadingComplete();
                    ReferencePhotoContentLoadingWindow();
                    ProgressBar.IsIndeterminate = false;
                }
            }
            else
            {
                _notifier.ShowInformation("Нет референсного снимка для удаления.");
            }
        }

        private async void SaveReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.ReferencePhotoForSeries.SimpleLine != null)
            {
                SingleSeriesLoading();
                ReferencePhotoContentLoadingWindow();

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    MainWindowPixelDrawer.IsEnabled = false;
                    ChangeReferenceLine.Visibility = Visibility.Visible;
                    SaveReferenceLine.Visibility = Visibility.Hidden;

                    var dbSeries = _dDropRepository.GetSeriesByUserId(User.UserId);
                    var dbReferencePhoto = DDropDbEntitiesMapper.ReferencePhotoToDbReferencePhoto(CurrentSeries.ReferencePhotoForSeries);

                    await Task.Run(() => _dDropRepository.UpdateReferencePhoto(dbReferencePhoto));

                    _notifier.ShowSuccess("Сохранено новое референсное расстояние.");
                    
                    if (CurrentSeries.DropPhotosSeries.Any(x => x.XDiameterInPixels > 0 && x.YDiameterInPixels > 0))
                    {
                        await ReCalculateDropParameters();
                    }
                }
                catch
                {
                    _notifier.ShowError($"Не удалось сохранить новое референсное расстояние.");
                }

                SingleSeriesLoadingComplete();
                ReferencePhotoContentLoadingWindow();
                ProgressBar.IsIndeterminate = false;
            }
            else
            {
                MainWindowPixelDrawer.IsEnabled = false;
                ChangeReferenceLine.Visibility = Visibility.Visible;
                SaveReferenceLine.Visibility = Visibility.Hidden;
                _notifier.ShowInformation("Нет референсного референсного расстояния для сохранения.");
            }
        }

        private async Task ReCalculateDropParameters()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Пересчитать параметры капель?", "Подтверждение", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                {
                    if (dropPhoto.XDiameterInPixels > 0 && dropPhoto.YDiameterInPixels > 0)
                    {
                        await CalculateDropParameters(dropPhoto);
                    }
                }
        }

        private void ChangeReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.ReferencePhotoForSeries?.Content != null)
            {
                MainWindowPixelDrawer.IsEnabled = true;
                ChangeReferenceLine.Visibility = Visibility.Hidden;
                SaveReferenceLine.Visibility = Visibility.Visible;
            }
            else
            {
                _notifier.ShowInformation("Загрузите референсный снимок.");
            }
        }

        private void ReferencePhotoContentLoadingWindow()
        {
            ReferenceImageLoading.IsAdornerVisible = !ReferenceImageLoading.IsAdornerVisible;
        }

        #endregion

        #region Calculation

        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            int checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);

            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                for (int i = 0; i < CurrentSeries.DropPhotosSeries.Count; i++)
                {

                    if (checkedCount > 0 && !CurrentSeries.DropPhotosSeries[i].IsChecked)
                    {
                        continue;
                    }

                    if (PythonMenuItem.IsChecked)
                    {
                        var points = CalculateWithPython(CurrentSeries.DropPhotosSeries[i]);

                        DrawContour(CurrentSeries.DropPhotosSeries[i], points);
                    }
                    else
                    {
                        var points = _dropletImageProcessor.GetDiameters(CurrentSeries.DropPhotosSeries[i].Content);

                        DrawContour(CurrentSeries.DropPhotosSeries[i], points);
                    }
                    foreach (var item in CurrentSeries.DropPhotosSeries[i].Contour.Lines)
                    {
                        PhotosPreviewCanvas.Children.Add(item);
                    }                    
                }
               
                _notifier.ShowSuccess("Расчет завершен.");
            }
            else
                _notifier.ShowInformation("Нет фотографий для расчета.");
        }

        private void DrawContour(DropPhoto dropPhoto, System.Drawing.Point[] points)
        {
            for (int j = 0; j < points.Length; j++)
            {
                if (dropPhoto.Contour == null)
                {
                    dropPhoto.Contour = new Contour
                    {
                        ContourId = dropPhoto.DropPhotoId,
                        CurrentDropPhoto = dropPhoto,
                        SimpleLines = new ObservableCollection<SimpleLine>(),
                        Lines = new ObservableCollection<Line>(),
                    };
                }

                dropPhoto.Contour.SimpleLines.Add(new SimpleLine
                {
                    SimpleLineId = Guid.NewGuid(),
                    X1 = points[j].X,
                    X2 = j < points.Length - 1 ? points[j + 1].X : points[0].X,
                    Y1 = points[j].Y,
                    Y2 = j < points.Length - 1 ? points[j + 1].Y : points[0].Y
                });

                dropPhoto.Contour.Lines.Add(new Line
                {
                    X1 = points[j].X,
                    X2 = j < points.Length - 1 ? points[j + 1].X : points[0].X,
                    Y1 = points[j].Y,
                    Y2 = j < points.Length - 1 ? points[j + 1].Y : points[0].Y,
                    StrokeThickness = 2,
                    Stroke = Brushes.Red
                });
            }
        }

        private System.Drawing.Point[] CalculateWithPython(DropPhoto dropPhoto)
        {
            bool interpreterAndScriptCheck = string.IsNullOrWhiteSpace(Properties.Settings.Default.ScriptToRun) || string.IsNullOrWhiteSpace(Properties.Settings.Default.Interpreter);
            if (!interpreterAndScriptCheck)
            {
                return _pythonProvider.GetDiameters(dropPhoto.Content, dropPhoto.Name, Properties.Settings.Default.ScriptToRun, Properties.Settings.Default.Interpreter);
            }
            else
            {
                _notifier.ShowInformation("Выберите интерпритатор python и исполняемый скрипт в меню \"Опции\"");

                return null;
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

        private async void ReCalculate_Click(object sender, RoutedEventArgs e)
        {
            await ReCalculateDropParameters();
        }

        #endregion

        #region Account

        private void LoginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(_dDropRepository, _notifier)
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
            Account accountMenu = new Account(User, _notifier, _dDropRepository);
            accountMenu.ShowDialog();
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Выйти из учетной записи {User.Email}?", "Подтверждение выхода", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MainTabControl.SelectedIndex = 0;

                if (MainWindowPixelDrawer.CanDrawing.Children != null && CurrentSeries?.ReferencePhotoForSeries?.Line != null)
                {
                    MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);
                }

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
                Login login = new Login(_dDropRepository, _notifier)
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
            
            if(options.ShowLinesOnPreviewIsChanged)
            {
                if (CurrentSeries != null && CurrentDropPhoto != null)
                {
                    LoadPreviewPhoto(CurrentDropPhoto);
                }

                if (CurrentSeriesPreviewPhoto != null)
                {
                    LoadSeriesPreviewPhoto(CurrentSeriesPreviewPhoto);
                }
            }
        }

        private void AppMainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (User != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Закрыть приложение?", "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _notifier.Dispose();
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

        #region Utility

        private void PrepareLines(DropPhoto selectedPhoto, out Line horizontalLine, out Line verticalLine)
        {
            if (selectedPhoto.HorizontalLine != null && Properties.Settings.Default.ShowLinesOnPreview)
                horizontalLine = new Line
                {
                    X1 = selectedPhoto.HorizontalLine.X1,
                    X2 = selectedPhoto.HorizontalLine.X2,
                    Y1 = selectedPhoto.HorizontalLine.Y1,
                    Y2 = selectedPhoto.HorizontalLine.Y2,
                    StrokeThickness = 2,
                    Stroke = Brushes.DeepPink
                };
            else
                horizontalLine = null;

            if(selectedPhoto.VerticalLine != null && Properties.Settings.Default.ShowLinesOnPreview)
                verticalLine = new Line
                {
                    X1 = selectedPhoto.VerticalLine.X1,
                    X2 = selectedPhoto.VerticalLine.X2,
                    Y1 = selectedPhoto.VerticalLine.Y1,
                    Y2 = selectedPhoto.VerticalLine.Y2,
                    StrokeThickness = 2,
                    Stroke = Brushes.Green
                };
            else
                verticalLine = null;
        }

        private void SeriesManagerIsLoading(bool blockSeriesTable = true)
        {
            SeriesManager.IsEnabled = false;
            if (blockSeriesTable)
                SeriesDataGrid.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
            AddSeriesButton.IsEnabled = false;
            OneLineSetterValue.IsEnabled = false;
            ExportSeriesLocal.IsEnabled = false;
            ImportLocalSeries.IsEnabled = false;
            ExportSeriesButton.IsEnabled = false;
            DeleteSeriesButton.IsEnabled = false;
            AutoCalculateSeriesButton.IsEnabled = false;
        }

        private void SeriesManagerLoadingComplete(bool blockSeriesTable = true)
        {
            SeriesManager.IsEnabled = true;
            if (blockSeriesTable)
                SeriesDataGrid.IsEnabled = true;
            MainMenuBar.IsEnabled = true;
            AddSeriesButton.IsEnabled = true;
            OneLineSetterValue.IsEnabled = true;
            ExportSeriesLocal.IsEnabled = true;
            ImportLocalSeries.IsEnabled = true;
            ExportSeriesButton.IsEnabled = true;
            DeleteSeriesButton.IsEnabled = true;
            AutoCalculateSeriesButton.IsEnabled = true;
        }

        private async void SingleSeriesLoading(bool disablePhotos = true)
        {
            if (CurrentSeries != null)
                CurrentSeries.Loaded = false;
            if (disablePhotos)
                Photos.IsEnabled = false;
            PhotosTab.IsEnabled = false;
            SeriesManager.IsEnabled = false;
            ReferenceTab.IsEnabled = false;
            AddPhotoButton.IsEnabled = false;
            DeleteInputPhotosButton.IsEnabled = false;
            EditPhotosOrder.IsEnabled = false;
            EditIntervalBetweenPhotos.IsEnabled = false;
            ReCalculate.IsEnabled = false;
            AutoCalculate.IsEnabled = false;

            if (IntervalBetweenPhotos.IsEnabled)
            {
                await SaveIntervalBetweenPhotosAsync();
                IntervalBetweenPhotos.IsEnabled = false;

            }

            CreationTimeCheckBox.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
            ChangeReferenceLine.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            ChooseReferenceButton.IsEnabled = false;
        }

        private async void SingleSeriesLoadingComplete(bool disablePhotos = true)
        {
            if (CurrentSeries != null)
                CurrentSeries.Loaded = true;
            if (disablePhotos)
                Photos.IsEnabled = true;
            PhotosTab.IsEnabled = true;
            SeriesManager.IsEnabled = true;
            ReferenceTab.IsEnabled = true;
            AddPhotoButton.IsEnabled = true;
            DeleteInputPhotosButton.IsEnabled = true;
            EditPhotosOrder.IsEnabled = true;
            EditIntervalBetweenPhotos.IsEnabled = true;
            ReCalculate.IsEnabled = true;
            AutoCalculate.IsEnabled = true;

            if (IntervalBetweenPhotos.IsEnabled)
            {
                await SaveIntervalBetweenPhotosAsync();
                                
                IntervalBetweenPhotos.IsEnabled = false;
            }

            CreationTimeCheckBox.IsEnabled = true;
            MainMenuBar.IsEnabled = true;
            ChangeReferenceLine.IsEnabled = true;
            DeleteButton.IsEnabled = true;
            ChooseReferenceButton.IsEnabled = true;
        }

        #endregion
    }
}
