using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using System.IO;
using TRADENET.DAL;

namespace TRADENET
{
    public class CustomReportStorageWebExtension : ReportStorageWebExtension
    {
        public override bool CanSetData(string url)
        {
            // Check if the URL is available in the report storage.
            using (var session = SessionFactory.Create())
            {
                return session.GetObjectByKey<ReportEntity>(url) != null;
            }
        }


        public override byte[] GetData(string url)
        {

            //object NIVEL;
            //object DESCRIPCION;

            // Get the report data from the storage.
            using (var session = SessionFactory.Create())
            {
                var reportEntity = session.GetObjectByKey<ReportEntity>(url);

                // This fragment is just to ensure the parameters are arriving well up to here
                //NIVEL = HttpContext.Current.Session["_LEVEL"];
                //DESCRIPCION = HttpContext.Current.Session["_DESCRIPTION"];

                XtraReport report = new XtraReport();
                MemoryStream ms = new MemoryStream(reportEntity.Layout);
                report.LoadLayoutFromXml(ms);
                report.Parameters["CMSCHEDULE"].Value = HttpContext.Current.Session["CMSCHEDULE"];
                report.Parameters["LoginAccessOld"].Value = HttpContext.Current.Session["LoginAccessOld"];
                report.SaveLayoutToXml(ms);
                reportEntity.Layout = ms.ToArray();
                session.CommitChanges();

                return reportEntity.Layout;
            }
        }


        public override Dictionary<string, string> GetUrls()
        {
            // Get URLs and display names for all reports available in the storage
            using (var session = SessionFactory.Create())
            {
                return session.Query<ReportEntity>().ToDictionary<ReportEntity, string, string>(report => report.Url, report => report.Url);
            }
        }


        public override bool IsValidUrl(string url)
        {
            // Check if the specified URL is valid for the current report storage.
            // In this example, a URL should be a string containing a numeric value that is used as a data row primary key.
            return true;
        }


        public override void SetData(XtraReport report, string url)
        {
            // Write a report to the storage under the specified URL.
            using (var session = SessionFactory.Create())
            {
                var reportEntity = session.GetObjectByKey<ReportEntity>(url);

                MemoryStream ms = new MemoryStream();
                report.SaveLayoutToXml(ms);
                reportEntity.Layout = ms.ToArray();

                session.CommitChanges();
            }
        }


        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            // Save a report to the storage under a new URL. 
            // The defaultUrl parameter contains the report display name specified by a user.
            if (CanSetData(defaultUrl))
                SetData(report, defaultUrl);
            else
                using (var session = SessionFactory.Create())
                {
                    MemoryStream ms = new MemoryStream();
                    report.SaveLayoutToXml(ms);

                    var reportEntity = new ReportEntity(session)
                    {
                        Url = defaultUrl,
                        Layout = ms.ToArray()
                    };

                    session.CommitChanges();
                }
            return defaultUrl;
        }
    }
}