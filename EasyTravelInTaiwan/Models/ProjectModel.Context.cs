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
    
    public partial class ProjectEntities1 : DbContext
    {
        public ProjectEntities1()
            : base("name=ProjectEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<accommodation> accommodations { get; set; }
        public DbSet<accomodationimage> accomodationimages { get; set; }
        public DbSet<city> cities { get; set; }
        public DbSet<facebookprofile> facebookprofiles { get; set; }
        public DbSet<favorite> favorites { get; set; }
        public DbSet<food> foods { get; set; }
        public DbSet<foodimage> foodimages { get; set; }
        public DbSet<friend> friends { get; set; }
        public DbSet<hotel> hotels { get; set; }
        public DbSet<hotelimage> hotelimages { get; set; }
        public DbSet<maplatlng> maplatlngs { get; set; }
        public DbSet<member> members { get; set; }
        public DbSet<notfoundimage> notfoundimages { get; set; }
        public DbSet<oneinfodata> oneinfodatas { get; set; }
        public DbSet<place> places { get; set; }
        public DbSet<placeimage> placeimages { get; set; }
        public DbSet<rating> ratings { get; set; }
        public DbSet<sortedhistory> sortedhistories { get; set; }
        public DbSet<travellist> travellists { get; set; }
        public DbSet<travellistplace> travellistplaces { get; set; }
        public DbSet<view> views { get; set; }
        public DbSet<viewtype> viewtypes { get; set; }
    }
}
