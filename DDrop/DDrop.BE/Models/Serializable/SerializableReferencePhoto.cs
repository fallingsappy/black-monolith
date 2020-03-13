using System;

namespace DDrop.BE.Models.Serializable
{
    public class SerializableReferencePhoto
    {
        public Guid ReferencePhotoId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public virtual SimpleLine SimpleLine { get; set; }
        public int PixelsInMillimeter { get; set; }

        public Series Series { get; set; }
    }
}