using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Models
{
    public class SearchViewModel
    {
        public string searchWord { get; set; }
        public int searchType { get; set; }
    }

    public class FavoritePlaces
    {
        public string CityName { get; set; }
        public string CityId { get; set; }
        public List<viewtype> viewtypes { get; set; }
    }
}