using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.DAL.DbEntities
{
    [Table("SimpleLines")]
    public class DbSimpleLine
    {
        [Key]
        public Guid SimpleLineId { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public virtual DbReferencePhoto ReferencePhoto { get; set; }
        public virtual DbDropPhoto DropPhotoHorizontalLine { get; set; }
        public virtual DbDropPhoto DropPhotoVerticalLine { get; set; }
    }
}
