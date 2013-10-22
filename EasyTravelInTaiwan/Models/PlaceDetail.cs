using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public class PlaceDetail
    {
        public virtual string Id { get; set; }
        public virtual string Description { get; set; }
        public virtual string Name { get; set; }
        public virtual string Citynumber { get; set; }
        public virtual string CityName { get; set; }
        public virtual int Typenumber { get; set; }
        public virtual string TypeName { get; set; }
        public virtual string Telphone { get; set; }
        public virtual string Address { get; set; }
        public virtual string Url { get; set; }
        public virtual string Carmethod { get; set; }
        public virtual string Busmethod { get; set; }
        public virtual string pt { get; set; }
        public virtual ICollection<placeimage> placeimages { get; set; }

        public virtual string FaxNumber { get; set; }
        public virtual Nullable<int> Rooms { get; set; }
        public virtual string Price { get; set; }
        public virtual string Email { get; set; }
        public virtual Nullable<int> star { get; set; }
        public virtual string Lat { get; set; }
        public virtual string Lng { get; set; }

        public virtual ICollection<hotelimage> hotelimages { get; set; }

        public virtual string type { get; set; }
        public virtual string title { get; set; }
        public virtual string catalogs_id { get; set; }
        public virtual string phone { get; set; }
        public virtual string business_hrs { get; set; }
        public virtual string price { get; set; }
        public virtual string route { get; set; }
        public virtual string tianmama { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual string sc_id { get; set; }
        public virtual string post_id { get; set; }
        public virtual string link { get; set; }
        public virtual Nullable<double> x { get; set; }
        public virtual Nullable<double> y { get; set; }
        public virtual string create_date { get; set; }
        public virtual string modify_date { get; set; }

        public virtual ICollection<ViewImage> viewimages { get; set; }

        public void CheckEmptyData()
        {
            string noInfo = "尚無資料";
            if (Description == string.Empty) Description = noInfo;
            if (Telphone == null) Telphone = noInfo;
            if (Address == null) Address = noInfo;
            if (Url == null) Url = noInfo;
            if (Carmethod == null) Carmethod = noInfo;
            if (Busmethod == null) Busmethod = noInfo;
        }
    }

    public class ViewDetail : PlaceDetail
    {
        public ViewDetail(place view, string pt)
        {
            Id = view.Id;
            Name = view.Name;
            Citynumber = view.Citynumber;
            CityName = SearchResultModel.GetCityNameById(Citynumber);
            Typenumber = view.Typenumber;
            TypeName = SearchResultModel.GetViewTypeNameById(Typenumber);
            Description = view.Description;
            Telphone = view.Telphone;
            Address = view.Address;
            Url = view.Url;
            Carmethod = view.Carmethod;
            Busmethod = view.Busmethod;

            this.viewimages = ViewImage.TransformToViewCollection(view.placeimages, pt);
        }
    }

    public class HotelDetail : PlaceDetail
    {
        public HotelDetail(hotel hotel, string pt)
        {
            Id = hotel.Id;
            Name = hotel.Name;
            Citynumber = hotel.Citynumber;
            CityName = SearchResultModel.GetCityNameById(Citynumber);
            Typenumber = hotel.Typenumber;
            TypeName = SearchResultModel.GetViewTypeNameById(Typenumber);
            Description = hotel.Description;
            Address = hotel.Address;
            Url = hotel.Url;
            Telphone = hotel.Telphone;
            FaxNumber = hotel.FaxNumber;
            Rooms = hotel.Rooms;
            Price = hotel.Price;
            Email = hotel.Email;
            star = hotel.star;
            Lat = hotel.Lat;
            Lng = hotel.Lng;

            this.viewimages = ViewImage.TransformToViewCollection(hotel.hotelimages, pt);
        }
    }

    public class AccommodationDetail : PlaceDetail
    {
        public AccommodationDetail(accommodation acco, string pt, int typeId)
        {
            Id = acco.id;
            type = acco.type;
            Name = acco.title;
            TypeName = SearchResultModel.GetViewTypeNameById(typeId);
            catalogs_id = acco.catalogs_id;
            Address = acco.address;
            Telphone = acco.phone;
            business_hrs = acco.business_hrs;
            price = acco.price;
            Description = acco.Description;
            route = acco.route;
            ImageUrl = acco.ImageUrl;
            sc_id = acco.sc_id;
            Citynumber = sc_id;
            CityName = SearchResultModel.GetCityNameById(sc_id);
            post_id = acco.post_id;
            link = acco.link;
            x = acco.x;
            y = acco.y;
            create_date = acco.create_date;
            modify_date = acco.modify_date;

            ICollection<accomodationimage> accimages = new HashSet<accomodationimage>();
            using (var db = new ProjectEntities())
            {
                accimages.Add(db.accomodationimages.Where(o => o.Id == acco.id).Single());
            }

            this.viewimages = ViewImage.TransformToViewCollection(accimages, pt);
        }
    }

    public class FoodDetail : PlaceDetail
    {
        public FoodDetail(food food, string pt, int typeId)
        {
            Id = food.id;
            type = food.type;
            Name = food.title;
            TypeName = SearchResultModel.GetViewTypeNameById(typeId);
            catalogs_id = food.catalogs_id;
            Address = food.address;
            Telphone = food.phone;
            business_hrs = food.business_hrs;
            price = food.price;
            Description = food.Description;
            route = food.route;
            tianmama = food.tianmama;
            ImageUrl = food.ImageUrl;
            sc_id = food.sc_id;
            Citynumber = sc_id;
            CityName = SearchResultModel.GetCityNameById(sc_id);
            post_id = food.post_id;
            link = food.link;
            x = food.x;
            y = food.y;
            create_date = food.create_date;
            modify_date = food.modify_date;

            ICollection<foodimage> foodimages = new HashSet<foodimage>();
            using (var db = new ProjectEntities())
            {
                foodimages.Add(db.foodimages.Where(o => o.Id == food.id).Single());
            }

            this.viewimages = ViewImage.TransformToViewCollection(foodimages, pt);
        }
    }
}