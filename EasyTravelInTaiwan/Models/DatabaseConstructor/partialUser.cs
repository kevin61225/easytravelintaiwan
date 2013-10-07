using System;
using System.Collections.Generic;
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
        public void RunSeparate()
        {
            List<member> group = users;
            foreach (var item in group)
            {
                item.SeperateTags();
                item.RunInit();
            }
            int noChange = 0;
            //while (group.Count > 20)
            while(true)
            {
                int ci = 0, cj = 1;
                double maxS = 0.0;  // 相似的最大值
                for (int i = 0; i < group.Count; i++)
                {
                    for (int j = i + 1; j < group.Count; j++)
                    {
                        // compare to every group
                        double s = group[i].GetNearestSimilarity(group[j].users);
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

                //if (group[ci].TagString == null)
                //{
                //}

                //if (group[cj].TagString == null)
                //{
                //}

                // i 一定會大於 j
                if (maxS > 0.3)//相似度
                {
                    group.RemoveAt(ci);
                    group.RemoveAt(cj - 1);
                    group.Add(newGroup);
                }
                else
                {   //這裡是跑完的意思
                    noChange++;
                    if (noChange == 10) //固定不動
                    {
                        break;
                    }
                }
            }
        }
        #endregion
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