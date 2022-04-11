using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRADENET.Models;

namespace TRADENET.Controllers
{
    public class ReportController : Controller
    {
        public void ClearUnwantedSessions()
        {
            //Clear All Sessions that are not required
            Session["RiskManagement"] = null;
        }

        //Layout Menu
        //#region Menu
        public ActionResult CombineMarginAnalysis()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }


        }

        public ActionResult DematReport()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult deliverypending()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult holding()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult PerformanceReport()
        {
            ClearUnwantedSessions();
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login1");
            }
        }

        public ActionResult RequestForReport(string cmbReport, string Code, string FDate, string ToDate, string cmbYear, string chkCash, string chkFO, string chkCurrency, string cmbYearAsOn, string chkPeriod, string chkYear)
        {
            ClearUnwantedSessions();
            if (Code != null)
            {
                LibraryModel myLib = new LibraryModel();
                UtilityModel myutil = new UtilityModel();
                UtilityDBModel mydbutil = new UtilityDBModel();

                string StrLstSeg = string.Empty;

                string StrFromDt = string.Empty;
                string StrToDt = string.Empty;
                if (chkPeriod == "1")
                {
                    StrFromDt = FDate;
                    StrToDt = ToDate;
                }

                if (chkYear == "1")
                {
                    StrFromDt = Strings.Left(cmbYear, 5);
                    StrFromDt = StrFromDt + Strings.Right(cmbYear, 2);
                    StrToDt = "";
                }


                if (myLib.fnFireQuery("Client_master", "cm_email", "cm_cd", Code.Trim(), true) == "")
                {

                    return Content("Can't proceed, email address is blank.");

                }

                if (cmbReport == "DP Holding" | cmbReport == "Retained Holding")
                {
                    StrFromDt = "";
                    StrToDt = myutil.dtos("31/03/" + cmbYearAsOn);
                    if (Convert.ToInt32(myLib.fnFireQuery("dematact", "count(*)", "da_clientcd", Code.Trim() + "' and  da_status = 'A", true)) == 0)
                    {
                        return Content("Active Demat a/c not found in Client Master.");

                    }
                }

                if (!(myLib.fnFireQuery("Requests", "count(*)", "Rq_Clientcd", Code.Trim() + "' and Rq_Type='" + cmbReport + "' and Rq_DateFrom='" + StrFromDt + "' and Rq_DateTo='" + StrToDt + "' and Rq_Satus1= 'P", true) == "0"))
                {
                    return Content("Request Already Exists...");

                }

                if (cmbReport == "Profit & Loss")
                {
                    if (chkCash == "0" && chkFO == "0" && chkCurrency == "0")
                    {
                        return Content("Select atleast one segment");

                    }

                    if (chkCash == "1")
                    {
                        StrLstSeg = StrLstSeg + "C,";
                    }

                    if (chkFO == "1")
                    {
                        StrLstSeg = StrLstSeg + "F,";
                    }

                    if (chkCurrency == "1")
                    {
                        StrLstSeg = StrLstSeg + "K,";
                    }
                }

                bool blnIdentityOn = false;
                DataTable dt = new DataTable();
                dt = myLib.OpenDataTable("SELECT isnull (IDENT_CURRENT('Requests'),0)");
               
                var intcnt = default(long);
                if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                {
                    blnIdentityOn = true;
                }
                else
                {
                    blnIdentityOn = false;
                    dt = myLib.OpenDataTable("SELECT isNull(Max(Rq_srNo),0) from Requests");

                    intcnt = Convert.ToInt32(dt.Rows[0][0]) + 1;
                }
               
                string todatdate = DateTime.Now.ToString("yyyyMMdd");
                string strsql = "";
                strsql = "insert into Requests values ( " + Interaction.IIf(blnIdentityOn, "", intcnt + ",");
                strsql = strsql + " '" + Code.Trim() + "','" + cmbReport + "','" + StrFromDt + "','" + StrToDt + "','" + System.Environment.GetEnvironmentVariable("COMPUTERNAME") + "',";
                strsql = strsql + " '" + todatdate + "',";
                strsql = strsql + " convert(char(8),getdate(),108),";
                strsql = strsql + " 'P','P','P','" + myutil.Encrypt(Strings.Trim(todatdate)) + "','',";
                strsql = strsql + " '" + StrLstSeg + "')";
                myLib.ExecSQL(strsql);

                if (blnIdentityOn)
                {
                    dt = myLib.OpenDataTable("SELECT isNull(Max(Rq_srNo),0) from Requests");

                    intcnt = Convert.ToInt32(dt.Rows[0][0]);
                }


                return Content("Request has been registered \r\n Email will be forwarded to registered Email ID of Client. \r\n\r\n Reference ID :" + intcnt + "");
            }
            else
            { return View(); }


        }

        public ActionResult InvplGainLoss()
        {
            ClearUnwantedSessions();
            return View();
        }

        public ActionResult INVPLDividend()
        {
            ClearUnwantedSessions();
            return View();
        }

        public ActionResult Invpltradelisting()
        {
            ClearUnwantedSessions();
            return View();
        }



        //#Endregion Menu




        public ActionResult PerformanceReportDetail(string Select, string Code, string FromDate, string ToDate, string strDPID = "", string cmbRep = "", string cmbGroupBy = "")
        {
            ViewBag.cmbRep = cmbRep;
            ViewBag.cmbGroupBy = cmbGroupBy;

            if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            {
                return RedirectToAction("LogoutPage", "Tplus");
            }

            Report account = new Report();
            DataTable ulist = account.GetPerformance(Select, Code, FromDate, ToDate, strDPID, cmbRep, cmbGroupBy);
            return View(ulist);
        }

        // GET: PerformanceReport

        public ActionResult deliverypendingReport(string Code, string strDpid, string cmbSelect, string cmbReport, string strdate, string cmbBS)
        {

            ViewBag.cmbReport = cmbReport;
            Report pending = new Report();
            DataTable ulist = pending.GetDeliverypending(Code, strDpid, cmbSelect, cmbReport, strdate, cmbBS);
            return View(ulist);
        }

        //public ActionResult CombineMarginAnalysisReport(string Select, string Code, string strdate, string grouping, string chkOnlyShortFall)
        //{

        //    UtilityDBModel mydbutil = new UtilityDBModel();
        //    LibraryModel mylib = new LibraryModel();

        //    string StrValidConn = mydbutil.CheckConnection("ESIGN-TRADEPLUS");
        //    string StrActiveConn = mylib.fnFireQuery("other_products", "Count(0)", "op_status='A' AND op_product", "ESIGN-TRADEPLUS", true);

        //    if (StrValidConn.Trim() == "1" || StrActiveConn.Trim() == "1")
        //    {
        //        ViewBag.strdate = strdate;
        //        Report Report = new Report();
        //        DataTable ulist = Report.GetCMAReport(Select, Code, strdate, grouping, chkOnlyShortFall);
        //        return View(ulist);
        //    }
        //    else
        //    {
        //        ViewBag.message = "unable to connect esign database";
        //        return View();
        //    }


        //}

        //public ActionResult CombineMarginAnalysisDetail(string strcd, string strdate)
        //{

        //    ViewBag.code = strcd;
        //    Report Report = new Report();
        //    DataTable ulist = Report.GetCMADetail(strcd, strdate);
        //    return View(ulist);
        //}

        public ActionResult CombineMarginAnalysisReport(string Select, string Code, string strdate, string grouping, string chkOnlyShortFall)
        {

            UtilityDBModel mydbutil = new UtilityDBModel();
            LibraryModel mylib = new LibraryModel();
            ViewBag.strdate = strdate;
            Report Report = new Report();
            DataTable ulist = Report.GetCMAReport(Select, Code, strdate, grouping, chkOnlyShortFall);
            return View(ulist);
        }

        public ActionResult CombineMarginAnalysisDetail(string strcd, string strdate)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            ViewBag.code = strcd;
            strdate = myutil.dtos(strdate);
            string strPeakC = "Peak Margin To Be Collected @ " + mylib.fnPeakFactor(strdate) + "%";
            ViewBag.strCollected = strPeakC;
            Report Report = new Report();
            DataSet ulist = Report.GetCMADetail(strcd, strdate);
            return View(ulist);
        }


        [HttpGet]
        public FileResult CombineDownload(string strClient, string strdate)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            SqlConnection connection;

            connection = mydbutil.EsignConnectionString("ESIGN-TRADEPLUS");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var ds = new DataTable();
            ds = (DataTable)Session["Data"];

            //string strFile = Server.MapPath("..") + @"\TempFolder\" + FDate + ToDate + Session.SessionID + Session["gstrUsercd"] + ".PDF";
            //  string strClient = string.Empty;

            //  strClient = ds.Rows[0]["dd_clientcd"].ToString();

            DataTable dsFilename;

            strdate = myutil.dtos(strdate);

            string strsql = "";
            strsql = " select dd_filename,dd_document from  digital_details ";
            strsql += " Where dd_dt ='" + strdate + "'";
            strsql += " and dd_filetype='CMRG' and dd_clientcd ='" + strClient + "'";
            strsql += " Order by dd_Dt ";

            dsFilename = myLib.OpenDataTable(strsql, connection);


            byte[] filecontent;
            string filename = "Margin_Statement" + ".PDF";

            if (dsFilename.Rows.Count == 1)
            {
                filecontent = (byte[])dsFilename.Rows[0]["dd_document"];
            }
            else
            {
                List<byte[]> pdfFileList = new List<byte[]>();
                for (int intCnt = 0; intCnt < dsFilename.Rows.Count; intCnt++)
                {
                    pdfFileList.Add((byte[])dsFilename.Rows[intCnt]["dd_document"]);
                }
                filecontent = MergePDFFromDataBase(pdfFileList);
            }

            return File(filecontent, "application/pdf", filename);

        }

        public static byte[] MergePDFFromDataBase(List<byte[]> pdfByteContent)
        {

            using (var ms = new MemoryStream())
            {
                var outputDocument = new Document();
                var writer = new PdfCopy(outputDocument, ms);
                outputDocument.Open();

                foreach (var doc in pdfByteContent)
                {
                    var reader = new PdfReader(doc);
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        writer.AddPage(writer.GetImportedPage(reader, i));
                    }
                    writer.FreeReader(reader);
                    reader.Close();
                }

                writer.Close();
                outputDocument.Close();
                var allPagesContent = ms.GetBuffer();
                ms.Flush();

                return allPagesContent;
            }
        }


        //public ActionResult holdingReport(string cmbSelect, string strClient, string ChkExchange, string Chkdeposit, string chkInStNR, string CheckAllCompany, string chkDetails, string chkDPBalances, string chkShowLedger, string DT, string cmbActType, string ledgerBalDate, string ChkBranchWise, string chkExecuted)
        //{
        //    ViewBag.Chkdeposit = Chkdeposit;
        //    ViewBag.chkInStNR = chkInStNR;
        //    ViewBag.chkDetails = chkDetails;
        //    ViewBag.chkShowLedger = chkShowLedger;
        //    ViewBag.ledgerBalDate = ledgerBalDate;
        //    ViewBag.ChkBranchWise = ChkBranchWise;
        //    ViewBag.chkDPBalances = chkDPBalances;
        //    Report holding = new Report();
        //    DataTable ulist = holding.Getholding(cmbSelect, strClient, ChkExchange, Chkdeposit, chkInStNR, CheckAllCompany, chkDetails, chkDPBalances, chkShowLedger, DT, cmbActType, ledgerBalDate, ChkBranchWise, chkExecuted);
        //    return View(ulist);
        //}

        public ActionResult holdingReport(string cmbSelect, string strClient, string ChkExchange, string Chkdeposit, string chkInStNR, string CheckAllCompany, string chkDetails, string chkDPBalances, string chkShowLedger, string DT, string cmbActType, string ledgerBalDate, string ChkBranchWise, string chkExecuted)
        {
            ViewBag.Chkdeposit = Chkdeposit;
            ViewBag.chkExecuted = chkExecuted;
            ViewBag.chkInStNR = chkInStNR;
            ViewBag.chkDetails = chkDetails;
            ViewBag.chkShowLedger = chkShowLedger;
            ViewBag.ledgerBalDate = ledgerBalDate;
            ViewBag.ChkBranchWise = ChkBranchWise;
            ViewBag.chkDPBalances = chkDPBalances;
            Report holding = new Report();

            DataTable ulist = holding.Getholding(cmbSelect, strClient, ChkExchange, Chkdeposit, chkInStNR, CheckAllCompany, chkDetails, chkDPBalances, chkShowLedger, DT, cmbActType, ledgerBalDate, ChkBranchWise, chkExecuted);
            return View(ulist);
        }

        public ActionResult HoldingPledgeReq(string lasttd, string tmp_scripcd, string tmp_ssname, string tmp_bcqty, string tmp_Code1)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();

            ViewBag.tmp_Code1 = tmp_Code1;

            string[] array = tmp_Code1.Split('/');


            ViewBag.lblClNm = array[0].Trim();
            ViewBag.lblScripcd = array[1].Trim();
            ViewBag.Scrip = array[2].Trim() + " [" + array[1].Trim() + "]";
            ViewBag.tmp_ssname = tmp_ssname;
            ViewBag.lblMaxQty = array[3].Trim();
            ViewBag.lblclcd = array[4].Trim();

            string PlgQty = mylib.fnFireQuery("PledgeRequest", "isNull((case when Rq_Qty > 0 then convert(varchar,Rq_Qty) else '' end),'')", " Rq_Status1='X' and Rq_Status3='" + array[5].Trim() + "' and Rq_Scripcd = '" + array[1].Trim() + "' and Rq_Clientcd", array[4].Trim(), true);
            ViewBag.PlgQty = PlgQty;

            string caption = "";
            if (array[5].Trim() == "R")
            {
                caption = "Un-Re-Pledge";
            }
            else
            { caption = "Un-Pledge"; }

            ViewBag.lblimgcaption = caption;

            return View();
        }


        public ActionResult SaveHoldingPledgeReq(string InputPlgQty, string SaveValue)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            string[] array = SaveValue.Split('/');


            ViewBag.lblClNm = array[0].Trim();
            ViewBag.lblScripcd = array[1].Trim();
            ViewBag.Scrip = array[2].Trim() + " [" + array[1].Trim() + "]";
            ViewBag.lblMaxQty = array[3].Trim();
            ViewBag.lblclcd = array[4].Trim();


            mylib.ExecSQL("delete from PledgeRequest where Rq_Clientcd='" + array[4].Trim() + "' and Rq_Scripcd = '" + array[1].Trim() + "' and Rq_Status1='X' and Rq_Status3 ='" + array[5].Trim() + "'");

            bool blnIdentityOn;
            blnIdentityOn = false;

            DataTable dt = new DataTable();
            dt = mylib.OpenDataTable("SELECT isnull (IDENT_CURRENT('PledgeRequest'),0)");

            int intcnt = 0;
            if (dt.Rows.Count > 0)
            {
                blnIdentityOn = true;
                DataTable dtReqId = new DataTable();
                dtReqId = mylib.OpenDataTable("SELECT IDENT_CURRENT('PledgeRequest')");
                intcnt = Convert.ToInt32(dtReqId.Rows[0][0]);
            }

            string strsql = "";
            if (blnIdentityOn)
            {
                strsql = "insert into PledgeRequest values ( ";
            }
            else
            {
                strsql = "insert into PledgeRequest values ( " + intcnt + ",";
            }

            strsql += " '" + array[4].Trim() + "','','" + array[1].Trim() + "','" + Conversion.Val(InputPlgQty.Trim()) + "','" + Convert.ToString(Environment.MachineName).Trim() + "',";
            strsql += " '" + DateTime.Now.ToString("yyyyMMdd") + "',";
            strsql += " convert(char(8),getdate(),108),";
            strsql += " 'X','B','" + array[5].Trim() + "','" + myutil.Encrypt(DateTime.Now.ToString("yyyyMMdd")) + "','')";
            mylib.ExecSQL(strsql);


            return Content("Records Save Successfully");

        }


        public ActionResult InvplGainLossReport(string cmbReport, string Code, string FDate, string ToDate, string AsonDate, string CBJobbing, string CBDelivery, string CBIgnoreEffect, string CBDetail)
        {
            ViewBag.CBJobbing = CBJobbing;
            ViewBag.CBDelivery = CBDelivery;
            ViewBag.CBDetail = CBDetail;
            ViewBag.CBIgnoreEffect = CBIgnoreEffect;
            ViewBag.cmbReport = cmbReport;

            Session["cmbReport"] = cmbReport;
            Session["CBJobbing"] = CBJobbing;
            Session["CBDelivery"] = CBDelivery;
            Session["CBDetail"] = CBDetail;
            Session["CBIgnoreEffect"] = CBIgnoreEffect;
            LibraryModel myLib = new LibraryModel();
            ViewBag.blnIncSttDel = Convert.ToBoolean((myLib.fnGetSysParam("GAINLOSSTTDL") == "Y") ? true : false);
            ViewBag.blnIncSttTrd = Convert.ToBoolean((myLib.fnGetSysParam("GAINLOSSTTTR") == "Y") ? true : false);
            Report Report = new Report();
            DataTable ulist = Report.GetInvplGainLossDetail(cmbReport, Code, FDate, ToDate, AsonDate, CBJobbing, CBDelivery, CBIgnoreEffect, CBDetail);
            return View(ulist);

        }

        public ActionResult INVPLGainlossDetails(string strclientcd, string strfrdt, string strtodt, string strscripcd, string strscripname, string strclientname, string strIgnoresection)
        {
            ViewBag.cmbReport = Session["cmbReport"];
            ViewBag.CBJobbing = Session["CBJobbing"];
            ViewBag.CBDelivery = Session["CBDelivery"];
            ViewBag.CBDetail = Session["CBDetail"];
            ViewBag.CBIgnoreEffect = Session["CBIgnoreEffect"];

            ViewBag.strclientname = strclientname + "[" + strclientcd + "]";
            ViewBag.strscripname = strscripname + "[" + strscripcd + "]";
            string strtype = "";
            string strignorsection = (Session["CBIgnoreEffect"].ToString() == "1") ? "Y" : "N";

            if (Session["CBDelivery"].ToString() == "1" && Session["CBJobbing"].ToString() == "1")
            {
                strtype = "B";
            }
            else if (Session["CBDelivery"].ToString() == "1")
            {
                strtype = "D";
            }
            else if (Session["CBJobbing"].ToString() == "1")
            {
                strtype = "T";
            }

            Report Report = new Report();
            DataTable ulist = Report.GetInvplGainLossPopupDetail(strtype, strclientcd, strfrdt, strtodt, strscripcd, strscripname, strclientname, strignorsection);
            return View(ulist);

        }


        public ActionResult InvplGainLossReportPopUp(string strscripcd)
        {
            Report Report = new Report();
            DataTable ulist = Report.GetInvplGainLossPopUp(strscripcd);
            return View(ulist);

        }

        public ActionResult INVPLDividendReport(string Code, string FDate, string ToDate)
        {
            Report Report = new Report();
            DataTable ulist = Report.GetINVPLDividendReport(Code, FDate, ToDate);
            return View(ulist);

        }

        public ActionResult InvpltradelistingReport(string Code, string FDate, string ToDate)
        {
            Report Report = new Report();
            DataTable ulist = Report.GetInvpltradelistingReport(Code, FDate, ToDate);
            return View(ulist);

        }
        public ActionResult InvpltradelistingDetail(string Code, string FDate, string ToDate, string scripcd, string scripName)
        {
            ViewBag.scripName = scripName;
            ViewBag.scripcd = scripcd;
            ViewBag.Code = Code;
            ViewBag.FDate = FDate;
            ViewBag.ToDate = ToDate;

            //Report Report = new Report();
            //DataTable ulist = Report.GetInvpltradelistingDetail(Code, FDate, ToDate, scripcd);
            return View();

        }

        public ActionResult InvpltradelistingDetailDrillDown(string Code, string FDate, string ToDate, string scripcd, string scripName)
        {
            ViewBag.scripName = scripName;
            ViewBag.scripcd = scripcd;
            DataTable ulist = new DataTable();
            if (scripName != "")
            {
                Report Report = new Report();
                ulist = Report.GetInvpltradelistingDetail(Code, FDate, ToDate, scripcd);

            }

            return View(ulist);

        }

        public ActionResult GetScriptName(string scripcd)
        {
            TplusMOd objMod = new TplusMOd();
            string strResult = objMod.fnvalidate(scripcd, 2);

            return Content(strResult);
        }

        public ActionResult SaveRecordInvplRequest(string Code, string dateBulk, string settlement, string strTRDType, string ScripCode, string qty, string rate, string value, string bsflag, string Charge1, string ServiceTax, string STT, string Charge2)
        {
            
            Report SaveINVPLTL = new Report();
            string Mgs = SaveINVPLTL.SaveRecordInvpltradelisting(Code, dateBulk, settlement, strTRDType, ScripCode, qty, rate, value, bsflag, Charge1, ServiceTax, STT, Charge2);

            return Content(Mgs);
        }

        public ActionResult UpdateRecordInvplRequest(string Code, string dateBulk, string settlement, string strTRDType, string ScripCode, string qty, string rate, string value, string Charge1, string ServiceTax, string STT, string Charge2, string bsflag, string td_srno, string td_TRXFlag)
        {
           
            Report updateINVPL = new Report();
            string Mgs = updateINVPL.UpdateRecordInvpltradelisting(Code, dateBulk, settlement, strTRDType, ScripCode, qty, rate, value, Charge1, ServiceTax, STT, Charge2, bsflag, td_srno, td_TRXFlag);

            return Content(Mgs);
        }
       
        public ActionResult DeleteRecordInvplRequest(string Code, string td_srno)
        { //strcomcode
            Report deleteINVPL = new Report();
            string Mgs = deleteINVPL.DeleteRecordInvplRequest(Code, td_srno);

            return Content(Mgs);
        }


        public ActionResult DematReportDetail(string strDpid, string cmbDate, string cmbClient, string cmbGroupBy, string cmbStatus, string cmbOrderBy, string txtCode, string cmbBranchWise, string dateFrom, string dateTo)
        {
            ViewBag.cmbClient = cmbClient;

            Report account = new Report();
            DataTable ulist = account.getDematReport(strDpid, cmbDate, cmbClient, cmbGroupBy, cmbStatus, cmbOrderBy, txtCode, cmbBranchWise, dateFrom, dateTo);

            return View(ulist);
        }

        public ActionResult DeliveryStatement()
        {
            return View();
        }
        public ActionResult DeliveryStatementReport(string Code, string strDpid, string cmbSelect, string cmbReport, string strdate, string cmbBS, string cmbNoDelivery)
        {
            ViewBag.cmbReport = cmbReport;
            ViewBag.cmbBS = cmbBS;
            Report deilvery = new Report();
            DataTable ulist = deilvery.getDeliveryStatementReport(Code, strDpid, cmbSelect, cmbReport, strdate, cmbBS, cmbNoDelivery);

            return View(ulist);

        }
        public string NotionalSummary(string ClientCode, string strDate, string strignoresection = "N", string AdvisoryTrade = "I")
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetNotionalSummary(ClientCode, strDate, strignoresection, AdvisoryTrade);
            string json = JsonConvert.SerializeObject(ulist);
            return json;

        }

        public ActionResult InvesterBasedF()
        {

            return View();
        }

        public ActionResult InvesterBasedFReport(string Code, string Cname, string cmbSelect, string FromDt, string ToDt, string cmbExchSeg, string openopt, string ChkAvgRate, string ChkBuySellValue, string chkconsider)
        {
            Report investor = new Report();
            DataTable dt = new DataTable();
            ViewBag.ChkAvgRate = ChkAvgRate;
            ViewBag.ChkBuySellValue = ChkBuySellValue;
            ViewBag.chkconsider = chkconsider;
            ViewBag.Code = Code;
            ViewBag.Cname = Cname;
            dt = investor.getInvesterBasedFReport(Code, Cname, cmbSelect, FromDt, ToDt, cmbExchSeg, openopt, ChkAvgRate, ChkBuySellValue, chkconsider);


            return View(dt);
        }

        public ActionResult InvestorBasedRepCash()
        {
            return View();
        }

        public ActionResult InvestorBasedReportCash(string Select, string Code, string FromDate, string ToDate, string strDPID, string cmbReport, string cmbtype, string cmbstock)
        {
            ViewBag.cmbReport1 = cmbReport;
            ViewBag.cmbstock = cmbstock;
            Report ObjINBRC = new Report();
            DataTable ulist = ObjINBRC.getInvestorBasedReportCash(Select, Code, FromDate, ToDate, strDPID, cmbReport, cmbtype, cmbstock);

            return View(ulist);

        }

        public ActionResult InvestorBasedReportCashDetail(string Code, string Cname, string FromDate, string ToDate, string ScripCd, string ScripName)
        {

            UtilityModel myutil = new UtilityModel();

            ViewBag.Code = Code;
            ViewBag.Cname = Cname;
            ViewBag.FromDate = myutil.stod(FromDate).ToString("dd/MM/yyyy");
            ViewBag.ToDate = myutil.stod(ToDate).ToString("dd/MM/yyyy");
            ViewBag.ScripCd = ScripCd;
            ViewBag.ScripName = ScripName;
            Report ObjINBRC = new Report();
            DataTable ulist = ObjINBRC.getInvestorBasedReportCashDetail(Code, Cname, FromDate, ToDate, ScripCd, ScripName);

            return View(ulist);

        }

        public ActionResult InvestorBasedCom()
        {
            return View();
        }

        public ActionResult InvestorBasedComReport(string Code, string Cname, string cmbSelect, string FromDt, string ToDt, string cmbExchSeg, string openopt, string ChkAvgRate, string ChkBuySellValue, string chkconsider)
        {
            Report ObjINBRC = new Report();
            ViewBag.ChkAvgRate = ChkAvgRate;
            ViewBag.ChkBuySellValue = ChkBuySellValue;
            ViewBag.chkconsider = chkconsider;
            ViewBag.Code = Code;
            ViewBag.Cname = Cname;
            DataTable ulist = ObjINBRC.getInvestorBasedReportCom(Code, Cname, cmbSelect, FromDt, ToDt, cmbExchSeg, openopt, ChkAvgRate, ChkBuySellValue, chkconsider);

            return View(ulist);



        }

        public ActionResult SettlementSearch()
        {
            return View();
        }

        public ActionResult SettlementSearchReport(string cmbExch, string cmbYear, string cmbType)
        {
            LibraryModel mylib = new LibraryModel();
            DataTable dt = new DataTable();
            dt = mylib.OpenDataTable("select se_stlmnt Settlement,convert(char(10),convert(datetime,se_stdt),103)+(case se_stdt when se_endt then '' else ' to '+convert(char(10),convert(datetime,se_endt),103) end) Period,convert(char(10),convert (datetime,se_shpayoutdt),103)StlmntDt,case cs_billflag when 'Y' then 'Yes' else ''end as 'Billed',case cs_lockflag when 'Y' then 'Locked' else '' end as  'Locked'from Settlements,Company_settlement where se_stlmnt=cs_stlmnt and cs_billflag<>'N' and se_exchange='" + cmbExch.Trim() + "' and se_year='" + cmbYear.Trim() + "' and se_type='" + cmbType.Trim() + "' and cs_companycode='" + Session["CompanyCode"] + "' order by se_stdt,se_stlmnt desc");


            return View(dt);
        }
        public ActionResult SettlementSearch1()
        {
            DDLSettlementSearch ddlSettlementSearch = new DDLSettlementSearch();

            Report ObjddlSettlementSearch = new Report();
            IEnumerable<DDLSettlementSearch> ulist = ObjddlSettlementSearch.PopulateDDLSettlementSearch();

            return Json(ulist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SettlementSearch2(string cmbExch)
        {
            DDLSettlementSearch ddlSettlementSearch = new DDLSettlementSearch();

            Report ObjddlSettlementSearch = new Report();
            IEnumerable<DDLSettlementSearch> ulist = ObjddlSettlementSearch.PopulateDDLSettlementSearchcmbType(cmbExch);

            return Json(ulist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PayInStatus()
        {

            return View();
        }
        public ActionResult PayInStatusReport(string code, string StlMnt, string Select, string Exchange, string SettType, string Order, string ReportType)
        {
            UtilityModel myutil = new UtilityModel();
            Report ObjINBRC = new Report();
            DataTable ulist = ObjINBRC.PayInStatusReport(code, StlMnt, Select, Exchange, SettType, Order, ReportType);

            return View(ulist);
        }

    }
}
