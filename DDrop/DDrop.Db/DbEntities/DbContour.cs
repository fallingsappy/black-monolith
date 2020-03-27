using System;
using System.Collections.Generic;

namespace DDrop.Db.DbEntities
{
    public class DbContour
    {
        public Guid ContourId { get; set; }
        public List<DbSimpleLine> Contour { get; set; }
        public Guid CurrentDropPhotoId { get; set; }
        public DbDropPhoto CurrentDropPhoto { get; set; }
    }
}