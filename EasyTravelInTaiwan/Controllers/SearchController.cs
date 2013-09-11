using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EasyTravelInTaiwan.Models;
using EasyTravelInTaiwan.Models.Repository;
using BootstrapSupport.HtmlHelpers;
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

        public ActionResult SearchResultPartial(SearchViewModel searchViewModel, int page = 1)
        {
            var pageSize = 15;

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
            ViewBag.key = searchViewModel.searchType;
            ViewBag.FoundNum = model.Count();

            return PartialView("SearchResultPartial", model.ToPagedList(page, pageSize));
        }

        public ActionResult RenderBookImage(string id)
        {
            
            string temp = id.ToString();
            place place = db.places.Where(o => o.Id == id).Single();
            try
            {
                byte[] img = place.placeimages.First().Image;
                return File(img, "image/jpeg");
            }
            catch
            {
                return new FilePathResult("Content/images/ImageNotFound.jpg", "image/jpg");
            }
                
        }
        public ActionResult SearchDropdownList()
        {
            SearchViewModel searchViewModel = new SearchViewModel();
            return PartialView("_BasicSearchDropdownListPartial", searchViewModel);
        }
    }
}
