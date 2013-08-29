using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapSupport.HtmlHelpers;
using System.Web.Services.Description;
using System.Web.Security;

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

        [Authorize(Roles = "Admin, Clerk, Customer")]
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                //if (Session["UserId"] == null)
                //{
                //    FormsAuthentication.SignOut();
                //    Session.Clear();
                //    return RedirectToAction("Index");
                //}
                FindUserIdByName(User.Identity.Name);
                Session["Role"] = FindRoleIdByName(User);
                //if ((string)Session["Role"] == "Admin" || (string)Session["Role"] == "Clerk")
                //{
                //    return RedirectToAction("Index", "Author");
                //}
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


        public ActionResult TravelListPartial()
        {
            int uid = (int)Session["UserId"];
            List<travellist> travelList = db.travellists.Where(list => list.UserId == uid).ToList<travellist>();
            //if (TravelListName == null)
            //{
                if (travelList == null)
                {
                    Session["TempTid"] = -1;
                    return HttpNotFound();
            
                }
                Session["TempTid"] = travelList[0].Tid;
            //}
            //else
            //{
            //    travellist newList = new travellist();
            //    newList.TName = TravelListName;
            //    newList.UserId = (int)Session["UserId"];

            //    try
            //    {
            //        db.travellists.Add(newList);
            //        db.SaveChanges();
            //    }
            //    catch
            //    {
            //        TempData["Error"] = "儲存錯誤";
            //    }
            //    return RedirectToAction("Index", "Map");
            //}
            //return RedirectToAction("Index", "Map");
            return PartialView("_travelListPartial", travelList);
        }


        public ActionResult TravelListPlacePartial()
        {
            int tid = (int)Session["TempTid"];
            List<travellistplace> travelListPlace = db.travellistplaces.Where(list => list.Tid == tid).ToList<travellistplace>();
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
        public JsonResult PostPlace(List<travellistplace> info)
        {
            travellist test = new travellist();
            member testm = new member();
            test.Tid = (int)Session["TempTid"];
            test.UserId = (int)Session["UserId"];
            test =  db.travellists.Find(test.Tid);
            testm = db.members.Find(test.UserId);
            int tid = (int)Session["TempTid"];
            if (info != null)
            {
                for (int i = 0; i < info.Count; i++)
                {
                    info[i].Tid = tid;
                    db.travellistplaces.Add(info[i]);
                }
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    TempData["Error"] = "儲存錯誤";
                    return Json(new { Status = 3, Message = "Saving Error in "  });
                }
                    //foreach(travellistplace item in info)
                    //{
                    //    item.Tid = tid;
                    //    //item.travellist = test;

                    //    try
                    //    {
                    //        db.travellistplaces.Add(item);
                    //        db.SaveChanges();
                    //    }
                    //    catch
                    //    {
                    //        TempData["Error"] = "儲存錯誤";
                    //        return Json(new { Status = 3, Message = "Saving Error in " + item.Sno });
                    //    }
                    //}
                    return Json(new { Status = 1, Message = "Success" });
            }
            return Json(new { Status = 2, Message = "info is null" });
        }

        [HttpPost]
        public ActionResult CreateNewList(string TravelListName)
        {
            travellist newList = new travellist();
            newList.TName = TravelListName;
            newList.UserId = (int)Session["UserId"];

            int uid = (int)Session["UserId"];

            try
            {
                db.travellists.Add(newList);
                db.SaveChanges();
            }
            catch
            {
                TempData["Error"] = "儲存錯誤";
            }

            List<travellist> travelList = db.travellists.Where(list => list.UserId == newList.UserId).ToList<travellist>();
            Session["TempTid"] = travelList[(travelList.Count)-1].Tid;
            //Session["TempTid"] = (int)travelList.Where(o => o.TName == TravelListName).Single().Tid;
            //return PartialView("_travelListPartial", travelList);
            return RedirectToAction("Index", "Map");
        }

    }
}
