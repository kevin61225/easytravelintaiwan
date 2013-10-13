using System.Linq;
using System.Collections.Generic;

namespace EasyTravelInTaiwan.Models                      //搜尋結果的Model
{
    public class SearchResultModel : List<ResultView>
    {
        public void TopRatingFoodByAmount(int resultCount)
        {
            using (var db = new ProjectEntities())
            {
                var query = (from ratingItems in db.ratings
                             where ratingItems.pt == "07"
                             group ratingItems by ratingItems.Sno into ratingItemsGrp
                             orderby ratingItemsGrp.Sum(o => o.Point) descending
                             select ratingItemsGrp.Key).Take(resultCount).ToList();
                foreach (var item in query)
                {
                    try
                    {
                        Add(GetView(db.views.Where(o => o.Id == item).Single()));
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void TopRatingHotelByAmount(int resultCount)
        {
            using (var db = new ProjectEntities())
            {
                var query = (from ratingItems in db.ratings
                             where ratingItems.pt == "06"
                             group ratingItems by ratingItems.Sno into ratingItemsGrp
                             orderby ratingItemsGrp.Sum(o => o.Point) descending
                             select ratingItemsGrp.Key).Take(resultCount).ToList();
                foreach (var item in query)
                {
                    try
                    {
                        Add(GetView(db.views.Where(o => o.Id == item).Single()));
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void TopRatingViewByAmount(int resultCount)
        {
            using (var db = new ProjectEntities())
            {
                var query = (from ratingItems in db.ratings
                             where ratingItems.pt == "10"
                             group ratingItems by ratingItems.Sno into ratingItemsGrp
                             orderby ratingItemsGrp.Sum(o => o.Point) descending
                             select ratingItemsGrp.Key).Take(resultCount).ToList();
                foreach (var item in query)
                {
                    try
                    {
                        Add(GetView(db.views.Where(o => o.Id == item).Single()));
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void TopRatingByType(string type)
        {
            int temp = int.Parse(type);
            using (var db = new ProjectEntities())
            {
                int counts = 0;
                List<view> viewlist = db.views.Where(o => o.viewtype1.Typenumber == temp).ToList();
                var query = (from ratingItems in db.ratings
                             group ratingItems by ratingItems.Sno into ratingItemsGrp
                             orderby ratingItemsGrp.Sum(o => o.Point) descending
                             select ratingItemsGrp.Key).ToList();
                foreach (var item in query)
                {
                    for (int i = 0; i < viewlist.Count(); i++)
                    {
                        try
                        {
                            if (viewlist[i].Id == item)
                            {
                                Add(GetView(viewlist[i]));
                                i++;
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (counts == 10) break;
                }
            }
        }

        static public List<view> GetViewsByRating(int User)
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

        //public void TopSellByAmount(int resultCount)
        //{
        //    using (var db = new ProjectEntities())
        //    {
        //        var query = (from orderItems in db.order_item
        //                     group orderItems by orderItems.ProductID into orderItemsGrp
        //                     orderby orderItemsGrp.Sum(q => q.Quantity) descending
        //                     select orderItemsGrp.Key).Take(resultCount).ToList();
        //        foreach (var item in query)
        //        {
        //            Add(new book(db.products.Find(item).book));
        //        }
        //    }
        //}
        //public void TopSellByAmount(int bookID, int categoryID, int resultCount)
        //{
        //    using (var db = new ProjectEntities())
        //    {
        //        var query = (from books in db.books
        //                     where books.Category == categoryID && books.BookID != bookID
        //                     from orderItems in db.order_item
        //                     where orderItems.ProductID == books.products.FirstOrDefault().ProductID
        //                     group orderItems by orderItems.ProductID into orderItemsGrp
        //                     orderby orderItemsGrp.Sum(q => q.Quantity) descending
        //                     select orderItemsGrp.Key).Take(resultCount).ToList();
        //        foreach (var item in query)
        //        {
        //            Add(new book(db.products.Find(item).book));
        //        }
        //    }
        //}

        public void GetAllPlaces()
        {
            using (var db = new ProjectEntities())
            {
                var query = from views in db.views
                            select views;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        public void GetAllHotels()
        {
            using (var db = new ProjectEntities())
            {
                var query = from hotels in db.views
                            where hotels.Pt == "06"
                            select hotels;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        public void GetAllFoods()
        {
            using (var db = new ProjectEntities())
            {
                var query = from foods in db.views
                            where foods.Pt == "07"
                            select foods;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        public void GetAllViews()
        {
            using (var db = new ProjectEntities())
            {
                var query = from views in db.views
                            where views.Pt == "10"
                            select views;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        public void GetPersonalFavorite(int type, string city, int uId)
        {
            using (var db = new ProjectEntities())
            {
                List<view> views = SearchFavoriteModel.GetViewsByTravellistplace(uId);

                foreach (view item in views)
                {
                    if (item.Viewtype == type && item.City == city)
                    {
                        Add(GetView(item));
                    }
                }
            }
        }

        public ResultView GetView(view input)
        {
            ResultView temp = new ResultView();
            temp.Id = input.Id;
            temp.Name = input.Name;
            temp.Lat = input.Lat;
            temp.Lng = input.Lng;
            temp.Address = input.Address;
            temp.City = input.City;
            temp.Viewtype = input.Viewtype;
            temp.Pt = input.Pt;
            temp.IconType = input.IconType;
            temp.Description = ShortDescription(input.Description);
            temp.cityName = GetCityNameById(temp.City);
            temp.viewTypeName = GetViewTypeNameById(temp.Viewtype);
            return temp;
        }

        static public string GetCityNameById(string cityId)
        {
            string output = string.Empty;
            using (var db = new ProjectEntities())
            {
                return output = db.cities.Where(o => o.Citynumber == cityId).Single().Cityname;
            }
        }

        static public string GetViewTypeNameById(int typeId)
        {
            string output = string.Empty;
            using (var db = new ProjectEntities())
            {
                return output = db.viewtypes.Where(o => o.Typenumber == typeId).Single().Typename;
            }
        }

        public string ShortDescription(string input)
        {
            if (input == null) return string.Empty;
            return (input.Length > 100) ? input.Substring(0, 100) + "  ..." : input;
        }

        public void ByTitle(string keyWord)
        {
            using (var db = new ProjectEntities())
            {
                var query = from places in db.views
                            where places.Name.Contains(keyWord)
                            select places;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }


        public void ByFood(string keyWord)
        {
            using (var db = new ProjectEntities())
            {
                var query = from foods in db.views
                            where foods.Name.Contains(keyWord) && foods.Pt == "07"
                            select foods;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        public void ByView(string keyWord)
        {
            using (var db = new ProjectEntities())
            {
                var query = from views in db.views
                            where views.Name.Contains(keyWord) && views.Pt == "10"
                            select views;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        public void ByHotel(string keyWord)
        {
            using (var db = new ProjectEntities())
            {
                var query = from hotels in db.views
                            where hotels.Name.Contains(keyWord) && hotels.Pt == "06"
                            select hotels;
                foreach (var item in query)
                {
                    Add(GetView(item));
                }
            }
        }

        static public string FilterType(int type)
        {
            string output = string.Empty;
            switch (type)
            {
                case 0:
                    output = "全部";
                    break;
                case 1:
                    output = "美食";
                    break;
                case 2:
                    output = "景點";
                    break;
                case 3:
                    output = "旅館";
                    break;
            }
            return output;
        }

        //public void ByView(string keyWord)
        //{
        //    using (var db = new ProjectEntities())
        //    {
        //        var query = (from authors in db.authors
        //                     where authors.AuthorName.Contains(keyWord)
        //                     from books in authors.books
        //                     select books).Distinct();
        //        foreach (var item in query)
        //        {
        //            Add(new book(item));
        //        }
        //    }
        //}

        //public void ByHotel(string keyWord)
        //{
        //    using (var db = new ProjectEntities())
        //    {
        //        var query = (from publishers in db.publishers
        //                     where publishers.PublisherName.Contains(keyWord)
        //                     from books in publishers.books
        //                     select books).Distinct();
        //        foreach (var item in query)
        //        {
        //            Add(new book(item));
        //        }
        //    }
        //}

    }

    public class ResultView : view
    {
        public string viewTypeName { get; set; }
        public string cityName { get; set; }
        public ViewImage image { get; set; }
    }
}