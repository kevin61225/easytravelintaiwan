using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public partial class sortedhistory
    {
        public List<string> SeperateString()
        {
            string[] strTags = this.historyString.Split(',');
            List<string> listTags = new List<string>();
            foreach (string tag in strTags)
            {
                listTags.Add(tag);
            }

            return listTags;
        }
    }
}