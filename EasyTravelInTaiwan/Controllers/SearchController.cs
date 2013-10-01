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

        public ActionResult SearchPlace(int type)
        {
            var entityList = new SearchModel();
            
            switch (type)
            {
                case 0:
                    // 搜全部
                    entityList.AddPlaceSearch();
                    break;
                case 1:
                    entityList.AddFoodSearch();
                    break;
                case 2:
                    entityList.AddViewSearch();
                    break;
                case 3:
                    entityList.AddHotelSearch();
                    break;
            }
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

                switch (searchViewModel.searchType)
                {
                    case 0:
                        if (searchViewModel.searchWord == null) model.GetAllPlaces(); 
                        model.ByTitle(searchViewModel.searchWord);
                        //model.ByAuthor(searchViewModel.searchWord);
                        //model.ByPublisher(searchViewModel.searchWord);
                        break;
                    case 1:
                        if (searchViewModel.searchWord == null) model.GetAllFoods(); 
                        model.ByFood(searchViewModel.searchWord);
                        break;
                    case 2:
                        if (searchViewModel.searchWord == null)  model.GetAllViews();                      
                        model.ByView(searchViewModel.searchWord);
                        break;
                    case 3:
                        if (searchViewModel.searchWord == null) model.GetAllHotels();
                        model.ByHotel(searchViewModel.searchWord);
                        break;
                }

            ViewBag.keyWord = searchViewModel.searchWord;
            ViewBag.key = searchViewModel.searchType;
            ViewBag.Filters = SearchResultModel.FilterType(searchViewModel.searchType);
            ViewBag.FoundNum = model.Count();

            return PartialView("SearchResultPartial", model.ToPagedList(page, pageSize));
        }

        public ActionResult RenderBookImage(string id)
        {
            
            string temp = id.ToString();
            
            try
            {
                place place = db.places.Where(o => o.Id == id).Single();
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
