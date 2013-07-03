using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;

namespace EasyTravelInTaiwan.Models
{
    public class AccountModels
    {
        //dbproject1Entities _db = new dbproject1Entities();

        public AccountModels(string account, string password)
        {
        }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "請輸入登入帳號")]
        [Display(Name = "使用者名稱")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "記住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "請輸入登入帳號")]
        [StringLength(20, ErrorMessage = "請勿超過20個字")]
        public string Account { get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        [StringLength(20, ErrorMessage = "請勿超過20個字")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請再輸入一次登入密碼")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "與登入密碼不符")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "請輸入您的姓名")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "請輸入電子郵件位址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入手機號碼")]
        [StringLength(10, ErrorMessage = "請勿超過10個字")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "請選擇性別")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "請輸入住址")]
        public string UserAddress { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "輸入格式錯誤(ex: 2012-09-06)")]
        public System.DateTime Birthday { get; set; }

        public int Role { get; set; }
        public int CustomerType { get; set; }
    }

    public class ForgetPassword
    {
        public int user_id { get; set; }
        public string user_login_name { get; set; }
        public string user_password { get; set; }

        [Required]
        [Display(Name = "Email Address : ")]
        public string user_email_address { get; set; }
    }

}