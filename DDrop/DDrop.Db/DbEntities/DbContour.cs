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
        //public string CSharpParameters { get; set; }
        //public string PythonParameters { get; set; }

        public virtual DbDropPhoto CurrentDropPhoto { get; set; }
    }
}