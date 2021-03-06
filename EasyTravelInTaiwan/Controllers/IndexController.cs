﻿using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View();
        }
        
        [ChildActionOnly]
        public ActionResult HotFoodPartial()
        {

            SearchResultModel model = new SearchResultModel();

            model.TopRatingFoodByAmount(5);

            return PartialView("_hotFoodPartial", model);
        }

        [ChildActionOnly]
        public ActionResult HotHotelPartial()
        {

            SearchResultModel model = new SearchResultModel();

            model.TopRatingHotelByAmount(5);

            return PartialView("_hotHotelPartial", model);
        }

        [ChildActionOnly]
        public ActionResult HotViewPartial()
        {

            SearchResultModel model = new SearchResultModel();

            model.TopRatingViewByAmount(5);

            return PartialView("_hotViewPartial", model);
        }
    }
}
