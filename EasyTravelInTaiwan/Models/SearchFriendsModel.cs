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
                List<friend> list = db.members.Where(o => o.UserID == UserId).Single().friends.ToList();
                foreach (friend item in list)
                {
                    Add(item.member1);
                }
            }
        }

        public void RecommendFriends(int UserId)
        {
            using (var db = new ProjectEntities())
            {
                int gId = db.members.Where(o => o.UserID == UserId).Single().GId;
                List<member> output = new List<member>();
                List<friend> myFriends = db.members.Where(o => o.UserID == UserId).Single().friends.ToList();
                List<member> myFriendsMember = this.GetMembersByFriends(myFriends);
                List<member> recommendlist = db.members.Where(o => o.GId == gId).Where(o => o.UserID != UserId).ToList();
                foreach (member item in recommendlist)
                {
                    if (!myFriendsMember.Contains(item))
                        output.Add(item);
                }
                AddRange(output);
            }
        }

        public List<member> GetMembersByFriends(List<friend> myFriends)
        {
            List<member> myFriendsMember = new List<member>();
            foreach (friend item in myFriends)
            {
                myFriendsMember.Add(item.member1);
            }
            return myFriendsMember;
        }

        static public int FindIfIsFriend(int userId, int friendsId)
        {
            int output;
            using (var db = new ProjectEntities())
            {
                try
                {
                    db.friends.Where(o => o.Fid == userId).Where(o => o.Uid == friendsId).Single();
                    output = 1;  // 是朋友
                }
                catch
                {
                    output = 2; // 不是朋友
                }
            }
            return output;
        }

        static public void AddFriend(int uid, int friendId)
        {
            using (var db = new ProjectEntities())
            {
                friend relation1 = new friend();
                relation1.Fid = uid;
                relation1.Uid = friendId;

                friend relation2 = new friend();
                relation2.Fid = friendId;
                relation2.Uid = uid;
                db.friends.Add(relation1);
                db.friends.Add(relation2);

                db.SaveChanges();
            }
        }
    }
}