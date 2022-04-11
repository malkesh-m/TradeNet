using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using TRADENET.App_Start;

namespace TRADENET
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Application["OnlineUsers"] = 0;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Dev Extream
            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);

            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState =
                System.Web.SessionState.SessionStateBehavior.Required;

            DevExpress.XtraReports.Web.QueryBuilder.Native.QueryBuilderBootstrapper.SessionState =
                System.Web.SessionState.SessionStateBehavior.Required;

            DevExpress.XtraReports.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState =
                System.Web.SessionState.SessionStateBehavior.Required;
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }


        protected void Application_PostAuthorizeRequest()

        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            Application.Lock();
            Application["OnlineUsers"] = (int)Application["OnlineUsers"] + 1;
            Application.UnLock();
            //Session["ApplicationName"] = "TRADENET";
            //HttpContext.Current.Session["CompanyCode"] = "A";
            // your code here, it will be executed upon session start
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate 
            // mode is set to InProc in the Web.config file. 
            // If session mode is set to StateServer or SQLServer, 
            // the event is not raised.
            Application.Lock();
            Application["OnlineUsers"] = (int)Application["OnlineUsers"] - 1;
            Application.UnLock();
        }
        public class MyExceptionHandler : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                //if (filterContext.ExceptionHandled || filterContext.HttpContext.IsCustomErrorEnabled)
                //{
                //    return;
                //}
                Exception e = filterContext.Exception;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new ViewResult()
                {
                    ViewName = "Error"
                };
            }
        }
    }
}
