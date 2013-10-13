using EasyTravelInTaiwan.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
            double averageRate;
            int userRate;

            try
            {
                averageRate = db.ratings.Where(o => o.Sno == sno).Average(o => o.Point);
            }
            catch
            {
                averageRate = -1;
            }

            try
            {
                int uid = (int)Session["UserId"];
                userRate = db.ratings.Where(o => o.Sno == sno).Where(o => o.UserId == uid).Single().Point;
            }
            catch
            {
                userRate = -1;
            }

            ViewBag.RateAverage = averageRate;
            ViewBag.UserRate = userRate;
            Session["Sno"] = sno;

            return PartialView("_placeRatingPartial");
        }

        [HttpPost]
        public ActionResult PostRating(int rate, string pt, string sno)
        {
            int uid;
            string _sno = sno;
            string _pt = pt;

            try
            {
                uid = (int)Session["UserId"];
                rating temp = new rating();

                try
                {
                    // 已經評分過，想要更動評分
                    temp = db.ratings.Where(o => o.UserId == uid).Where(o => o.Sno == _sno).Single();
                    temp.Point = rate;
                    db.Entry(temp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    // 尚未評分過
                    temp.Sno = _sno;
                    temp.Point = rate;
                    temp.UserId = uid;
                    temp.pt = _pt;
                    temp.Comment = string.Empty;
                    try
                    {
                        db.ratings.Add(temp);
                        db.SaveChanges();
                    }
                    catch
                    {
                        TempData["Error"] = "儲存錯誤";
                        return Json(new { Status = 3, Message = "Saving Error in " + temp.RId, Sno = _sno });
                    }
                }
                TempData["SaveSuccess"] = "儲存成功 !!";
                return RedirectToAction("PlaceRatePartial", "Rating", new { sno = _sno });
            }
            catch
            {
                return Json(new { Status = 2, Message = "請先登入才能進行評分 !!", Sno = _sno });
            }
        }
    }
}
