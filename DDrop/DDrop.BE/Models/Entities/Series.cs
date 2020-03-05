using System;
using System.Collections.Generic;

namespace DDrop.BE.Models.Entities
{
    public class Series
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; }
        public List<DropPhoto> DropPhotosSeries { get; set; }
        public ReferencePhoto ReferencePhotoForSeries { get; set; }
        public double IntervalBetweenPhotos { get; set; }
    }
}
