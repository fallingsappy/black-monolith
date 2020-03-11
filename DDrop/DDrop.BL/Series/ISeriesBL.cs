using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDrop.BL.Series
{
    public interface ISeriesBL
    {
        Task<ObservableCollection<BE.Models.Series>> ImportLocalSeriesAsync(string fileName, User User);

        ObservableCollection<BE.Models.Series> ConvertSeriesToSeriesViewModel(List<BE.Models.Entities.Series> series,
            User user);
        Task ExportSeriesLocalAsync(string fileName, User user);
        List<BE.Models.Entities.Series> SeriesViewModelToSeries(User user);
        BE.Models.Entities.Series SingleSeriesViewModelToSingleSeries(BE.Models.Series userSeries);
    }
}
