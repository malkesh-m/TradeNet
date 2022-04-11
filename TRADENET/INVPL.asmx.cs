using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using TRADENET.Models;

namespace TRADENET
{

    /// <summary>
    /// Summary description for INVPL
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class INVPL : System.Web.Services.WebService
    {
        INVPLProcess Report = new INVPLProcess();
        LibraryModel myLib = new LibraryModel();
        DataTable ObjDataTable = new DataTable();
        DataAccesslayer objDAL = new DataAccesslayer();

        [WebMethod]
        public string INVPLVersion()
        {
            return "1.0";
        }

        [WebMethod]
        public string ITACT112A()
        {
            return "Y";
        }

        [WebMethod]
        public string INVPLREPROCESS()
        {
            if (myLib.fnFireQuery("process_log", "count(0)", "pr_process", "INVPLREPRC", true) == "N")
            {
                return "N";

            }
            else
            {
                return "Y";
            }


        }

        [WebMethod]
        public string NotionalSummary(string ClientCode, string strDate, string strignoresection = "N", string AdvisoryTrade = "I")
        {


            DataTable ulist = Report.FnGetNotionalSummary(ClientCode, strDate, strignoresection, AdvisoryTrade);
            if (ulist.Rows.Count > 0)
            {
                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            };

        }

        [WebMethod]
        public string NotionalDetail(string ClientCode, string strDate, string ScripCode, string Ignore112A)
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetNotionalDetail(ClientCode, strDate, ScripCode, Ignore112A);
            if (ulist.Rows.Count > 0)
            {
                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            };

        }

        [WebMethod]
        public string ActualPLSummary(string ClientCode, string FromDt, string ToDt, string Type, string Ignore112A = "N", string AdvisoryTrade = "I")
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetActualPLSummary(ClientCode, FromDt, ToDt, Type, Ignore112A, AdvisoryTrade);
            if (ulist.Rows.Count > 0)
            {
                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            }

        }

        [WebMethod]
        public string ActualPLDetail(string ClientCode, string FromDt, string ToDt, string Type, string ScripCode, string Ignore112A = "N", string AdvisoryTrade = "I")
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetActualPLDetail(ClientCode, FromDt, ToDt, Type, ScripCode, Ignore112A, AdvisoryTrade);
            if (ulist.Rows.Count > 0)
            {
                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            }

        }
        [WebMethod]
        public string Dividend(string ClientCode, string FromDt, string ToDt, string AdvisoryTrade = "I")
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetDividend(ClientCode, FromDt, ToDt, AdvisoryTrade);
            if (ulist.Rows.Count > 0)
            {
                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            }
        }

        [WebMethod]
        public string TradeListingSummary(string ClientCode, string FromDt, string ToDt, string AdvisoryTrade = "I")
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetTradeListingSummary(ClientCode, FromDt, ToDt, AdvisoryTrade);
            if (ulist.Rows.Count > 0)
            {
                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            }



        }
        [WebMethod]
        public string TradeListingDetail(string ClientCode, string FromDt, string ToDt, string ScripCode, string AdvisoryTrade = "I")
        {
            INVPLProcess Report = new INVPLProcess();

            DataTable ulist = Report.FnGetTradeListingDetail(ClientCode, FromDt, ToDt, ScripCode, AdvisoryTrade);
            if (ulist.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ulist);
            }
            else
            {
                return "No Record Found";
            }

        }

        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

        [WebMethod()]
        public string TradeInsert(string ClientCode, string ScripCode, string strDataXml)
        {
            string strreturn = Report.FnTradeInsert(ClientCode, ScripCode, strDataXml);

            ObjDataTable = objDAL.OpendatatableXml(strreturn);

            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }
        [WebMethod()]
        public string TradeUpdate(string ClientCode, string ScripCode, string strDataXml)
        {
            string strreturn = Report.FnTradeUpdate(ClientCode, ScripCode, strDataXml);
            ObjDataTable = objDAL.OpendatatableXml(strreturn);

            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }
        [WebMethod()]
        public string TradeDelete(string ClientCode, string SrNo)
        {
            string strreturn = Report.FnTradeDelete(ClientCode, SrNo);
            ObjDataTable = objDAL.OpendatatableXml(strreturn);

            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }
        [WebMethod()]
        public string SpecialCharges(string ClientCode, string FromDt, string ToDt)
        {
            ObjDataTable = Report.FnGetChargesDetails(ClientCode, FromDt, ToDt);           

            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }

        [WebMethod()]
        public string SLBMSummary(string ClientCode, string FromDt, string ToDt)
        {
            ObjDataTable = Report.FnGetSLBMSummary(ClientCode, FromDt, ToDt);
            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }
        [WebMethod()]
        public string SLBMDetail(string ClientCode, string FromDt, string ToDt)
        {
            ObjDataTable = Report.FnGetSLBMDetail(ClientCode, FromDt, ToDt);
            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }
        [WebMethod()]
        public string TradeListingReport(string ClientCode, string ScripCode, string FromDt, string ToDt, string CorporateAction)
        {
            ObjDataTable = Report.FnGetCorporateAction(ClientCode, ScripCode, FromDt, ToDt, CorporateAction);
            if (ObjDataTable.Rows.Count > 0)
            {

                return DataTableToJSONWithStringBuilder(ObjDataTable);
            }
            else
            {
                return "No Record Found";
            }
        }


    }
}
