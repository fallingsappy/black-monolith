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
        Task<ObservableCollection<SeriesViewModel>> ImportLocalSeriesAsync(string fileName, UserViewModel User);
        Task ExportSeriesLocalAsync(string fileName, UserViewModel user);
    }
}
