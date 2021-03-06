﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EasyTravelInTaiwan.Models;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Mail;
using WebMatrix.WebData;
using System.Text;
using BootstrapSupport.HtmlHelpers;
using System.Security.Cryptography;

namespace EasyTravelInTaiwan.Controllers
{
    //[Authorize(Roles="Admin")]
    public class MemberController : Controller
    {
        //private dbproject1Entities db = new dbproject1Entities();
        private ProjectEntities1 db = new ProjectEntities1();

        //
        // GET: /Member/

        [Authorize(Roles = "Admin, Clerk, Customer")]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            member tempMember;
            try
            {
                tempMember = db.members.Where(o => o.Account == User.Identity.Name).Single();
            }
            catch
            {
                tempMember = null;
            }
            if (tempMember == null)
            {
                return HttpNotFound();
            }
            return View(tempMember);
        }

        [Authorize(Roles = "Admin, Clerk")]
        public ActionResult IndexForManager()
        {
            return View(db.members.ToList());
        }

        public ActionResult Home(string User)
        {
            int userId = int.Parse(User);
            if (db.members.Where(o => o.UserID == userId).Single() == null)
            {
                return RedirectToAction("ErrorPage", "Error");
            }
            int isFriend;

            try
            {
                isFriend = SearchFriendsModel.FindIfIsFriend((int)Session["UserId"], userId);
                if (userId == (int)Session["UserId"]) isFriend = 0; // 自己
            }
            catch
            {
                isFriend = 2; // 不顯示
            }

            ViewBag.UserId = User;
            ViewBag.FriendType = isFriend;
            ViewBag.UserName = db.members.Where(o => o.UserID == userId).Single().Name;
            Session["NowUid"] = userId;
            try
            {
                Session["FbId"] = db.members.Where(o => o.UserID == userId).Single().facebookprofiles.Single().FacebookId;
            }
            catch
            {
                Session["FbId"] = "";
            }
            return View();
        }

        public ActionResult PersonalInfo(string User)
        {
            int uid = int.Parse(User);
            List<travellist> filterdInfo = new List<travellist>();
            ViewBag.UserId = User;
            member user = db.members.Where(o => o.UserID == uid).Single();

            // 篩選
            foreach(travellist item in user.travellists.ToList())
            {
                List<travellistplace> places = db.travellistplaces.Where(o => o.Tid == item.Tid).ToList<travellistplace>();
                if (places.Count > 1) filterdInfo.Add(item);
            }

            user.travellists.ToList();
            user.SeperateTags();
            List<viewtype> types = new List<viewtype>();
            foreach (int i in user._tags)
            {
                types.Add(db.viewtypes.Where(o => o.Typenumber == i).Single());
            }
            ViewBag.Favorites = types;
            ViewBag.TravelList = filterdInfo.ToList();
            ViewBag.TravelListCount = filterdInfo.Count();
            return PartialView("PersonalInfo/_personalInfoViewPartial");
        }

        #region Friends Partial

        public ActionResult Friends(string User)
        {
            ViewBag.UserId = User;
            return PartialView("Friends");
        }

        public ActionResult FriendsTreeList(string Uid)
        {
            int uid = int.Parse(Uid);
            SearchFriendsModel model = new SearchFriendsModel();
            model.SearchFriends(uid);
            ViewBag.FriendCount = model.Count();
            model.Clear();
            model.RecommendFriends(uid);
            ViewBag.RecommendFriendCount = model.Count();
            //Session["NowUid"] = Uid;
            return PartialView("Friends/_friendsTreeViewPartial", model);
        }

        public ActionResult FriendsResultPartial(int uId, string type, int page = 1)
        {
            var pageSize = 15;

            SearchFriendsModel model = new SearchFriendsModel();
            if (type == "RecommendFriendsList")
            {
                model.RecommendFriends(uId);
            }
            else
            {
                model.SearchFriends(uId);
            }
            return PartialView("Friends/_friendsResultPartial", model.ToPagedList(page, pageSize));
        }

