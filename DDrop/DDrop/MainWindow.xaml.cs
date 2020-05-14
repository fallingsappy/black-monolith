﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.BL.AppStateBL;
using DDrop.BL.Calculation;
using DDrop.BL.DropPhoto;
using DDrop.BL.GeometryBL;
using DDrop.BL.ImageProcessing.CSharp;
using DDrop.BL.ImageProcessing.Python;
using DDrop.BL.Series;
using DDrop.DAL;
using DDrop.Properties;
using DDrop.Utility.Animation;
using DDrop.Utility.DataGrid;
using DDrop.Utility.ExcelOperations;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.Logger;
using DDrop.Utility.Mappers;
using DDrop.Utility.SeriesLocalStorageOperations;
using Microsoft.Win32;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Brushes = System.Windows.Media.Brushes;
using pbu = RFM.RFM_WPFProgressBarUpdate;
using Point = System.Drawing.Point;

namespace DDrop
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow(ISeriesBL seriesBL, IDropPhotoBL dropPhotoBL, IDDropRepository dDropRepository,
            IDropletImageProcessor dropletImageProcessor, IPythonProvider pythonProvider, ILogger logger,
            IGeometryBL geometryBL, IAppStateBL appStateBL, ICalculationBL calculationBL)
        {
            InitializeComponent();
            InitializePaths();

            _seriesBL = seriesBL;
            _dropPhotoBL = dropPhotoBL;
            _dDropRepository = dDropRepository;
            _dropletImageProcessor = dropletImageProcessor;
            _pythonProvider = pythonProvider;
            _logger = logger;
            _geometryBL = geometryBL;
            _appStateBL = appStateBL;
            _calculationBL = calculationBL;

            AppMainWindow.Show();

            SeriesManagerIsLoading();
            ProgressBar.IsIndeterminate = true;

            Login();
        }

        #region Variable Declaration

        private bool _allSelectedSeriesChanging;
        private bool? _allSelectedSeries = false;
        private bool _allSelectedPhotosChanging;
        private bool? _allSelectedPhotos = false;
        private Line _horizontalLineSeriesPreview;
        private Line _verticalLineSeriesPreview;

        private ObservableCollection<Line> _contourSeriesPreview;

        private readonly ISeriesBL _seriesBL;
        private readonly IDropPhotoBL _dropPhotoBL;
        private readonly IDDropRepository _dDropRepository;
        private readonly IDropletImageProcessor _dropletImageProcessor;
        private readonly IPythonProvider _pythonProvider;
        private readonly ILogger _logger;
        private readonly IGeometryBL _geometryBL;
        private readonly IAppStateBL _appStateBL;
        private readonly ICalculationBL _calculationBL;

        private readonly ObservableCollection<AutoCalculationTemplate> _autoCalculationDefaultTemplates =
            new ObservableCollection<AutoCalculationTemplate>();

        private ObservableCollection<AutoCalculationTemplate> _userAutoCalculationTemplates =
            new ObservableCollection<AutoCalculationTemplate>();

        private AutoCalculationTemplate _currentPhotoAutoCalculationTemplate;

        private DropPhoto _currentSeriesPreviewPhoto = new DropPhoto();
        private ObservableCollection<DropPhoto> _storedDropPhotos;

        private int _initialXDiameterInPixels;
        private int _initialYDiameterInPixels;
        public bool SaveRequired;
        private DropPhoto _copiedDropPhoto;

        public static readonly DependencyProperty СurrentPythonAutoCalculationTemplateProperty =
            DependencyProperty.Register("СurrentPythonAutoCalculationTemplate", typeof(AutoCalculationTemplate),
                typeof(MainWindow));

        public static readonly DependencyProperty СurrentCSharpAutoCalculationTemplateProperty =
            DependencyProperty.Register("СurrentCSharpAutoCalculationTemplate", typeof(AutoCalculationTemplate),
                typeof(MainWindow));

        public static readonly DependencyProperty PythonAutoCalculationTemplateProperty =
            DependencyProperty.Register("PythonAutoCalculationTemplate",
                typeof(ObservableCollection<AutoCalculationTemplate>), typeof(MainWindow));

        public static readonly DependencyProperty CSharpAutoCalculationTemplateProperty =
            DependencyProperty.Register("CSharpAutoCalculationTemplate",
                typeof(ObservableCollection<AutoCalculationTemplate>), typeof(MainWindow));

        public static readonly DependencyProperty ImageForEditProperty =
            DependencyProperty.Register("ImageForEdit", typeof(ImageSource), typeof(MainWindow));

        public static readonly DependencyProperty CurrentSeriesProperty =
            DependencyProperty.Register("CurrentSeries", typeof(Series), typeof(MainWindow));

        public static readonly DependencyProperty CurrentDropPhotoProperty =
            DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(MainWindow));

        public static readonly DependencyProperty ReferenceImageProperty =
            DependencyProperty.Register("ReferenceImage", typeof(ImageSource), typeof(MainWindow));

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(User), typeof(MainWindow));

        public static readonly DependencyProperty ParticularSeriesIndexProperty =
            DependencyProperty.Register("ParticularSeriesIndex", typeof(int), typeof(MainWindow));

        private readonly Notifier _notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                Application.Current.MainWindow,
                Corner.BottomRight,
                10,
                10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                TimeSpan.FromSeconds(3),
                MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        #endregion

        #region Properties

        public AutoCalculationTemplate СurrentPythonAutoCalculationTemplate
        {
            get => (AutoCalculationTemplate)GetValue(СurrentPythonAutoCalculationTemplateProperty);
            set => SetValue(СurrentPythonAutoCalculationTemplateProperty, value);
        }

        public AutoCalculationTemplate СurrentCSharpAutoCalculationTemplate
        {
            get => (AutoCalculationTemplate)GetValue(СurrentCSharpAutoCalculationTemplateProperty);
            set => SetValue(СurrentCSharpAutoCalculationTemplateProperty, value);
        }

        public ObservableCollection<AutoCalculationTemplate> PythonAutoCalculationTemplate
        {
            get => (ObservableCollection<AutoCalculationTemplate>)GetValue(PythonAutoCalculationTemplateProperty);
            set => SetValue(PythonAutoCalculationTemplateProperty, value);
        }

        public ObservableCollection<AutoCalculationTemplate> CSharpAutoCalculationTemplate
        {
            get => (ObservableCollection<AutoCalculationTemplate>)GetValue(CSharpAutoCalculationTemplateProperty);
            set => SetValue(CSharpAutoCalculationTemplateProperty, value);
        }

        public ImageSource ImageForEdit
        {
            get => (ImageSource)GetValue(ImageForEditProperty);
            set => SetValue(ImageForEditProperty, value);
        }

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
            set
            {
                SetValue(ParticularSeriesIndexProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        #region Authorization

        private void Login()
        {
            var login = new Login(_dDropRepository, _notifier, _logger)
            {
                Owner = AppMainWindow
            };
            login.ShowDialog();

            if (login.LoginSucceeded)
            {
                User = login.UserLogin;

                SeriesDataGrid.ItemsSource = User.UserSeries;

                SeriesManagerLoadingComplete();
                ProgressBar.IsIndeterminate = false;
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

        #endregion

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
                        foreach (var userSeries in User.UserSeries)
                            userSeries.IsChecked = true;
                    else if (AllSelectedSeries == false)
                        foreach (var userSeries in User.UserSeries)
                            userSeries.IsChecked = false;
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

                var seriesToAdd = new Series
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
                    var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));

                    await Task.Run(() =>
                        _dDropRepository.CreateSeries(
                            DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(seriesToAdd, dbUser)));

                    seriesToAdd.PropertyChanged += EntryOnPropertyChanged;

                    User.UserSeries.Add(seriesToAdd);
                    SeriesDataGrid.ItemsSource = User.UserSeries;
                    OneLineSetterValue.Text = "";

                    _notifier.ShowSuccess($"Добавлена новая серия {seriesToAdd.Title}");
                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Series,
                        Message = $"Добавлена новая серия {seriesToAdd.Title}"
                    });
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Серия {seriesToAdd.Title} не добавлена. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
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
                PreviewCanvas.Children.Remove(_horizontalLineSeriesPreview);
                PreviewCanvas.Children.Remove(_verticalLineSeriesPreview);
                if (_contourSeriesPreview != null)
                {
                    foreach (var line in _contourSeriesPreview)
                    {
                        PreviewCanvas.Children.Remove(line);
                    }
                }
                
                if (CurrentDropPhoto?.HorizontalLine != null)
                    ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);
                if (CurrentDropPhoto?.VerticalLine != null)
                    ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);

                if (CurrentDropPhoto?.Contour != null)
                {
                    foreach (var line in CurrentDropPhoto.Contour.Lines)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(line);
                    }
                }

                SingleSeries.IsEnabled = false;
                ProgressBar.IsIndeterminate = true;
                SeriesManagerIsLoading(false);

                var old = new Series
                {
                    CurrentUser = User,
                    CurrentUserId = User.UserId
                };

                if (e.RemovedItems.Count > 0)
                    old = e.RemovedItems[0] as Series;

                CurrentSeries = User.UserSeries[SeriesDataGrid.SelectedIndex];

                if (CurrentSeries.ReferencePhotoForSeries != null)
                    try
                    {
                        var referencePhotoId = CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId;
                        CurrentSeries.ReferencePhotoForSeries.Content = await Task.Run(() =>
                            _dDropRepository.GetReferencePhotoContent(referencePhotoId));
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            "Не удалось загрузить референсный снимок. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = User.Email,
                            Details = exception.HelpLink
                        });
                        throw;
                    }

                SeriesDrawerSwap(old);
                Photos.ItemsSource = null;
                SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                ReferenceImage = null;
                ParticularSeriesIndex = SeriesDataGrid.SelectedIndex;

                if (CurrentSeries?.ReferencePhotoForSeries?.Content != null)
                    ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);

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
            var selectedPhoto = (DropPhoto) SeriesPreviewDataGrid.SelectedItem;
            if (selectedPhoto != null)
            {
                _currentSeriesPreviewPhoto = selectedPhoto;

                PreviewCanvas.Children.Clear();
                _appStateBL.ShowAdorner(PreviewLoading);

                try
                {
                    if (_tokenSource != null) _tokenSource.Cancel();

                    _tokenSource = new CancellationTokenSource();

                    ImgPreview.Source = null;

                    ProgressBar.IsIndeterminate = true;

                    _currentSeriesPreviewPhoto.Content = await Task.Run(() =>
                        _dDropRepository.GetDropPhotoContent(_currentSeriesPreviewPhoto.DropPhotoId,
                            _tokenSource.Token));

                    ImgPreview.Source = ImageInterpreter.LoadImage(_currentSeriesPreviewPhoto.Content);

                    _geometryBL.PrepareLines(_currentSeriesPreviewPhoto, out _horizontalLineSeriesPreview,
                        out _verticalLineSeriesPreview, Settings.Default.ShowLinesOnPreview);

                    _geometryBL.PrepareContour(_currentSeriesPreviewPhoto, out _contourSeriesPreview,
                        Settings.Default.ShowContourOnPreview);
                }
                catch (OperationCanceledException)
                {
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось загрузить снимок {_currentSeriesPreviewPhoto.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                _appStateBL.HideAdorner(PreviewLoading);

                PreviewCanvas.Children.Clear();
                PreviewCanvas.Children.Add(ImgPreview);
                if (_horizontalLineSeriesPreview != null)
                    PreviewCanvas.Children.Add(_horizontalLineSeriesPreview);
                if (_verticalLineSeriesPreview != null)
                    PreviewCanvas.Children.Add(_verticalLineSeriesPreview);
                if (_contourSeriesPreview != null)
                    foreach (var line in _contourSeriesPreview)
                        PreviewCanvas.Children.Add(line);

                ProgressBar.IsIndeterminate = false;
            }
            else
            {
                ImgPreview.Source = null;
            }
        }

        private void CombinedSeriesPlot_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem) sender;

            if (tabItem.IsEnabled && User != null) _notifier.ShowSuccess("Новый общий график серий построен");
        }

        private void ExportSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Any(x => x.IsChecked))
            {
                if (User.IsAnySelectedSeriesCanDrawPlot)
                {
                    SeriesManagerIsLoading();
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        AddExtension = true,
                        CheckPathExists = true
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                            ExcelOperations.CreateSingleSeriesExcelFile(User, saveFileDialog.FileName);

                            _notifier.ShowSuccess($"Файл {saveFileDialog.SafeFileName} успешно сохранен.");
                        }
                        catch (InvalidOperationException exception)
                        {
                            _notifier.ShowError($"{exception.InnerException?.InnerException?.Message}");
                        }
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

                var messageBoxResult =
                    MessageBox.Show(checkedCount > 0 ? "Удалить выбранные серии?" : "Удалить все серии?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0, User.UserSeries.Count, 0);
                    SeriesWindowLoading(false);
                    SeriesManagerIsLoading();
                    for (var i = User.UserSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && !User.UserSeries[i].IsChecked) continue;

                            if (User.UserSeries[i] == CurrentSeries)
                            {
                                ClearSeriesView();
                            }

                            await _seriesBL.DeleteSeries(User.UserSeries[i], CurrentSeries, MainWindowPixelDrawer.CanDrawing);

                            _notifier.ShowSuccess($"Серия {User.UserSeries[i].Title} была удалена.");

                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Series,
                                Message = $"Серия {User.UserSeries[i].Title} была удалена."
                            });

                            PreviewCanvas.Children.Remove(_horizontalLineSeriesPreview);
                            PreviewCanvas.Children.Remove(_verticalLineSeriesPreview);
                            if (_contourSeriesPreview != null)
                            {
                                foreach (var line in _contourSeriesPreview)
                                {
                                    PreviewCanvas.Children.Remove(line);
                                }
                            }

                            if (CurrentDropPhoto?.HorizontalLine != null)
                                ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);
                            if (CurrentDropPhoto?.VerticalLine != null)
                                ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);

                            User.UserSeries.Remove(User.UserSeries[i]);
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Exception = exception.ToString(),
                                LogCategory = LogCategory.Common,
                                InnerException = exception.InnerException?.Message,
                                Message = exception.Message,
                                StackTrace = exception.StackTrace,
                                Username = User.Email,
                                Details = exception.TargetSite.Name
                            });
                            throw;
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
            var messageBoxResult =
                MessageBox.Show($"Удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    SeriesWindowLoading();
                    SeriesManagerIsLoading();
                    PreviewCanvas.Children.Remove(_horizontalLineSeriesPreview);
                    PreviewCanvas.Children.Remove(_verticalLineSeriesPreview);
                    if (_contourSeriesPreview != null)
                    {
                        foreach (var line in _contourSeriesPreview)
                        {
                            PreviewCanvas.Children.Remove(line);
                        }
                    }

                    if (CurrentDropPhoto?.HorizontalLine != null)
                        ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);
                    if (CurrentDropPhoto?.VerticalLine != null)
                        ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);

                    await _seriesBL.DeleteSeries(User.UserSeries[SeriesDataGrid.SelectedIndex], CurrentSeries, MainWindowPixelDrawer.CanDrawing);

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Series,
                        Message = $"Серия {User.UserSeries[SeriesDataGrid.SelectedIndex].Title} была удалена."
                    });
                    _notifier.ShowSuccess($"Серия {User.UserSeries[SeriesDataGrid.SelectedIndex].Title} была удалена.");

                    User.UserSeries.RemoveAt(SeriesDataGrid.SelectedIndex);
                    ClearSeriesView();
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                SeriesManagerLoadingComplete();
                SeriesWindowLoading();
            }
        }

        private void ClearSeriesView()
        {
            Photos.ItemsSource = null;
            ImgPreview.Source = null;
            SeriesPreviewDataGrid.ItemsSource = null;
            SeriesPreviewDataGrid.SelectedIndex = -1;
        }

        private void SingleSeriesPlotTabItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabItem = (TabItem) sender;

            if (tabItem.IsEnabled && User != null) _notifier.ShowSuccess("Новый график построен");
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc)
            {
                var item = (TabItem) tc.SelectedItem;

                if (item?.Name != null)
                {
                    if (item.Name == "SingleSeries")
                    {
                        Photos.ItemsSource = CurrentSeries.DropPhotosSeries;

                        if (!CurrentSeries.CanDrawPlot && SingleSeriesTabControl.SelectedIndex == 2)
                            SingleSeriesTabControl.SelectedIndex = 0;
                    }
                    else if (item.Name == "SeriesManager")
                    {
                        if (SeriesDataGrid.SelectedItems.Count > 0 && CurrentSeries?.DropPhotosSeries != null)
                            SeriesPreviewDataGrid.ItemsSource = CurrentSeries.DropPhotosSeries;
                    }
                }
            }
        }

        private async void ExportSeriesLocal_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Count > 0)
            {
                var saveFileDialog = new VistaFolderBrowserDialog
                {
                    UseDescriptionForTitle = true,
                    Description = "Выберите папку для сохранения..."
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    SeriesWindowLoading(false);
                    SeriesManagerIsLoading();

                    var checkedCount = User.UserSeries.Count(x => x.IsChecked);

                    var pbuHandle1 = pbu.New(ProgressBar, 0, checkedCount > 0 ? checkedCount : User.UserSeries.Count,
                        0);

                    foreach (var series in User.UserSeries)
                    {
                        if (checkedCount > 0 && !series.IsChecked) continue;

                        try
                        {
                            var fullDbSeries = await Task.Run(() =>
                                _dDropRepository.GetFullDbSeriesForExportById(series.SeriesId));

                            await Task.Run(() => DDropDbEntitiesMapper.ExportSeriesLocalAsync(
                                $"{saveFileDialog.SelectedPath}\\{series.Title}.ddrops", fullDbSeries));

                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Series,
                                Message = $"файл {series.Title}.ddrops сохранен на диске."
                            });
                            _notifier.ShowSuccess($"файл {series.Title}.ddrops сохранен на диске.");
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Не загрузить серию {series.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Exception = exception.ToString(),
                                LogCategory = LogCategory.Common,
                                InnerException = exception.InnerException?.Message,
                                Message = exception.Message,
                                StackTrace = exception.StackTrace,
                                Username = User.Email,
                                Details = exception.TargetSite.Name
                            });
                            throw;
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
            var openFileDialog = new OpenFileDialog
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
                try
                {
                    var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));

                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            var dbSerieForAdd = await Task.Run(() =>
                                DDropDbEntitiesMapper.ImportLocalSeriesAsync(fileName, dbUser));

                            var deserializedSerie =
                                DDropDbEntitiesMapper.SingleDbSerieToSerie(dbSerieForAdd, User, true);

                            try
                            {
                                await Task.Run(() =>
                                    _dDropRepository.CreateFullSeries(
                                        DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(deserializedSerie, dbUser)));
                                
                                if (deserializedSerie.ReferencePhotoForSeries?.Content != null)
                                    deserializedSerie.ReferencePhotoForSeries.Content = null;
                                
                                foreach (var dropPhoto in deserializedSerie.DropPhotosSeries) dropPhoto.Content = null;

                                User.UserSeries.Add(deserializedSerie);

                                _logger.LogInfo(new LogEntry
                                {
                                    Username = User.Email,
                                    LogCategory = LogCategory.Series,
                                    Message = $"Серия {dbSerieForAdd.Title} добавлена."
                                });
                                _notifier.ShowSuccess($"Серия {dbSerieForAdd.Title} добавлена.");
                            }
                            catch (TimeoutException)
                            {
                                _notifier.ShowError(
                                    $"Не удалось сохранить серию серию {dbSerieForAdd.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                            }
                            catch (Exception exception)
                            {
                                _logger.LogError(new LogEntry
                                {
                                    Exception = exception.ToString(),
                                    LogCategory = LogCategory.Common,
                                    InnerException = exception.InnerException?.Message,
                                    Message = exception.Message,
                                    StackTrace = exception.StackTrace,
                                    Username = User.Email,
                                    Details = exception.TargetSite.Name
                                });
                                throw;
                            }
                        }
                        catch (JsonException exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Series,
                                Message =
                                    $"Не удалось десериализовать файл {fileName}. Файл не является файлом серии или поврежден.",
                                Exception = exception.Message,
                                Details = exception.ToString(),
                                InnerException = exception.InnerException?.Message,
                                StackTrace = exception.StackTrace
                            });
                            _notifier.ShowError(
                                $"Не удалось десериализовать файл {fileName}. Файл не является файлом серии или поврежден.");
                        }

                        pbu.CurValue[pbuHandle1] += 1;
                    }
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось получить информацию о пользователе {User.Email}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
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
                    if (!string.IsNullOrWhiteSpace(seriesNameCell.Text))
                    {
                        var seriesId = CurrentSeries.SeriesId;
                        var text = seriesNameCell.Text;
                        await Task.Run(() => _dDropRepository.UpdateSeriesName(text, seriesId));

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Series,
                            Message = "Название серии изменено успешно."
                        });
                        _notifier.ShowSuccess("Название серии изменено успешно.");
                    }
                    else
                    {
                        _notifier.ShowInformation("Название серии не может быть пустым.");
                        seriesNameCell.Text = CurrentSeries.Title;
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название серии. Не удалось установить подключение. Проверьте интернет соединение.");
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogEntry
                {
                    Exception = exception.ToString(),
                    LogCategory = LogCategory.Common,
                    InnerException = exception.InnerException?.Message,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Username = User.Email,
                    Details = exception.TargetSite.Name
                });
                throw;
            }

            SeriesWindowLoading(false);
            SeriesManagerLoadingComplete();
        }

        private void LoadSeriesPreviewPhoto(DropPhoto dropPhoto)
        {
            PreviewCanvas.Children.Clear();

            ProgressBar.IsIndeterminate = true;
            ImgPreview.Source = null;
            _appStateBL.ShowAdorner(CurrentSeriesImageLoading);

            ImgPreview.Source = ImageInterpreter.LoadImage(_currentSeriesPreviewPhoto.Content);

            _geometryBL.PrepareLines(dropPhoto, out _horizontalLineSeriesPreview, out _verticalLineSeriesPreview,
                Settings.Default.ShowLinesOnPreview);
            _geometryBL.PrepareContour(dropPhoto, out _contourSeriesPreview, Settings.Default.ShowContourOnPreview);

            ProgressBar.IsIndeterminate = false;
            _appStateBL.HideAdorner(CurrentSeriesImageLoading);

            PreviewCanvas.Children.Clear();
            if (ImgPreview != null)
                PreviewCanvas.Children.Add(ImgPreview);
            if (_horizontalLineSeriesPreview != null)
                PreviewCanvas.Children.Add(_horizontalLineSeriesPreview);
            if (_verticalLineSeriesPreview != null)
                PreviewCanvas.Children.Add(_verticalLineSeriesPreview);
            if (_contourSeriesPreview != null)
                foreach (var line in _contourSeriesPreview)
                    PreviewCanvas.Children.Add(line);
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
                if (int.TryParse(IntervalBetweenPhotos?.Text, out var intervalBetweenPhotos))
                {
                    try
                    {
                        SaveIntervalBetweenPhotos.Visibility = Visibility.Hidden;
                        EditIntervalBetweenPhotos.Visibility = Visibility.Visible;
                        if (IntervalBetweenPhotos != null) IntervalBetweenPhotos.IsEnabled = false;
                        ProgressBar.IsIndeterminate = true;

                        var seriesId = CurrentSeries.SeriesId;
                        await Task.Run(() =>
                            _dDropRepository.UpdateSeriesIntervalBetweenPhotos(intervalBetweenPhotos, seriesId));

                        CurrentSeries.IntervalBetweenPhotos = intervalBetweenPhotos;

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Series,
                            Message =
                                $"Сохранен новый интервал между снимками для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение."
                        });
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Не удалось сохранить новый временной интервал между снимками для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        if (IntervalBetweenPhotos != null)
                            IntervalBetweenPhotos.Text =
                                CurrentSeries.IntervalBetweenPhotos.ToString(CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = User.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }

                    ProgressBar.IsIndeterminate = false;
                }
                else
                {
                    CurrentSeries.IntervalBetweenPhotos = 0;
                    _notifier.ShowInformation(
                        "Некорректное значение для интервала между снимками. Укажите интервал между снимками в секундах.");
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
                        foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                            dropPhoto.IsChecked = true;
                    else if (AllSelectedPhotos == false)
                        foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                            dropPhoto.IsChecked = false;
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
            var openFileDialog = new OpenFileDialog
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

                SingleSeriesLoading();
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);

                for (var i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    pbu.CurValue[pbuHandle1] += 1;

                    if (CurrentSeries.DropPhotosSeries == null)
                    {
                        CurrentSeries.DropPhotosSeries = new ObservableCollection<DropPhoto>();
                        Photos.ItemsSource = CurrentSeries.DropPhotosSeries;
                    }

                    var imageForAdding = new DropPhoto
                    {
                        DropPhotoId = Guid.NewGuid(),
                        Name = openFileDialog.SafeFileNames[i],
                        Path = openFileDialog.FileNames[i],
                        Content = File.ReadAllBytes(openFileDialog.FileNames[i]),
                        AddedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        CurrentSeries = CurrentSeries,
                        CurrentSeriesId = CurrentSeries.SeriesId,
                        CreationDateTime = File.GetCreationTime(openFileDialog.FileNames[i])
                            .ToString(CultureInfo.InvariantCulture),
                        PhotoOrderInSeries = CurrentSeries.DropPhotosSeries.Count
                    };
                    imageForAdding.Drop = new Drop
                    {
                        DropId = imageForAdding.DropPhotoId,
                        Series = CurrentSeries,
                        DropPhoto = imageForAdding
                    };

                    imageForAdding.PropertyChanged += PhotosOnPropertyChanged;

                    if (ImageValidator.ValidateImage(imageForAdding.Content))
                        try
                        {
                            var seriesId = CurrentSeries.SeriesId;
                            await Task.Run(() =>
                                _dDropRepository.CreateDropPhoto(
                                    DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(imageForAdding, seriesId), seriesId));

                            imageForAdding.Content = null;
                            CurrentSeries.DropPhotosSeries.Add(imageForAdding);

                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.DropPhoto,
                                Message = $"Снимок {imageForAdding.Name} добавлен."
                            });
                            _notifier.ShowSuccess($"Снимок {imageForAdding.Name} добавлен.");
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Снимок {imageForAdding.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Exception = exception.ToString(),
                                LogCategory = LogCategory.Common,
                                InnerException = exception.InnerException?.Message,
                                Message = exception.Message,
                                StackTrace = exception.StackTrace,
                                Username = User.Email,
                                Details = exception.TargetSite.Name
                            });
                            throw;
                        }
                    else
                        _notifier.ShowError($"Файл {imageForAdding.Name} имеет неизвестный формат.");
                }

                SingleSeriesLoadingComplete();
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                _notifier.ShowSuccess("Новые снимки успешно добавлены.");
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }
        }

        private async void Photos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFile = (DropPhoto) Photos.SelectedItem;
            if (selectedFile != null)
            {
                try
                {
                    ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto?.HorizontalLine);
                    ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto?.VerticalLine);

                    if (CurrentDropPhoto?.Contour != null)
                        foreach (var line in CurrentDropPhoto.Contour.Lines)
                            ImgCurrent.CanDrawing.Children.Remove(line);


                    ProgressBar.IsIndeterminate = true;
                    ImageForEdit = null;
                    _appStateBL.ShowAdorner(CurrentSeriesImageLoading);
                    SingleSeriesLoading(false);

                    if (e.RemovedItems.Count > 0)
                    {
                        var oldCurrentPhoto = e.RemovedItems[0] as DropPhoto;
                        CurrentSeries.DropPhotosSeries.FirstOrDefault(x =>
                                oldCurrentPhoto != null && x.DropPhotoId == oldCurrentPhoto.DropPhotoId)
                            .Content = null;
                    }

                    _tokenSource?.Cancel();

                    _tokenSource = new CancellationTokenSource();

                    CurrentDropPhoto = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex];

                    if (CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Contour?.Parameters != null)
                    {
                        _currentPhotoAutoCalculationTemplate = new AutoCalculationTemplate
                        {
                            Title = "Текущий контур",
                            TemplateType = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Contour
                                .CalculationVariants,
                            Parameters = CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Contour.Parameters
                        };
                    }
                    else
                    {
                        _currentPhotoAutoCalculationTemplate = null;
                        CSharpTemplatesCombobox.SelectedIndex = 0;
                        PythonTemplatesCombobox.SelectedIndex = 0;
                        СurrentCSharpAutoCalculationTemplate =
                            _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                                x.TemplateType == CalculationVariants.CalculateWithCSharp);
                        СurrentPythonAutoCalculationTemplate =
                            _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                                x.TemplateType == CalculationVariants.CalculateWithPython);
                    }

                    BuildTemplates();

                    if (_loadPhotosContent)
                    {
                        CurrentDropPhoto.Content = await Task.Run(() =>
                            _dDropRepository.GetDropPhotoContent(selectedFile.DropPhotoId, _tokenSource.Token));
                        ImageForEdit = ImageInterpreter.LoadImage(CurrentDropPhoto.Content);
                    }

                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                    SingleSeriesLoadingComplete(false);
                }
                catch (OperationCanceledException)
                {
                    if (CurrentDropPhoto != null)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);
                        ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);
                    }
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось загрузить снимок {selectedFile.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                    SingleSeriesLoadingComplete(false);
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                ProgressBar.IsIndeterminate = false;
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                ImageForEdit = null;
            }
        }

        private void ShowLinesOnPhotosPreview(DropPhoto dropPhoto, Canvas canvas)
        {
            if (dropPhoto != null && canvas != null)
            {
                canvas.Children.Remove(dropPhoto.HorizontalLine);
                canvas.Children.Remove(dropPhoto.VerticalLine);

                if (dropPhoto.Contour != null)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Remove(line);

                if (dropPhoto.HorizontalLine != null && Settings.Default.ShowLinesOnPreview ||
                    dropPhoto.HorizontalLine != null && _photoEditModeOn)
                    canvas.Children.Add(dropPhoto.HorizontalLine);
                if (dropPhoto.VerticalLine != null && Settings.Default.ShowLinesOnPreview ||
                    dropPhoto.VerticalLine != null && _photoEditModeOn)
                    canvas.Children.Add(dropPhoto.VerticalLine);
                if (dropPhoto.Contour != null && Settings.Default.ShowContourOnPreview ||
                    dropPhoto.Contour != null && _autoCalculationModeOn)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Add(line);
            }
        }

        private async void DeleteInputPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                var checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);

                var messageBoxResult =
                    MessageBox.Show(checkedCount > 0 ? "Удалить выбранные снимки?" : "Удалить все снимки?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0,
                        checkedCount > 0 ? checkedCount : CurrentSeries.DropPhotosSeries.Count, 0);
                    SingleSeriesLoading();
                    _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);

                    for (var i = CurrentSeries.DropPhotosSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && !CurrentSeries.DropPhotosSeries[i].IsChecked) continue;

                            await DeleteDropPhoto(CurrentSeries.DropPhotosSeries[i], CurrentDropPhoto, ImgCurrent.CanDrawing);

                            _notifier.ShowSuccess($"Снимок {CurrentSeries.DropPhotosSeries[i].Name} успешно удален.");
                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.DropPhoto,
                                Message = $"Снимок {CurrentSeries.DropPhotosSeries[i].Name} успешно удален."
                            });

                            CurrentSeries.DropPhotosSeries.RemoveAt(i);
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[i].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Exception = exception.ToString(),
                                LogCategory = LogCategory.Common,
                                InnerException = exception.InnerException?.Message,
                                Message = exception.Message,
                                StackTrace = exception.StackTrace,
                                Username = User.Email,
                                Details = exception.TargetSite.Name
                            });
                            throw;
                        }

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                    SingleSeriesLoadingComplete();
                }
            }
            else
            {
                _notifier.ShowInformation("Нет снимков для удаления.");
            }
        }

        public async Task DeleteDropPhoto(DropPhoto dropPhoto, DropPhoto currentDropPhoto, Canvas canvas)
        {
            if (currentDropPhoto != null && dropPhoto.DropPhotoId ==
                currentDropPhoto.DropPhotoId)
            {
                canvas.Children.Remove(dropPhoto.HorizontalLine);
                canvas.Children.Remove(dropPhoto.VerticalLine);

                if (dropPhoto.Contour != null)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Remove(line);
            }

            var photoForDeleteId = dropPhoto.DropPhotoId;
            await Task.Run(() => _dDropRepository.DeleteDropPhoto(photoForDeleteId));
        }

        private async void DeleteSingleInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult =
                MessageBox.Show($"Удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ProgressBar.IsIndeterminate = true;
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                try
                {
                    await DeleteDropPhoto(CurrentSeries.DropPhotosSeries[Photos.SelectedIndex], CurrentDropPhoto, ImgCurrent.CanDrawing);

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.DropPhoto,
                        Message = $"Снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} удален."
                    });

                    _notifier.ShowSuccess(
                        $"Снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name} удален.");
                    CurrentSeries.DropPhotosSeries.RemoveAt(Photos.SelectedIndex);
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось удалить снимок {CurrentSeries.DropPhotosSeries[Photos.SelectedIndex].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                ProgressBar.IsIndeterminate = false;
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                SingleSeriesLoadingComplete();
            }
        }

        private async void EditInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PixelsInMillimeterTextBox.Text) && PixelsInMillimeterTextBox.Text != "0")
            {
                await PhotoEditModeOn();

                ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                if (CurrentDropPhoto.HorizontalLine == null)
                    CurrentDropPhoto.HorizontalLine = new Line();

                if (CurrentDropPhoto.VerticalLine == null)
                    CurrentDropPhoto.VerticalLine = new Line();

                _copiedDropPhoto = new DropPhoto
                {
                    DropPhotoId = CurrentDropPhoto.DropPhotoId,
                    HorizontalLine = CurrentDropPhoto.HorizontalLine,
                    VerticalLine = CurrentDropPhoto.VerticalLine,
                    SimpleHorizontalLine = CurrentDropPhoto.SimpleHorizontalLine,
                    SimpleVerticalLine = CurrentDropPhoto.SimpleVerticalLine
                };


                _geometryBL.StoreContour(CurrentDropPhoto, _copiedDropPhoto);

                Photos.ItemsSource = new ObservableCollection<DropPhoto> {CurrentDropPhoto};

                _initialXDiameterInPixels = CurrentDropPhoto.XDiameterInPixels;
                _initialYDiameterInPixels = CurrentDropPhoto.YDiameterInPixels;

                PixelsInMillimeterHorizontalTextBox.Text = _initialXDiameterInPixels.ToString();
                PixelsInMillimeterVerticalTextBox.Text = _initialYDiameterInPixels.ToString();
            }
            else
            {
                _notifier.ShowInformation("Выберите референсное расстояние на референсном снимке.");
            }
        }

        private bool _photoEditModeOn;

        private async Task PhotoEditModeOn()
        {
            SaveInputPhotoEditButton.IsEnabled = true;
            DiscardManualPhotoEdit.IsEnabled = true;
            _photoEditModeOn = true;
            _autoCalculationModeOn = true;
            SeriesEditMenu.Visibility = Visibility.Hidden;
            EditPhotosColumn.Visibility = Visibility.Hidden;
            DeletePhotosColumn.Visibility = Visibility.Hidden;
            PhotosCheckedColumn.Visibility = Visibility.Hidden;
            ImgCurrent.DrawningIsEnabled = true;

            VisualHelper.SetEnableRowsMove(Photos, true);
            if (Application.Current.MainWindow != null)
            {
                if (PhotosPreviewRow.ActualHeight > PhotosGrid.ActualHeight * 0.6)
                {
                    VisualHelper.SetEnableRowsMove(Photos, false);
                    await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, PhotosPreviewRow.ActualHeight, PhotosGrid.ActualHeight * 0.6, 0, 0, 200);
                }
                else
                {
                    await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true,
                        PhotosGrid.ActualHeight * 0.6,
                        PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
                }
            }

            ManualEditMenu.Visibility = Visibility.Visible;

            _overrideLoadingBehaviour = true;
            SingleSeriesLoading();
            _loadPhotosContent = false;
        }

        public async Task PhotoEditModeOff()
        {
            _photoEditModeOn = false;
            _autoCalculationModeOn = false;
            SeriesEditMenu.Visibility = Visibility.Visible;
            EditPhotosColumn.Visibility = Visibility.Visible;
            DeletePhotosColumn.Visibility = Visibility.Visible;
            PhotosCheckedColumn.Visibility = Visibility.Visible;
            ImgCurrent.DrawningIsEnabled = false;

            VisualHelper.SetEnableRowsMove(Photos, false);

            if (PhotosPreviewRow.ActualHeight > PhotosGrid.ActualHeight * 0.5)
            {
                VisualHelper.SetEnableRowsMove(Photos, false);
                await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, PhotosPreviewRow.ActualHeight, PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
            }
            else
            {
                await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true,
                    PhotosPreviewRow.ActualHeight,
                    PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
            }

            ManualEditMenu.Visibility = Visibility.Hidden;

            Photos.ItemsSource = CurrentSeries.DropPhotosSeries;
            _overrideLoadingBehaviour = false;
            SingleSeriesLoadingComplete();
            _loadPhotosContent = true;

            Photos.ScrollIntoView(Photos.SelectedItem);
            _copiedDropPhoto = null;
        }

        private void VerticalRulerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            HorizontalRulerToggleButton.IsChecked = false;
            ImgCurrent._drawingVerticalLine = true;
        }

        private void VerticalRulerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ImgCurrent._drawingVerticalLine = false;
        }

        private void HorizontalRulerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            VerticalRulerToggleButton.IsChecked = false;
            ImgCurrent._drawingHorizontalLine = true;
        }

        private void HorizontalRulerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ImgCurrent._drawingHorizontalLine = false;
        }

        private async void SaveInputPhotoEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaveRequired()) await SavePixelDiameters();

            await PhotoEditModeOff();
            ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
        }

        private async Task SavePixelDiameters()
        {
            SaveInputPhotoEditButton.IsEnabled = false;
            DiscardManualPhotoEdit.IsEnabled = false;

            var xDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterHorizontalTextBox.Text);
            var yDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterVerticalTextBox.Text);

            CurrentDropPhoto.Drop.RadiusInMeters = 0;
            CurrentDropPhoto.Drop.VolumeInCubicalMeters = 0;
            CurrentDropPhoto.Drop.XDiameterInMeters = 0;
            CurrentDropPhoto.Drop.YDiameterInMeters = 0;
            CurrentDropPhoto.Drop.ZDiameterInMeters = 0;

            if (_initialXDiameterInPixels != xDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.XDiameterInPixels = xDiameterInPixelsTextBox;
                _initialXDiameterInPixels = xDiameterInPixelsTextBox;
            }

            if (_initialYDiameterInPixels != yDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.YDiameterInPixels = yDiameterInPixelsTextBox;
                _initialYDiameterInPixels = yDiameterInPixelsTextBox;
            }

            SaveRequired = false;

            if (CurrentDropPhoto.XDiameterInPixels != 0 && CurrentDropPhoto.YDiameterInPixels != 0)
            {
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                _appStateBL.ShowAdorner(CurrentSeriesImageLoading);

                SingleSeriesLoading();

                await ReCalculateDropParameters(CurrentDropPhoto);

                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                await _dropPhotoBL.UpdateDropPhoto(CurrentDropPhoto);
                _notifier.ShowInformation(
                    $"Не указан один из диаметров. Расчет для снимка {CurrentDropPhoto.Name} не выполнен.");
            }
        }

        private bool IsSaveRequired()
        {
            int xDiameterInPixelsTextBox;
            int yDiameterInPixelsTextBox;

            if (int.TryParse(PixelsInMillimeterHorizontalTextBox.Text, out xDiameterInPixelsTextBox) &&
                int.TryParse(PixelsInMillimeterVerticalTextBox.Text, out yDiameterInPixelsTextBox))
                if (_initialXDiameterInPixels != xDiameterInPixelsTextBox ||
                    _initialYDiameterInPixels != yDiameterInPixelsTextBox)
                    return SaveRequired = true;

            return SaveRequired;
        }

        private async void DiscardManualPhotoEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsSaveRequired())
            {
                if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    await SavePixelDiameters();
                }
                else
                {
                    ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);
                    ImgCurrent.CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);

                    _geometryBL.RestoreOriginalContour(CurrentDropPhoto, _copiedDropPhoto, ImgCurrent.CanDrawing,
                        CurrentDropPhoto.DropPhotoId);

                    if (CurrentDropPhoto.Contour != null)
                    {
                        CurrentDropPhoto.Contour.Lines = new ObservableCollection<Line>();

                        foreach (var contourSimpleLine in CurrentDropPhoto.Contour.SimpleLines)
                            CurrentDropPhoto.Contour.Lines.Add(new Line
                            {
                                X1 = contourSimpleLine.X1,
                                X2 = contourSimpleLine.X2,
                                Y1 = contourSimpleLine.Y1,
                                Y2 = contourSimpleLine.Y2,
                                Stroke = Brushes.Red,
                                StrokeThickness = 2
                            });
                    }

                    CurrentDropPhoto.HorizontalLine = _copiedDropPhoto.HorizontalLine;
                    CurrentDropPhoto.VerticalLine = _copiedDropPhoto.VerticalLine;
                    CurrentDropPhoto.SimpleVerticalLine = _copiedDropPhoto.SimpleVerticalLine;
                    CurrentDropPhoto.SimpleHorizontalLine = _copiedDropPhoto.SimpleHorizontalLine;

                    CurrentDropPhoto.XDiameterInPixels = _initialXDiameterInPixels;
                    CurrentDropPhoto.YDiameterInPixels = _initialYDiameterInPixels;

                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
                }
            }

            await PhotoEditModeOff();
            ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
        }

        private async void Photos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SingleSeriesLoading();

            var photoNameCell = e.EditingElement as TextBox;
            try
            {
                if (photoNameCell != null)
                {
                    if (!string.IsNullOrWhiteSpace(photoNameCell.Text))
                    {
                        ProgressBar.IsIndeterminate = true;

                        var currentDropPhotoId = CurrentDropPhoto.DropPhotoId;
                        var text = photoNameCell.Text;
                        await Task.Run(() => _dDropRepository.UpdateDropPhotoName(text, currentDropPhotoId));

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = "Название снимка изменено успешно."
                        });
                        _notifier.ShowSuccess("Название снимка изменено успешно.");
                    }
                    else
                    {
                        _notifier.ShowInformation("Название снимка не может быть пустым.");
                        photoNameCell.Text = CurrentDropPhoto.Name;
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название снимка. Не удалось установить подключение. Проверьте интернет соединение.");
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogEntry
                {
                    Exception = exception.ToString(),
                    LogCategory = LogCategory.Common,
                    InnerException = exception.InnerException?.Message,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Username = User.Email,
                    Details = exception.TargetSite.Name
                });
                throw;
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

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.DropPhoto,
                        Message =
                            $"Серия {CurrentSeries.Title} использует время создания снимков. Порядок фотографий будет проигнорирован."
                    });
                    _notifier.ShowSuccess(
                        $"Серия {CurrentSeries.Title} использует время создания снимков. Порядок фотографий будет проигнорирован.");
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        "Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
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

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.DropPhoto,
                        Message = $"Серия {CurrentSeries.Title} использует интервал между снимками."
                    });
                    _notifier.ShowSuccess($"Серия {CurrentSeries.Title} использует интервал между снимками.");
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        "Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                ProgressBar.IsIndeterminate = false;
            }
        }

        private async void ReCalculate_Click(object sender, RoutedEventArgs e)
        {
            await ReCalculateDropParameters(true);
        }

        private Dictionary<Guid, int> _originalOrder;
        private bool _loadPhotosContent = true;

        private async void EditPhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            await PhotosReOrderModeOn();

            _originalOrder = new Dictionary<Guid, int>();

            foreach (var item in CurrentSeries.DropPhotosSeries)
                _originalOrder.Add(item.DropPhotoId, item.PhotoOrderInSeries);
        }

        private async Task PhotosReOrderModeOn()
        {
            SeriesEditMenu.Visibility = Visibility.Hidden;
            EditPhotosColumn.Visibility = Visibility.Hidden;
            DeletePhotosColumn.Visibility = Visibility.Hidden;
            PhotosCheckedColumn.Visibility = Visibility.Hidden;
            PhotosPreviewGridSplitter.IsEnabled = false;
            VisualHelper.SetEnableRowsMove(Photos, true);
            await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, 300, 0, 0, 0, 200);
            SavePhotoOrderMenu.Visibility = Visibility.Visible;

            _overrideLoadingBehaviour = true;
            SingleSeriesLoading(false);
            _loadPhotosContent = false;
        }

        public async Task PhotosReOrderModeOff()
        {
            SeriesEditMenu.Visibility = Visibility.Visible;
            EditPhotosColumn.Visibility = Visibility.Visible;
            DeletePhotosColumn.Visibility = Visibility.Visible;
            PhotosCheckedColumn.Visibility = Visibility.Visible;
            PhotosPreviewGridSplitter.IsEnabled = true;
            VisualHelper.SetEnableRowsMove(Photos, false);
            await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true, 300, 0, 0, 0, 200);
            SavePhotoOrderMenu.Visibility = Visibility.Hidden;

            _overrideLoadingBehaviour = false;
            SingleSeriesLoadingComplete(false);
            _loadPhotosContent = true;
        }

        private async void SavePhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var orderChanged = OrderChanged();

            if (orderChanged)
            {
                var messageBoxResult = MessageBox.Show("Сохранить новый порядок снимков?",
                    "Подтверждение выхода", MessageBoxButton.YesNoCancel);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _dDropRepository.UpdatePhotosOrderInSeries(
                            DDropDbEntitiesMapper.ListOfDropPhotosToListOfDbDropPhotos(CurrentSeries.DropPhotosSeries,
                                CurrentSeries.SeriesId));

                        _logger.LogInfo(new LogEntry
                        {
                            Username = CurrentSeries.CurrentUser.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = $"Порядок снимков для серии {CurrentSeries.Title} обновлен."
                        });

                        await PhotosReOrderModeOff();
                        _notifier.ShowSuccess($"Порядок снимков для серии {CurrentSeries.Title} обновлен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Не удалось обновить порядок снимков для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        DiscardNewPhotosOrder();
                        await PhotosReOrderModeOff();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = CurrentSeries.CurrentUser.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    DiscardNewPhotosOrder();

                    _logger.LogInfo(new LogEntry
                    {
                        Username = CurrentSeries.CurrentUser.Email,
                        LogCategory = LogCategory.DropPhoto,
                        Message = "Cтарый порядок снимков восстановлен."
                    });

                    await PhotosReOrderModeOff();
                }
            }
            else
            {
                var messageBoxResult = MessageBox.Show("Закончить редактирование порядка снимков",
                    "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes) await PhotosReOrderModeOff();
            }
        }

        private bool OrderChanged()
        {
            var orderChanged = false;

            foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                if (_originalOrder[dropPhoto.DropPhotoId] != dropPhoto.PhotoOrderInSeries)
                {
                    orderChanged = true;
                    break;
                }

            return orderChanged;
        }

        private void DiscardNewPhotosOrder()
        {
            foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                dropPhoto.PhotoOrderInSeries = _originalOrder[dropPhoto.DropPhotoId];

            CurrentSeries.DropPhotosSeries = OrderByPhotoOrderInSeries(CurrentSeries.DropPhotosSeries);
            _notifier.ShowInformation("Cтарый порядок снимков восстановлен.");
        }

        private async void DiscardPhotosReOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (OrderChanged())
            {
                var messageBoxResult = MessageBox.Show("Отменить изменение порядка снимков?",
                    "Подтверждение", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DiscardNewPhotosOrder();
                    await PhotosReOrderModeOff();
                }
            }
            else
            {
                _notifier.ShowInformation("Порядок снимков остался без изменений.");
                await PhotosReOrderModeOff();
            }
        }

        private static ObservableCollection<DropPhoto> OrderByPhotoOrderInSeries(
            ObservableCollection<DropPhoto> orderThoseGroups)
        {
            ObservableCollection<DropPhoto> temp;
            temp = new ObservableCollection<DropPhoto>(orderThoseGroups.OrderBy(p => p.PhotoOrderInSeries));
            orderThoseGroups.Clear();
            foreach (var j in temp) orderThoseGroups.Add(j);
            return orderThoseGroups;
        }

        #endregion

        #region Reference Photo

        private async void ChooseReference_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
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
                _appStateBL.ShowAdorner(ReferenceImageLoading);

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
                            Line = new Line()
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
                        await Task.Run(() =>
                            _dDropRepository.UpdateReferencePhoto(
                                DDropDbEntitiesMapper.ReferencePhotoToDbReferencePhoto(newReferencePhoto)));

                        if (CurrentSeries.ReferencePhotoForSeries.Line != null)
                        {
                            MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries
                                .Line);

                            MainWindowPixelDrawer.PixelsInMillimeter = "";
                            CurrentSeries.ReferencePhotoForSeries.Line = null;
                            CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                        }

                        CurrentSeries.ReferencePhotoForSeries = newReferencePhoto;
                        ReferenceImage = ImageInterpreter.LoadImage(CurrentSeries.ReferencePhotoForSeries.Content);

                        MainWindowPixelDrawer.IsEnabled = false;
                        ChangeReferenceLine.Visibility = Visibility.Visible;
                        SaveReferenceLine.Visibility = Visibility.Hidden;

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.ReferencePhoto,
                            Message = $"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} добавлен."
                        });
                        _notifier.ShowSuccess(
                            $"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} добавлен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = User.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }

                    ProgressBar.IsIndeterminate = false;
                    SingleSeriesLoadingComplete();
                    _appStateBL.HideAdorner(ReferenceImageLoading);
                }
                else
                {
                    _notifier.ShowError($"Файл {openFileDialog.FileName} имеет неизвестный формат.");
                }
            }
        }

        private async void DeleteReferencePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.ReferencePhotoForSeries?.Content != null)
            {
                var messageBoxResult = MessageBox.Show("Удалить референсный снимок?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SingleSeriesLoading();
                    _appStateBL.ShowAdorner(ReferenceImageLoading);

                    ProgressBar.IsIndeterminate = true;
                    MainWindowPixelDrawer.IsEnabled = false;
                    ChangeReferenceLine.Visibility = Visibility.Visible;
                    SaveReferenceLine.Visibility = Visibility.Hidden;

                    try
                    {
                        var referencePhotoId = CurrentSeries.ReferencePhotoForSeries.ReferencePhotoId;
                        await Task.Run(() => _dDropRepository.DeleteReferencePhoto(referencePhotoId));

                        MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                        _notifier.ShowSuccess(
                            $"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} удален.");
                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.ReferencePhoto,
                            Message = $"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} удален."
                        });

                        MainWindowPixelDrawer.PixelsInMillimeter = "";
                        CurrentSeries.ReferencePhotoForSeries.Name = null;
                        CurrentSeries.ReferencePhotoForSeries.Line = null;
                        CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                        CurrentSeries.ReferencePhotoForSeries.Content = null;
                        ReferenceImage = null;
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Референсный снимок {CurrentSeries.ReferencePhotoForSeries.Name} не удален. Не удалось установить подключение. Проверьте интернет соединение.");
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = User.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }

                    SingleSeriesLoadingComplete();
                    _appStateBL.HideAdorner(ReferenceImageLoading);
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
            if (CurrentSeries.ReferencePhotoForSeries.SimpleLine != null && IsReferencePhotoLineChanged())
            {
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(ReferenceImageLoading);

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var dbReferencePhoto =
                        DDropDbEntitiesMapper.ReferencePhotoToDbReferencePhoto(CurrentSeries.ReferencePhotoForSeries);

                    await Task.Run(() => _dDropRepository.UpdateReferencePhoto(dbReferencePhoto));

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.ReferencePhoto,
                        Message = $"Сохранено новое референсное расстояние для серии {CurrentSeries.Title}."
                    });
                    _notifier.ShowSuccess($"Сохранено новое референсное расстояние для серии {CurrentSeries.Title}.");

                    if (CurrentSeries.DropPhotosSeries.Any(x => x.XDiameterInPixels > 0 && x.YDiameterInPixels > 0))
                        await ReCalculateDropParameters();
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось сохранить новое референсное расстояние для серии {CurrentSeries.Title}.");
                    DiscardReferenceLineChanges();
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                ReferenceEditModeOff();
                _appStateBL.HideAdorner(ReferenceImageLoading);
                ProgressBar.IsIndeterminate = false;
            }
            else
            {
                ReferenceEditModeOff();
                _notifier.ShowInformation("Нет изменений для сохранения.");
            }
        }

        private async Task ReCalculateDropParameters(bool checkForChecked = false)
        {
            if (CurrentSeries.ReferencePhotoForSeries?.PixelsInMillimeter > 0)
            {
                var checkedCount = 0;

                if (checkForChecked)
                    checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);

                var messageBoxResult =
                    MessageBox.Show("Пересчитать параметры капель?", "Подтверждение", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ProgressBar.IsIndeterminate = false;

                    var pbuHandle1 = pbu.New(ProgressBar, 0, CurrentSeries.DropPhotosSeries.Count, 0);

                    foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                    {
                        if (checkForChecked && checkedCount > 0 && !dropPhoto.IsChecked)
                            continue;

                        if (dropPhoto.XDiameterInPixels > 0 && dropPhoto.YDiameterInPixels > 0)
                            await ReCalculateDropParameters(dropPhoto);

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);
                }
            }
            else
            {
                _notifier.ShowInformation("Выберите референсное расстояние на референсном снимке.");
            }
        }

        private async Task ReCalculateDropParameters(DropPhoto dropPhoto)
        {
            {
                var tempDrop = new Drop
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
                    await _calculationBL.CalculateDropParameters(dropPhoto, PixelsInMillimeterTextBox.Text,
                        dropPhoto.DropPhotoId);

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Calculation,
                        Message = $"Расчет для снимка {dropPhoto.Name} выполнен."
                    });
                    _notifier.ShowSuccess($"Расчет для снимка {dropPhoto.Name} выполнен.");
                }
                catch (TimeoutException)
                {
                    dropPhoto.Drop = tempDrop;
                    _notifier.ShowError(
                        "Не удалось сохранить результаты расчета. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.Common,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }
            }
        }

        private int _storedReferencePhotoPixelsInMillimeter;
        private Line _storedReferenceLine;

        private void ChangeReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.ReferencePhotoForSeries?.Content != null)
            {
                if (CurrentSeries.ReferencePhotoForSeries.SimpleLine != null)
                {
                    _storedReferencePhotoPixelsInMillimeter = CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter;
                    _storedReferenceLine = new Line
                    {
                        X1 = CurrentSeries.ReferencePhotoForSeries.SimpleLine.X1,
                        X2 = CurrentSeries.ReferencePhotoForSeries.SimpleLine.X2,
                        Y1 = CurrentSeries.ReferencePhotoForSeries.SimpleLine.Y1,
                        Y2 = CurrentSeries.ReferencePhotoForSeries.SimpleLine.Y2,
                        Stroke = Brushes.DeepPink,
                        StrokeThickness = 2
                    };
                }

                ReferenceEditModeOn();
            }
            else
            {
                _notifier.ShowInformation("Загрузите референсный снимок.");
            }
        }

        private void ReferenceEditModeOn()
        {
            MainWindowPixelDrawer.IsEnabled = true;
            MainWindowPixelDrawer.DrawningIsEnabled = true;
            ChangeReferenceLine.Visibility = Visibility.Hidden;
            SaveReferenceLine.Visibility = Visibility.Visible;
            CancelReferencePhotoEditing.Visibility = Visibility.Visible;


            PhotosTab.IsEnabled = false;
            SeriesManager.IsEnabled = false;
            ReferenceTab.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            ChooseReferenceButton.IsEnabled = false;
        }

        public void ReferenceEditModeOff()
        {
            MainWindowPixelDrawer.IsEnabled = false;
            MainWindowPixelDrawer.DrawningIsEnabled = false;
            ChangeReferenceLine.Visibility = Visibility.Visible;
            SaveReferenceLine.Visibility = Visibility.Hidden;
            CancelReferencePhotoEditing.Visibility = Visibility.Hidden;
            SingleSeriesLoadingComplete();
        }

        private void CancelReferencePhotoEditing_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult =
                MessageBox.Show("Отменить изменения?", "Подтверждение", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) DiscardReferenceLineChanges();
        }

        private void DiscardReferenceLineChanges()
        {
            if (_storedReferenceLine != null)
            {
                MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                CurrentSeries.ReferencePhotoForSeries.Line.X1 = _storedReferenceLine.X1;
                CurrentSeries.ReferencePhotoForSeries.Line.X2 = _storedReferenceLine.X2;
                CurrentSeries.ReferencePhotoForSeries.Line.Y1 = _storedReferenceLine.Y1;
                CurrentSeries.ReferencePhotoForSeries.Line.Y2 = _storedReferenceLine.Y2;
                CurrentSeries.ReferencePhotoForSeries.Line.Stroke = _storedReferenceLine.Stroke;
                CurrentSeries.ReferencePhotoForSeries.Line.StrokeThickness = _storedReferenceLine.StrokeThickness;

                CurrentSeries.ReferencePhotoForSeries.SimpleLine.X1 = _storedReferenceLine.X1;
                CurrentSeries.ReferencePhotoForSeries.SimpleLine.X2 = _storedReferenceLine.X2;
                CurrentSeries.ReferencePhotoForSeries.SimpleLine.Y1 = _storedReferenceLine.Y1;
                CurrentSeries.ReferencePhotoForSeries.SimpleLine.Y2 = _storedReferenceLine.Y2;

                CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = _storedReferencePhotoPixelsInMillimeter;

                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentSeries.ReferencePhotoForSeries.Line);
            }
            else
            {
                CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = 0;
                MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);
                CurrentSeries.ReferencePhotoForSeries.Line = null;
                CurrentSeries.ReferencePhotoForSeries.SimpleLine = null;
            }

            ReferenceEditModeOff();
        }

        private bool IsReferencePhotoLineChanged()
        {
            if (_storedReferenceLine == null && CurrentSeries.ReferencePhotoForSeries.Line != null) return true;

            if (CurrentSeries.ReferencePhotoForSeries.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentSeries.ReferencePhotoForSeries.Line.X1 - _storedReferenceLine.X1) > 0.001)
                    return true;

            if (CurrentSeries.ReferencePhotoForSeries.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentSeries.ReferencePhotoForSeries.Line.X2 - _storedReferenceLine.X2) > 0.001)
                    return true;

            if (CurrentSeries.ReferencePhotoForSeries.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentSeries.ReferencePhotoForSeries.Line.Y1 - _storedReferenceLine.Y1) > 0.001)
                    return true;

            if (CurrentSeries.ReferencePhotoForSeries.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentSeries.ReferencePhotoForSeries.Line.Y2 - _storedReferenceLine.Y2) > 0.001)
                    return true;

            return false;
        }

        #endregion

        #region AutoCalculation

        private bool _overrideLoadingBehaviour;

        private async void StartAutoCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PixelsInMillimeterTextBox.Text) && PixelsInMillimeterTextBox.Text != "0")
            {
                InitilizeTemplates();
                EndPythonTemplateAdding();
                EndCSharpTemplateAdding();

                _autoCalculationModeOn = true;
                _photoEditModeOn = true;
                SeriesEditMenu.Visibility = Visibility.Hidden;
                EditPhotosColumn.Visibility = Visibility.Hidden;
                DeletePhotosColumn.Visibility = Visibility.Hidden;
                AutoCalculationGridSplitter.IsEnabled = true;
                if (CurrentDropPhoto != null)
                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
                
                AutoCalculationMenu.Visibility = Visibility.Visible;

                SingleSeriesLoading(false);
                _overrideLoadingBehaviour = true;

                _storedDropPhotos = new ObservableCollection<DropPhoto>();

                foreach (var dropPhoto in CurrentSeries.DropPhotosSeries) StoreDropPhoto(dropPhoto, _storedDropPhotos);

                await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, true, 300, 0,
                    AutoCalculationColumn.MinWidth, 0, 200);
            }
            else
            {
                _notifier.ShowInformation("Выберите референсное расстояние на референсном снимке.");
            }
        }

        private void InitilizeTemplates()
        {
            _autoCalculationDefaultTemplates.Clear();
            _userAutoCalculationTemplates.Clear();

            InitilizeUserTemplates();
            InitilizeDefaultTemplates();
            BuildTemplates();
        }

        private void StoreDropPhoto(DropPhoto dropPhoto, ObservableCollection<DropPhoto> storeTo)
        {
            storeTo.Add(new DropPhoto
            {
                DropPhotoId = dropPhoto.DropPhotoId,
                SimpleHorizontalLine = dropPhoto.SimpleHorizontalLine != null
                    ? new SimpleLine
                    {
                        X1 = dropPhoto.SimpleHorizontalLine.X1,
                        X2 = dropPhoto.SimpleHorizontalLine.X2,
                        Y1 = dropPhoto.SimpleHorizontalLine.Y1,
                        Y2 = dropPhoto.SimpleHorizontalLine.Y2,
                        SimpleLineId = dropPhoto.SimpleHorizontalLineId.GetValueOrDefault()
                    }
                    : null,
                SimpleVerticalLine = dropPhoto.SimpleVerticalLine != null
                    ? new SimpleLine
                    {
                        X1 = dropPhoto.SimpleVerticalLine.X1,
                        X2 = dropPhoto.SimpleVerticalLine.X2,
                        Y1 = dropPhoto.SimpleVerticalLine.Y1,
                        Y2 = dropPhoto.SimpleVerticalLine.Y2,
                        SimpleLineId = dropPhoto.SimpleVerticalLineId.GetValueOrDefault()
                    }
                    : null
            });

            _geometryBL.StoreContour(dropPhoto, _storedDropPhotos[_storedDropPhotos.Count - 1]);
        }

        private async void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            _appStateBL.ShowAdorner(CurrentSeriesImageLoading);
            _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
            AutoCalculationMenu.IsEnabled = false;

            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, false, 300, 0,
                AutoCalculationColumn.MinWidth, 0, 200);

            var checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);
            CalculationVariants calculationVariant;

            if (CurrentSeries.DropPhotosSeries.Count > 0)
            {
                var pbuHandle1 = pbu.New(ProgressBar, 0,
                    checkedCount > 0 ? checkedCount : CurrentSeries.DropPhotosSeries.Count, 0);

                for (var i = 0; i < CurrentSeries.DropPhotosSeries.Count; i++)
                {
                    if (checkedCount > 0 && !CurrentSeries.DropPhotosSeries[i].IsChecked) continue;

                    CurrentSeries.DropPhotosSeries[i].RequireSaving = true;

                    Point[] points;
                    try
                    {
                        if (CurrentSeries.DropPhotosSeries[i].Content == null)
                            CurrentSeries.DropPhotosSeries[i].Content =
                                await _dDropRepository.GetDropPhotoContent(
                                    CurrentSeries.DropPhotosSeries[i].DropPhotoId, CancellationToken.None);


                        if ((CalculationVariants) Settings.Default.AutoCalculationType ==
                            CalculationVariants.CalculateWithPython)
                        {
                            if (!PythonFilesIsSelected())
                            {
                                _notifier.ShowInformation("Выберите интерпритатор python и исполняемый скрипт.");
                                break;
                            }

                            try
                            {
                                points = CalculateWithPython(CurrentSeries.DropPhotosSeries[i]);
                                calculationVariant = CalculationVariants.CalculateWithPython;
                            }
                            catch (InvalidOperationException exception)
                            {
                                _notifier.ShowError(
                                    $"{exception.Message} для снимка {CurrentSeries.DropPhotosSeries[i].Name}.");

                                _logger.LogError(new LogEntry
                                {
                                    Exception = exception.ToString(),
                                    LogCategory = LogCategory.Common,
                                    InnerException = exception.InnerException?.Message,
                                    Message = exception.Message,
                                    StackTrace = exception.StackTrace,
                                    Username = User.Email,
                                    Details = exception.TargetSite.Name
                                });

                                continue;
                            }
                            catch (Win32Exception exception)
                            {
                                _notifier.ShowError("Указанный в качестве интерпретатора файл не является исполняемым.");

                                _logger.LogError(new LogEntry
                                {
                                    Exception = exception.ToString(),
                                    LogCategory = LogCategory.Common,
                                    InnerException = exception.InnerException?.Message,
                                    Message = exception.Message,
                                    StackTrace = exception.StackTrace,
                                    Username = User.Email,
                                    Details = exception.TargetSite.Name
                                });

                                continue;
                            }
                            catch (FormatException exception)
                            {
                                _notifier.ShowError("Не удалось построить точки.");

                                _logger.LogError(new LogEntry
                                {
                                    Exception = exception.ToString(),
                                    LogCategory = LogCategory.Common,
                                    InnerException = exception.InnerException?.Message,
                                    Message = exception.Message,
                                    StackTrace = exception.StackTrace,
                                    Username = User.Email,
                                    Details = exception.TargetSite.Name
                                });

                                continue;
                            }
                        }
                        else if ((CalculationVariants) Settings.Default.AutoCalculationType == CalculationVariants.CalculateWithCSharp)
                        {
                            try
                            {
                                points = _dropletImageProcessor.GetDiameters(CurrentSeries.DropPhotosSeries[i].Content);
                                calculationVariant = CalculationVariants.CalculateWithCSharp;
                            }
                            catch (InvalidOperationException exception)
                            {
                                _notifier.ShowError(
                                    $"{exception.Message} для снимка {CurrentSeries.DropPhotosSeries[i].Name}. Попробуйте изменить параметры расчета.");
                                continue;
                            }
                        }
                        else
                        {
                            _notifier.ShowInformation("Не выбран обработчик для расчета.");
                            break;
                        }

                        if (CurrentSeries.DropPhotosSeries[i] != CurrentDropPhoto)
                            CurrentSeries.DropPhotosSeries[i].Content = null;

                        if (points != null)
                        {
                            _geometryBL.CreateContour(CurrentSeries.DropPhotosSeries[i], points, calculationVariant,
                                CShrpKsize.Text, CShrpSize1.Text, CShrpSize2.Text, CShrpThreshold1.Text,
                                CShrpThreshold2.Text, Ksize.Text, Size1.Text, Size2.Text, Threshold1.Text, Threshold2.Text,
                                CurrentDropPhoto, ImgCurrent);
                            _geometryBL.CreateDiameters(CurrentSeries.DropPhotosSeries[i], points);
                        }
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Не удалось произвести расчет для фотографии {CurrentSeries.DropPhotosSeries[i].Name}.");
                        continue;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = User.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }

                    pbu.CurValue[pbuHandle1] += 1;

                    if (CurrentDropPhoto != null &&
                        CurrentDropPhoto.DropPhotoId == CurrentSeries.DropPhotosSeries[i].DropPhotoId)
                        ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                    _calculationBL.ReCalculateAllParametersFromLines(CurrentSeries.DropPhotosSeries[i],
                        PixelsInMillimeterTextBox.Text);
                }

                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
                _notifier.ShowSuccess("Расчет завершен.");

                await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, true, 300, 0,
                    AutoCalculationColumn.MinWidth, 0, 200);
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                Photos.IsEnabled = true;
                AutoCalculationMenu.IsEnabled = true;
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                _notifier.ShowInformation("Нет фотографий для расчета.");
            }
        }

        private void ComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox != null && comboBox.SelectedIndex == -1)
            {
                comboBox.SelectedIndex = Settings.Default.AutoCalculationType;
            }
            else
            {
                if (comboBox != null) Settings.Default.AutoCalculationType = comboBox.SelectedIndex;
                Settings.Default.Save();
            }
        }

        private void UndoCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            var checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);
            var message = GetDiscardMessage(checkedCount);
            var messageBoxResult = MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) DiscardAutoCalculationChanges(false, checkedCount);
        }

        private void InitializePaths()
        {
            InterpreterTextBox.Text = Settings.Default.Interpreter;
            ScriptToRunTextBox.Text = Settings.Default.ScriptToRun;
        }

        private void ChooseFilePath_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddExtension = true
            };

            if (openFileDialog.ShowDialog() == true)
                UpdateOptions((OptionsEnum) Enum.Parse(typeof(OptionsEnum), button.Name), openFileDialog.FileName);
        }

        private void UpdateOptions(OptionsEnum option, object value)
        {
            switch (option)
            {
                case OptionsEnum.Interpreter:
                    Settings.Default.Interpreter = (string) value;
                    Settings.Default.Save();
                    InterpreterTextBox.Text = (string) value;
                    break;
                case OptionsEnum.ScriptToRun:
                    Settings.Default.ScriptToRun = (string) value;
                    Settings.Default.Save();
                    ScriptToRunTextBox.Text = (string) value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        private Point[] CalculateWithPython(DropPhoto dropPhoto)
        {
            if (PythonFilesIsSelected())
                return _pythonProvider.GetDiameters(dropPhoto.Content, dropPhoto.Name,
                    Settings.Default.ScriptToRun, Settings.Default.Interpreter);

            _notifier.ShowInformation("Выберите интерпритатор python и исполняемый скрипт.");

            return null;
        }

        private bool PythonFilesIsSelected()
        {
            return !string.IsNullOrWhiteSpace(Settings.Default.ScriptToRun) ||
                                            !string.IsNullOrWhiteSpace(Settings.Default.Interpreter);
        }

        private async void SaveCalculationResults_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Any(x => x.RequireSaving))
            {
                var checkedCount = CurrentSeries.DropPhotosSeries.Count(x => x.IsChecked);
                var requireSavingCount = CurrentSeries.DropPhotosSeries.Count(x => x.RequireSaving);

                var messageBoxResult =
                    MessageBox.Show(
                        checkedCount > 0
                            ? "Сохранить результаты расчета для выбранных снимков?"
                            : "Сохранить результаты расчета ? ",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0,
                        checkedCount > 0 ? checkedCount : requireSavingCount, 0);

                    _appStateBL.ShowAdorner(CurrentSeriesImageLoading);
                    _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                    AutoCalculationMenu.IsEnabled = false;

                    for (var i = CurrentSeries.DropPhotosSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && CurrentSeries.DropPhotosSeries[i].RequireSaving &&
                                !CurrentSeries.DropPhotosSeries[i].IsChecked)
                            {
                                DiscardAutoCalculationChange(CurrentSeries.DropPhotosSeries[i]);
                                continue;
                            }

                            if (checkedCount > 0 && !CurrentSeries.DropPhotosSeries[i].IsChecked) continue;

                            if (!CurrentSeries.DropPhotosSeries[i].RequireSaving) continue;

                            if (CurrentSeries.DropPhotosSeries[i].Content == null)
                                CurrentSeries.DropPhotosSeries[i].Content =
                                    await _dDropRepository.GetDropPhotoContent(
                                        CurrentSeries.DropPhotosSeries[i].DropPhotoId,
                                        CancellationToken.None);

                            var dbPhoto =
                                DDropDbEntitiesMapper.DropPhotoToDbDropPhoto(CurrentSeries.DropPhotosSeries[i],
                                    CurrentSeries.SeriesId);
                            await Task.Run(() => _dDropRepository.UpdateDropPhoto(dbPhoto));

                            if (CurrentDropPhoto != null && CurrentSeries.DropPhotosSeries[i].DropPhotoId !=
                                CurrentDropPhoto.DropPhotoId) CurrentSeries.DropPhotosSeries[i].Content = null;

                            _notifier.ShowSuccess(
                                $"Результаты авторасчета для {CurrentSeries.DropPhotosSeries[i].Name} успешно сохранены.");
                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.AutoCalculation,
                                Message =
                                    $"Результаты авторасчета для {CurrentSeries.DropPhotosSeries[i].Name} успешно сохранены."
                            });
                        }
                        catch (TimeoutException)
                        {
                            DiscardAutoCalculationChange(CurrentSeries.DropPhotosSeries[i]);
                            _notifier.ShowError(
                                $"Не удалось сохранить результаты авторасчета для {CurrentSeries.DropPhotosSeries[i].Name}. Не удалось установить подключение. Проверьте интернет соединение. Результаты авторасчета отменены.");
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Exception = exception.ToString(),
                                LogCategory = LogCategory.Common,
                                InnerException = exception.InnerException?.Message,
                                Message = exception.Message,
                                StackTrace = exception.StackTrace,
                                Username = User.Email,
                                Details = exception.TargetSite.Name
                            });
                            throw;
                        }

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    
                    _appStateBL.HideAdorner(CurrentSeriesImageLoading);
                    _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                    AutoCalculationMenu.IsEnabled = true;

                    await AutoCalculationModeOff();

                    SingleSeriesLoadingComplete(false);
                }
            }
            else
            {
                _notifier.ShowInformation("Нет изменений для сохранения.");
            }
        }

        private async void DiscardCalculationResults_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.DropPhotosSeries.Any(x => x.RequireSaving))
            {
                var messageBoxResult = MessageBox.Show("Закончить авторасчет без сохранения?",
                    "Подтверждение", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DiscardAutoCalculationChanges(true);

                    await AutoCalculationModeOff();
                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
                }
            }
            else
            {
                await AutoCalculationModeOff();
                ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
            }
        }


        private bool _autoCalculationModeOn;
        private async Task AutoCalculationModeOff()
        {
            SeriesEditMenu.Visibility = Visibility.Visible;
            EditPhotosColumn.Visibility = Visibility.Visible;
            DeletePhotosColumn.Visibility = Visibility.Visible;
            AutoCalculationGridSplitter.IsEnabled = false;

            AutoCalculationMenu.Visibility = Visibility.Hidden;

            _autoCalculationModeOn = false;
            _photoEditModeOn = false;

            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, false, 300, 0,
                AutoCalculationColumn.MinWidth, 0, 200);
            _overrideLoadingBehaviour = false;
            SingleSeriesLoadingComplete(false);
            _storedDropPhotos = null;
        }

        private void DiscardAutoCalculationChanges(bool discardAll = false, int checkedCount = 0)
        {
            if (CurrentSeries.DropPhotosSeries.Any(x => x.RequireSaving))
            {
                foreach (var dropPhoto in CurrentSeries.DropPhotosSeries)
                {
                    if (checkedCount > 0 && !dropPhoto.IsChecked && discardAll == false) continue;

                    if (dropPhoto.RequireSaving == false && dropPhoto.IsChecked)
                    {
                        _notifier.ShowInformation($"Нет изменений для снимка {dropPhoto.Name}.");
                        continue;
                    }

                    DiscardAutoCalculationChange(dropPhoto);
                }

                Photos.IsEnabled = true;
                _notifier.ShowSuccess("Изменения успешно отменены.");
            }
            else
            {
                _notifier.ShowInformation($"Нет изменений для серии {CurrentSeries.Title}.");
            }
        }

        private void DiscardAutoCalculationChange(DropPhoto dropPhoto)
        {
            dropPhoto.RequireSaving = false;

            var storedPhoto = _storedDropPhotos.FirstOrDefault(x => x.DropPhotoId == dropPhoto.DropPhotoId);

            if (storedPhoto != null)
            {
                _geometryBL.RestoreOriginalLines(dropPhoto, storedPhoto, ImgCurrent.CanDrawing);

                _geometryBL.RestoreOriginalContour(dropPhoto, storedPhoto, ImgCurrent.CanDrawing,
                    CurrentDropPhoto.DropPhotoId);
            }

            _calculationBL.ReCalculateAllParametersFromLines(dropPhoto, PixelsInMillimeterTextBox.Text);

            if (dropPhoto.DropPhotoId == CurrentDropPhoto?.DropPhotoId)
                ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
        }

        private static string GetDiscardMessage(int checkedCount)
        {
            if (checkedCount > 0) return "Отменить изменения для выбранных снимков?";

            return "Отменить все изменения?";
        }

        private void InitilizeUserTemplates()
        {
            if (!string.IsNullOrEmpty(Settings.Default.AutoCalculationTemplates))
                _userAutoCalculationTemplates =
                    JsonSerializeProvider.DeserializeFromString<ObservableCollection<AutoCalculationTemplate>>(
                        Settings.Default.AutoCalculationTemplates);
            else
                _userAutoCalculationTemplates = new ObservableCollection<AutoCalculationTemplate>();
        }

        private void InitilizeDefaultTemplates()
        {
            _autoCalculationDefaultTemplates.Add(new AutoCalculationTemplate
            {
                Title = "Default",
                Parameters = new AutoCalculationParameters
                {
                    Ksize = 9,
                    Size1 = 100,
                    Size2 = 250,
                    Treshold1 = 50,
                    Treshold2 = 100
                },
                TemplateType = CalculationVariants.CalculateWithCSharp
            });

            _autoCalculationDefaultTemplates.Add(new AutoCalculationTemplate
            {
                Title = "Default",
                Parameters = new AutoCalculationParameters
                {
                    Ksize = 9,
                    Size1 = 100,
                    Size2 = 250,
                    Treshold1 = 50,
                    Treshold2 = 5
                },
                TemplateType = CalculationVariants.CalculateWithPython
            });
        }

        private void BuildTemplates()
        {
            PythonAutoCalculationTemplate = new ObservableCollection<AutoCalculationTemplate>();
            CSharpAutoCalculationTemplate = new ObservableCollection<AutoCalculationTemplate>();

            foreach (var autoCalculationDefaultTemplate in _autoCalculationDefaultTemplates)
            {
                if (autoCalculationDefaultTemplate.TemplateType == CalculationVariants.CalculateWithPython)
                    PythonAutoCalculationTemplate.Add(autoCalculationDefaultTemplate);
                if (autoCalculationDefaultTemplate.TemplateType == CalculationVariants.CalculateWithCSharp)
                    CSharpAutoCalculationTemplate.Add(autoCalculationDefaultTemplate);
            }

            foreach (var userAutoCalculationTemplate in _userAutoCalculationTemplates)
                switch (userAutoCalculationTemplate.TemplateType)
                {
                    case CalculationVariants.CalculateWithPython:
                        PythonAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        break;
                    case CalculationVariants.CalculateWithCSharp:
                        CSharpAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        break;
                    case CalculationVariants.Common:
                        CSharpAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        PythonAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            if (_currentPhotoAutoCalculationTemplate != null)
            {
                if (_currentPhotoAutoCalculationTemplate.TemplateType == CalculationVariants.CalculateWithCSharp)
                {
                    CSharpAutoCalculationTemplate.Add(_currentPhotoAutoCalculationTemplate);
                    CSharpTemplatesCombobox.SelectedIndex = CSharpAutoCalculationTemplate.Count - 1;
                    СurrentCSharpAutoCalculationTemplate = _currentPhotoAutoCalculationTemplate;

                    СurrentPythonAutoCalculationTemplate = null;
                    PythonTemplatesCombobox.SelectedIndex = -1;
                }
                else
                {
                    PythonAutoCalculationTemplate.Add(_currentPhotoAutoCalculationTemplate);
                    PythonTemplatesCombobox.SelectedIndex = PythonAutoCalculationTemplate.Count - 1;
                    СurrentPythonAutoCalculationTemplate = _currentPhotoAutoCalculationTemplate;

                    СurrentCSharpAutoCalculationTemplate = null;
                    CSharpTemplatesCombobox.SelectedIndex = -1;
                }
            }
            else
            {
                CSharpTemplatesCombobox.SelectedIndex = 0;
                PythonTemplatesCombobox.SelectedIndex = 0;
                СurrentCSharpAutoCalculationTemplate =
                    _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                        x.TemplateType == CalculationVariants.CalculateWithCSharp);
                СurrentPythonAutoCalculationTemplate =
                    _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                        x.TemplateType == CalculationVariants.CalculateWithPython);
            }
        }

        private void PythonTemplatesCombobox_OnDropDownClosed(object sender, EventArgs e)
        {
            var combobox = sender as ComboBox;

            СurrentPythonAutoCalculationTemplate = new AutoCalculationTemplate();

            if (combobox != null && combobox.SelectedIndex != -1)
                СurrentPythonAutoCalculationTemplate = PythonAutoCalculationTemplate[combobox.SelectedIndex];
            else
                СurrentPythonAutoCalculationTemplate = null;
        }

        private void CSharpTemplatesCombobox_OnDropDownClosed(object sender, EventArgs e)
        {
            var combobox = sender as ComboBox;

            СurrentCSharpAutoCalculationTemplate = new AutoCalculationTemplate();

            if (combobox != null && combobox.SelectedIndex != -1)
                СurrentCSharpAutoCalculationTemplate = CSharpAutoCalculationTemplate[combobox.SelectedIndex];
            else
                СurrentCSharpAutoCalculationTemplate = null;
        }

        private void SavePythonTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxPythonTemplateName.Text))
            {
                _userAutoCalculationTemplates.Add(new AutoCalculationTemplate
                {
                    Title = TextBoxPythonTemplateName.Text,
                    Id = Guid.NewGuid(),
                    TemplateType = CalculationVariants.CalculateWithPython,
                    Parameters = new AutoCalculationParameters
                    {
                        Ksize = Ksize.Value ?? 1,
                        Size1 = Size1.Value ?? 1,
                        Size2 = Size2.Value ?? 1,
                        Treshold1 = Threshold1.Value ?? 1,
                        Treshold2 = Threshold2.Value ?? 1
                    }
                });

                Settings.Default.AutoCalculationTemplates =
                    JsonSerializeProvider.SerializeToString(_userAutoCalculationTemplates);

                Settings.Default.Save();
                InitilizeTemplates();

                EndPythonTemplateAdding();
            }
            else
            {
                _notifier.ShowInformation("Введите название для шаблона.");
            }
        }

        private void EndPythonTemplateAdding()
        {
            PythonTemplateName.Visibility = Visibility.Hidden;
            TextBoxPythonTemplateName.Visibility = Visibility.Hidden;
            SavePythonTemplate.Visibility = Visibility.Hidden;
            CancelPythonTemplateAdding.Visibility = Visibility.Hidden;

            AddPythonTemplate.Visibility = Visibility.Visible;
            PythonTemplatesCombobox.Visibility = Visibility.Visible;
            ChoosePythonTemplate.Visibility = Visibility.Visible;
        }

        private void SaveCSharpTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxCSharpTemplateName.Text))
            {
                _userAutoCalculationTemplates.Add(new AutoCalculationTemplate
                {
                    Title = TextBoxCSharpTemplateName.Text,
                    Id = Guid.NewGuid(),
                    TemplateType = CalculationVariants.CalculateWithCSharp,
                    Parameters = new AutoCalculationParameters
                    {
                        Ksize = CShrpKsize.Value ?? 1,
                        Size1 = CShrpSize1.Value ?? 1,
                        Size2 = CShrpSize2.Value ?? 1,
                        Treshold1 = CShrpThreshold1.Value ?? 1,
                        Treshold2 = CShrpThreshold2.Value ?? 1
                    }
                });

                Settings.Default.AutoCalculationTemplates =
                    JsonSerializeProvider.SerializeToString(_userAutoCalculationTemplates);

                Settings.Default.Save();
                InitilizeTemplates();

                EndCSharpTemplateAdding();
            }
            else
            {
                _notifier.ShowInformation("Введите название для шаблона.");
            }
        }

        private void EndCSharpTemplateAdding()
        {
            CSharpTemplateName.Visibility = Visibility.Hidden;
            TextBoxCSharpTemplateName.Visibility = Visibility.Hidden;
            SaveCSharpTemplate.Visibility = Visibility.Hidden;
            CancelCSharpTemplateAdding.Visibility = Visibility.Hidden;

            AddCSharpTemplate.Visibility = Visibility.Visible;
            CSharpTemplatesCombobox.Visibility = Visibility.Visible;
            ChooseCSharpTemplate.Visibility = Visibility.Visible;
        }

        private void AddPythonTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            PythonTemplateName.Visibility = Visibility.Visible;
            TextBoxPythonTemplateName.Visibility = Visibility.Visible;
            SavePythonTemplate.Visibility = Visibility.Visible;
            CancelPythonTemplateAdding.Visibility = Visibility.Visible;

            AddPythonTemplate.Visibility = Visibility.Hidden;
            PythonTemplatesCombobox.Visibility = Visibility.Hidden;
            ChoosePythonTemplate.Visibility = Visibility.Hidden;
        }

        private void AddCSharpTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            CSharpTemplateName.Visibility = Visibility.Visible;
            TextBoxCSharpTemplateName.Visibility = Visibility.Visible;
            SaveCSharpTemplate.Visibility = Visibility.Visible;
            CancelCSharpTemplateAdding.Visibility = Visibility.Visible;

            AddCSharpTemplate.Visibility = Visibility.Hidden;
            CSharpTemplatesCombobox.Visibility = Visibility.Hidden;
            ChooseCSharpTemplate.Visibility = Visibility.Hidden;
        }

        private void CancelPythonTemplateAdding_OnClick(object sender, RoutedEventArgs e)
        {
            EndPythonTemplateAdding();
        }

        private void CancelCSharpTemplateAdding_OnClick(object sender, RoutedEventArgs e)
        {
            EndCSharpTemplateAdding();
        }

        #endregion

        #region Account

        private void LoginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SeriesManagerIsLoading();
            ProgressBar.IsIndeterminate = true;

            Login();
        }

        private void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var accountMenu = new Account(User, _notifier, _dDropRepository, _logger);
            accountMenu.ShowDialog();
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Выйти из учетной записи {User.Email}?",
                "Подтверждение выхода", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MainTabControl.SelectedIndex = 0;

                if (CurrentSeries?.ReferencePhotoForSeries?.Line != null)
                    MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

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

                SeriesManagerIsLoading();
                ProgressBar.IsIndeterminate = true;

                Login();
            }
        }

        #endregion

        #region Menu

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Options_OnClick(object sender, RoutedEventArgs e)
        {
            var options = new Options(_notifier, _logger, User);
            options.ShowDialog();

            if (options.ShowLinesOnPreviewIsChanged || options.ShowContourOnPreviewIsChanged)
            {
                if (CurrentSeries != null && CurrentDropPhoto != null)
                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                if (_currentSeriesPreviewPhoto != null) LoadSeriesPreviewPhoto(_currentSeriesPreviewPhoto);
            }
        }

        private void AppMainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (User != null)
            {
                var messageBoxResult =
                    MessageBox.Show("Закрыть приложение?", "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    _notifier.Dispose();
                else
                    e.Cancel = true;
            }
        }

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            var information = new Information();
            information.ShowDialog();
        }

        #endregion

        #region Utility

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
            StartAutoCalculate.IsEnabled = false;
            CancelReferencePhotoEditing.IsEnabled = false;
            SaveReferenceLine.IsEnabled = false;

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
            if (_overrideLoadingBehaviour)
            {
                Photos.IsEnabled = true;
                return;
            }

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
            StartAutoCalculate.IsEnabled = true;
            CancelReferencePhotoEditing.IsEnabled = true;
            SaveReferenceLine.IsEnabled = true;

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

        private void SeriesWindowLoading(bool indeterminateLoadingBar = true)
        {
            if (indeterminateLoadingBar)
                ProgressBar.IsIndeterminate = !ProgressBar.IsIndeterminate;
            SeriesLoading.IsAdornerVisible = !SeriesLoading.IsAdornerVisible;
        }

        #endregion

        private async void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_photoEditModeOn)
            {
                if (Application.Current.MainWindow != null)
                {
                    if (PhotosPreviewRow.ActualHeight > PhotosGrid.ActualHeight * 0.6)
                    {
                        VisualHelper.SetEnableRowsMove(Photos, false);
                        await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, PhotosPreviewRow.ActualHeight, PhotosGrid.ActualHeight * 0.6, 0, 0, 200);
                    }
                    else
                    {
                        await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true,
                            PhotosGrid.ActualHeight * 0.6,
                            PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
                    }
                }
            }
            else
            {
                PhotosPreviewRow.Height = new GridLength(PhotosGrid.ActualHeight / 2);
            }
        }

        private void PhotosGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            PhotosPreviewRow.Height = new GridLength(PhotosGrid.ActualHeight / 2);
        }
    }
}