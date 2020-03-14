using System;
using System.Collections.Generic;

namespace DDrop.BE.Models.Serializable
{
    public class SerializableSeries
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; }
        public virtual List<SerializableDropPhoto> DropPhotosSeries { get; set; }
        public virtual SerializableReferencePhoto ReferencePhotoForSeries { get; set; }
        public double IntervalBetweenPhotos { get; set; }
        public string AddedDate { get; set; }

        public Guid CurrentUserId { get; set; }
        public User CurrentUser { get; set; }
    }
}