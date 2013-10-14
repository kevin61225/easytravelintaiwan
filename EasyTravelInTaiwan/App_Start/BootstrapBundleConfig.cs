using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace BootstrapSupport
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js").Include(
                "~/scripts/jquery-{version}.js",
                "~/scripts/jquery-migrate-{version}.js",
                "~/scripts/bootstrap.js",
                "~/scripts/bootstrap-select.js",
                "~/scripts/jquery.validate.js",
                "~/scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js",
                "~/Scripts/jquery.nicescroll.js",
                "~/scripts/jquery.cookie.js",
                "~/Scripts/InitializeComponent.js",
                //
                "~/scripts/jquery-sortable.js",
                "~/scripts/bootstrap-typeahead.js",
                "~/scripts/jquery.rating.js",
                "~/scripts/jquery.blockUI.js"
                ));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-select.css",
                "~/Content/body.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/bootstrap-mvc-validation.css",
                "~/Content/nicescroll.css"
                ));
        }
    }
}