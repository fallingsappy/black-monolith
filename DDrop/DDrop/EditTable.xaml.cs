using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Utility.Mappers;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для EditTable.xaml
    /// </summary>
    public partial class EditTable : Window
    {
        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register("Series", typeof(Series), typeof(EditTable));

        public Series Series
        {
            get { return (Series)GetValue(SeriesProperty); }
            set
            {
                SetValue(SeriesProperty, value);
            }
        }

        private Series _originalSeries;
        private readonly IDDropRepository _dDropRepository;
        public EditTable(Series originalSeries, IDDropRepository dDropRepository)
        {
            InitializeComponent();

            var d = new ObservableCollection<DropPhoto>(originalSeries.DropPhotosSeries);

            Series seriesForEditing = new Series()
            {
                SeriesId = originalSeries.SeriesId,
                CurrentUser = originalSeries.CurrentUser
            };

            foreach (var dropPhoto in originalSeries.DropPhotosSeries)
            {
                seriesForEditing.DropPhotosSeries.Add(dropPhoto);
            }

            Series = seriesForEditing;
            _originalSeries = originalSeries;
            _dDropRepository = dDropRepository;
        }

        private void SavePhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void EditTable_OnClosing(object sender, CancelEventArgs e)
        {
            if (!Series.DropPhotosSeries.SequenceEqual(_originalSeries.DropPhotosSeries))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Сохранить новый порядок снимков?", "Подтверждение выхода", MessageBoxButton.YesNoCancel);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    await _dDropRepository.UpdatePhotosOrderInSeries(
                        DDropDbEntitiesMapper.ListOfDropPhotosToListOfDbDropPhotos(Series.DropPhotosSeries,
                            Series.SeriesId));
                    _originalSeries.DropPhotosSeries.Clear();
                    _originalSeries.DropPhotosSeries = Series.DropPhotosSeries;
                }
                else if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
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
    }
}
