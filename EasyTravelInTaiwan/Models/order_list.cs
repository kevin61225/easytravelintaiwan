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
    
    public partial class order_list
    {
        public order_list()
        {
            this.order_item = new HashSet<order_item>();
        }
    
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string Address { get; set; }
        public int Payment { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }
    
        public virtual member member { get; set; }
        public virtual ICollection<order_item> order_item { get; set; }
    }
}
