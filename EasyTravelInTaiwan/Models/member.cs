//------------------------------------------------------------------------------
// <auto-generated>
//    這個程式碼是由範本產生。
//
//    對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//    如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EasyTravelInTaiwan.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class member
    {
        public member()
        {
            this.ratings = new HashSet<rating>();
            this.travellists = new HashSet<travellist>();
            this.member1 = new HashSet<member>();
            this.members = new HashSet<member>();
        }
    
        public int UserID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Sex { get; set; }
        public string UserAddress { get; set; }
        public System.DateTime Birthday { get; set; }
        public int Role { get; set; }
        public string favorite { get; set; }
        public int GId { get; set; }
    
        public virtual ICollection<rating> ratings { get; set; }
        public virtual ICollection<travellist> travellists { get; set; }
        public virtual ICollection<member> member1 { get; set; }
        public virtual ICollection<member> members { get; set; }
    }
}
