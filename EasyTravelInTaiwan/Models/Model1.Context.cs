﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbproject1Entities : DbContext
    {
        public dbproject1Entities()
            : base("name=dbproject1Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<author> authors { get; set; }
        public DbSet<book> books { get; set; }
        public DbSet<book_author> book_author { get; set; }
        public DbSet<member> members { get; set; }
        public DbSet<order_item> order_item { get; set; }
        public DbSet<order_list> order_list { get; set; }
        public DbSet<product> products { get; set; }
        public DbSet<publisher> publishers { get; set; }
        public DbSet<rating> ratings { get; set; }
        public DbSet<shoppingcart> shoppingcarts { get; set; }
        public DbSet<stock> stocks { get; set; }
    }
}
