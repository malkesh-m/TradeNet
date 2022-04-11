using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRADENET.Models;

namespace TRADENET.Views.Payout
{
    public class PayoutController : Controller
    {
        // GET: Payout

        public ActionResult Bulkpayout()
        {
            ViewBag.cmbFormatView = false;
            ViewBag.cmbFormat = 0;
            //ViewBag.cmbFormat = "false";
            LibraryModel myLib = new LibraryModel();
            DataTable dtcheck = new DataTable();
            string strsql;
            DataTable dtcheckRMS = new DataTable();
            strsql = "select Tnc_Filler1 from tradenetcontrol where Tnc_optcode='946'";
            dtcheckRMS = myLib.OpenDataTable(strsql);
            if (dtcheckRMS.Rows.Count > 0)
            {
                if (dtcheckRMS.Rows[0]["Tnc_Filler1"].ToString() == "R")
                {
                    ViewBag.cmbFormatView = true;                   
                    Session["RMS"] = "RMS";
                }
            }
            if (Information.IsNothing(Session["946"]) == false)
            {
                string POCriteria = null;               
                string[] TdRight1 = Strings.Split(Session["946"].ToString(), ",");
                POCriteria = Strings.Trim(TdRight1[1]);
                if (POCriteria == "A")
                {
                }
                else if (POCriteria == "D")
                {
                    ViewBag.cmbFormat = 0;
                    ViewBag.cmbFormatView = true;
                }
                else if (POCriteria == "C")
                {
                    ViewBag.cmbFormat = 1;
                    ViewBag.cmbFormatView = true;
                }
                else
                {
                }
            }



            string str = "";
            string time = "";
            string[] arr;

            string TdRight = "";
            str = myLib.fnGetSysParam("FPOUT_RQ_TM");
            //str = "08:30,15.00";
            if (str != "")
            {
                arr = str.Split(',');


                strsql = "select substring(convert(char,convert(datetime,GETDATE()),108),1,5) as Times";
                dtcheck = myLib.OpenDataTable(strsql);
                if (dtcheck.Rows.Count > 0)
                {
                    time = dtcheck.Rows[0]["Times"].ToString().Replace(",", ":");
                }

                int t1 = Int32.Parse(arr[0].ToString().Replace(":", "").Replace(".", ""));
                int t2 = Int32.Parse(arr[1].ToString().Replace(":", "").Replace(".", ""));
                int t3 = Int32.Parse(time.ToString().Replace(":", ""));

                if (t1 > t3 || t2 < t3)
                {
                    ViewBag.chkTime = "Payout Request can be made between " + arr[0] + " and " + arr[1] + " hours";
                    //this.ClientScript.RegisterStartupScript(GetType(), "Msg", "<script language=\"javascript\">alert('Payout Request can be made between " + arr[0] + " and " + arr[1] + " hours')</script>;");
                }
                else
                {
                    ViewBag.chkTime = "";
                    if (Convert.ToInt32(myLib.fnFireQuery("Entity_master", "count(0)", "em_bclearingno = '6283' Or em_nclearingno", "13264", true)) > 0)
                    {
                        ViewBag.chk = "true";

                    }

                }
            }


            //   ViewBag.chkTime = "";
            return View();

        }

        public ActionResult BulkPayoutReport(string Code, string select, string date, string chkDeductOthrDeb, string cmbFormat, string clickby, Boolean chkInclOnDemPayOut)
        {
            if (clickby == "Normal")
            { Session["ClientCode"] = null; }
            if (clickby == "Save")
            {
                //Code = Session["ClientCode"].ToString();
                //select = "CL";
                Session["ClientCode"] = null;
            }

            string gstrDBToday = DateTime.Today.ToString("yyyyMMdd");
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            PayOut account = new PayOut();
            DataTable dt = null;
            //date = "20200518";
            if (date == "")
            {
                ViewBag.Msg = "As On Date can't left blank!";
            }
            else
            {

                dt = account.GetBulkPayoutNew(Code, select, date, chkDeductOthrDeb, cmbFormat, chkInclOnDemPayOut);
            }

            return View(dt);


        }

        public ActionResult BulkPayoutRequestRMS(string Code, string select, string date, string chkDeductOthrDeb, string cmbFormat, string RMSAmount, Boolean chkInclOnDemPayOut)
        {
            ViewBag.RMSAmount = RMSAmount;
            LibraryModel myLib = new LibraryModel();

            ViewBag.ClientRequest = myLib.fnFireQuery("FundsRequest", "isnull(CAST(sum(Rq_Amount) as decimal(15,2)),0) Rq_Amount", "Rq_Satus1='P' and Rq_Clientcd", Code, true);


            DataTable dt = null;
            PayOut account = new PayOut();
            IEnumerable<DataTable> ulist = account.GetBulkPayoutRequest(Code, select, date, chkDeductOthrDeb, cmbFormat, chkInclOnDemPayOut);
            return View(ulist);

        }

        public ActionResult BulkPayoutRequestRMSSubmit()
        {

            return View();

        }


        //Bulk Payout Save
        public ActionResult SaveBulkPayoutRequest(IEnumerable<BulkPayoutRequestModel> BulkPayout)
        {
            PayOut SaveBP = new PayOut();
            SaveBP.SaveBulkPayout(BulkPayout);
            return Json(new { success = true, responseText = "Record Saved Successfully.." }, JsonRequestBehavior.AllowGet);
        }
        //Bulk Payout Save
        public ActionResult SaveBulkFullPayoutRequest(string strDt)
        {
            PayOut SaveFullBP = new PayOut();
            SaveFullBP.SaveBulkFullPR(strDt);
            return Json(new { success = true, responseText = "Record Saved Successfully.." }, JsonRequestBehavior.AllowGet);
        }
        //Bulk Payout Save
        public ActionResult RemoveAllBulkPayoutRequest()
        {
            PayOut RemoveBP = new PayOut();
            RemoveBP.RemoveAllBulkPR();
            return Json(new { success = true, responseText = "Request Remove Successfully.." }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveBulkCheckPayoutRequest(string strDt, Boolean chkAll, IEnumerable<string> ClientCode)
        {
            PayOut SaveBP = new PayOut();
            SaveBP.SaveBulkCheckPR(strDt, chkAll, ClientCode);
            return Json(new { success = true, responseText = "Record Saved Successfully.." }, JsonRequestBehavior.AllowGet);
        }
    }
}