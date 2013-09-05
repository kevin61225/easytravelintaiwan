using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EasyTravelInTaiwan.Models;
using EasyTravelInTaiwan.Models.Repository;
//using EasyTravelInTaiwan.Service;

namespace EasyTravelInTaiwan.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult SearchPlace()
        {
            var entityList = new SearchModel();
            entityList.AddPlaceSearch();
            return Json(entityList, JsonRequestBehavior.AllowGet);
        }

        public void SearchResult()                  //測試用
        {
            GenericRepository<place> test = new GenericRepository<place>();
            //test.Get(o => o.ProductID == 3).Quantity += 200;
            //test.Update(test.Get(o => o.ProductID == 3));
        }

        public void CartTest(int key = 0)
        {
            ViewBag.searchSelect = key;
        }

        public ActionResult SearchDropdownList()
        {
            SearchViewModel searchViewModel = new SearchViewModel();
            return PartialView("_BasicSearchDropdownListPartial", searchViewModel);
        }
    }
}
