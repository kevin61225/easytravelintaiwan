using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class History
    {
        public string gId { get; set; }
        public string title { get; set; }
    }

    public class HistoryModel
    {
        public List<History> histories { get; set; }

        public HistoryModel()
        {
        }

        public HistoryModel(List<History> input)
        {
            this.histories = input;
        }
    }

}