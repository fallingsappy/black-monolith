﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("ReferencePhotos")]
    public class DbReferencePhoto
    {
        [Key]
        public Guid ReferencePhotoId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public virtual DbSimpleLine SimpleLine { get; set; }
        public int PixelsInMillimeter { get; set; }

        public virtual DbSeries Series { get; set; }
    }
}