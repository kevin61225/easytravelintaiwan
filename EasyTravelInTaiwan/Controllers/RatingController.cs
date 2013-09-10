using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Controllers
{
    public class RatingController : Controller
    {
        //
        // GET: /Rating/
        ProjectEntities db = new ProjectEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PlaceRatePartial(string sno)
        {
            int rate;

            try
            {
                rate = db.ratings.Where(o => o.Sno == sno).Sum(o => o.Point);
            }
            catch
            {
                rate = -1;
            }

            ViewBag.RateSum = rate;

            return PartialView("_placeRatingPartial");
        }

    }
}
