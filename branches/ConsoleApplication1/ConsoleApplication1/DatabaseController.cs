using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class DatabaseController
    {
        private projectEntities db = new projectEntities();

        public projectEntities Database // ok
        {
            get
            {
                return db;
            }
        }

        public bool InsertViewType(viewtype insertViewType) //ok
        {
            List<viewtype> typeList = db.viewtypes.Where(o => o.Typename == insertViewType.Typename).ToList();
            if (typeList.Count == 0)
            {
                db.viewtypes.Add(insertViewType);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void InsertCity(city insertCity) // ok
        {
            db.cities.Add(insertCity);
            db.SaveChanges();
        }

        public int GetViewTypeNumber(string typeName) //ok
        {
            return db.viewtypes.Where(o => o.Typename == typeName).First().Typenumber;
        }

        public void InsertPlace(place insertPlace)
        {
            db.places.Add(insertPlace);
            db.SaveChanges();
        }

        public void InsertPlaceImage(placeimage insertImage) //
        {
            db.placeimages.Add(insertImage);
            db.SaveChanges();
        }

        public void InsertHotel(hotel inserthotel)
        {
            db.hotels.Add(inserthotel);
            db.SaveChanges();
        }

        public void InsertHotelImage(hotelimage inserthotelimage)
        {
            db.hotelimages.Add(inserthotelimage);
            db.SaveChanges();
        }

        public void EditHotel(hotel hotel)
        {
            db.Entry(hotel).State = System.Data.EntityState.Modified;
            
            db.SaveChanges();
        }
    }
}
