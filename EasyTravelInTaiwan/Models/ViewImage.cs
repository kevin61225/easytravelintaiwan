using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class ViewImage
    {
        public int sid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Imagetype { get; set; }
        public byte[] Image { get; set; }
        public string pt { get; set; }

        static public ViewImage ConverToViewImage(hotelimage image, string pt)
        {
            ViewImage outputImage = new ViewImage();
            outputImage.Id = image.Id;
            outputImage.sid = image.sid;
            outputImage.Name = image.Name;
            outputImage.Imagetype = 0;
            outputImage.Image = image.Image;
            outputImage.pt = pt;
            return outputImage;
        }

        static public ViewImage ConverToViewImage(placeimage image, string pt)
        {
            ViewImage outputImage = new ViewImage();
            outputImage.Id = image.Id;
            outputImage.sid = image.sid;
            outputImage.Name = image.Name;
            outputImage.Imagetype = image.Imagetype;
            outputImage.Image = image.Image;
            outputImage.pt = pt;
            return outputImage;
        }

        static public byte[] GetImageById(ProjectEntities db, string id, string pt)
        {
            ViewImage outputImage = new ViewImage();
            switch (pt)
            {
                case "06":
                    //hotel
                    try
                    {
                        outputImage = ConverToViewImage(db.hotelimages.Where(o => o.Id == id).First(), pt);
                    }
                    catch
                    {
                        outputImage.Image = null;
                    }
                    break;
                case "07":
                    //food
                    try
                    {
                        outputImage = ConverToViewImage(db.placeimages.Where(o => o.Id == id).First(), pt);
                    }
                    catch
                    {
                        outputImage.Image = null;
                    }
                    break;
                case "10":
                    //view 
                    try
                    {
                        outputImage = ConverToViewImage(db.placeimages.Where(o => o.Id == id).First(), pt);
                    }
                    catch
                    {
                        outputImage.Image = null;
                    }
                    break;
            }
            return outputImage.Image;
        }

        static public byte[] GetImageById(ProjectEntities db, string id, string pt, int sid)
        {
            ViewImage outputImage = new ViewImage();
            switch (pt)
            {
                case "06":
                    //hotel
                    try
                    {
                        outputImage = ConverToViewImage(db.hotelimages.Where(o => o.Id == id).Where( o=> o.sid == sid).Single(), pt);
                    }
                    catch
                    {
                        outputImage.Image = null;
                    }
                    break;
                case "07":
                    //food
                    try
                    {
                        outputImage = ConverToViewImage(db.placeimages.Where(o => o.Id == id).Where(o => o.sid == sid).Single(), pt);
                    }
                    catch
                    {
                        outputImage.Image = null;
                    }
                    break;
                case "10":
                    //view 
                    try
                    {
                        outputImage = ConverToViewImage(db.placeimages.Where(o => o.Id == id).Where(o => o.sid == sid).Single(), pt);
                    }
                    catch
                    {
                        outputImage.Image = null;
                    }
                    break;
            }
            return outputImage.Image;
        }

        static public ICollection<ViewImage> TransformToViewCollection(ICollection<hotelimage> hotelImages, string pt)
        {
            ICollection<ViewImage> images = new HashSet<ViewImage>();
            foreach (hotelimage image in hotelImages)
            {
                images.Add(ConverToViewImage(image, pt));
            }
            return images;
        }

        static public ICollection<ViewImage> TransformToViewCollection(ICollection<placeimage> placeImages, string pt)
        {
            ICollection<ViewImage> images = new HashSet<ViewImage>();
            foreach (placeimage image in placeImages)
            {
                images.Add(ConverToViewImage(image, pt));
            }
            return images;
        }
    }
}

