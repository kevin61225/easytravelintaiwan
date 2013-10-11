using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class SetCalculation
    {
        /// <summary>
        /// 回傳交集
        /// </summary>
        /// <param name="t1">整數集合1</param>
        /// <param name="t2">整數集合1</param>
        /// <returns>回傳交集</returns>
        static public int[] AND(int[] t1, int[] t2)
        {
            List<int> output = new List<int>();
            foreach (int tag in t1)
            {
                if (t2.Contains(tag))
                {
                    output.Add(tag);
                }
            }
            return output.ToArray();
        }

        /// <summary>
        /// 回傳聯集
        /// </summary>
        /// <param name="t1">整數集合1</param>
        /// <param name="t2">整數集合2</param>
        /// <returns>回傳聯集</returns>
        static public int[] OR(int[] t1, int[] t2)
        {
            List<int> output = t1.ToList();
            foreach (int tag in t2)
            {
                if (!output.Contains(tag))
                {
                    output.Add(tag);
                }
            }
            return output.ToArray();
        }

        /// <summary>
        /// 減法
        /// </summary>
        /// <param name="t1">被減數</param>
        /// <param name="t2">減數</param>
        /// <returns>結果</returns>
        static public int[] Minus(int[] t1, int[] t2)
        {
            List<int> result = t1.ToList();
            foreach (int item in t2)
            {
                if (result.Contains(item))
                {
                    result.Remove(item);
                }
            }
            return result.ToArray();
        }
    }
}