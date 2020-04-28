﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using DDrop.BE.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace DDrop.Utility.ExcelOperations
{
    public class ExcelOperations
    {
        public static void CreateSingleSeriesExcelFile(User User, string fileName)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var mainWorksheet = excelPackage.Workbook.Worksheets.Add("Общая информация");

                mainWorksheet.Cells["A1:C1"].Merge = true;
                mainWorksheet.Cells["A2:C2"].Merge = true;
                mainWorksheet.Cells["A3:C3"].Merge = true;

                mainWorksheet.Cells["D1:H1"].Merge = true;
                mainWorksheet.Cells["D2:H2"].Merge = true;
                mainWorksheet.Cells["D3:H3"].Merge = true;

                mainWorksheet.Cells["A1"].Value = "Имя:";
                mainWorksheet.Cells["A2"].Value = "Фамилия:";
                mainWorksheet.Cells["A3"].Value = "Email:";

                mainWorksheet.Cells["D1"].Value = User.FirstName;
                mainWorksheet.Cells["D2"].Value = User.LastName;
                mainWorksheet.Cells["D3"].Value = User.Email;

                var seriesCombinedChart =
                    mainWorksheet.Drawings.AddChart("seriesCombinedChart", eChartType.XYScatterLines) as
                        ExcelScatterChart;

                seriesCombinedChart.Title.Text = "Зависимость радиуса капли от времени испарения";
                seriesCombinedChart.Legend.Position = eLegendPosition.Right;
                seriesCombinedChart.XAxis.Title.Text = "Время, с";
                seriesCombinedChart.YAxis.Title.Text = "Радиус, м";

                var indexer = 0;

                var exportAll = User.UserSeries.Where(x => x.IsChecked).ToList().Count == 0;

                foreach (var currentSeries in User.UserSeries)
                    if (currentSeries.IsChecked || exportAll)
                    {
                        var worksheet = excelPackage.Workbook.Worksheets.Add($"{currentSeries.Title}");

                        worksheet.Cells["A1:C1"].Merge = true;
                        worksheet.Cells["A2:C2"].Merge = true;
                        worksheet.Cells["A3:C3"].Merge = true;
                        worksheet.Cells["A4:C4"].Merge = true;

                        worksheet.Cells["D1:H1"].Merge = true;
                        worksheet.Cells["D2:H2"].Merge = true;
                        worksheet.Cells["D3:H3"].Merge = true;
                        worksheet.Cells["D4:H4"].Merge = true;

                        worksheet.Cells["A1"].Value = "Название серии:";
                        worksheet.Cells["A2"].Value = "Очет от:";
                        worksheet.Cells["A3"].Value = "Интервал между снимками, c:";
                        worksheet.Cells["A4"].Value = "Пикселей в миллиметре, px:";

                        worksheet.Cells["D1"].Value = currentSeries.Title;
                        worksheet.Cells["D2"].Value = DateTime.Now.ToString();
                        worksheet.Cells["D3"].Value = currentSeries.IntervalBetweenPhotos;
                        worksheet.Cells["D4"].Value = currentSeries.ReferencePhotoForSeries?.PixelsInMillimeter ?? 0;

                        var singleSeriesToExcelOutput = new ObservableCollection<SeriesToExcel>();

                        if (currentSeries.UseCreationDateTime)
                        {
                            var orderedDropPhotos = currentSeries.DropPhotosSeries
                                .OrderBy(x => DateTime.Parse(x.CreationDateTime, CultureInfo.InvariantCulture)).ToList();

                            for (var i = 0; i < currentSeries.DropPhotosSeries.Count; i++)
                            {
                                var dropPhoto = currentSeries.DropPhotosSeries[i];
                                
                                singleSeriesToExcelOutput.Add(new SeriesToExcel
                                {
                                    Time = (DateTime.Parse(orderedDropPhotos[i].CreationDateTime,
                                                CultureInfo.InvariantCulture) -
                                            DateTime.Parse(orderedDropPhotos[0].CreationDateTime,
                                                CultureInfo.InvariantCulture)).TotalSeconds,
                                    Name = dropPhoto.Name,
                                    RadiusInMeters = dropPhoto.Drop.RadiusInMeters.Value,
                                    VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                                    XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                                    YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters
                                });
                            }
                        }
                        else
                        {
                            for (var i = 0; i < currentSeries.DropPhotosSeries.Count; i++)
                            {
                                var dropPhoto = currentSeries.DropPhotosSeries[i];
                                singleSeriesToExcelOutput.Add(new SeriesToExcel
                                {
                                    Time = i * currentSeries.IntervalBetweenPhotos,
                                    Name = dropPhoto.Name,
                                    RadiusInMeters = dropPhoto.Drop.RadiusInMeters.Value,
                                    VolumeInCubicalMeters = dropPhoto.Drop.VolumeInCubicalMeters,
                                    XDiameterInPixels = dropPhoto.XDiameterInPixels,
                                    YDiameterInPixels = dropPhoto.YDiameterInPixels,
                                    XDiameterInMeters = dropPhoto.Drop.XDiameterInMeters,
                                    YDiameterInMeters = dropPhoto.Drop.YDiameterInMeters
                                });
                            }
                        }

                        worksheet.Cells["A6"].LoadFromCollection(singleSeriesToExcelOutput, true);

                        var end = worksheet.Dimension.End.Row;

                        var seriesChart =
                            worksheet.Drawings.AddChart("seriesChart", eChartType.XYScatterLines) as ExcelScatterChart;

                        seriesChart.Title.Text =
                            $"Зависимость радиуса капли от времени испарения для серии {currentSeries.Title}";
                        seriesChart.Legend.Position = eLegendPosition.Right;

                        seriesChart.Series.Add(worksheet.Cells[$"G7:G{end}"], worksheet.Cells[$"A7:A{end}"]);
                        seriesCombinedChart.Series.Add(worksheet.Cells[$"G7:G{end}"], worksheet.Cells[$"A7:A{end}"]);

                        seriesChart.XAxis.Title.Text = "Время, с";
                        seriesChart.YAxis.Title.Text = "Радиус, м";

                        seriesChart.Series[0].Header = worksheet.Cells["D1"].Value.ToString();
                        seriesCombinedChart.Series[indexer].Header = worksheet.Cells["D1"].Value.ToString();

                        seriesChart.SetSize(510, 660);
                        seriesChart.SetPosition(end + 1, 0, 0, 0);

                        worksheet.Cells.AutoFitColumns();
                        indexer++;
                    }

                seriesCombinedChart.SetSize(510, 660);
                seriesCombinedChart.SetPosition(mainWorksheet.Dimension.End.Row + 1, 0, 0, 0);
                mainWorksheet.Cells.AutoFitColumns();

                var excelFile = new FileInfo($@"{fileName}");
                excelPackage.SaveAs(excelFile);
            }
        }
    }
}