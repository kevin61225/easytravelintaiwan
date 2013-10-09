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

        public ActionResult RankFilter()
        {
            RankResultOptions options = new RankResultOptions();

            return PartialView("_rankFilterPartial", options);
        }

        public ActionResult RankResult()
        {
            return PartialView("_rankResultPartial");
        }

        public ActionResult ViewRankPartial()
        {
            SearchResultModel model = new SearchResultModel();
            model.TopRatingViewByAmount(10);
            return PartialView("_rankResultPartial", model);
        }

        public ActionResult FoodRankPartial()
        {
            SearchResultModel model = new SearchResultModel();
            model.TopRatingFoodByAmount(10);
            return PartialView("_rankResultPartial", model);
        }

        public ActionResult HotelRankPartial()
        {
            SearchResultModel model = new SearchResultModel();
            model.TopRatingHotelByAmount(10);
            return PartialView("_rankResultPartial", model);
        }

    }
}
