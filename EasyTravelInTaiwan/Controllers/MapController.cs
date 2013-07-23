using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyTravelInTaiwan.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult GetMap()
        {
            var mapMarkerList = new MapRepository();

            return Json(mapMarkerList, JsonRequestBehavior.AllowGet);
        }

    }
}
