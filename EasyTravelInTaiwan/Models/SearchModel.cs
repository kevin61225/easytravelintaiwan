using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models                          //這個Model是TypeAhead使用的
{
    public class SearchModel    :List<SearchTypeAheadEntity>
    {
        public void AddPlaceSearch()
        {
            using (var db = new ProjectEntities())
            {
                var placeQuery = from places in db.places
                                select places;
                foreach (var item in placeQuery)
                {
                    Add(new SearchTypeAheadEntity { entity = item.Name });
                }
            }
        }

        //public void AddPublishersSearch()
        //{
        //    using (var db = new ProjectEntities())
        //    {
        //        var publisherQuery = from publishers in db.publishers
        //                             select publishers;
        //        foreach (var item in publisherQuery)
        //        {
        //            Add(new SearchTypeAheadEntity { entity = item.PublisherName });
        //        }
        //    }
        //}

        //public void AddAuthorsSearch()
        //{
        //    using (var db = new ProjectEntities())
        //    {

        //        var authorQuery = from authors in db.authors
        //                          select authors;
        //        foreach (var item in authorQuery)
        //        {
        //            Add(new SearchTypeAheadEntity { entity = item.AuthorName });
        //        }

        //    }
        //}
    }
}