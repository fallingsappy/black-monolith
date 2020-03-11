using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Drops")]
    public class DbDrop
    {
        [Key]
        public Guid DropId { get; set; }
        public double XDiameterInMeters { get; set; }
        public double YDiameterInMeters { get; set; }
        public double ZDiameterInMeters { get; set; }
        public double VolumeInCubicalMeters { get; set; }
        public double? RadiusInMeters { get; set; }

        public virtual DbDropPhoto DropPhoto { get; set; }
    }
}
