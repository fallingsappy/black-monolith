using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Information : Window
    {
        public Information()
        {
            InitializeComponent();

            TextRange textRange;

            textRange = new TextRange(AboutRTB.Document.ContentStart, AboutRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.About)), DataFormats.Rtf);

            textRange = new TextRange(DevelopmentRTB.Document.ContentStart, DevelopmentRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Developers)), DataFormats.Rtf);

            textRange = new TextRange(TestingRTB.Document.ContentStart, TestingRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Testers)), DataFormats.Rtf);

            textRange = new TextRange(MenuRTB.Document.ContentStart, MenuRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Menu)), DataFormats.Rtf);

            textRange = new TextRange(CalculationRTB.Document.ContentStart, CalculationRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Calculation)), DataFormats.Rtf);

            textRange = new TextRange(AccountRTB.Document.ContentStart, AccountRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Account)), DataFormats.Rtf);

            textRange = new TextRange(SeriesManagerRTB.Document.ContentStart, SeriesManagerRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.SeriesManager)), DataFormats.Rtf);

            textRange = new TextRange(CommonSeriesPlotRTB.Document.ContentStart, CommonSeriesPlotRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonSeriesPlot)), DataFormats.Rtf);

            textRange = new TextRange(CalculationModelRTB.Document.ContentStart, CalculationModelRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CalculationModel)), DataFormats.Rtf);

            textRange = new TextRange(CommonSeriesInformationRTB.Document.ContentStart, CommonSeriesInformationRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonSeriesInformation)), DataFormats.Rtf);

            textRange = new TextRange(PhotosRTB.Document.ContentStart, PhotosRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Photos)), DataFormats.Rtf);

            textRange = new TextRange(ReferenceRTB.Document.ContentStart, ReferenceRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Reference)), DataFormats.Rtf);

            textRange = new TextRange(SeriesPlotRTB.Document.ContentStart, SeriesPlotRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Plot)), DataFormats.Rtf);
        }
    }
}
