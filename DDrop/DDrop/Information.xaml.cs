using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace DDrop
{
    /// <summary>
    ///     Interaction logic for Help.xaml
    /// </summary>
    public partial class Information : Window
    {
        public Information()
        {
            InitializeComponent();

            TextRange textRange;

            #region About

            textRange = new TextRange(AboutRTB.Document.ContentStart, AboutRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.About)), DataFormats.Rtf);

            #endregion

            #region Help

            #region Interface

            textRange = new TextRange(MenuRTB.Document.ContentStart, MenuRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Menu)), DataFormats.Rtf);

            textRange = new TextRange(AccountRTB.Document.ContentStart, AccountRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Account)), DataFormats.Rtf);

            textRange = new TextRange(SeriesManagerRTB.Document.ContentStart, SeriesManagerRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.SeriesManager)), DataFormats.Rtf);

            textRange = new TextRange(CommonSeriesPlotRTB.Document.ContentStart, CommonSeriesPlotRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonSeriesPlot)), DataFormats.Rtf);

            #region Series

            textRange = new TextRange(CommonSeriesInformationRTB.Document.ContentStart, CommonSeriesInformationRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonSeriesInformation)), DataFormats.Rtf);

            textRange = new TextRange(AutoCalculationInformationRTB.Document.ContentStart, AutoCalculationInformationRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.AutoCalculation)), DataFormats.Rtf);

            textRange = new TextRange(ManualEditInformationRTB.Document.ContentStart, ManualEditInformationRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ManualEdit)), DataFormats.Rtf);

            textRange = new TextRange(PhotoReOrderInformationRTB.Document.ContentStart, PhotoReOrderInformationRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ReOrdering)), DataFormats.Rtf);

            #endregion

            #endregion

            #region Operations

            textRange = new TextRange(CommonSeriesPlotOpRTB.Document.ContentStart, CommonSeriesPlotOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonSeriesPlotOp)), DataFormats.Rtf);

            textRange = new TextRange(CommonOperationsRTB.Document.ContentStart, CommonOperationsRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonOperations)), DataFormats.Rtf);

            #region Series Manager

            textRange = new TextRange(AddSeriesOpRTB.Document.ContentStart, AddSeriesOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.AddSeries)), DataFormats.Rtf);

            textRange = new TextRange(ImportExportOpRTB.Document.ContentStart, ImportExportOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ImportExport)), DataFormats.Rtf);

            textRange = new TextRange(AutoExcelReportOpRTB.Document.ContentStart, AutoExcelReportOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.AutoExcelReport)), DataFormats.Rtf);

            #endregion

            #region Menu

            textRange = new TextRange(CommonOptionsOpRTB.Document.ContentStart, CommonOptionsOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CommonOptionsOp)), DataFormats.Rtf);

            textRange = new TextRange(LocalStoredUsersOpRTB.Document.ContentStart, LocalStoredUsersOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.LocalStoredUsersOp)), DataFormats.Rtf);

            textRange = new TextRange(AutoCalculationTemplatesOpRTB.Document.ContentStart, AutoCalculationTemplatesOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.AutoCalculationTemplatesOp)), DataFormats.Rtf);

            #endregion

            #region Series

            textRange = new TextRange(SeriesPlotOpRTB.Document.ContentStart, SeriesPlotOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Plot)), DataFormats.Rtf);

            #endregion

            #region Reference

            textRange = new TextRange(ReferenceAddOpRTB.Document.ContentStart, ReferenceAddOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ReferenceAdd)), DataFormats.Rtf);

            textRange = new TextRange(ReferenceEditOpRTB.Document.ContentStart, ReferenceEditOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ReferenceEdit)), DataFormats.Rtf);

            #endregion

            #region Photos

            textRange = new TextRange(AddPhotosOpRTB.Document.ContentStart, AddPhotosOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.PhotosAdd)), DataFormats.Rtf);

            textRange = new TextRange(EditPhotosOpRTB.Document.ContentStart, EditPhotosOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.PhotosEdit)), DataFormats.Rtf);

            textRange = new TextRange(ReCalculatePhotosOpRTB.Document.ContentStart, ReCalculatePhotosOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.PhotosReCalculate)), DataFormats.Rtf);

            textRange = new TextRange(ReOrderPhotosOpRTB.Document.ContentStart, ReOrderPhotosOpRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.PhotosReOrder)), DataFormats.Rtf);

            #endregion

            #endregion

            #region Calculation Model

            textRange = new TextRange(CalculationModelRTB.Document.ContentStart, CalculationModelRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CalculationModel)), DataFormats.Rtf);

            #endregion

            #endregion

            #region Development

            textRange = new TextRange(DevelopmentRTB.Document.ContentStart, DevelopmentRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Developers)), DataFormats.Rtf);

            textRange = new TextRange(TestingRTB.Document.ContentStart, TestingRTB.Document.ContentEnd);
            textRange.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.Testers)), DataFormats.Rtf);

            #endregion
        }
    }
}