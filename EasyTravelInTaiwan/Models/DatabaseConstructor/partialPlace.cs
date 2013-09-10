using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyTravelInTaiwan.Models
{
    public partial class place
    {
        public place(place copiedPlace)
        {
            this.placeimages = new HashSet<placeimage>();
            Id = copiedPlace.Id;
            Name = copiedPlace.Name;
            Citynumber = copiedPlace.Citynumber;
            Typenumber = copiedPlace.Typenumber;
            Description = copiedPlace.Description;
            Telphone = copiedPlace.Telphone;
            Address = copiedPlace.Address;
            Url = copiedPlace.Url;
            Carmethod = copiedPlace.Carmethod;
            Busmethod = copiedPlace.Busmethod;
        }

        public string ShortDescription
        {
            get { return (Description.Length > 100) ? Description.Substring(0, 100) + "  ..." : Description; }
        }
    }
}