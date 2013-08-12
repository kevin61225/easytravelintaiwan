using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapSupport.HtmlHelpers;
using System.Web.Services.Description;

namespace EasyTravelInTaiwan.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/
        projectEntities db = new projectEntities();

        public string FindRoleIdByName(System.Security.Principal.IPrincipal User)
        {
            try
            {
                if (User.IsInRole("Customer"))
                {
                    return "Customer";
                }
                else if (User.IsInRole("Clerk"))
                {
                    return "Clerk";
                }
                else if (User.IsInRole("Admin"))
                {
                    return "Admin";
                }
            }
            catch
            {
            }
            return "null";
        }

        public void FindUserIdByName(string userAccount)
        {
            member user;
            try
            {
                user = db.members.Where(o => o.Account == userAccount).Single();
                Session["UserName"] = user.Name;
                Session["UserId"] = user.UserID;
            }
            catch
            {
                return;
            }
            return;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                FindUserIdByName(User.Identity.Name);
                Session["Role"] = FindRoleIdByName(User);
                if ((string)Session["Role"] == "Admin" || (string)Session["Role"] == "Clerk")
                {
                    return RedirectToAction("Index", "Author");
                }
            }
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public JsonResult GetMap()
        {
            var mapMarkerList = new MapRepository();

            return Json(mapMarkerList, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult TravelListPartial()
        {
            int uid = 2;
            //int uid = (int)Session["UserId"];
            List<travellist> travelList = db.travellists.Where(list => list.UserId == uid).ToList<travellist>();
            if (travelList == null)
            {
                return HttpNotFound();
            }
            return PartialView("_travelListPartial", travelList);
        }

        [ChildActionOnly]
        public ActionResult TravelListPlacePartial()
        {
            int uid = 2;
            //int uid = (int)Session["UserId"];
            List<travellistplace> travelListPlace = db.travellistplaces.Where(list => list.Tid == uid).ToList<travellistplace>();
            if (travelListPlace == null)
            {
                return HttpNotFound();
            }
            return PartialView("_travelListPlacePartial", travelListPlace);
        }

        public ActionResult ViewPointList()
        {
            ViewBag.Title = "景點清單";
            return View(db.places.ToList());
        }

        public ActionResult ViewPointDetails(string id)
        {
            place place = db.places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            return View(place);
        }

        public ActionResult ViewPointEdit(string id)
        {
            place place = db.places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            ViewBag.Typenumber = new SelectList(db.viewtypes, "Typenumber", "Typename", place.Typenumber);
            ViewBag.Citynumber = new SelectList(db.cities, "Citynumber", "Cityname", place.Citynumber);
            return View(place);
        }

        public ActionResult MapLatLng(int page = 1)
        {
            var pageSize = 100;
            IEnumerable<maplatlng> maplatlng = db.maplatlngs.OrderBy(o => o.Lng); ;
            ViewData.Model = maplatlng;
            return View(maplatlng.ToPagedList(page, pageSize));
        }

        public FileContentResult RenderBookImage(int id)
        {
            byte[] img = db.placeimages.Find(id).Image;
            return File(img, "image/jpeg");
        }

        public ActionResult ShowImage(string id)
        {
            place place = db.places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            List<placeimage> imageList = place.placeimages.ToList();
            return View(imageList);
        }

        public ActionResult CityViews(string id)
        {
            List<place> placeInCity = db.places.Where(o => o.Citynumber == id).ToList();
            List<viewtype> TypeList = new List<viewtype>();
            foreach (viewtype type in db.viewtypes)
            {
                int count = placeInCity.Where(o => o.Typenumber == type.Typenumber).Count();
                if (count != 0)
                {
                    TypeList.Add(type);
                }
            }
            ViewBag.TypeList = TypeList;
            ViewBag.Title = db.cities.Find(id).Cityname;
            return View(placeInCity);
        }

        public ActionResult CityList()
        {
            return View(db.cities.ToList());
        }

        [HttpPost]
        public JsonResult PostPlace(List<PlaceInfo> info)
        {
            if (info != null)
            {
                return Json(new { Status = 1, Message = "Success" });
            }
            return Json(new { Status = 2, Message = "info is null" });
        }
    }
}
