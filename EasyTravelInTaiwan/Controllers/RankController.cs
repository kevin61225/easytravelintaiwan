using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Controllers
{
    public class RankController : Controller
    {
        //
        // GET: /Rank/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RankFilter(string type)
        {
            RankResultOptions options = new RankResultOptions(type);

            return PartialView("_rankFilterPartial", options);
        }

        public ActionResult RankPartial(string type)
        {
            SearchResultModel model = new SearchResultModel();
            model.TopRatingByType(type);
            return PartialView("_rankResultPartial", model);
        }
    }
}
