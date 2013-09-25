using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public partial class member
    {
        public member(RegisterModel copiedMember)
        {
            this.travellists = new HashSet<travellist>();
            this.ratings = new HashSet<rating>();

            UserID = copiedMember.UserID;
            Account = copiedMember.Account;
            Password = copiedMember.Password;
            Name = copiedMember.Name;
            Email = copiedMember.Email;
            PhoneNumber = "none";
            Sex = copiedMember.Sex;
            UserAddress = "none";
            Birthday = copiedMember.Birthday;
            favorite = string.Empty;
            for (int i = 0; i < copiedMember.ViewTypeList.Count; i++)
            {
                if(copiedMember.ViewTypeList[i].isCheck)
                {
                    favorite += copiedMember.ViewTypeList[i].viewtype.Typenumber + "-";
                }
            }
            favorite = favorite.Remove(favorite.Length-1);
            Role = 2;
        }
    }
}