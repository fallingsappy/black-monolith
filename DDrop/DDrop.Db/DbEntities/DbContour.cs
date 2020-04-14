using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DDrop.Db.DbEntities
{
    public class DbContour
    {
        [Key]
        public Guid ContourId { get; set; }
        public List<DbSimpleLine> SimpleLines { get; set; }
        public string CalculationParameters { get; set; }
        public string CalculationProvider { get; set; }

        public virtual DbDropPhoto CurrentDropPhoto { get; set; }
    }
}