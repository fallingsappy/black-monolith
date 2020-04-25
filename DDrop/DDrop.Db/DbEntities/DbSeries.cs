using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Series")]
    public class DbSeries
    {
        [Key] public Guid SeriesId { get; set; }

        public string Title { get; set; }
        public List<DbDropPhoto> DropPhotosSeries { get; set; }
        public virtual DbReferencePhoto ReferencePhotoForSeries { get; set; }
        public double IntervalBetweenPhotos { get; set; }
        public string AddedDate { get; set; }
        public bool UseCreationDateTime { get; set; }
        public Guid CurrentUserId { get; set; }
        public DbUser CurrentUser { get; set; }
    }
}