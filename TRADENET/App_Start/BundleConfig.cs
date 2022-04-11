using System.Web;
using System.Web.Optimization;

namespace TRADENET
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/AdminLTE").Include(
                      "~/AdminLTE/dist/js/adminlte.js"));

            bundles.Add(new ScriptBundle("~/bundles/DatePicker").Include(
                        "~/Scripts/bootstrap-datepicker.js*"));

            bundles.Add(new ScriptBundle("~/bundles/DataTable").Include(
                        "~/AdminLTE/DataTables/datatables.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/SlimScroll").Include(
                        "~/AdminLTE/jquery-slimscroll/jquery.slimscroll.min.js"));

            //Styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/library-tplus.css",
                      "~/Content/bootstrap-datepicker.css"
                      ));

            bundles.Add(new Bundle(
                "~/Content/less",
                new CssMinify()).Include(
                "~/Content/sidebar-mini.less",
                "~/Content/sidebar.less"
                ));

            bundles.Add(new StyleBundle("~/AdminLTE/css").Include(
                      "~/AdminLTE/dist/css/AdminLTE.css",
                      "~/AdminLTE/dist/css/skins/skin-blue.css"
                      ));

            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                      "~/Content/font-awesome.css"
                      ));


            bundles.Add(new StyleBundle("~/Ionicons/css").Include(
                      "~/AdminLTE/Ionicons/css/ionicons.css"
                      ));


            bundles.Add(new StyleBundle("~/DataTables/css").Include(
                "~/AdminLTE/DataTables/datatables.min.css"
                ));
        }
    }
}
