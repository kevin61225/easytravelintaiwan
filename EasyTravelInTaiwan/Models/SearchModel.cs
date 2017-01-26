using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models                          //這個Model是TypeAhead使用的
{
    public class SearchModel : List<SearchTypeAheadEntity>
    {
        public void AddPlaceSearch()
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddPlaceSearch(string query)
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Name.Contains(query)
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddFoodSearch()
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Pt == "07"
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddFoodSearch(string query)
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Pt == "07" && places.Name.Contains(query)
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddViewSearch()
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Pt == "10"
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddViewSearch(string query)
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Pt == "10" && places.Name.Contains(query)
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddHotelSearch()
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Pt == "06"
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        public void AddHotelSearch(string query)
        {
            using (var db = new ProjectEntities1())
            {
                var placeQuery = from places in db.views
                                 where places.Pt == "06" && places.Name.Contains(query)
                                 select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }
    }
}