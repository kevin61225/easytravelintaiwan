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
        ProjectEntities db = new ProjectEntities();

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

        [Authorize]
        public ActionResult TravelListPartial()
        {
            int uid = (int)Session["UserId"];
            List<travellist> travelList = db.travellists.Where(list => list.UserId == uid).ToList<travellist>();

            if (Session["TempTid"] == null)
            {
                try
                {
                    Session["TempTid"] = travelList[0].Tid;
                }
                catch
                {
                }
            }

            if (travelList.Count == 0)
            {
                Session["TempTid"] = -1;
                ViewBag.SelectList = null;
            }
            else
            {
                ViewBag.SelectList = travelList.Where(o => o.Tid == (int)Session["TempTid"]).Single();
            }

            return PartialView("_travelListPartial", travelList);
        }

        [Authorize]
        public ActionResult TravelListPlacePartial()
        {
            int tid = (int)Session["TempTid"];
            if (tid == -1)
            {
                return HttpNotFound();
            }
            List<travellistplace> travelListPlace = db.travellistplaces.Where(list => list.Tid == tid).ToList<travellistplace>();
            if (travelListPlace == null)
            {
                return HttpNotFound();
            }
            List<maplatlng> placeInfo = new List<maplatlng>();
            foreach (travellistplace place in travelListPlace)
            {
                maplatlng temp = new maplatlng();
                try
                {
                    temp = db.maplatlngs.Where(o => o.sno == place.Sno).Single();
                }
                catch
                {
                }
                placeInfo.Add(temp);
            }
            ViewBag.TravelListPlaces = placeInfo;
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

        // 將細項加入清單
        [HttpPost]
        public ActionResult PostPlace(List<travellistplace> info)
        {
            int tid = (int)Session["TempTid"];

            if (info != null)
            {
                foreach (travellistplace item in info)
                {
                    travellistplace temp = new travellistplace();
                    try
                    {
                        temp = db.travellistplaces.Where(o => o.Sno == item.Sno).Single();
                    }
                    catch
                    {

                    }
                    if (!db.travellistplaces.ToList().Contains(temp))
                    {
                        item.Tid = tid;
                        try
                        {
                            db.travellistplaces.Add(item);
                            db.SaveChanges();
                        }
                        catch
                        {
                            TempData["Error"] = "儲存錯誤";
                            return Json(new { Status = 3, Message = "Saving Error in " + item.Sno });
                        }
                        
                    }
                }
                TempData["SaveSuccess"] = "儲存成功 !!";
                return RedirectToAction("TravelListPlacePartial", "Map");
                //return Json(new { Status = 1, Message = "Success" });
            }
            return Json(new { Status = 2, Message = "info is null" });
        }

        // 新增清單
        [HttpPost]
        public ActionResult CreateNewList(string travelListName)
        {
            travellist newList = new travellist();
            newList.TName = travelListName;
            newList.UserId = (int)Session["UserId"];

            int uid = (int)Session["UserId"];

            try
            {
                db.travellists.Add(newList);
                db.SaveChanges();
            }
            catch
            {
                TempData["Error"] = "創建錯誤";
                return RedirectToAction("Index", "Map");
            }

            List<travellist> travelList = db.travellists.Where(list => list.UserId == newList.UserId).ToList<travellist>();
            Session["TempTid"] = travelList[(travelList.Count) - 1].Tid;
            TempData["CreateSuccess"] = "建立成功 !!";

            return RedirectToAction("TravelListPartial", "Map");
        }

        [HttpPost]
        public ActionResult DeleteList(string deleteListId)
        {
            deleteListId = deleteListId.Remove(0, deleteListId.IndexOf("-") + 1);
            travellist deleteItem = db.travellists.Find(int.Parse(deleteListId));
            if (deleteItem != null)
            {
                db.travellists.Remove(deleteItem);
                db.SaveChanges();
                Session["TempTid"] = null;
            }
            return RedirectToAction("TravelListPartial", "Map");
        }

        // 切換清單
        [HttpPost]
        public ActionResult OnChangeTravelList(string selectedList)
        {
            Session["TempTid"] = Convert.ToInt32(selectedList);

            return RedirectToAction("TravelListPlacePartial", "Map");
            //return Json(new { Status = 1, Message = "Success" });
        }


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

    }
}
