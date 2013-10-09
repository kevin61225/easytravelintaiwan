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

        public RankResultOptions()
        {
            ViewTypes = new List<SelectListItem>();
            Cities = new List<SelectListItem>();
            using (var db = new ProjectEntities())
            {
                foreach (viewtype type in db.viewtypes.ToList())
                {
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