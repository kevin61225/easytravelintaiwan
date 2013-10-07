using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EasyTravelInTaiwan.Models;
using Models;
using WebMatrix.WebData;

namespace BootstrapMvcSample.Controllers
{
    public class HomeController : BootstrapBaseController
    {
        //dbproject1Entities db = new dbproject1Entities();
        private ProjectEntities db = new ProjectEntities();
        private static List<HomeInputModel> _models = ModelIntializer.CreateHomeInputModels();
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
            var homeInputModels = _models;                                      
            return View(homeInputModels);
        }

        // Membership
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            //WebSecurity.Logout();
            TempData["success"] = "已登出";
            Session.Clear();
            return RedirectToAction("Index", "Index");
        }

        public ActionResult LogIn(string account, string password)
        {
            if (account != "123" || password != "123")
            {
                TempData["Error"] = "您輸入的帳號不存在或者密碼錯誤!";
                return RedirectToAction("SignIn", "ExampleLayouts");
            }

            // 登入時清空所有 Session 資料
            Session.RemoveAll();

            string userData = "ApplicationSpecific data for this user";

            string strUsername = "123";
            FormsAuthentication.SetAuthCookie(strUsername, false, FormsAuthentication.FormsCookiePath);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
              strUsername,
              DateTime.Now,
              DateTime.Now.AddMinutes(30), true,
              userData,
              FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return RedirectToAction("Index", "Home");
        }

        private string FindRoleIdByName(System.Security.Principal.IPrincipal User)
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

        [HttpPost]
        public ActionResult Create(HomeInputModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _models.Count==0?1:_models.Select(x => x.Id).Max() + 1;
                _models.Add(model);
                Success("Your information was saved!");
                return RedirectToAction("Index");
            }
            Error("there were some errors in your form.");
            return View(model);
        }

        public ActionResult Create()
        {
            return View(new HomeInputModel());
        }

        public ActionResult Delete(int id)
        {
            _models.Remove(_models.Get(id));
            Information("Your widget was deleted");
            if(_models.Count==0)
            {
                Attention("You have deleted all the models! Create a new one to continue the demo.");
            }
            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var model = _models.Get(id);
            return View("Create", model);
        }
        [HttpPost]        
        public ActionResult Edit(HomeInputModel model,int id)
        {
            if(ModelState.IsValid)
            {
                _models.Remove(_models.Get(id));
                model.Id = id;
                _models.Add(model);
                Success("The model was updated!");
                return RedirectToAction("index");
            }
            return View("Create", model);
        }

		public ActionResult Details(int id)
        {
            var model = _models.Get(id);
            return View(model);
        }

    }
}
