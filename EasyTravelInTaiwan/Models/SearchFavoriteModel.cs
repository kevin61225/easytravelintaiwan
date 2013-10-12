using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Models
{
    public class SearchFavoriteModel : List<FavoritePlaces>
    {
        public SearchFavoriteModel()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            using (var db = new ProjectEntities())
            {
                List<city> cities = db.cities.ToList();
                foreach (city item in cities)
                {
                    AddViewType(item);
                }
            }
        }

        public void AddViewType(city item)
        {
            using (var db = new ProjectEntities())
            {
                FavoritePlaces favorite = new FavoritePlaces();
                favorite.CityId = item.Citynumber;
                favorite.CityName = item.Cityname;
                List<viewtype> types = db.viewtypes.ToList();
                favorite.viewtypes = types;
                Add(favorite);       
            }
        }

        public void GetAllInfo(int UserId)
        {
            using (var db = new ProjectEntities())
            {

            }
        }

    }
}