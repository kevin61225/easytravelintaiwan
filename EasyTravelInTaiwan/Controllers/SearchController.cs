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
        ProjectEntities db = new ProjectEntities();

        public ActionResult SearchPlace()
        {
            var entityList = new SearchModel();
            entityList.AddPlaceSearch();
            return Json(entityList, JsonRequestBehavior.AllowGet);
        }

        //public void SearchResult()                  //測試用
        //{
        //    GenericRepository<place> test = new GenericRepository<place>();
        //    //test.Get(o => o.ProductID == 3).Quantity += 200;
        //    //test.Update(test.Get(o => o.ProductID == 3));
        //}

        public void CartTest(int key = 0)
        {
            ViewBag.searchSelect = key;
        }

        public ActionResult SearchResult(SearchViewModel searchViewModel)
        {
            ViewBag.key = searchViewModel.searchType;
            ViewBag.keyWord = searchViewModel.searchWord;
            return View();
        }

        public ActionResult SearchResultPartial(SearchViewModel searchViewModel)
        {
            SearchResultModel model = new SearchResultModel();
            if (searchViewModel.searchWord == null)
            {
                model.GetAllPlaces();
            }
            else
            {
                switch (searchViewModel.searchType)
                {
                    case 0:
                        model.ByTitle(searchViewModel.searchWord);
                        //model.ByAuthor(searchViewModel.searchWord);
                        //model.ByPublisher(searchViewModel.searchWord);
                        break;
                    //case 1:
                    //    model.ByTitle(searchViewModel.searchWord);
                    //    break;
                    //case 2:
                    //    model.ByAuthor(searchViewModel.searchWord);
                    //    break;
                    //case 3:
                    //    model.ByPublisher(searchViewModel.searchWord);
                    //    break;
                }
            }
            ViewBag.keyWord = searchViewModel.searchWord;
            ViewBag.FoundNum = model.Count();
            return PartialView("SearchResultPartial", model);
        }

        public FileContentResult RenderBookImage(int id)
        {
            string temp = id.ToString();
            try
            {
                byte[] img = db.placeimages.Where(o => o.Id == temp).First().Image;
                return File(img, "image/jpeg");
            }
            catch
            {
                byte[] img = null;
                return File(img, "image/jpeg");
            }
            
        }
        public ActionResult SearchDropdownList()
        {
            SearchViewModel searchViewModel = new SearchViewModel();
            return PartialView("_BasicSearchDropdownListPartial", searchViewModel);
        }
    }
}
