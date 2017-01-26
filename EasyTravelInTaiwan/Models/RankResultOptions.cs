using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{

    public class RankResultOptions
    {
        public List<SelectListItem> ViewTypes { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public int SelectCity { get; set; }
        public int SelectViewType { get; set; }

        public RankResultOptions(string type)
        {
            ViewTypes = new List<SelectListItem>();
            Cities = new List<SelectListItem>();
            if (type == "view" || type == null) GetView();
            if (type == "food") GetFood();
            if (type == "hotel") GetHotel();
        }

        public void GetFood()
        {
            using (var db = new ProjectEntities1())
            {
                foreach (viewtype type in db.viewtypes.ToList())
                {
                    if (type.Typenumber == 3 || type.Typenumber == 18)
                        ViewTypes.Add(new SelectListItem(type.Typename, type.Typenumber));
                }
                //foreach (city city in db.cities.ToList())
                //{
                //    Cities.Add(new SelectListItem(city.Cityname, city.Citynumber));
                //}
            }
        }

        public void GetHotel()
        {
            using (var db = new ProjectEntities1())
            {
                foreach (viewtype type in db.viewtypes.ToList())
                {
                    if (type.Typenumber == 16 || type.Typenumber == 17 || type.Typenumber == 19)
                        ViewTypes.Add(new SelectListItem(type.Typename, type.Typenumber));
                }
                //foreach (city city in db.cities.ToList())
                //{
                //    Cities.Add(new SelectListItem(city.Cityname, city.Citynumber));
                //}
            }
        }

        public void GetView()
        {
            using (var db = new ProjectEntities1())
            {
                foreach (viewtype type in db.viewtypes.ToList())
                {
                    if (type.Typenumber != 3 && type.Typenumber != 16 && type.Typenumber != 17 && type.Typenumber != 18 && type.Typenumber != 19)
                        ViewTypes.Add(new SelectListItem(type.Typename, type.Typenumber));
                }
                foreach (city city in db.cities.ToList())
                {
                    Cities.Add(new SelectListItem(city.Cityname, city.Citynumber));
                }
            }
        }


    }

    public class SelectListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public SelectListItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public SelectListItem(string text, int value)
        {
            Text = text;
            Value = value.ToString();
        }
    }
}