using System.ComponentModel;

namespace DDrop.BE.Models
{
    public class SeriesToExcel
    {
        [DisplayName("Время, с")] public double Time { get; set; }

        [DisplayName("Имя файла")] public string Name { get; set; }

        [DisplayName("Горизонтальный диаметр, px")]
        public int XDiameterInPixels { get; set; }

        [DisplayName("Вертикальный диаметр, px")]
        public int YDiameterInPixels { get; set; }

        [DisplayName("Вертикальный диаметр, px")]
        public double XDiameterInMeters { get; set; }

        [DisplayName("Горизонтальный диаметр, м")]
        public double YDiameterInMeters { get; set; }

        [DisplayName("Радиус, м")] public double RadiusInMeters { get; set; }

        [DisplayName("Объем, кубические метры")]
        public double VolumeInCubicalMeters { get; set; }
    }
}