using System;
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

namespace EasyTravelInTaiwan.Controllers
{
    //[Authorize(Roles="Admin")]
    public class MemberController : Controller
    {
        //private dbproject1Entities db = new dbproject1Entities();
        private projectEntities db = new projectEntities();

        //
        // GET: /Member/

        [Authorize(Roles = "Admin, Clerk, Customer")]
        public ActionResult Index()
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

        [Authorize(Roles = "Admin, Clerk")]
        public ActionResult IndexForManager()
        {
            return View(db.members.ToList());
        }

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
                    return RedirectToAction("Login", "Member");
                }
            }
            TempData["success"] = "登入成功 ! 歡迎使用";

            return RedirectToAction("Index", "Map");
        }

        private bool AutoLogin(LoginModel model)
        {
            member user;
            try
            {
                user = db.members.Where(o => o.Account == model.UserName).Where(o => o.Password == model.Password).Single();
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
            return View();
        }

        // GET: /Member/Login

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Member/Register
        // 創立會員帳號(bootstrap.basic)

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(member member)
        {
            try
            {
                member.Role = 2;
                db.members.Add(member);
                db.SaveChanges();
            }
            catch
            {
                TempData["Error"] = "帳號已存在，請更換";
                return RedirectToAction("Register", "Member");
            }
            SendEmailForRegist(member);
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

        public ActionResult FacebookLogin(string fbID, string fbName, string fbEmail)
        {
            LoginModel loginModel = new LoginModel();

            RegisterForFB(fbID, fbName, fbEmail);
            loginModel.UserName = fbID;
            loginModel.Password = fbID;
            loginModel.RememberMe = false;
            if (!AutoLogin(loginModel))
            {
                return RedirectToAction("Login", "Member");
            }
            TempData["success"] = "登入成功 !!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public bool RegisterForFB(string fbID, string fbName, string fbEmail)
        {
            member fbMember = new member();
            fbMember.Account = fbID;
            fbMember.Password = fbID;
            fbMember.Name = fbName;
            fbMember.Email = fbEmail;
            fbMember.Role = 2;
            fbMember.Birthday = DateTime.Now;
            fbMember.PhoneNumber = "unknown";
            fbMember.Sex = "Male";
            fbMember.UserAddress = "unknown";

            if (ModelState.IsValid)
            {
                try
                {
                    db.members.Add(fbMember);
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }
            }
            return true;
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
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("taipetechbookstore@gmail.com");
                msg.To.Add(user_email_address);
                msg.Subject = "TaipeiTech BookStore - Forget Your Password ?";
                msg.Body = "Hello !! Here is your Password: " + memberList[i].Password + "\n\n by TaipeiTech Bookstore";
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

            msg.From = new MailAddress("taipetechbookstore@gmail.com");
            msg.To.Add(member.Email);
            msg.Subject = "Welcome to TaipeiTech BookStore !!";
            msg.Body = "Hello !! " + member.Name + " ,welcome to TaipeiTech Bookstore !! Enjoy your shopping ~" + "\n\n by TaipeiTech Bookstore";
            msg.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient();

            SmtpServer.Send(msg);

            return RedirectToAction("ForgetPassword", "Member");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}