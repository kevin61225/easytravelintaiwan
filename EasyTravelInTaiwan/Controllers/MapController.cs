using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapSupport.HtmlHelpers;
using System.Web.Services.Description;
using System.Web.Security;
using System.IO;
using System.Data;

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

        public ActionResult Direction(int tid)
        {
            if (db.travellistplaces.Where(list => list.Tid == tid).ToList<travellistplace>().Count() < 2)
            {
                return RedirectToAction("ErrorPage", "Error");
            }

            List<travellistplace> travelListPlace = db.travellistplaces.Where(list => list.Tid == tid).ToList<travellistplace>();
            List<view> viewList = new List<view>();
            foreach (travellistplace item in travelListPlace)
            {
                view tempView = db.views.Where(o => o.Id == item.Sno).Single();
                viewList.Add(tempView);
            }
            ViewBag.WaypointId = tid;
            try
            {
                ViewBag.SId = db.sortedhistories.Where(o => o.Tid == tid).ToList<sortedhistory>()[0].SId;
            }
            catch
            {
                // 表示無路線紀錄
                ViewBag.SId = -1;
            }

            return View(viewList);
        }

        [HttpPost]
        public JsonResult GetDirection(string tid)
        {
            var mapMarkerList = new MapRepository();
            mapMarkerList.GetByTid(tid);

            return Json(mapMarkerList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostDirectionHistory(string tid, string sortedList)
        {
            sortedList = sortedList.Remove(sortedList.Length - 1);
            sortedhistory history = new sortedhistory();
            history.historyString = sortedList;
            history.Tid = int.Parse(tid);
            try
            {
                db.sortedhistories.Where(o => o.Tid == history.Tid).Where(o => o.historyString == history.historyString).Single();
                List<sortedhistory> list = db.sortedhistories.Where(o => o.Tid == history.Tid).ToList<sortedhistory>();
                int index = list.FindIndex(x => x.historyString == history.historyString);
                return Json(new { Status = "1", Messages = "路徑已存在 ! (於規劃 " + (index + 1) + ")" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                db.sortedhistories.Add(history);
                db.SaveChanges();
            }

            return RedirectToAction("SortedHistory", "Map", new { tid = tid });
        }

        [HttpPost]
        public ActionResult DeleteDirectionHistory(string tid, string sortedList)
        {
            sortedList = sortedList.Remove(sortedList.Length - 1);
            sortedhistory history = new sortedhistory();
            history.historyString = sortedList;
            history.Tid = int.Parse(tid);
            try
            {
                sortedhistory historyTemp = db.sortedhistories.Where(o => o.Tid == history.Tid).Where(o => o.historyString == history.historyString).Single();
                db.sortedhistories.Remove(historyTemp);
                db.SaveChanges();
            }
            catch
            {
              
            }

            return RedirectToAction("SortedHistory", "Map", new { tid = tid });
        }

        public JsonResult GetMap()
        {
            var mapMarkerList = new MapRepository();
            mapMarkerList.GetAll();

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

            return PartialView("Map/_travelListPartial", travelList);
        }

        [Authorize]
        public ActionResult TravelListPlacePartial()
        {
            int tid = (int)Session["TempTid"];

            List<travellistplace> travelListPlace = db.travellistplaces.Where(list => list.Tid == tid).ToList<travellistplace>();

            List<view> placeInfo = new List<view>();
            foreach (travellistplace place in travelListPlace)
            {
                view temp = new view();
                try
                {
                    temp = db.views.Where(o => o.Id == place.Sno).Single();
                }
                catch
                {
                }
                placeInfo.Add(temp);
            }

            ViewBag.TravelListPlaces = placeInfo;
            ViewBag.TravelListLength = placeInfo.Count();
            return PartialView("Map/_travelListPlacePartial", travelListPlace);
        }

        public ActionResult ViewPointList()
        {
            ViewBag.Title = "景點清單";
            return View(db.places.ToList());
        }

        public ActionResult ViewPointDetails(string id)
        {
            view place = db.views.Find(id);
            PlaceDetail detail = new PlaceDetail();
            if (place == null)
            {
                return RedirectToAction("ErrorPage", "Error");
            }
            switch (place.Pt)
            {
                case "06":
                    try
                    {
                        hotel hotel = db.hotels.Where(o => o.Id == place.Id).Single();
                        detail = new HotelDetail(hotel, place.Pt);
                    }
                    catch
                    {
                        accommodation acco = db.accommodations.Where(o => o.id == place.Id).Single();
                        detail = new AccommodationDetail(acco, place.Pt, place.Viewtype);
                    }
                    break;
                case "07":
                    food food = db.foods.Where(o => o.id == place.Id).Single();
                    detail = new FoodDetail(food, place.Pt, place.Viewtype);
                    break;
                case "10":
                    place viewplace = db.places.Where(o => o.Id == place.Id).Single();
                    detail = new ViewDetail(viewplace, place.Pt);
                    break;
            }
            detail.CheckEmptyData();

            if (detail.viewimages.Count() == 0) detail.viewimages.Add(ViewImage.GetNotFoundImage(db));
            TempData["Title"] = place.Name;
            Session["Pt"] = place.Pt;
            try
            {
                ViewBag.FavoriteType = SearchFavoriteModel.CheckIsFavorite((int)Session["UserId"], place.Id);
            }
            catch
            {
                ViewBag.FavoriteType = 2;
            }
            return View(detail);
        }

        [HttpPost]
        public ActionResult AddToFavorite(string userId, string placeId)
        {
            if (userId == string.Empty)
            {
                return Json(new { Status = 2, Message = "請先登入會員 !!" }, JsonRequestBehavior.AllowGet);
            }
            int uId = int.Parse(userId);

            SearchFavoriteModel.AddFavorite(uId, placeId);
            return Json(new { Status = 1, Message = "已收藏 !!" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult ImageSlidePartial(ViewImage images)
        {
            return PartialView("_imageSlidePartial", images);
        }

        public FileResult RenderBookImage(string id, string pt, int sid)
        {
            byte[] img = ViewImage.GetImageById(db, id, pt, sid);
            byte[] notFound = db.notfoundimages.Where(o => o.NId == 2).Single().Image;

            if (img != null) return File(img, "image/jpeg");
            return File(notFound, "image/jpeg");
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
            List<view> placeInCity = db.views.Where(o => o.City == id).Where(o => o.Name != "").ToList();
            List<viewtype> TypeList = new List<viewtype>();
            foreach (viewtype type in db.viewtypes)
            {
                int count = placeInCity.Where(o => o.Viewtype == type.Typenumber).Where(o => o.Name != null).Count();
                if (count != 0)
                {
                    TypeList.Add(type);
                }
            }
            ViewBag.TypeList = TypeList;
            ViewBag.ListTitle = db.cities.Find(id).Cityname;
            return View(placeInCity);
        }

        public ActionResult CityList()
        {
            return View(db.cities.ToList());
        }

        public ActionResult SortPlace(int tid, int sid)
        {
            List<travellistplace> travelListPlace = db.travellistplaces.Where(list => list.Tid == tid).ToList<travellistplace>();
            sortedhistory histories = new sortedhistory();
            List<view> placeInfo = new List<view>();
            if (sid == -1)
            {
                foreach (travellistplace place in travelListPlace)
                {
                    view temp = new view();
                    try
                    {
                        temp = db.views.Where(o => o.Id == place.Sno).Single();
                    }
                    catch
                    {
                    }
                    placeInfo.Add(temp);
                }
            }
            else
            {
                histories = db.sortedhistories.Where(o => o.SId == sid).Single();
                List<string> historylist = histories.SeperateString();
                foreach (string place in historylist)
                {
                    view temp = new view();
                    try
                    {
                        temp = db.views.Where(o => o.Id == place).Single();
                    }
                    catch
                    {
                    }
                    placeInfo.Add(temp);
                }
            }
            ViewBag.TravelListPlaces = placeInfo;
            ViewBag.TravelListLength = placeInfo.Count();
            return PartialView("Direction/_sortPlacePartial", travelListPlace);
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
            try
            {
                travellist temp = db.travellists.Where(o => o.TName == travelListName).Single();
                TempData["CreateList"] = "該清單名稱已存在，請換名稱";
                return RedirectToAction("TravelListPartial", "Map");
            }
            catch
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
                    TempData["CreateError"] = "儲存時發生問題，請重新整理頁面";
                    return RedirectToAction("Index", "Map");
                }

                List<travellist> travelList = db.travellists.Where(list => list.UserId == newList.UserId).ToList<travellist>();
                Session["TempTid"] = travelList[(travelList.Count) - 1].Tid;
                TempData["CreateList"] = "建立成功 !!";
                return RedirectToAction("TravelListPartial", "Map");
            }
        }

        [HttpPost]
        public ActionResult ChangeListName(string listId, string newName)
        {
            listId = listId.Remove(0, listId.IndexOf("-") + 1);
            travellist renameItem = db.travellists.Find(int.Parse(listId));
            if (renameItem != null)
            {
                renameItem.TName = newName;
                db.Entry(renameItem).State = EntityState.Modified;
                db.SaveChanges();
                Session["TempTid"] = null;
                TempData["CreateList"] = "更名成功 !!";
            }
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
                TempData["CreateList"] = "刪除清單成功 !!";
            }
            return RedirectToAction("TravelListPartial", "Map");
        }

        [HttpPost]
        public ActionResult DeletePlace(string deletePlaceId)
        {
            deletePlaceId = deletePlaceId.Remove(0, deletePlaceId.IndexOf(".") + 1);
            int id = (int)Session["TempTid"];
            travellist currentList = db.travellists.Where(o => o.Tid == id).ToList().First();

            //List<travellistplace> deleteItems = db.travellistplaces.Where(m => m.Tid == (int)Session["TempTid"]).ToList();
            try
            {
                travellistplace deleteItem = currentList.travellistplaces.Where(o => o.Sno == deletePlaceId).Single();
                if (deleteItem != null)
                {
                    db.travellistplaces.Remove(deleteItem);
                    db.SaveChanges();
                    TempData["SaveSuccess"] = "刪除成功 !!";
                }
            }
            catch
            {
                return Json(new { Status = 2, Message = "Error" });
            }
            //

            return RedirectToAction("TravelListPlacePartial", "Map");
        }

        // 切換清單
        [HttpPost]
        public ActionResult OnChangeTravelList(string selectedList)
        {
            //if (selectedList == null)
            //{
            //    Session["TempTid"] = -1;
            //}
            //else
            //{
            //    Session["TempTid"] = Convert.ToInt32(selectedList);
            //} 
            Session["TempTid"] = Convert.ToInt32(selectedList);

            return RedirectToAction("TravelListPlacePartial", "Map");
            //return Json(new { Status = 1, Message = "Success" });
        }

        // 切換清單
        [HttpPost]
        public ActionResult OnChangeSortList(string selectedList, string Tid)
        {
            try
            {
                return RedirectToAction("SortPlace", "Map", new { tid = int.Parse(Tid), sid = int.Parse(selectedList) });
            }
            catch
            {
                return Json(new { Status = 1, Message = "刪除失敗 !" });
            }
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

        private void FindUserIdByName(string userAccount)
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

        public ActionResult SortedHistory(string tid)
        {
            int id = int.Parse(tid);
            List<sortedhistory> list = db.sortedhistories.Where(o => o.Tid == id).ToList<sortedhistory>();
            try
            {
                int uid = (int)Session["UserId"];
                travellist tlist = db.travellists.Where(o => o.Tid == id).Where(o => o.UserId == uid).Single();
                ViewBag.Deletable = 0;
            }
            catch
            {
                ViewBag.Deletable = 1;
            }
            ViewBag.SelectList = list;
            ViewBag.ListCount = list.Count();
            ViewBag.ListName = GenerateListNameByNumber(list.Count());
            return PartialView("Direction/_sortedPlaceHistory");
        }

        private List<string> GenerateListNameByNumber(int count)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string temp = "規劃 ";
                temp += (i + 1).ToString();
                output.Add(temp);
            }
            return output;
        }

        [Authorize]
        public ActionResult SuggestPlacePartial()
        {
            return PartialView("Map/_suggestPlacePartial");
        }

        [Authorize]
        public ActionResult OthersSuggestPlacePartial()
        {
            int userId = (int)Session["UserId"];
            Suggestor suggestor = new Suggestor();
            List<view> suggestions = suggestor.GetOtherSuggestPlaceSno(userId);
            return PartialView("Map/_othersSuggestPlacePartial", suggestions);
        }

        [Authorize]
        public ActionResult MyFavorPartial()
        {
            int userId = (int)Session["UserId"];
            Suggestor suggestor = new Suggestor();
            List<view> suggestions = suggestor.GetMyFavorPlaceSno(userId);
            return PartialView("Map/_myFavorPartial", suggestions);
        }

        public ActionResult HistoryPartial()
        {
            List<History> history = (List<History>)Session["Histories"];
            return PartialView("Map/_historyPartial", history);
        }

        [HttpPost]
        public ActionResult RetrieveHistory(List<History> jsonHistory)
        {
            List<History> output;
            if (jsonHistory != null)
            {
                output = new List<History>();
                foreach (History h in jsonHistory)
                {
                    if (!output.Exists(o => o.gId == h.gId))
                    {
                        output.Add(h);
                    }
                }
            }
            else
            {
                output = null;
            }
            Session["Histories"] = output;
            return RedirectToAction("HistoryPartial");
        }
    }
}
