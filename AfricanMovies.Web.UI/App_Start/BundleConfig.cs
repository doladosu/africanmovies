using System.Web.Optimization;
using System.Web.Optimization.React;

namespace AfricanMovies.Web.UI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libs/jquery-{version}.js",
                        "~/Scripts/libs/jquery-ui-{version}.js",
                        "~/Scripts/libs/bootstrap.js",
                        "~/Scripts/libs/react-0.12.0.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").IncludeDirectory("~/Content/themes/base", "*.css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"
                ));

            bundles.Add(new JsxBundle("~/bundles/main").Include(
                "~/Scripts/components/filters.jsx",
                "~/Scripts/components/videosList.jsx",
                "~/Scripts/components/videosController.js"
                ));
        }
    }
}