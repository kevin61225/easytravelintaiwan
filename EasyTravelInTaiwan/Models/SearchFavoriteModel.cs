using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Models
{
    public class SearchFavoriteModel : List<FavoritePlaces>
    {
        int _userId;
        public int UserId
        {
            get
            {
                return _userId;
            }
        }

        public SearchFavoriteModel(int Uid)
        {
            _userId = Uid;
            this.Initialize();
        }

        public void Initialize()
        {
            using (var db = new ProjectEntities())
            {
                List<city> cities = db.cities.ToList();
                // 先找到 view

                List<view> views = GetAllViewFromFavoriteByUserId(UserId);

                foreach (city item in cities)
                {
                    AddViewType(item, views);
                }
            }
        }

        static public List<view> GetViewsByTravellistplace(int User)
        {
            using (var db = new ProjectEntities())
            {
                List<travellistplace> places = db.travellistplaces.Where(o => o.travellist.UserId == User).Distinct().ToList();
                List<view> views = new List<view>();

                // 根據 travellistplace 找到 view
                foreach (travellistplace item in places)
                {
                    view temp = db.views.Where(o => o.Id == item.Sno).Single();
                    views.Add(temp);
                }
                return views;
            }
        }

        public void AddViewType(city item, List<view> viewList)
        {
            using (var db = new ProjectEntities())
            {
                FavoritePlaces favorite = new FavoritePlaces();
                favorite.ViewTypeSum = new List<int>();
                favorite.CityId = item.Citynumber;
                favorite.CityName = item.Cityname;
                List<viewtype> types = db.viewtypes.ToList();
                favorite.Viewtypes = types;

                foreach (viewtype type in favorite.Viewtypes)
                {
                    favorite.ViewTypeSum.Add(this.GetViewTypeSumByUserId(type, item.Citynumber, viewList));
                }

                favorite.CitySum = favorite.ViewTypeSum.Sum();
                Add(favorite);
            }
        }

        public int GetViewTypeSumByUserId(viewtype input, string city, List<view> viewList)
        {
            int sum = 0;
            using (var db = new ProjectEntities())
            {
                // 再根據 view 找到 citys & viewtype
                foreach (view item in viewList)
                {
                    if (item.Viewtype == input.Typenumber && item.City == city) sum++;
                }
            }
            return sum;
        }

        public int GetCitySumByUserid(List<int> input)
        {
            int output = input.Sum();
            return output;
        }

        static public int CheckIsFavorite(int userId, string place)
        {
            using (var db = new ProjectEntities())
            {
                try
                {
                    db.favorites.Where(o => o.UserId == userId).Where(o => o.PlaceId == place).Single();
                    return 1;
                }
                catch
                {
                    return 2;
                }
            }
        }

        static public void AddFavorite(int uId, string placeId)
        {
            using (var db = new ProjectEntities())
            {
                favorite temp = new favorite();
                temp.UserId = uId;
                temp.PlaceId = placeId;
                db.favorites.Add(temp);
                db.SaveChanges();
            }
        }

        static public List<view> GetAllViewFromFavoriteByUserId(int uId)
        {
            using (var db = new ProjectEntities())
            {
                List<favorite> favorites = db.favorites.Where(o => o.UserId == uId).ToList<favorite>();

                List<view> allViews = db.views.ToList();
                List<view> views = new List<view>();
                foreach (favorite item in favorites)
                {
                    int index = allViews.FindIndex(o => o.Id == item.PlaceId);
                    views.Add(allViews[index]);
                }
                return views;
            }
        }
    }
}