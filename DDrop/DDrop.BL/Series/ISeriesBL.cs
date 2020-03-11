using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDrop.BE.Models.Serializable;

namespace DDrop.BL.Series
{
    public interface ISeriesBL
    {
        Task<ObservableCollection<BE.Models.Series>> ImportLocalSeriesAsync(string fileName, User User);

        ObservableCollection<BE.Models.Series> ConvertSeriesToSeriesViewModel(List<SerializableSeries> series,
            User user);
        Task ExportSeriesLocalAsync(string fileName, User user);
        List<SerializableSeries> SeriesViewModelToSeries(User user);
        SerializableSeries SingleSeriesViewModelToSingleSeries(BE.Models.Series userSeries);
    }
}
