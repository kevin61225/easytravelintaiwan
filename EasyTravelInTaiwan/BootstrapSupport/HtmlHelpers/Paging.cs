using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

/*
<!-- Usage in razor (note @model): -->
@using BootstrapSupport
@model IPagedList

@Html.Pager(Model.PageIndex,
            Model.TotalPages,
            x => Url.Action("Index", new {page = x}),
            " pagination-right")

// Index action on the HomeController from the sample project:
public ActionResult Index(int page = 1)
{
    var pageSize = 3;
    var homeInputModels = _models;
    return View(homeInputModels.ToPagedList(page, pageSize));
}
*/

namespace BootstrapSupport.HtmlHelpers
{
    public static class PaginiationHelperExtensions
    {
        /// <summary>
        /// Renders a bootstrap standard pagination bar
        /// </summary>
        /// <remarks>
        /// http://twitter.github.com/bootstrap/components.html#pagination
        /// </remarks>
        /// <param name="helper">The html helper</param>
        /// <param name="currentPage">Zero-based page number of the page on which the pagination bar should be rendered</param>
        /// <param name="totalPages">The total number of pages</param>
        /// <param name="pageUrl">
        ///     Expression to construct page url (e.g.: x => Url.Action("Index", new {page = x}))
        /// </param>
        /// <param name="additionalPagerCssClass">Additional classes for the navigation div (e.g. "pagination-right pagination-mini")</param>
        /// <returns></returns>
        public static MvcHtmlString Pager(this HtmlHelper helper,
            int currentPage, int totalPages,
            Func<int, string> pageUrl,
            string additionalPagerCssClass = "")
        {
            if (totalPages <= 1)
                return MvcHtmlString.Empty;

            int maxPagesCount = 7;
            int pageStart = 1;
            int pageEnd = totalPages;
            int halfPagerSize = maxPagesCount / 2;
            int cp = currentPage + 1;
            if (totalPages > maxPagesCount)  // 大於7頁
            {
                // page 在全頁數的前三個
                if (cp <= halfPagerSize)
                {
                    pageEnd = maxPagesCount;
                }
                // page 在全頁數的後三個
                else if (cp >= totalPages - halfPagerSize)
                {
                    pageStart = totalPages - maxPagesCount + 1;
                }
                // page 在全頁數的中間部分
                else
                {
                    pageStart = cp - halfPagerSize;
                    pageEnd = cp + halfPagerSize;
                }
            }

            var div = new TagBuilder("div");
            div.AddCssClass("pagination");
            div.AddCssClass(additionalPagerCssClass);

            var ul = new TagBuilder("ul");

            //var firstLi = new TagBuilder("li");
            //var innerSpan = new TagBuilder("span");
            //var innerA = new TagBuilder("a");

            //var iconFirst = new TagBuilder("i");
            //iconFirst.AddCssClass("icon-backward");

            //firstLi.InnerHtml += iconFirst;
            //ul.InnerHtml += firstLi;

            //var previous = new TagBuilder("li");
            //ul.InnerHtml += previous;

            for (var i = pageStart; i < pageEnd + 1; i++)
            {
                var li = new TagBuilder("li");
                if (i == (currentPage + 1))
                    li.AddCssClass("active");

                var a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(i));
                a.SetInnerText(i.ToString());

                li.InnerHtml = a.ToString();

                ul.InnerHtml += li;
            }

            var next = new TagBuilder("li");
            ul.InnerHtml += next;

            var last = new TagBuilder("li");
            ul.InnerHtml += last;

            div.InnerHtml = ul.ToString();

            return MvcHtmlString.Create(div.ToString());
        }
    }

    public interface IPagedList : IEnumerable
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public interface IPagedList<T> : IPagedList, IList<T>
    {
    }

    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize) :
            this(source.GetPage(pageIndex, pageSize), pageIndex, pageSize, source.Count()) { }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            this.TotalCount = totalCount;
            this.TotalPages = totalCount / pageSize;

            if (totalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;

            this.AddRange(source.ToList());
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage { get { return (PageIndex > 0); } }
        public bool HasNextPage { get { return (PageIndex + 1 < TotalPages); } }
    }

    public static class PagingExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> query, int page, int pageSize)
        {
            return new PagedList<T>(query, page - 1, pageSize);
        }

        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        // You can create your own paging extension that delegates to your
        // persistence layer such as NHibernate or Entity Framework.
        // This is an example how an `IPagedList<T>` can be created from 
        // an `IRavenQueryable<T>`:        
        /*
        public static IPagedList<T> ToPagedList<T>(this IRavenQueryable<T> query, int page, int pageSize)
        {
            RavenQueryStatistics stats;
            var q2 = query.Statistics(out stats)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var list = new PagedList<T>(
                            q2,
                            page - 1,
                            pageSize,
                            stats.TotalResults
                        );
            return list;
        }
        */
    }
}