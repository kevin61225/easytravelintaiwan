using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class ViewImage
    {
        public virtual int sid { get; set; }
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Imagetype { get; set; }
        public virtual byte[] Image { get; set; }

        static public ViewImage ConverToViewImage(hotelimage image)
        {
            ViewImage outputImage = new ViewImage();
            outputImage.Id = image.Id;
            outputImage.sid = image.sid;
            outputImage.Name = image.Name;
            outputImage.Imagetype = 0;
            outputImage.Image = image.Image;
            return outputImage;
        }

        static public ViewImage ConverToViewImage(placeimage image)
        {
            ViewImage outputImage = new ViewImage();
            outputImage.Id = image.Id;
            outputImage.sid = image.sid;
            outputImage.Name = image.Name;
            outputImage.Imagetype = image.Imagetype;
            outputImage.Image = image.Image;
            return outputImage;
        }

        static public ICollection<ViewImage> TransformToViewCollection(ICollection<hotelimage> hotelImages)
        {
            ICollection<ViewImage> images = new HashSet<ViewImage>();
            foreach (hotelimage image in hotelImages)
            {
                images.Add(ConverToViewImage(image));
            }
            return images;
        }

        static public ICollection<ViewImage> TransformToViewCollection(ICollection<placeimage> placeImages)
        {
            ICollection<ViewImage> images = new HashSet<ViewImage>();
            foreach (placeimage image in placeImages)
            {
                images.Add(ConverToViewImage(image));
            }
            return images;
        }
    }
}

