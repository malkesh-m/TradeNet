using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class TplusMOd: ConnectionModel
    {
        ApplicationUser objApplicationUser = new ApplicationUser();
 

        public string fnvalidate(string strvalue, int intindex, string strCond = "")
        {
            DataTable objDataset = new DataTable();
            UtilityDBModel util = new UtilityDBModel();
            LibraryModel lib = new LibraryModel();
            bool flg;
            string strsql = "";
            string strName = "";
            if (intindex == 1)
            {
                strsql = "select cm_name,cm_type from Client_master ";
                strsql += " where cm_cd = '" + strvalue + "' " + HttpContext.Current.Session["loginaccess"];
            }
            else if (intindex == 2)
            {
                strsql = "Select ss_name from Securities where ss_cd ='" + strvalue + "'";
            }
            else if (intindex == 3)
            {
                strsql = "Select se_stlmnt,se_stdt,se_endt  from  Settlements  where se_stlmnt = '" + strvalue + "'";
            }
            else if (intindex == 4)
            {
                strsql = "Select gr_desc  from  Group_master  where gr_cd ='" + strvalue + "'";
            }
            else if (intindex == 5)
            {
                strsql = "Select fm_desc  from  Family_master  where fm_cd ='" + strvalue + "'";
            }
            else if (intindex == 6)
            {
                strsql = "Select RM_Name  from  Subbrokers  where RM_CD ='" + strvalue + "'";
            }
            else if (intindex == 7)
            {
                strsql = "select bm_branchname from Branch_master where bm_branchcd= '" + strvalue + "'";
            }
            else if (intindex == 8)
            {
                strsql = "select dp_name from Dps where dp_dpid= '" + strvalue + "'";
            }
            else if (intindex == 9)
            {
                strsql = "select dp_name from  Ourdps,Dps where od_cd= '" + strvalue + "' and od_dpid = dp_dpid ";
            }
            else if (intindex == 10)
            {
                strsql = "Select sm_desc from series_master where sm_seriesid =" + Conversion.Val(strvalue) + "";
            }
            else if (intindex == 11) // RM
            {
                strsql = "Select rm_name from RM_Master where RM_CD ='" + strvalue + "'";
            }
            else if (intindex == 13) // new comm
            {
                strsql = "select distinct pm_assetcd , pm_assetcd from product_master where pm_assetcd = '" + strvalue + "'";
            }

            if (!string.IsNullOrEmpty(strCond))
            {
                strsql = strsql + strCond;
            }


            if (intindex == 13)
            {
                var ObjCommConnection = new SqlConnection();
                ObjCommConnection = util.commexTemp_conn(Convert.ToString(HttpContext.Current.Application["CommDatabase"]));
                objDataset= lib.OpenDataTable(strsql,ObjCommConnection);
            }
            else
            {
                var ObjConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString);
                if (ObjConnection.State == ConnectionState.Closed)
                {
                    ObjConnection.Open();
                }

                objDataset= lib.OpenDataTable(strsql, ObjConnection);
            }

          
            if (objDataset.Rows.Count != 0)
            {
                flg = true;
                if (intindex != 3)
                {
                    strName = objDataset.Rows[0][0].ToString();
                }
                else if (intindex == 3)
                {
                    strName = (objDataset.Rows[0][0].ToString()) + "|" + (objDataset.Rows[0][1].ToString()) + "|" + (objDataset.Rows[0][2].ToString()) + "|";
                }
            }
            else
            {
                strName = "";
                flg = false;
            }

            objDataset.Dispose();
            return strName;
        }

    }

   
}