        [HttpPost]
        public ActionResult AddFriend(string uId, int friendId)
        {
            if (uId == string.Empty)
            {
                return Json(new { Status = 2, Message = "請先登入會員 !!" }, JsonRequestBehavior.AllowGet);
            }
            int userId = int.Parse(uId);
            if (userId == friendId)
            {
                return Json(new { Status = 3, Message = "自己" }, JsonRequestBehavior.AllowGet);
            }
            SearchFriendsModel.AddFriend(userId, friendId);
            return Json(new { Status = 1, Message = "已加為好友 !!" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Favorite Partial

        public ActionResult Favorite(string User)
        {
            ViewBag.UserId = User;
            return PartialView("Favorite");
        }

        public ActionResult FavoriteTreeList(string Uid)
        {
            int uid = int.Parse(Uid);
            SearchFavoriteModel model = new SearchFavoriteModel(uid);
            return PartialView("Favorite/_favoriteTreeViewPartial", model);
        }

        public ActionResult FavoriteResultPartial(int type, string city, int uId, int page = 1)
        {
            var pageSize = 15;

            SearchResultModel model = new SearchResultModel();

            model.GetPersonalFavorite(type, city, uId);

            ViewBag.City = city;
            ViewBag.Type = type;
            ViewBag.FoundNum = model.Count();

            try
            {
                if (uId == (int)Session["UserId"]) ViewBag.Deletable = 0;
            }
            catch
            {
                ViewBag.Deletable = 1;
            }

            return PartialView("Favorite/_favoriteResultPartial", model.ToPagedList(page, pageSize));
        }

        [HttpPost]
        public ActionResult DeleteFavoritePlace(string UserId, string PlaceId, string city, string type)
        {
            int uid = int.Parse(UserId);
            favorite temp = db.favorites.Where(o => o.UserId == uid).Where(o => o.PlaceId == PlaceId).Single();
            db.favorites.Remove(temp);
            db.SaveChanges();
            return RedirectToAction("FavoriteResultPartial", "Member", new { type = type, city = city, uId = uid, page = 1 });
            //return Json(new { Status = "1", Messages = "Success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LoginModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // if login fail then redirect to sign in page
                if (!AutoLogin(loginModel))
                {
                    return RedirectToAction("Login", "Member", new { returnUrl = returnUrl });
                }
            }
            TempData["success"] = "登入成功 ! 歡迎使用";

            return RedirectToLocal(returnUrl);

            //if (ModelState.IsValid && WebSecurity.Login(loginModel.UserName, loginModel.Password, persistCookie: loginModel.RememberMe))
            //{
            //    return RedirectToLocal(returnUrl);
            //}

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            //ModelState.AddModelError("", "所提供的使用者名稱或密碼不正確。");
            //return RedirectToAction("Index", "Map");
        }

        private bool AutoLogin(LoginModel model)
        {
            member user;
            string decodedPw = Encrypt(model.Password, true);
            try
            {
                user = db.members.Where(o => o.Account == model.UserName).Where(o => o.Password == decodedPw).Single();
            }
            catch
            {
                TempData["Error"] = "您輸入的帳號不存在或者密碼錯誤!";
                return false;
            }

            // 登入時清空所有 Session 資料
            Session.RemoveAll();
            string userData = CheckRole(user.Role);

            string strUsername = user.Account;
            FormsAuthentication.SetAuthCookie(strUsername, model.RememberMe, FormsAuthentication.FormsCookiePath);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
              strUsername,
              DateTime.Now,
              DateTime.Now.AddMinutes(30),
              model.RememberMe,
              userData,
              FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            //Session["UserName"] = user.Name;
            //Session["UserId"] = user.UserID;

            return true;
        }

        private string CheckRole(int p)
        {
            string role;
            switch (p)
            {
                case 0:
                    role = "Admin";
                    break;
                case 1:
                    role = "Clerk";
                    break;
                case 2:
                    role = "Customer";
                    break;
                default:
                    role = "Error";
                    break;
            }
            return role;
        }

        //
        // GET: /Member/Details/5

        public ActionResult Details(int id = 0)
        {
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // GET: /Member/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: /Member/
        // 創立會員帳號(創立頁面)

        [HttpPost]
        public ActionResult Create(member member)
        {
            if (ModelState.IsValid)
            {
                member.Role = 2;
                member.Password = Encrypt(member.Password, true);
                db.members.Add(member);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    TempData["Error"] = "帳號已存在，請更換";
                    return RedirectToAction("Register", "Member");
                }

                return RedirectToAction("Index", "Home");
            }

            return View(member);
        }

        // GET: /Member/Register

        public ActionResult Register()
        {
            List<viewtype> types = db.viewtypes.ToList();
            List<ViewTypeCheckbox> list = new List<ViewTypeCheckbox>();
            RegisterModel model = new RegisterModel();
            for (int i = 0; i < types.Count; i++)
            {
                ViewTypeCheckbox temp = new ViewTypeCheckbox();
                temp.viewtype = types[i];
                list.Add(temp);
            }
            model.ViewTypeList = list;
            return View(model);
        }

        // GET: /Member/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Member/Register
        // 創立會員帳號(bootstrap.basic)

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterModel member)
        {
            //return RedirectToAction("Login", "Member");
            if (RegisterModel.GetCheckedNumber(member.ViewTypeList) < 3)
            {
                TempData["Error"] = "請選擇至少三項的個人喜好 !!";
                return RedirectToAction("Register", "Member");
            }
            member registMember = new member(member);
            partialUser kernel = new partialUser();
            try
            {
                registMember.Password = Encrypt(member.Password, true);
                registMember.UserID = db.members.Count()+1;
                db.members.Add(registMember);
                db.SaveChanges();
            }
            catch
            {
                TempData["Error"] = "帳號已存在，請更換";
                return RedirectToAction("Register", "Member");
            }
            kernel.LoadUserData();
            //kernel.RunSeparate();
            kernel.SetGroupIdBySingle(registMember.Account);
            SendEmailForRegist(registMember);
            TempData["success"] = "註冊成功，已寄信至您的電子郵件信箱。並請您重新登入 !! ";
            return RedirectToAction("Login", "Member");
        }

        //
        // GET: /Member/Edit/5

        public ActionResult Edit(int id = 0)
        {
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        [Authorize(Roles = "Admin, Clerk, Customer")]
        public ActionResult EditAccount()
        {
            member tempMember;
            try
            {
                tempMember = db.members.Where(o => o.Account == User.Identity.Name).Single();
            }
            catch
            {
                tempMember = null;
            }

            if (tempMember == null)
            {
                return HttpNotFound();
            }
            return View(tempMember);
        }

        [Authorize(Roles = "Admin, Clerk, Customer")]
        [HttpPost]
        public ActionResult EditAccount(member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "修改成功 !!";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Error"] = "更新錯誤";
                return RedirectToAction("Index");
            }
            return View(member);
        }

        //
        // POST: /Member/Edit/5

        [HttpPost]
        public ActionResult Edit(member member)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Error"] = "更新錯誤";
                return RedirectToAction("index");
            }
            return View(member);
        }

        //
        // GET: /Member/Delete/5

        public ActionResult Delete(int id = 0)
        {
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // POST: /Member/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                member member = db.members.Find(id);
                db.members.Remove(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["error"] = "刪除錯誤";
                return RedirectToAction("index");
            }
        }

        #region Facebook Login

        public ActionResult FacebookRegister()
        {
            List<viewtype> types = db.viewtypes.ToList();
            List<ViewTypeCheckbox> list = new List<ViewTypeCheckbox>();
            RegisterModel model = new RegisterModel();
            for (int i = 0; i < types.Count; i++)
            {
                ViewTypeCheckbox temp = new ViewTypeCheckbox();
                temp.viewtype = types[i];
                list.Add(temp);
            }
            model.ViewTypeList = list;
            ViewBag.FbID = (string)TempData["FBId"];
            return View(model);
        }

        [HttpPost]
        public ActionResult FacebookRegist(RegisterModel member, string fbID, string returnUrl)
        {
            if (RegisterModel.GetCheckedNumber(member.ViewTypeList) < 3)
            {
                TempData["Error"] = "請選擇至少三項的個人喜好 !!";
                return RedirectToAction("FacebookRegister", "Member", new { fbID = fbID });
            }
            member registMember = new member(member);

            // 將 favorite 匯入 fbmember
            member fbmember = db.members.Where(o => o.Account == fbID).Single();
            fbmember.favorite = registMember.favorite;
            db.Entry(fbmember).State = EntityState.Modified;
            db.SaveChanges();

            partialUser kernel = new partialUser();

            kernel.LoadUserData();
            //kernel.RunSeparate();
            kernel.SetGroupIdBySingle(fbmember.Account);
            SendEmailForRegist(fbmember);

            LoginModel loginModel = new LoginModel();
            loginModel.UserName = fbID;
            loginModel.Password = fbID;
            loginModel.RememberMe = false;

            if (!AutoLogin(loginModel))
            {
                return RedirectToAction("Login", "Member");
            }
            Session["FBUser"] = fbID;
            //return RedirectToAction("Index", "Index");
            return RedirectToLocal(returnUrl);
        }

        public ActionResult FacebookLogin(string fbID, string fbName, string fbEmail, string sex, string returnUrl)
        {
            LoginModel loginModel = new LoginModel();
            if (!CheckFacebookAccountExist(fbID))
            {
                RegisterForFB(fbID, fbName, fbEmail, sex);
                TempData["FBId"] = fbID;
                return RedirectToAction("FacebookRegister", "Member");          
            }
            if (!CheckHasFavorite(fbID))
            {
                TempData["FBId"] = fbID;
                return RedirectToAction("FacebookRegister", "Member");     
            }

            loginModel.UserName = fbID;
            loginModel.Password = fbID;
            loginModel.RememberMe = false;

            if (!AutoLogin(loginModel))
            {
                return RedirectToAction("Login", "Member");
            }

            Session["FBUser"] = fbID;
            TempData["success"] = "登入成功 !!";
            //return RedirectToAction("Index", "Index");
            return RedirectToLocal(returnUrl);
        }

        public bool CheckHasFavorite(string fbID)
        {
            try
            {
                member temp = db.members.Where(o => o.Account == fbID).Single();
                if (temp.favorite == string.Empty) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool CheckFacebookAccountExist(string fbID)
        {
            try
            {
                facebookprofile temp = db.facebookprofiles.Where(o => o.FacebookId == fbID).Single();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void RegisterForFB(string fbID, string fbName, string fbEmail, string sex)
        {
            member fbMember = new member();
            fbMember.Account = fbID;
            fbMember.Password = Encrypt(fbID, true); ;
            fbMember.Name = fbName;
            fbMember.Email = fbEmail;
            fbMember.Role = 2;
            fbMember.favorite = string.Empty;
            fbMember.Birthday = DateTime.Now;
            fbMember.PhoneNumber = "unknown";
            if(sex == "male")
            {
                fbMember.Sex = "Male";
            }
            else if(sex == "female")
            {
                fbMember.Sex = "Female";
            }
            fbMember.UserAddress = "unknown";

            db.members.Add(fbMember);
            db.SaveChanges();

            member addedMember = db.members.Where(o => o.Account == fbID).Single();
            facebookprofile fbpro = new facebookprofile();
            fbpro.FacebookId = fbID;
            fbpro.UserId = addedMember.UserID;
            db.facebookprofiles.Add(fbpro);
            db.SaveChanges();
        }

        #endregion

        [ChildActionOnly]
        public ActionResult HomeSignInPartial()
        {
            return PartialView("HomeSignInPartial");
        }

        [ChildActionOnly]
        public ActionResult HomeSignUpPartial()
        {
            return PartialView("HomeSignUpPartial");
        }

        #region ForgetPassword

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string user_email_address)
        {
            List<member> memberList = new List<member>();
            try
            {
                memberList = db.members.Where(o => o.Email == user_email_address).ToList();
            }
            catch
            {
                TempData["Error"] = "查無此會員，請重試 !!";
                return RedirectToAction("ForgetPassword", "Member");
            }

            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            for (int i = 0; i < memberList.Count; i++)
            {
                string pw = memberList[i].Password;
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("taipetechbookstore@gmail.com");
                msg.To.Add(user_email_address);
                msg.Subject = "TaipeiTech BookStore - Forget Your Password ?";
                msg.Body = "Hello !! Here is your Password: " + Decrypt(pw, true) + "\n\n by TaipeiTech Bookstore";
                msg.Priority = MailPriority.High;

                //SmtpClient client = new SmtpClient();
                SmtpServer.Send(msg);
            }

            TempData["success"] = "請求已送出，請至您的電子信箱收件 !!";
            return RedirectToAction("ForgetPassword", "Member");
        }

        #endregion

        [HttpPost]
        public ActionResult SendEmailForRegist(member member)
        {
            MailMessage msg = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            msg.From = new MailAddress("easytravelintaiwan@gmail.com");
            msg.To.Add(member.Email);
            msg.Subject = "Welcome to Easy Travel In Taiwan !!";
            msg.Body = "Hello !! " + member.Name + " ,welcome to Easy Travel In Taiwan !! Enjoy ~" + "\n\n by Easy Travel In Taiwan";
            msg.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient();

            SmtpServer.Send(msg);

            return RedirectToAction("ForgetPassword", "Member");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Index");
            }
        }

        public static string Encrypt(string toEncrypt, bool useHashing)
        {

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "MySeCrEtKeY";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = " MySeCrEtKeY";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}