using System;

namespace DDrop.BE.Models.Serializable
{
    public class SerializableDropPhoto
    {
        public Guid DropPhotoId { get; set; }
        public string Name { get; set; }
        public int XDiameterInPixels { get; set; }
        public int YDiameterInPixels { get; set; }
        public int ZDiameterInPixels { get; set; }
        public Guid? SimpleHorizontalLineId { get; set; }
        public virtual SimpleLine SimpleHorizontalLine { get; set; }
        public Guid? SimpleVerticalLineId { get; set; }
        public virtual SimpleLine SimpleVerticalLine { get; set; }
        public byte[] Content { get; set; }
        public virtual SerializableDrop Drop { get; set; }
        public string AddedDate { get; set; }
        public string CreationDateTime { get; set; }
        public Guid CurrentSeriesId { get; set; }
        public Series CurrentSeries { get; set; }
    }
}