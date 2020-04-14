using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Utility.Mappers;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using DDrop.BE.Enums.Logger;
using DDrop.Utility.Logger;
using ToastNotifications;
using ToastNotifications.Messages;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для EditTable.xaml
    /// </summary>
    public partial class EditTable
    {
        public static readonly DependencyProperty OriginalSeriesProperty = DependencyProperty.Register("OriginalSeries", typeof(Series), typeof(EditTable));

        public Series OriginalSeries
        {
            get { return (Series)GetValue(OriginalSeriesProperty); }
            set
            {
                SetValue(OriginalSeriesProperty, value);
            }
        }

        private Dictionary<Guid, int> _originalOrder;
        private readonly IDDropRepository _dDropRepository;
        private readonly ILogger _logger;
        private readonly Notifier _notifier;

        public EditTable(Series originalSeries, IDDropRepository dDropRepository, ILogger logger, Notifier notifier)
        {
            InitializeComponent();

            _originalOrder = new Dictionary<Guid, int>();
            _logger = logger;
            _notifier = notifier;

            foreach (var item in originalSeries.DropPhotosSeries)
            {
                _originalOrder.Add(item.DropPhotoId, item.PhotoOrderInSeries);
            }

            OriginalSeries = originalSeries;

            _dDropRepository = dDropRepository;
        }

        private void SavePhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void EditTable_OnClosing(object sender, CancelEventArgs e)
        {
            bool orderChanged = false;

            foreach (var dropPhoto in OriginalSeries.DropPhotosSeries)
            {
                if (_originalOrder[dropPhoto.DropPhotoId] != dropPhoto.PhotoOrderInSeries)
                {
                    orderChanged = true;
                    break;
                }                    
            }           

            if (orderChanged)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Сохранить новый порядок снимков?", "Подтверждение выхода", MessageBoxButton.YesNoCancel);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _dDropRepository.UpdatePhotosOrderInSeries(
                            DDropDbEntitiesMapper.ListOfDropPhotosToListOfDbDropPhotos(OriginalSeries.DropPhotosSeries,
                                OriginalSeries.SeriesId));

                        _logger.LogInfo(new LogEntry()
                        {
                            Username = OriginalSeries.CurrentUser.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = "Новый порядок снимков сохранен.",
                        });
                        _notifier.ShowSuccess("Новый порядок снимков сохранен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError("Не удалось сохранить новый порядок снимков. Проверьте интернет соединение.");
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
                            Username = OriginalSeries.CurrentUser.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }
                }
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    foreach (var dropPhoto in OriginalSeries.DropPhotosSeries)
                    {
                        dropPhoto.PhotoOrderInSeries = _originalOrder[dropPhoto.DropPhotoId];
                    }

                    OriginalSeries.DropPhotosSeries = OrderByPhotoOrderInSeries(OriginalSeries.DropPhotosSeries);

                    _logger.LogInfo(new LogEntry()
                    {
                        Username = OriginalSeries.CurrentUser.Email,
                        LogCategory = LogCategory.DropPhoto,
                        Message = "Cтарый порядок снимков восстановлен.",
                    });
                    _notifier.ShowInformation("Cтарый порядок снимков восстановлен.");
                }
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Закончить редактирование порядка снимков", "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private static ObservableCollection<DropPhoto> OrderByPhotoOrderInSeries(ObservableCollection<DropPhoto> orderThoseGroups)
        {
            ObservableCollection<DropPhoto> temp;
            temp = new ObservableCollection<DropPhoto>(orderThoseGroups.OrderBy(p => p.PhotoOrderInSeries));
            orderThoseGroups.Clear();
            foreach (DropPhoto j in temp) orderThoseGroups.Add(j);
            return orderThoseGroups;
        }
    }
}
