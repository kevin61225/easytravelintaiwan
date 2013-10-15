using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyTravelInTaiwan.Models
{
    public class AccountModels
    {
        //dbproject1Entities _db = new dbproject1Entities();

        public AccountModels(string account, string password)
        {
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
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

        [Display(Name = "記住帳號")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "請輸入登入帳號")]
        [StringLength(20, ErrorMessage = "請勿超過20個字")]
        [Display(Name = "帳號名稱")]
        public string Account { get; set; }

        [Required(ErrorMessage = "請輸入登入密碼")]
        [StringLength(20, ErrorMessage = "請勿超過20個字")]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "請再輸入一次登入密碼")]
        [Display(Name = "確認密碼")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "與登入密碼不符")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "請輸入您的姓名")]
        [Display(Name = "使用者名稱")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "電子郵件信箱")]
        [Required(ErrorMessage = "請輸入電子郵件位址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請選擇性別")]
        [Display(Name = "性別")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "請選擇至少三項")]
        [Display(Name = "個人喜好")]
        public List<ViewTypeCheckbox> ViewTypeList { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "輸入格式錯誤(ex: 2012-09-06)")]
        [Display(Name = "出生年月日")]
        public System.DateTime Birthday { get; set; }

        public int Role { get; set; }

        static public int GetCheckedNumber(List<ViewTypeCheckbox> TypeList)
        {
            int output = 0;
            foreach (ViewTypeCheckbox item in TypeList)
            {
                if (item.isCheck) output++;
            }
            return output;
        }
    }

    public class ViewTypeCheckbox 
    {
        public viewtype viewtype { get; set; }
        public bool isCheck { get; set; }
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