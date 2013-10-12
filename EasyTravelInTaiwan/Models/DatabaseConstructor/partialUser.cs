using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class partialUser
    {
        ProjectEntities db = new ProjectEntities();

        List<member> users;

        public void LoadUserData()
        {
            users = db.members.ToList();
        }

        #region 演算法
        /// <summary>
        /// Complete Linkage
        /// </summary>
        /// <remarks>
        /// Reset all member's group id
        /// Need a lot of time to run it
        /// </remarks>
        public void RunSeparate()
        {
            List<member> group = users;
            foreach (var item in group)
            {
                item.SeperateTags();
                item.RunInit();
            }
            while(true)
            {
                int ci = 0, cj = 1;
                double maxS = 0.0;  // 相似的最大值
                for (int i = 0; i < group.Count; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                    {
                        // compare to every group
                        double s = group[i].AverageLinkage(group[j].users);
                        if (s > maxS)
                        {
                            ci = i;
                            cj = j;
                            maxS = s;
                        }
                    }
                }
                // mix to group (ci, cj)
                member newGroup = new member();
                newGroup.users.AddRange(group[ci].users);
                newGroup.users.AddRange(group[cj].users);

                // i 一定會大於 j
                if (maxS >= 0.5)//相似度
                {
                    group.RemoveAt(ci);
                    group.RemoveAt(cj - 1);
                    group.Add(newGroup);
                }
                else
                {   //這裡是跑完的意思
                    for (int i = 0; i < group.Count; i++)
                    {
                        foreach (var item in group[i].users)
                        {
                            item.GId = i;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Single Linkage
        /// </summary>
        /// <param name="account">Set Account Group id</param>
        public void SetGroupIdBySingle(string account)
        {
            member current = db.members.Where(o => o.Account == account).First();
            users.Remove(current);

            current.SeperateTags();
            double biggestSimilar = 0.0;
            int b_index = 0;
            for (int i = 0; i < users.Count(); i++)
			{
			    users[i].SeperateTags();
                double cur = current.SimilarTo(users[i]);
                if (cur > biggestSimilar)
                {
                    biggestSimilar = cur;
                    b_index = i;
                }
			}
            current.GId = users[b_index].GId;

            db.Entry(current).State = System.Data.EntityState.Modified;

            db.SaveChanges();
        }
    }

    public partial class member
    {
        public int[] _tags { get; set; }
        public List<member> users = new List<member>();

        public void SeperateTags()
        {
            string[] strTags = this.favorite.Split('-');
            List<int> listTags = new List<int>();
            foreach (string tag in strTags)
            {
                listTags.Add(int.Parse(tag));
            }

            _tags = listTags.ToArray();
        }

        public void RunInit()
        {
            users.Add(this);
        }

        public double GetNearestSimilarity(List<member> otherNode)
        {
            double maxS = 0.0;
            for (int i = 0; i < users.Count; i++)   // compare to inside node
            {
                for (int j = 0; j < otherNode.Count; j++)
                {
                    // compare eyerything outside
                    double similar = users[i].SimilarTo(otherNode[j]);    // current similarity
                    if (similar > maxS)
                    {
                        maxS = similar;
                    }
                }
            }
            return maxS;
        }

        public double AverageLinkage(List<member> otherNode)
        {
            double totalSimilar = 0;
            for (int i = 0; i < users.Count; i++)   // compare to inside node
            {
                for (int j = 0; j < otherNode.Count; j++)
                {
                    // compare eyerything outside
                    double similar = users[i].SimilarTo(otherNode[j]);    // current similarity
                    totalSimilar += similar;
                }
            }
            return totalSimilar / (users.Count * otherNode.Count);
        }

        public int[] AND(member other)
        {
            List<int> output = new List<int>();
            foreach (int tag in _tags)
            {
                if (other._tags.Contains(tag))
                {
                    output.Add(tag);
                }
            }
            return output.ToArray();
        }

        public int[] OR(member other)
        {
            List<int> output = _tags.ToList();
            foreach (int tag in other._tags)
            {
                if (!output.Contains(tag))
                {
                    output.Add(tag);
                }
            }
            return output.ToArray();
        }

        public double SimilarTo(member other)
        {
            double denominator = this.OR(other).Count();   // 分母
            double numerator = this.AND(other).Count();  // 分子

            return numerator / denominator;
        }
    }
}