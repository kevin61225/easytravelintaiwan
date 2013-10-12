using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class Suggestor
    {
        private ProjectEntities db = new ProjectEntities();
        public List<view> GetSuggestPlaceSno(int id)
        {
            List<member> mems = db.members.ToList();
            member current = db.members.Where(o => o.UserID == id).Single();

            List<member> sameGroup = mems.Where(o => o.GId == current.GId).ToList();
            current.SeperateTags();
            foreach (member item in sameGroup)
            {
                item.SeperateTags();
            }
            int[] suggestTags = GetSuggestTags(current, sameGroup);
            List<rating> ratings = new List<rating>();

            List<rating> dbRatingData = db.ratings.ToList();

            foreach (int tag in suggestTags)
            {
                ratings.AddRange(dbRatingData.Where(o => o.view.Viewtype == tag));
            }
            //foreach (int tag in suggestTags)
            //{
            //    ratings.AddRange(db.ratings.Where(o => o.view.Viewtype == tag));
            //}

            var query = (from ratingItems in ratings
                         group ratingItems by ratingItems.Sno into ratingItemsGrp
                         orderby ratingItemsGrp.Sum(o => o.Point) descending
                         select ratingItemsGrp.Key).ToList();
            view vie = new view();
            List<rating> hasBeen = current.ratings.ToList();
            query.RemoveAll(o => hasBeen.Where(p => p.Sno == o).Count() != 0);
            List<view> result = new List<view>();
            query.ForEach(delegate(String sno)
            {
                result.Add(db.views.Find(sno));
            });
            return result;
        }

        private int[] GetSuggestTags(member current, List<member> sameGroup)
        {
            int[] orSet = GetAllOR(current, sameGroup);
            int[] selfSet = current._tags;
            return SetCalculation.Minus(orSet, selfSet);
        }

        private int[] GetAllOR(member current, List<member> group)
        {
            int[] set = current._tags;
            for (int i = 0; i < group.Count(); i++)
            {
                set = SetCalculation.OR(set, group[i]._tags);
            }
            return set;
        }
    }
}