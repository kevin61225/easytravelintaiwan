using System.Linq;
using System.Collections.Generic;

namespace EasyTravelInTaiwan.Models                      //搜尋結果的Model
{
    public class SearchResultModel  :List<place>
    {
        //public void TopRatingByAmount(int resultCount)
        //{
        //    using (var db = new ProjectEntities())
        //    {
        //        var query = (from ratingItems in db.ratings
        //                     group ratingItems by ratingItems.ProductID into ratingItemsGrp
        //                     orderby ratingItemsGrp.Average(o => o.Score) descending
        //                     select ratingItemsGrp.Key).Take(resultCount).ToList();
        //        foreach (var item in query)
        //        {
        //            Add(new book(db.products.Find(item).book));
        //        }
        //    }
        //}
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
                var query = from places in db.places
                            select places;
                foreach (var item in query)
                {
                    Add(new place(item));
                }
            }
        }

        public void ByTitle(string keyWord)
        {
            using (var db = new ProjectEntities())
            {
                var query = from places in db.places
                            where places.Name.Contains(keyWord)
                            select places;
                foreach (var item in query)
                {
                    Add(new place(item));
                }
            }
        }


        //public void ByFood(string keyWord)              
        //{
        //    using (var db = new ProjectEntities())
        //        {
        //            var query = from books in db.books
        //                        where books.Title.Contains(keyWord)
        //                        select books;
        //            foreach (var item in query)
        //            {
        //                Add(new book(item));
        //            }
        //        }
        //}

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
}