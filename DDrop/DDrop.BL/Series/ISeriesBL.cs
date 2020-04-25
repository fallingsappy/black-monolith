using System.Threading.Tasks;
using System.Windows.Controls;

namespace DDrop.BL.Series
{
    public interface ISeriesBL
    {
        Task DeleteSeries(BE.Models.Series series, BE.Models.Series currentSeries, Canvas canvas);
    }
}