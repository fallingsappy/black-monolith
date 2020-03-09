using System;

namespace DDrop.BE.Models.Entities
{
    public class ReferencePhoto
    {
        public Guid ReferencePhotoId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public virtual SimpleLine SimpleLine { get; set; }
        public int PixelsInMillimeter { get; set; }
    }
}
