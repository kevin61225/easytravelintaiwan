using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class MapModel
    {

    }

    public class MapRepository : List<maplatlng>
    {
        public MapRepository()
        {
            GetAll();
        }

        public void GetAll()
        {
            using (var db = new projectEntities())
            {
                for (int i = 0; i < db.maplatlngs.Count(); i++)
                {
                    Add(db.maplatlngs.ToList()[i]);
                }
            }
        }
    }

    public class PlaceInfo
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}