using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace EasyTravelInTaiwan.Models                      //搜尋結果的Model
{
    public class SearchFriendsModel : List<member>
    {
        public void SearchFriends(int UserId)
        {
            using (var db = new ProjectEntities())
            {
                List<member> list = db.members.Where(o => o.UserID == UserId).Single().member1.ToList();
                AddRange(list);
            }
        }

        public void RecommendFriends(int UserId)
        {
            using (var db = new ProjectEntities())
            {
                int gId = db.members.Where(o => o.UserID == UserId).Single().GId;
                List<member> list = db.members.Where(o => o.GId == gId).Where(o => o.UserID != UserId).ToList();
                AddRange(list);
            }
        }

        static public void AddFriend(int? uid, int friendId)
        {
            using (var db = new ProjectEntities())
            {
                member me = db.members.Where(o => o.UserID == uid).Single();
                member friend = db.members.Where(o => o.UserID == friendId).Single();
                me.member1.Add(friend);
                db.Entry(me).State = EntityState.Detached;
                db.SaveChanges();
                List<member> i = me.member1.ToList();
                List<member> j = me.members.ToList();
            }
        }
    }
}