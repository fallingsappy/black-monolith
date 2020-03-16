using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
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

        public ICollection<DbReferencePhoto> ReferencePhoto { get; set; }
        public ICollection<DbDropPhoto> DropPhotoHorizontalLine { get; set; }
        public ICollection<DbDropPhoto> DropPhotoVerticalLine { get; set; }
    }
}
