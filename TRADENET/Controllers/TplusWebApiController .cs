/*using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using TRADENET.Models;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;


namespace TRADENET.Controllers
{
    [Route("api/TplusWebApi/{action}", Name = "Tplus")]
    public class TplusWebApiController : ApiController
    {
        
        [HttpGet]
        public HttpResponseMessage RMSSummaryList(DataSourceLoadOptions optionsBase)
        {
            string rmsvalue = "";
            string chkTp = "1";
            string chkComm = "1";
            string chkNBFC = "0";
            string FilterRMS1 = "";
            bool blnRefresh = false;
            bool blnFetch = true;
            //if (this.Session["USERMENU"] == null && Session["LoginAccessOld"] == null)
            //{
            //    return RedirectToAction("LogoutPage", "Tplus");
            //}

            //DataTable dtRMSHEAD = new DataTable();
            //dtRMSHEAD.Clear();
            //dtRMSHEAD.Columns.Add("RMSHEAD");           

            //LibraryModel objRMSlib = new LibraryModel();
            //DataTable dt = null;
            //DataTable dt2 = null;

            //dt = objRMSlib.OpenDataTable("SELECT * FROM Sysparameter where sp_sysvalue like '%rs_NFiller2%'");

            //if (dt.Rows.Count > 0)
            //{
            //    for (int iR = 0; iR < dt.Columns.Count; iR++)
            //    {
            //        object[] o = { objRMSlib.GetScalarValueString("select sp_sysvalue from sysparameter where sp_parmcd='" + dt.Rows[iR]["sp_parmcd"].ToString() + "'") };
            //        dtRMSHEAD.Rows.Add(o);
            //    }

            //    Session["RMSFORMULA"] = dtRMSHEAD;
            //}


            ViewBag.chkTp = chkTp;
            ViewBag.chkComm = chkComm;
            ViewBag.chkNBFC = chkNBFC;
            DataTable Results;
            if (blnFetch)
            {
                Session["RiskManagement"] = null;

            }
            if (Session["RiskManagement"] != null)
            {
                Results = (DataTable)Session["RiskManagement"];
                if (FilterRMS1.Trim() != "")
                {
                    string filter1 = FilterRMS1.Split('~')[0];
                    string sign1 = FilterRMS1.Split('~')[1];
                    Double amount = Conversion.Val(FilterRMS1.Split('~')[2]);

                    DataView dv = new DataView(Results);
                    dv.RowFilter = filter1 + sign1 + amount; // query example = "id = 10"
                    Results = dv.ToTable();
                }
                //return View(Results);
                //return Request.CreateResponse(DataSourceLoader.Load());
            }
            else
            {
                if (blnRefresh && Session["RiskManagement"] != null)
                {
                    Results = (DataTable)Session["RiskManagement"];
                }
                string type = "";
                string code = "";
                if (rmsvalue.Trim() != "")
                {
                    type = rmsvalue.Split('~')[0];
                    code = rmsvalue.Split('~')[1];
                }

                modRiskManagementSystem rms = new modRiskManagementSystem()
                {
                    type = type,
                    Code = code
                };

                Results = rms.GetRMSReport(chkTp, chkComm, chkNBFC);
                Session["RiskManagement"] = Results;
            }

            List<RiskManagmentResponse> riskManagments = new List<RiskManagmentResponse>();

            riskManagments = Results.AsEnumerable()
                .Select(row => new RiskManagmentResponse
                {
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    TDay = row.Field<double>("TDay"),
                    T2Day = row.Field<double>("T2Day"),
                    UnCleared = row.Field<double>("UnCleared"),
                    CashDeposit = row.Field<double>("CashDeposit"),
                    ApprovedShares = row.Field<double>("ApprovedShares"),
                    Margin = row.Field<double>("Margin"),
                    Pool = row.Field<double>("Pool"),
                    DPHolding = row.Field<double>("DPHolding"),
                    Stoke = row.Field<double>("Stoke"),
                    Net = row.Field<double>("Net"),
                    AboveDays = row.Field<double>("AboveDays"),
                    Collection = row.Field<double>("Collection"),
                    ActualRisk = row.Field<double>("ActualRisk"),
                    FundPayout = row.Field<double>("FUNDPAYOUT"),
                    ProjectedRisk = row.Field<double>("PROJECTEDRISK"),
                    SharePayout = row.Field<double>("SHAREPAYOUT"),
                    FoMargin = row.Field<double>("fomargin"),
                    StokeBH = row.Field<double>("StokeBH"),
                    ProvInt = row.Field<double>("PtovInt"),
                    AvailMargin = row.Field<double>("AvailMargin"),
                    OptionM2M = row.Field<double>("OptionM2M"),
                    DebitOlder5Days = row.Field<double>("DebitOlder5Days"),
                    RmsLimit = row.Field<double>("RMSLIMIT"),
                    StokeAH = row.Field<double>("StokeAH"),
                    ExchApprStk = row.Field<double>("ExchApprStk"),
                }).ToList();

            return Json(DataSourceLoader.Load(riskManagments, optionsBase));
        }

        

    }
}




*/