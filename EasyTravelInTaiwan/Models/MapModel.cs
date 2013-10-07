using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class MapModel
    {

    }

    public class MapRepository : List<MapMarkers>
    {
        public MapRepository()
        {
            GetAll();
        }

        public void GetAll()
        {
            using (var db = new ProjectEntities())
            {
                var query = from views in db.views
                            select views;
                foreach (var item in query)
                {
                    Add(new MapMarkers(item));
                }
            }
        }
    }

    public class MapMarkers
    {
        public MapMarkers(view input)
        {
            Id = input.Id;
            Name = input.Name;
            Lat = input.Lat;
            Lng = input.Lng;
            Address = input.Address;
            City = input.City;
            Viewtype = input.Viewtype;
            Pt = input.Pt;
            IconType = input.IconType;
            Description = input.Description;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int Viewtype { get; set; }
        public string Pt { get; set; }
        public string IconType { get; set; }
        public string Description { get; set; }
    }

    public class PlaceInfo
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}