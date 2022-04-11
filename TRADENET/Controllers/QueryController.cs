using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRADENET.Models;

namespace TRADENET.Controllers
{
    public class QueryController : Controller
    {
        // GET: Query
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QueryReport(string pwd)
        {
            string Mgs = "";
            if (pwd != "")
            {
                if (pwd != mfnGetDemoMonthpwd().ToString())
                {
                    Mgs = "Wrong PassWord. Try again!";

                }
                else
                {
                    Session["QueryAccess"] = "Yes";
                    Mgs = "Success";



                }
            }
            else
            {
                Mgs = "Please Enter Pwd!!";
            }
            return Content(Mgs);

        }

        public ActionResult QueryReportView()
        {
            if ((String)Session["QueryAccess"] != "" && Session["QueryAccess"] != null)
            {
                return View();
            }
            else
            { return RedirectToAction("index", "Query"); }

        }
        public ActionResult GetQueryReportResult()
        {

            string query = System.Web.HttpContext.Current.Request["query"];
            string select = System.Web.HttpContext.Current.Request["select"];

            DataTable dt = new DataTable();
            UtilityDBModel mydbutil = new UtilityDBModel();
            LibraryModel mylib = new LibraryModel();
            string strsql = "";
            string msg = "";
            query = query.ToLower();
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString);
            try
            {

                if (select == "Commex")
                {
                    SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
                    SQLConnComex.Open();
                    dt = mylib.OpenDataTable(query, SQLConnComex);
                    SQLConnComex.Close();
                }
                else if (select == "Cross")
                {
                    SqlConnection SQLConn = mydbutil.crostemp_conn("Cross");
                    SQLConn.Open();
                    dt = mylib.OpenDataTable(query, SQLConn);
                    SQLConn.Close();
                }
                else if (select == "Estro")
                {
                    SqlConnection EstroConnection = mydbutil.commexTemp_conn("Estro");
                    EstroConnection.Open();
                    dt = mylib.OpenDataTable(query, EstroConnection);
                    EstroConnection.Close();
                }


                else if (select == "Tradeplus")
                {

                    con.Open();
                    dt = mylib.OpenDataTable(query, con);
                    con.Close();
                }

            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message.ToString();
                //string error = e.ToString();
                //return Content("<script language='javascript' type='text/javascript'>alert(/" + error + "/);</script>");
            }
            return View(dt);
        }
        public object mfnGetDemoMonthpwd()
        {
            object mfnGetDemoMonthpwdRet = "";
            string strHeader, sDate, sNewDate, Date3, Date4;
            string strsMonth, strsday, strsYear;
            string strMonth, strYear;
            int i;
            DateTime strdt;
            strsMonth = DateTime.Today.Month.ToString("00");
            strsYear = DateTime.Today.Year.ToString("00");
            strsday = DateTime.Today.Day.ToString("00");
            strHeader = "";
            sDate = strsday + "/" + strsMonth + "/" + strsYear; // dd/mm/yyyy
            string[] formats = { "dd/MM/yyyy" };
            strdt = DateTime.ParseExact(sDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
            sNewDate = Conversions.ToString(DateAndTime.DateAdd(DateInterval.Month, 0, strdt));
            strMonth = Strings.UCase(Strings.Left(DateAndTime.DateAdd(DateInterval.Month, DateAndTime.Month(Conversions.ToDate(sNewDate)) % 4, strdt).ToString("MMMM", new CultureInfo("en-US")), 3));
            Date3 = Strings.Right(Strings.FormatDateTime(Conversions.ToDate(sDate), 0), 2);
            Date4 = Strings.Left(Strings.FormatDateTime(Conversions.ToDate(sDate), 0), 2);
            if (Strings.InStr(1, Date4, "/") > 0)
            {
                Date4 = 0 + Strings.Mid(Strings.Replace(Strings.FormatDateTime(strdt, DateFormat.ShortDate), "-", "/"), 4, 1);
            }
            else
            {
                Date4 = Strings.Mid(Strings.Replace(Strings.FormatDateTime(strdt, DateFormat.ShortDate), "-", "/"), 4, 2);
            }

            strYear = Strings.Right(Conversions.ToString(DateAndTime.DateAdd("m", Conversions.ToDouble(Date3) * Conversions.ToDouble(Date4), sDate)), 2);
            var loopTo = Strings.Len(strMonth);
            for (i = 1; i <= loopTo; i++)
            {
                strHeader = strHeader + (char)(65 + Strings.Asc(Strings.Mid(strMonth, i, 1)) % 26);
                if (i < 3)
                {
                    strHeader = strHeader + (Strings.Asc(Strings.Mid(strYear, i, 1)) * 77 * i + 7) % 10;
                }
                else
                {
                    strHeader = strHeader + (char)(65 + (Strings.Asc(Strings.Mid(strMonth, i, 1)) + 7) % 26);
                }
            }

            mfnGetDemoMonthpwdRet = Strings.LCase(strHeader);
            return mfnGetDemoMonthpwdRet;
        }
    }
}