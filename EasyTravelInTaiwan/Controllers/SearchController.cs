using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EasyTravelInTaiwan.Models;
using EasyTravelInTaiwan.Models.Repository;
using BootstrapSupport.HtmlHelpers;
using System.IO;
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
                    if (searchViewModel.searchWord == null) model.GetAllViews();
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

        public FileResult RenderBookImage(string id, string pt)
        {
            byte[] img = ViewImage.GetImageById(db, id, pt);
            byte[] notFound = db.notfoundimages.Where(o => o.NId == 2).Single().Image;

            if (img != null) return File(img, "image/jpeg");
            return File(notFound, "image/jpeg");
        }

        public ActionResult SearchDropdownList()
        {
            SearchViewModel searchViewModel = new SearchViewModel();
            return PartialView("_BasicSearchDropdownListPartial", searchViewModel);
        }
    }
}
