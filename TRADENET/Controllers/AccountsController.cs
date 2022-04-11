using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRADENET.Models;

namespace TRADENET.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public void ClearUnwantedSessions()

        {
            //Clear All Sessions that are not required
            Session["RiskManagement"] = null;
        }

        //Layout Menu
        //#region Menu

        public ActionResult CReceiptsRpt()
        {
            return View();
        }

        public ActionResult OutstandingAgeWise()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UtilityDBModel util = new UtilityDBModel();
                string strGetT2BillDate = util.GetT2BillDate();
                ViewBag.T2BillDate = strGetT2BillDate;
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
            //return View();
        }


        //#Endregion Menu


        public ActionResult GetOutstandingAgeWise(string strClTo, string strClFrom, string cmbGroupBy, string strDpId, string strDays, string strAsOnDate, string strAllCompany, string cmbSelect, string cmbOrderBy, string cmbMarginAcBal)
        {
            Accounts account = new Accounts();
            DataTable dt = new DataTable();

            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            dt = account.GetOutstandingAgeWise(strClTo, strClFrom, cmbGroupBy, strDpId, strDays, strAsOnDate, strAllCompany, cmbSelect, cmbOrderBy, cmbMarginAcBal);
            return View(dt);
        }
        public ActionResult GetOutstandingAgeWiseReport(string strCode, string strname, string strAsOnDate, string FinYear)
        {
            FinYear = null;

            if (DateTime.Today.Month > 3)
            {

                FinYear = "01/04/" + DateTime.Today.Year;
            }

            else
            {
                FinYear = "01/04/" + (DateTime.Today.Year - 1);
            }
            ViewBag.strHead = "Ledger from " + FinYear + " To " + strAsOnDate + "";
            ViewBag.strCode = strCode;
            ViewBag.strname = strname;

            Accounts account = new Accounts();
            DataTable dt = new DataTable();
            dt = account.GetOutstandingAgeWiseReport(strCode, strname, strAsOnDate, FinYear);
            return View(dt);
        }

        public ActionResult CReceiptsReport(string Code, string strDpId, string Frmdt, string Todt, string cmbSelect, string ChkAll, string cmbBank, string cmbChoices, string newcombo1)
        {
            Accounts account = new Accounts();
            DataTable dt = new DataTable();

            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }
            dt = account.GetCReceiptsReport(Code, strDpId, Frmdt, Todt, cmbSelect, ChkAll, cmbBank, cmbChoices, newcombo1);
            return View(dt);
        }
        public JsonResult GetBankName(string strDpId)
        {
            //  IEnumerable<JsonComboModel> ulist = cm.GetCompanyExchSeg(criteria);
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            DataTable dt = new DataTable();
            string strSql = "";

            string IsTplusCommex;
            if ((string)Session["IsTplusCommex"] == "Y")
            {
                IsTplusCommex = "Y";
            }
            else
            {
                IsTplusCommex = "N";
            }

            SqlConnection ObjCommexCon = null;
            // SqlConnection con = null;
            if ((Strings.Mid(strDpId, 3, 1) ?? "") == "X")
            {

                ObjCommexCon = mydbutil.commexTemp_conn("Commex");

                if (ObjCommexCon!= null)
                {
                    if (ObjCommexCon.State == ConnectionState.Closed)
                    {
                        ObjCommexCon.Open();

                    }
                }
            }
            else
            {
                //con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TradenetDefaultConnectionString"].ConnectionString);

                //if (con.State == ConnectionState.Closed)
                //{
                //    con.Open();
                //}

            }

            if ((Strings.Mid(strDpId, 3, 1) ?? "") == "X")
            {
                if (IsTplusCommex == "N")
                {
                    //strDpId = strDpId.Substring(1);

                    strSql = myutil.getBankSql(strDpId);


                    dt = myLib.OpenDataTable(strSql);
                }
                else
                {
                    // strDpId = strDpId.Substring(1);

                    strSql = myutil.getBankSqlCommex(strDpId);
                    dt = myLib.OpenDataTable(strSql, ObjCommexCon);
                }
            }
            else
            {

                strSql = myutil.getBankSql(strDpId);
                dt = myLib.OpenDataTable(strSql);
            }
            //if (dt.Rows.Count < 0)
            //{
            //    dt.Rows[0]["cm_name"] = "NA";
            //    dt.Rows[0]["cm_cd"] = "NA";

            //}
            // 
            List<CreceiptBankNameModel> Bank = new List<CreceiptBankNameModel>();

            Bank = dt.AsEnumerable()
          .Select(row => new CreceiptBankNameModel
          {
              BankName = row.Field<string>("cm_name"),
              BankCode = row.Field<string>("cm_cd"),

          }).ToList();

            return Json(new SelectList(Bank, "BankCode", "BankName"));
        }


        public ActionResult ConvertDate()
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string StrQTRCOMPWISEView = "";
            string StrQTRCOMPWISEClient = "";
            string strprev90day = "";
            string strprev180day = "";
            string sqoffdate = "";

            sqoffdate = "2020812";

            strprev90day = myutil.dtos(myutil.AddDayDT(sqoffdate, -80).ToString("dd/MM/yyyy"));

            strprev180day = myutil.dtos(myutil.AddDayDT(strprev90day, -80).ToString("dd/MM/yyyy"));

            DateTime date1 = myutil.AddDayDT("2020812", -80);
            DateTime date2 = myutil.ConvertDT("2020812");

            ViewBag.Date1 = strprev180day;
            ViewBag.Date2 = strprev90day;
            ViewBag.Date3 = date1;
            ViewBag.Date4 = date2;
            return View();
        }


        public ActionResult AOutstandingBal()
         {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            
            ViewBag.dateFrom = myutil.stod(mylib.mfnGetT2Dt("", DateTime.Now.ToString("dd\\/MM\\/yyyy"))).ToString("dd\\/MM\\/yyyy");
           // ViewBag.dateFrom = myutil.stod(myutil.mfnGetT2Dt("", DateTime.Now.ToString("dd/MM/yyyy"))).ToString("dd/MM/yyyy").Trim();
            return View();
        }

        public ActionResult AOutstandingBalReport(string strClient,string dateFrom, string cmbRep, string CmbSelection, string cmbExchSeg, string cmbOrderBy, string cmbAmt, string cmbOutstanding, string cmbActType, string chkMarginact, string txtOutfrom, string txtOutTo)
        {
            ViewBag.condition = cmbRep;
            Accounts account = new Accounts();
            DataTable dt = new DataTable();
            

            dt = account.AOutstandingBalReport(strClient, dateFrom, cmbRep, CmbSelection, cmbExchSeg, cmbOrderBy, cmbAmt, cmbOutstanding, cmbActType, chkMarginact, txtOutfrom, txtOutTo);
            return View(dt);
        }
    }
}