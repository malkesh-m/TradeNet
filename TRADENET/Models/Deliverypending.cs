using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class Deliverypending : ConnectionModel
    {
        public DataTable GetDeliverypending(string Code,string strDpid, string cmbSelect, string cmbReport, string strdate, string cmbBS)
        {
            string strField = "";
            string strClientWhere;
            string strMode = "P";
            strClientWhere = "";
            string strsql = "";
            //string strTpExchangeSeg;
            // string strComExchange;
            // strTpExchangeSeg = strDpid;
            // strComExchange = strDpid;
            DataTable dt = null; 
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            strdate = myutil.dtos(strdate);

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                try
                {

                }
                catch (Exception)
                {

                }
                finally

                {
                    if (Code != "")
                    {
                        if (cmbSelect == "CL")
                        {
                            strClientWhere = " and cm_cd = '" + Code + "'";
                        }
                        else if (cmbSelect == "BR")
                        {
                            strClientWhere = " and cm_brboffcode = '" + Code + "'";
                        }
                        else if (cmbSelect == "GR")
                        {
                            strClientWhere = " and cm_groupcd = '" + Code + "'";
                        }
                        else if (cmbSelect == "FM")
                        {
                            strClientWhere = " and cm_familycd = '" + Code + "'";
                        }
                        else if (cmbSelect == "SB")
                        {
                            strClientWhere = " and cm_subbroker = '" + Code + "'";
                        }
                        else if(cmbSelect== "ALL")
                        {
                            strClientWhere = "";

                        }
                    }

                    strClientWhere = strClientWhere + strClientWhere.Replace("cm_cd", "dm_clientcd");
                    if (strMode == "E")
                    {
                        strField = "cm_email ,";
                    }

                    if (cmbReport == "1")
                        strsql = " select cm_name + (case when cm_poa = 'Y' then '(POA)' else '' end ) + '(def act)' as  GroupByValue ,";
                    else if (cmbReport == "2")
                        strsql = " select ss_name + '[' + dm_scripcd + ']'+' '+ 'ISIN :'+ isnull((select top 1 im_isin from Isin where im_scripcd = dm_scripcd order by im_priority),'') as GroupByValue,";
                    else if (cmbReport == "3")
                        strsql = " select gr_Desc + '['+ cm_GroupCd +']' as GroupByValue,";
                    else
                        strsql = " select cm_brboffcode + '-['+ bm_branchname + ']' as GroupByValue ,";
                    if (strDpid != "")
                        strsql = strsql + " demat.*, cm_name + (case when cm_poa = 'Y' then '(POA)' else '' end ) + '(def act)' as  ClientName,ss_name, dm_stlmnt,";
                    strsql = strsql + " cm_brboffcode,cm_groupcd,cm_familycd,cm_poa,bm_branchcd,bm_branchname,isnull(bm_email,'') bm_email, ";
                    strsql = strsql + " isnull((select top 1 im_isin from Isin where im_scripcd = dm_scripcd order by im_priority),'') im_isin, isnull(cm_email,'') cm_email,";
                    strsql = strsql + " gr_desc , fm_desc,(case when dm_qty < 0 then dm_qty * -1 else 0 end) Purchase,(case when dm_qty > 0 then dm_qty  else 0 end) Sell, ";
                    strsql = strsql + " isNull((select top 1 case When left(da_dpid,2) = 'IN' Then da_dpid + da_actno else da_actno end from DematAct,Dps Where da_dpid = dp_dpid and da_clientcd =dm_clientcd and da_defaultyn = 'Y'),'') Dematact";
                    strsql = strsql + " From Demat, Securities, Client_master, Branch_master, Ourdps, Group_master, Family_master, settlements ";
                    strsql = strsql + " where se_stdt = '" + strdate + "'";
                    strsql = strsql + " and dm_clientcd = cm_cd and od_cd = dm_ourdp and od_Acttype = 'P' ";
                    strsql = strsql + " and dm_stlmnt = se_stlmnt ";
                    strsql = strsql + " and cm_brboffcode = bm_branchcd and dm_scripcd=ss_cd ";
                    strsql = strsql + " and cm_groupcd = gr_cd and cm_Familycd = fm_cd ";
                    strsql = strsql + " and dm_companycode='" + HttpContext.Current.Session["CompanyCode"] + "' and left(dm_stlmnt,1)+'c' in ('" + strDpid.Replace(",", "','") + "') ";
                    strsql = strsql + " and ((dm_type = 'BC' and dm_locked = 'N' and dm_transfered = 'N') Or (dm_type = 'CB' and dm_locked = 'N' and dm_transfered <> 'S')) ";
                    //strsql = strsql + "and(cm_brboffcode = '000012')"; // and exists " + myutil.LoginAccess("ld_clientcd");
                    strsql = strsql + strClientWhere;
                    strsql = strsql + " and exists " + myutil.LoginAccess("dm_clientcd");
                    mylib.ExecSQL(strsql, curCon);

                    if (strMode == "E")
                    {
                        strsql = strsql + " and cm_email <> ''";
                    }


                    if (cmbBS == "1")
                        strsql = strsql + " and dm_qty < 0 ";
                    else if (cmbBS == "2")
                        strsql = strsql + " and dm_qty > 0 ";
                    if (cmbReport == "1")
                        strsql = strsql + " order by dm_clientcd,ss_name ";
                    else if (cmbReport == "2")
                        strsql = strsql + " order by dm_scripcd,ss_name ";
                    else if (cmbReport == "3")
                        strsql = strsql + " order by cm_groupcd,dm_clientcd,ss_name";
                    else if (cmbReport == "4")
                        strsql = strsql + " order by cm_brboffcode,dm_clientcd,ss_name ";

                   dt= mylib.OpenDataTable(strsql, curCon);

                    
                }
                return dt;
            }
        }

    }
}