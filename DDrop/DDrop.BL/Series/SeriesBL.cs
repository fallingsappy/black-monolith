using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using DDrop.DAL;
using DDrop.Utility.Mappers;

namespace DDrop.BL.Series
{
    public class SeriesBL : ISeriesBL
    {
        private readonly IDDropRepository _dDropRepository;

        public SeriesBL(IDDropRepository dDropRepository)
        {
            _dDropRepository = dDropRepository;
        }

        public async Task DeleteSeries(BE.Models.Series series, BE.Models.Series currentSeries, Canvas canvas)
        {
            var userEmail = series.CurrentUser.Email;
            var dbUser = await Task.Run(() => _dDropRepository.GetUserByLogin(userEmail));
            var userSeries = series;
            await Task.Run(() =>
                _dDropRepository.DeleteSingleSeries(
                    DDropDbEntitiesMapper.SingleSeriesToSingleDbSeries(userSeries, dbUser)));

            if (series == currentSeries)
            {
                if (currentSeries.ReferencePhotoForSeries?.Line != null)
                    canvas.Children.Remove(currentSeries.ReferencePhotoForSeries?.Line);

                currentSeries.Title = null;
            }
        }
    }
}