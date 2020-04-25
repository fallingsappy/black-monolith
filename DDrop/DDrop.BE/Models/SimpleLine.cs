using System;

namespace DDrop.BE.Models
{
    public class SimpleLine
    {
        public Guid SimpleLineId { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public Guid ContourId { get; set; }
    }
}