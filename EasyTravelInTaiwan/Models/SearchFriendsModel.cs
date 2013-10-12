using System.Linq;
using System.Collections.Generic;

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
    }
}