using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Utility.Mappers;
using System.Linq;
using System.Collections.ObjectModel;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для EditTable.xaml
    /// </summary>
    public partial class EditTable : Window
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
        public EditTable(Series originalSeries, IDDropRepository dDropRepository)
        {
            InitializeComponent();

            _originalOrder = new Dictionary<Guid, int>();

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
                    }
                    catch (Exception)
                    {
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
