using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TRADENET.Models
{
    public class DPService : ConnectionModel
    {

        
        public DataTable GetDPHoldingReport(string Code, string cmbRep, string bsecode, string nsesymbol, string isincode, string Ckekclient, string CkekSecurity, string AsOn, string PrpFromDate, string StrHldType)
        {
            DataTable dt = null;
            string strDPs;
            string strsql = "";
            string strGroup = "";
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            DataTable objdatatable = new DataTable();
            if (Ckekclient == "1")
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();


                    switch (cmbRep)
                    {

                        case "GR":
                            {
                                strGroup = " cm_groupcd = '" + Code.Trim() + "'";
                                break;
                            }

                        case "FM":
                            {
                                strGroup = "cm_Familycd = '" + Code.Trim() + "'";
                                break;
                            }

                        case "SU":
                            {
                                strGroup = "cm_subbroker = '" + Code.Trim() + "'";
                                break;
                            }


                    }

              
                    if (cmbRep == "CL")
                    {
                        strDPs = mylib.fnGetSysParam("POADPIDS");
                        strsql = "select da_actno 'Client ID',da_name 'Account Name' ,(case left(da_dpid,2) when 'IN' then rtrim(da_dpid)+'/' else '' end)+rtrim(dp_name) 'DP', ";
                        strsql = strsql + " case sign(patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "')) when 1 then 'Y' else 'N' end as HLinkYn from Dematact,Dps,Client_master where dp_dpid=da_dpid and da_clientcd=cm_cd ";
                        strsql = strsql + " and exists " + myutil.LoginAccess("da_clientcd");
                        strsql = strsql + " and da_status='A'";
                        if (Code != "")
                        {
                            strsql = strsql + "and da_clientcd= '" + Code + "' and sign(patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "'))=1";
                        }
                        strsql = strsql + "order by da_defaultyn desc";
                    }

                    else
                    {
                        string strrDPs;
                        strrDPs = Strings.Trim(mylib.fnGetSysParam("POADPIDS"));
                        strsql = " select da_actno 'Client ID',da_name 'Account Name', case da_defaultyn when 'Y' then 'Main' else '' end as '_'," + " (case left(da_dpid,2) when 'IN' then rtrim(da_dpid)+'/' else '' end)+rtrim(dp_name) 'DP', " + " case sign(patindex('%'+rtrim(da_dpid)+'%','" + strrDPs + "')) when 1 then 'Y' else 'N' end as HLinkYn " + " from Dematact,Dps,Client_master where dp_dpid=da_dpid and da_clientcd=cm_cd  and exists " + myutil.LoginAccess("da_clientcd") + " and da_status='A' and sign(patindex('%'+rtrim(da_dpid)+'%','" + strrDPs + "')) = 1 and " + strGroup + " order by da_defaultyn desc";


                    }
                    dt = mylib.OpenDataTable(strsql, curCon);
                    return dt;
                }
            }
            if (CkekSecurity == "1")
            {
                return getISINData("", "", "", AsOn, PrpFromDate, isincode, StrHldType);

            }
            return dt;

        }

        private DataTable getISINData(string strTable, string strCondition, string strSelect, string chkAsOn, string PrpFromDate, string txtisin,string StrHldType)
        {
            int i, j;

            string strDPs;
            string strsql;
            Boolean blnNSDL = false;
            Boolean blnCDSL = false;
            DataTable Acts = new DataTable();
            string[] Cross = new string[2];
            string[] Estro = new string[2];

            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            PrpFromDate = myutil.dtos(PrpFromDate);
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                strSQL = "Select OP_Server,OP_DataBase,OP_Owner,OP_User from Other_Products Where OP_Product = 'CROSS' and op_Status = 'A'";
                DataTable dtCross = mylib.OpenDataTable(strSQL, curCon);
                if (dtCross.Rows.Count > 0)
                {
                    DataRow dr = dtCross.Rows[0];

                    blnCDSL = true;
                    Cross = new string[3];
                    Cross[0] = dr["OP_Server"].ToString().Trim();
                    Cross[1] = dr["OP_DataBase"].ToString().Trim();
                    Cross[2] = dr["OP_Owner"].ToString().Trim();
                }

                strSQL = "Select OP_Server,OP_DataBase,OP_Owner,OP_User,OP_PWD from Other_Products Where OP_Product = 'ESTRO' and op_Status = 'A'";
                DataTable dtEstro = mylib.OpenDataTable(strSQL, curCon);
                if (dtEstro.Rows.Count > 0)
                {
                    DataRow dr = dtEstro.Rows[0];
                    blnNSDL = true;
                    Estro = new string[3];
                    Estro[0] = dr["OP_Server"].ToString().Trim();
                    Estro[1] = dr["OP_DataBase"].ToString().Trim();
                    Estro[2] = dr["OP_Owner"].ToString().Trim();
                }

                //strDPs = Strings.Trim(mylib.fnFireQuery("other_products", "sum(case op_product when 'Cross' then 1 else 2 end)", "op_product in ('Cross','Estro') and op_status", "A", true, curCon));
                //blnNSDL = (Strings.InStr(1, "23", strDPs) > 0);
                //blnCDSL = (Strings.InStr(1, "13", strDPs) > 0);
                strDPs = "";

                if (blnNSDL)
                {
                    // mydbutil.crostemp_conn("Estro");
                    strDPs = Strings.Trim(mylib.fnFireEstroQuery("sysparameter", "sp_sysvalue", "sp_parmcd", "DPID", true));
                }

                if (blnCDSL)
                {
                    // mydbutil.crostemp_conn("Cross");
                    strDPs = Strings.Trim(mylib.fnFireCrossQuery("sysparameter", "sp_sysvalue", "sp_parmcd", "DPID", true));
                }
                //strsql = "select cm_cd,cm_name,da_actno,0.0 as Holding,0.0 as value,'' as Type from dematact, client_master " + strTable + " where da_clientcd=cm_cd " + strCondition + "and exists " + myutil.LoginAccess("da_clientcd") + " and da_status='A' and patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "')>0 order by cm_Name ";
                //Acts = mylib.OpenDataTable(strsql, curCon);

                strsql = "";
                if (blnCDSL)
                {
                    if (chkAsOn == "1" & PrpFromDate != "")
                    {
                        strsql = "select b.cm_cd,b.cm_name,a.cm_cd as da_actno,";
                        strsql += " cast(round(abs(sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end)),3) as decimal (15,3)) as 'Holding', ";
                        strsql += " (cast(round(abs(sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end)),3) as decimal (15,3)) * 0) as 'value', sc_decimal_allow,bt_description as 'Type' ";
                        strsql += " From [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].TrxDetail, [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Client_master a, [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].security, ";
                        strsql += " [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Branch_master, [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].beneficiary_type ,Client_master b, Dematact  ";
                        strsql += " Where b.cm_cd = da_clientcd and da_actno=a.cm_cd and td_ac_code = a.cm_cd and td_ac_type = bt_code And td_isin_code = sc_isincode and a.cm_brboffcode = bm_branchcd and a.cm_active = '01' ";
                        strsql += " and patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "')>0 and da_status='A' and exists " + myutil.LoginAccess("da_clientcd");
                        strsql += " and td_curdate <='" + PrpFromDate.Trim() + "' and td_isin_code = '" + txtisin.Trim() + "' ";
                        if (StrHldType.Trim() != "")
                        {
                            strsql += " and td_ac_type in ( " + StrHldType + " )";
                        }
                        strsql += " and Len(Ltrim(da_actno))=16 ";
                        strsql += " Group by a.cm_cd ,td_ac_type,sc_decimal_allow,bt_description,b.cm_cd,b.cm_name,td_ac_code ";
                        strsql += " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0";
                        strsql += " Order by b.cm_name, td_ac_code, td_ac_type ";
                    }
                    else
                    {
                        strsql = "select cm_cd,cm_name,da_actno, hld_ac_pos as 'Holding',(hld_ac_pos*0) as 'value',sc_decimal_allow,hld_ac_type,bt_description as 'Type' ";
                        strsql += " from [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].holding,[" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].security,[" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].beneficiary_type, client_master, DematAct ";
                        strsql += " where cm_cd = da_clientcd and da_actno=hld_ac_code and hld_isin_code = sc_isincode and hld_ac_type = bt_code and hld_isin_code='" + txtisin.Trim() + "' and patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "')>0 and da_status='A' ";
                        if (StrHldType.Trim() != "")
                        {
                            strsql += " and hld_ac_type in ( " + StrHldType + " )";
                        }
                        strsql += " and Len(Ltrim(da_actno))=16 ";
                        strsql += " and exists " + myutil.LoginAccess("da_clientcd");
                        strsql += " order by cm_name, hld_ac_code, hld_ac_type ";
                    }
                }
                else if (blnNSDL)
                {
                    if (chkAsOn == "1" & PrpFromDate != "")
                    {
                        strsql = "select b.cm_cd,b.cm_name,a.cm_cd as da_actno, ";
                        strsql += " cast(round(abs(sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end)),3) as decimal (15,3)) as 'Holding', ";
                        strsql += " (cast(round(abs(sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end)),3) as decimal (15,3)) * 3) as 'value' ,sc_decimal_allow,bt_description as 'Type' ";
                        strsql += " From [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].TrxDetail, [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].Client_master a, [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].security, [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].Branch_master, [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].beneficiary_type, ";
                        strsql += " Client_master b, Dematact ";
                        strsql += " Where b.cm_cd = da_clientcd and da_actno=a.cm_cd and td_ac_code = a.cm_cd and td_ac_type = bt_code And td_isin_code = sc_isincode and a.cm_brboffcode = bm_branchcd and a.cm_active = '01' ";
                        strsql += " and td_ac_type Not in ('17','18') and td_category <> '03' and td_booking_type Not in('02')";
                        strsql += " and td_narration not in('001') and td_rdate <'" + PrpFromDate.Trim() + "' and td_curdate <='" + PrpFromDate.Trim() + "'";
                        strsql += " and td_isin_code = '" + txtisin.Trim() + "'"; // and td_ac_type ='22'"
                        strsql += " and patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "')>0 and da_status='A' and exists " + myutil.LoginAccess("da_clientcd");
                        if (StrHldType.Trim() != "")
                        {
                            strsql += " and td_ac_type in ( " + StrHldType + " )";
                        }
                        strsql += " and Len(Ltrim(da_actno))=8 ";
                        strsql += " Group by a.cm_cd ,td_ac_type,sc_decimal_allow,bt_description,b.cm_cd,b.cm_name,td_ac_code ";
                        strsql += " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0";
                        strsql += " Order by b.cm_name, td_ac_code, td_ac_type ";
                    }
                    else
                    {
                        strsql = "select cm_cd,cm_name,da_actno, hld_ac_pos as 'Holding',(hld_ac_pos * 3) as 'value',sc_decimal_allow,hld_ac_type,bt_description as 'Type' ";
                        strsql += " from [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].holding,[" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].security ,[" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].beneficiary_type, Client_master , DematAct ";
                        strsql += " where  cm_cd = da_clientcd and da_actno=hld_ac_code and hld_isin_code = sc_isincode  and hld_ac_type = bt_code and hld_isin_code='" + txtisin.Trim() + "' and patindex('%'+rtrim(da_dpid)+'%','" + strDPs + "')>0 and da_status='A' ";
                        if (StrHldType.Trim() != "")
                        {
                            strsql += " and hld_ac_type in ( " + StrHldType + " )";
                        }
                        strsql += " and Len(Ltrim(da_actno))=8 ";
                        strsql += "and exists " + myutil.LoginAccess("da_clientcd");
                        strsql += " order by cm_name, hld_ac_code, hld_ac_type ";
                    }
                }

                Acts = mylib.OpenDataTable(strsql, curCon);

                //if (blnCDSL) // Cross
                //{
                //    DataTable Cross = new DataTable();

                //    SqlConnection ObjConCross = mydbutil.crostemp_conn("Cross");

                //    if (ObjConCross.State == ConnectionState.Closed)
                //    {
                //        ObjConCross.Open();
                //    }
                //    //var ObjConCross = new SqlConnection(HttpContext.Current.Application["ConnectionCross"].ToString().Trim());
                //    //if (ObjConCross.State == ConnectionState.Closed)
                //    //{
                //    //    ObjConCross.Open();
                //    //}

                //    try
                //    {
                //        strsql = "Create Table #TmpClient (tmp_ac_code Char(16))";
                //        mylib.ExecSQL(strsql, ObjConCross);

                //    }
                //    catch (Exception objError)
                //    {
                //        strsql = "Drop Table #TmpClient";
                //        mylib.ExecSQL(strsql, ObjConCross);

                //    }

                //    for (i = 0; i <= Acts.Rows.Count - 1; i++)
                //    {
                //        if (Strings.Trim(Acts.Rows[i]["da_actno"].ToString()).Length == 16)
                //        {
                //            strsql = "Insert into #TmpClient(tmp_ac_code) Values ";
                //            strsql = strsql + "('" + Strings.Trim(Acts.Rows[i]["da_actno"].ToString()) + "')";
                //            mylib.ExecSQL(strsql, ObjConCross);
                //        }
                //    }

                //    if (chkAsOn == "1" & PrpFromDate != "")
                //    {
                //        strsql = "select cm_cd as hld_ac_code,cast(round(abs(sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end)),3) as decimal (15,3)) as hld_ac_pos,td_ac_type,sc_decimal_allow,bt_description as 'Balance Type'";
                //        strsql = strsql + " From TrxDetail, Client_master, security, Branch_master, beneficiary_type ";
                //        strsql = strsql + " Where td_ac_code = cm_cd and td_ac_type = bt_code And td_isin_code = sc_isincode and cm_brboffcode = bm_branchcd and cm_active = '01' ";
                //        strsql = strsql + " and td_curdate <='" + PrpFromDate.Trim() + "' and td_isin_code = '" + txtisin.Trim() + "' and td_ac_code in (Select tmp_ac_code from #TmpClient)";
                //        if (StrHldType.Trim() != "")
                //        {
                //            strsql = strsql + " and td_ac_type in ( " + StrHldType + " )";
                //        }
                //        strsql = strsql + " Group by cm_cd ,td_ac_type,sc_decimal_allow,bt_description ";
                //        strsql = strsql + " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0";
                //        strsql = strsql + " Order by cm_cd, td_ac_type ";
                //    }
                //    else
                //    {
                //        //strsql = "select hld_ac_code, hld_ac_pos,sc_decimal_allow from holding,security where hld_isin_code = sc_isincode and hld_isin_code='" + txtisin.Trim() + "' and hld_ac_type in ('11') and hld_ac_code in (Select tmp_ac_code from #TmpClient)";
                //        strsql = "select hld_ac_code, hld_ac_pos,sc_decimal_allow,hld_ac_type,bt_description as 'Balance Type' from holding,security,beneficiary_type where hld_isin_code = sc_isincode and hld_ac_type = bt_code and hld_isin_code='" + txtisin.Trim() + "'";
                //        if (StrHldType.Trim() != "")
                //        {
                //            strsql = strsql + " and hld_ac_type in ( " + StrHldType + " )";
                //        }

                //        strsql = strsql + "and hld_ac_code in (Select tmp_ac_code from #TmpClient)";
                //       // strsql = strsql + "and hld_ac_type='11'and hld_ac_code in (Select tmp_ac_code from #TmpClient)";
                //    }

                //    Cross = mylib.OpenDataTable(strsql, ObjConCross);


                //    if (Cross.Rows.Count == 0)
                //    {

                //    }
                //    else
                //    {
                //        int intDecimal = 0;
                //        for (i = 0; i <= Cross.Rows.Count - 1; i++) // Cross
                //        {
                //            intDecimal = ((Convert.ToInt32(Cross.Rows[i]["sc_decimal_allow"].ToString()) > 0 ? 3 : 0));
                //            for (j = 0; j <= Acts.Rows.Count - 1; j++) // Tradeplus
                //            {
                //                if ((Strings.Trim(Acts.Rows[j]["da_actno"].ToString())) == (Strings.Trim(Cross.Rows[i]["hld_ac_code"].ToString())))
                //                {
                //                    Acts.Rows[j]["Holding"] = Cross.Rows[i]["hld_ac_pos"].ToString();
                //                    Acts.Rows[j]["value"] = Convert.ToDecimal(Cross.Rows[i]["hld_ac_pos"]) * 0;
                //                    Acts.Rows[j]["Type"]= Cross.Rows[i]["Balance Type"].ToString();
                //                }
                //            }
                //        }
                //    }

                //}


                //if (blnNSDL)  // Estro
                //{
                //    DataTable Estro = new DataTable();

                //    SqlConnection ObjConESTRO = mydbutil.crostemp_conn("ESTRO");

                //    if (ObjConESTRO.State == ConnectionState.Closed)
                //    {
                //        ObjConESTRO.Open();
                //    }



                //    try
                //    {
                //        strsql = "Create Table #TmpClient (tmp_ac_code Char(16))";
                //        mylib.ExecSQL(strsql, ObjConESTRO);

                //    }
                //    catch (Exception objError)
                //    {
                //        strsql = "Drop Table #TmpClient";
                //        mylib.ExecSQL(strsql, ObjConESTRO);

                //    }


                //    for (i = 0; i <= Acts.Rows.Count - 1; i++)
                //    {
                //        if (Strings.Trim(Acts.Rows[i]["da_actno"].ToString()).Length == 8)
                //        {
                //            strsql = "Insert into #TmpClient(tmp_ac_code) Values ";
                //            strsql = strsql + "('" + Strings.Trim(Acts.Rows[i]["da_actno"].ToString()) + "')";
                //            mylib.ExecSQL(strsql, ObjConESTRO);
                //        }
                //    }


                //    if (chkAsOn == "1" & PrpFromDate != "")
                //    {
                //        strsql = "select cm_cd as hld_ac_code,cast(round(abs(sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end)),3) as decimal (15,3)) as hld_ac_pos,td_ac_type,sc_decimal_allow,bt_description as 'Balance Type'";
                //        strsql = strsql + " From TrxDetail, Client_master, security, Branch_master, beneficiary_type ";
                //        strsql = strsql + " Where td_ac_code = cm_cd and td_ac_type = bt_code And td_isin_code = sc_isincode and cm_brboffcode = bm_branchcd and cm_active = '01' ";
                //        strsql = strsql + " and td_ac_type Not in ('17','18') and td_category <> '03' and td_booking_type Not in('02')";
                //        strsql = strsql + " and td_narration not in('001') and td_rdate <'" + PrpFromDate.Trim() + "' and td_curdate <='" + PrpFromDate.Trim() + "'";
                //        strsql = strsql + " and td_ac_code in (Select tmp_ac_code from #TmpClient) and td_isin_code = '" + txtisin.Trim() + "'"; // and td_ac_type ='22'"
                //        if (StrHldType.Trim() != "")
                //        {
                //            strsql = strsql + " and td_ac_type in ( " + StrHldType + " )";
                //        }
                //        strsql = strsql + " Group by cm_cd ,td_ac_type,sc_decimal_allow,bt_description ";
                //        strsql = strsql + " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0";
                //        strsql = strsql + " Order by cm_cd, td_ac_type";
                //    }
                //    else
                //    {
                //        // strsql = "select hld_ac_code, hld_ac_pos ,sc_decimal_allow from holding,security where hld_isin_code = sc_isincode and hld_isin_code='" + txtisin.Trim() + "' and hld_ac_type ='22' and hld_ac_code in (Select tmp_ac_code from #TmpClient) ";

                //        strsql = "select hld_ac_code, hld_ac_pos,sc_decimal_allow ,bt_description as 'Balance Type' from holding,security ,beneficiary_type where hld_isin_code = sc_isincode  and hld_ac_type = bt_code and hld_isin_code='" + txtisin.Trim() + "'";
                //        if (StrHldType.Trim() != "")
                //        {
                //            strsql = strsql + " and hld_ac_type in ( " + StrHldType + " )";
                //        }

                //       // strsql = strsql + "and hld_ac_type='22' and hld_ac_code in (Select tmp_ac_code from #TmpClient)";
                //        strsql = strsql + "and hld_ac_code in (Select tmp_ac_code from #TmpClient)";
                //    }
                //    Estro = mylib.OpenDataTable(strsql, ObjConESTRO);

                //    if (Estro.Rows.Count == 0)
                //    {

                //    }
                //    else
                //    {
                //        int intDecimal = 0;
                //        for (i = 0; i <= Estro.Rows.Count - 1; i++) // Cross
                //        {
                //            intDecimal = ((Convert.ToInt32(Estro.Rows[i]["sc_decimal_allow"].ToString()) > 0 ? 3 : 0));
                //            for (j = 0; j <= Acts.Rows.Count - 1; j++) // Tradeplus
                //            {
                //                if ((Strings.Trim(Acts.Rows[j]["da_actno"].ToString()) ?? "") == (Strings.Trim(Estro.Rows[i]["hld_ac_code"].ToString()) ?? ""))
                //                {
                //                    Acts.Rows[j]["Holding"] = Estro.Rows[i]["hld_ac_pos"].ToString();
                //                    Acts.Rows[j]["value"] = Convert.ToDecimal(Estro.Rows[i]["hld_ac_pos"]) * 3;
                //                    Acts.Rows[j]["Type"] = Estro.Rows[i]["Balance Type"].ToString();
                //                }
                //            }
                //        }
                //    }

                //}

                //for (j = 0; j <= Acts.Rows.Count - 1; j++) // Tradeplus
                //{
                //    if (blnCDSL)
                //    {
                //        if (Acts.Rows[j]["Holding"].ToString() == "0.0")
                //        {
                //            Acts.Rows[j].Delete();
                //            // count = count + 1;
                //        }
                //    }
                //    else if (blnNSDL)
                //    {
                //        if (Acts.Rows[j]["Holding"].ToString() == "0.0")
                //        {
                //            Acts.Rows[j].Delete();
                //            // count = count + 1;
                //        }
                //    }
                //}
                //Acts.AcceptChanges();
            }
            return Acts;
        }


        public DataTable GetBenHoldingService(string Code, string BIOCode, string chkAsOn, string PrpFromDate, string ObjLBlName,string StrHldType)
        {
            Boolean blnfromCross;
            DataTable Acts = new DataTable();

            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strsql = "";
            BIOCode = BIOCode.Trim();
            string hld_ac_Code1 = BIOCode.Trim();
            string AsOnDate = PrpFromDate;
            PrpFromDate = myutil.dtos(PrpFromDate);
            SqlConnection ObjConCross = null;
            SqlConnection ObjConEstro = null;
            // SqlConnection ObjConCross = mydbutil.crostemp_conn("Cross");
            // SqlConnection ObjConEstro = mydbutil.crostemp_conn("Estro");

            if (hld_ac_Code1.Length > 8)
            {
                ObjConCross = mydbutil.crostemp_conn("Cross");
                if (ObjConCross != null)
                {
                    if (ObjConCross.State == ConnectionState.Closed)
                    {
                        ObjConCross.Open();
                    }
                }
            }
            else
            {

                ObjConEstro = mydbutil.crostemp_conn("Estro");
                if (ObjConEstro != null)
                {
                    if (ObjConEstro.State == ConnectionState.Closed)
                    {
                        ObjConEstro.Open();
                    }
                }
            }

            strsql = "SELECT * FROM Client_master , family_master,group_master , branch_master, beneficiary_status WHERE cm_familycd = fm_cd and cm_groupcd = gr_cd and cm_brboffcode = bm_branchcd and cm_active = bs_code and cm_cd = '" + BIOCode.Trim() + "'";
            DataTable dtBeninfo = null;
            if (Strings.Trim(BIOCode).Length > 8)
            {

                dtBeninfo = mylib.OpenDataTable(strsql, ObjConCross);

            }
            else
            {
                dtBeninfo = mylib.OpenDataTable(strsql, ObjConEstro);
            }

            if (Strings.Trim(BIOCode).Length > 8)
            {
                dtBeninfo.Rows[0]["cm_opendate"] = myutil.stod(dtBeninfo.Rows[0]["cm_opendate"].ToString().Trim()).ToString("dd/MM/yyyy");
                System.Web.HttpContext.Current.Session["dtBeninfo"] = dtBeninfo;
            }
            else
            {
                string newFormat = DateTime.Parse(dtBeninfo.Rows[0]["cm_opendate"].ToString().Trim()).ToString("dd/MM/yyyy");
                dtBeninfo.Rows[0]["cm_opendate"] = newFormat;
            }

            System.Web.HttpContext.Current.Session["dtBeninfo"] = dtBeninfo;

            strsql = "drop table #TmpHolding";
            if (Strings.Trim(BIOCode).Length > 8)
            { mylib.ExecSQL(strsql, ObjConCross); }
            else
            { mylib.ExecSQL(strsql, ObjConEstro); }


            strsql = "Create Table #TmpHolding (hld_ac_code Char(16),hld_isin_code Char(12),hld_ac_pos money,hld_ac_type char(4),hld_Rate Money,hld_hold_date char(8))";
            if (Strings.Trim(BIOCode).Length > 8)
            { mylib.ExecSQL(strsql, ObjConCross); }
            else
            { mylib.ExecSQL(strsql, ObjConEstro); }

            if (chkAsOn == "1" & PrpFromDate != "")
            {
                if (Strings.Trim(BIOCode).Length > 8)
                {
                    strsql = "Insert into #TmpHolding(hld_ac_code,hld_isin_code,hld_ac_pos,hld_ac_type,hld_rate,hld_hold_date) ";
                    strsql += " select cm_cd ,td_isin_code,sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end),td_ac_type,0,''";
                    strsql += " From TrxDetail, Client_master, Security,Branch_master";
                    strsql += " Where td_ac_code = cm_cd And td_isin_code = sc_isincode and cm_brboffcode = bm_branchcd and cm_active = '01'";
                    strsql += " and td_curdate <='" + PrpFromDate.Trim() + "'";
                    strsql = strsql + " and td_ac_code = '" + hld_ac_Code1.Trim() + "' ";
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and td_ac_type in ( " + StrHldType + " )";
                    }
                    strsql += " Group by cm_cd ,td_ac_type ,td_isin_code,sc_isinname,cm_clienttype";
                    strsql += " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0 ";

                    mylib.ExecSQL(strsql, ObjConCross);


                    strsql = "Update #TmpHolding set hld_Rate = IsNull((select rm_rate From Rate_master Where rm_isin_code = hld_isin_code and rm_trx_date = (select Max(rm_trx_date) From Rate_master Where rm_isin_code = hld_isin_code and rm_trx_date <= '" + PrpFromDate.Trim() + "')),0) ";
                    mylib.ExecSQL(strsql, ObjConCross);

                    strsql = "select convert(char, convert(datetime,hld_hold_date),112) as [Holding Date],hld_isin_code as ISIN,rtrim(sc_isinname) AS 'Isin Name',cast(round(abs(hld_ac_pos),3) as decimal (15,3)) as Holding,hld_Rate as 'Rate',cast(round(abs(Sum(hld_ac_pos * hld_Rate)),2) as decimal (15,2)) as  Value,'' as '%',bt_description as 'Balance Type', isnull(sc_decimal_allow,'0') as allowdecimal";
                    strsql += " From #TmpHolding ,Security ,Client_master ,Beneficiary_type,branch_master ";
                    strsql += " where cm_brboffcode = bm_branchcd  and hld_isin_code = sc_isincode And hld_ac_code = cm_cd and bt_code = hld_ac_type and cm_active = '01'";
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and hld_ac_type in ( " + StrHldType + " )";
                    }
                    strsql = strsql + " Group By hld_hold_date,hld_isin_code,sc_isinname,hld_ac_pos,hld_Rate,bt_description,sc_decimal_allow,hld_ac_type";
                    strsql += " Order by rtrim(sc_isinname), hld_ac_type ";
                    mylib.ExecSQL(strsql, ObjConCross);
                }
                else
                {
                    strsql = "Insert into #TmpHolding(hld_ac_code,hld_isin_code,hld_ac_pos,hld_ac_type,hld_rate,hld_hold_date) ";
                    strsql += " select cm_cd ,td_isin_code,sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end),td_ac_type,0,'' ";
                    strsql += " From TrxDetail, Client_master, Security,Branch_master";
                    strsql += " Where td_ac_code = cm_cd And td_isin_code = sc_isincode and cm_brboffcode = bm_branchcd and cm_active = '01'";
                    strsql += " and td_ac_type Not in ('17','18') and td_category <> '03' and td_booking_type Not in('02') ";
                    strsql += " and td_narration not in('001') and td_rdate <'" + PrpFromDate.Trim() + "'";
                    strsql += " and td_curdate <='" + PrpFromDate.Trim() + "'";
                    strsql = strsql + " and td_ac_code = '" + hld_ac_Code1.Trim() + "' ";
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and td_ac_type in ( " + StrHldType + " )";
                    }
                    strsql += " Group by cm_cd ,td_ac_type ,td_isin_code,sc_isinname,cm_clienttype";
                    strsql += " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0 ";
                    mylib.ExecSQL(strsql, ObjConEstro);

                    strsql = "Insert into #TmpHolding(hld_ac_code,hld_isin_code,hld_ac_pos,hld_ac_type,hld_rate,hld_hold_date) ";
                    strsql += " select cm_cd ,td_isin_code,sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end),td_ac_type,0,'' ";
                    strsql += " From TrxDetail, Client_master, Security,Branch_master";
                    strsql += " Where td_ac_code = cm_cd And td_isin_code = sc_isincode and cm_brboffcode = bm_branchcd and cm_active = '01'";
                    strsql += " and td_ac_type in ('17','18','23','26') and td_category <> '03' and td_booking_type Not in('02') ";
                    strsql += " and td_narration not in('001') and td_rdate >'" + PrpFromDate.Trim() + "'";
                    strsql += " and td_curdate <='" + PrpFromDate.Trim() + "'";
                    strsql = strsql + " and td_ac_code = '" + hld_ac_Code1.Trim() + "' ";
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and td_ac_type in ( " + StrHldType + " )";
                    }
                    strsql += " Group by cm_cd ,td_ac_type ,td_isin_code,sc_isinname,cm_clienttype";
                    strsql += " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0 ";
                    mylib.ExecSQL(strsql, ObjConEstro);

                    strsql = "Insert into #TmpHolding(hld_ac_code,hld_isin_code,hld_ac_pos,hld_ac_type,hld_rate,hld_hold_date) ";
                    strsql += " select cm_cd ,td_isin_code,sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end),td_ac_type,0,'' ";
                    strsql += " From TrxDetail, Client_master, Security,Branch_master";
                    strsql += " Where td_ac_code = cm_cd And td_isin_code = sc_isincode and cm_brboffcode = bm_branchcd and cm_active = '01'";
                    strsql += " and td_category = '03' and td_booking_type Not in('02') and td_ac_type <> '30'";
                    strsql += " and td_curdate <='" + PrpFromDate.Trim() + "'";
                    strsql = strsql + " and td_ac_code = '" + hld_ac_Code1.Trim() + "' ";
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and td_ac_type in ( " + StrHldType + " )";
                    }
                    strsql += " Group by cm_cd ,td_ac_type ,td_isin_code,sc_isinname,cm_clienttype";
                    strsql += " having sum(case td_debit_credit when 'C' then isnull(td_qty,0) else (-1) * isnull(td_qty,0) end) > 0 ";
                    mylib.ExecSQL(strsql, ObjConEstro);

                    strsql = "Update #TmpHolding set hld_Rate = IsNull((select rm_rate From Rate_master Where rm_isin_code = hld_isin_code and rm_trx_date = (select Max(rm_trx_date) From Rate_master Where rm_isin_code = hld_isin_code and rm_trx_date <= '" + PrpFromDate.Trim() + "')),0) ";
                    mylib.ExecSQL(strsql, ObjConEstro);

                    strsql = "select convert(char, convert(datetime,hld_hold_date),112) as [Holding Date],hld_isin_code as Isin,rtrim(sc_company_name) +' ('+ rtrim(sc_isinname) + ')' AS 'Isin Name',cast(round(abs(hld_ac_pos),3) as decimal (15,3)) as Holding,hld_Rate as 'Rate',cast(round(abs(Sum(hld_ac_pos * hld_Rate)),2) as decimal (15,2)) as  Value,'' as '%',";
                    strsql += " bt_description as 'Balance Type' ,'' ,isnull(sc_decimal_allow,'0') as allowdecimal";
                    strsql += " From #TmpHolding,Security ,Client_master ,Beneficiary_type,branch_master,Beneficiary_status,Beneficiary_category ";
                    strsql += " where cm_brboffcode = bm_branchcd  and hld_isin_code = sc_isincode And hld_ac_code = cm_cd and bt_code = hld_ac_type and cm_active = '01'";
                    strsql += " and cm_active = bs_code and cm_acctype = bc_code";
                    strsql += " Group by hld_hold_date,hld_isin_code,sc_company_name,sc_isinname,hld_ac_pos,hld_Rate,bt_description,cm_cd,hld_ac_type,sc_decimal_allow";
                    strsql += " Order By cm_cd , rtrim(sc_company_name) +' ('+ rtrim(sc_isinname) + ')' , hld_ac_type ";
                }
            }
            else
            {
                if (Strings.Trim(BIOCode).Length > 8)
                {
                    blnfromCross = true;
                    strsql = "Select  convert(char, convert(datetime,hld_hold_date),112) as [Holding Date],sc_isincode as ISIN,rtrim(sc_isinname) AS 'Isin Name' ,cast(round(abs(hld_ac_pos),3) as decimal (15,3)) as Holding,cast(sc_rate as decimal(15,2)) as 'Rate',cast(round(abs(Sum(hld_ac_pos * sc_rate)),2) as decimal (15,2)) as  Value,'' as '%',bt_description as 'Balance Type', isnull(sc_decimal_allow,'0') as allowdecimal , '" + ObjLBlName + " [ " + BIOCode + " ]" + "' as Code,sc_ratedt [Rate Date] from Holding , security ,beneficiary_type Where  hld_ac_type = bt_code and hld_isin_Code = sc_isinCode ";
                    if (hld_ac_Code1 != "")
                    {
                        strsql = strsql + " and hld_ac_Code = '" + hld_ac_Code1.Trim() + "' ";
                    }
                    else
                    {
                    }
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and hld_ac_type in ( " + StrHldType + " )";
                    }
                    strsql = strsql + " Group By sc_isincode ,bt_description, sc_isinname,hld_ac_pos,hld_hold_date,sc_company_name,sc_decimal_allow,sc_rate,sc_ratedt ";
                    strsql = strsql + " order by rtrim(sc_isinname) ";
                }
                else
                {
                    blnfromCross = false;
                    strsql = "Select convert(char, convert(datetime,hld_hold_date),112) as [Holding Date],sc_isincode as Isin,rtrim(sc_company_name) +' ('+ rtrim(sc_isinname) + ')' AS 'Isin Name' ,cast(round(abs(hld_ac_pos),3) as decimal (15,3)) as Holding,cast(sc_rate as decimal(15,2)) as 'Rate',cast(round(abs(Sum(hld_ac_pos * sc_rate)),2) as decimal (15,2)) as  Value,'' as '%',bt_description as 'Balance Type' ,  isnull(sc_decimal_allow,'0') as allowdecimal ,  '" + ObjLBlName + " [ " + BIOCode + " ]" + "' as Code";
                    strsql = strsql + " from Holding , security, beneficiary_type Where hld_ac_type = bt_code and hld_isin_Code = sc_isinCode ";
                    if (hld_ac_Code1 != "")
                    {
                        strsql = strsql + " and hld_ac_Code = '" + hld_ac_Code1.Trim() + "' ";
                    }
                    else
                    {
                    }
                    if (StrHldType.Trim() != "")
                    {
                        strsql = strsql + " and hld_ac_type in ( " + StrHldType + " )";
                    }
                    strsql += " Group By sc_isincode , sc_isinname,hld_ac_pos,hld_hold_date,sc_company_name,bt_description,sc_rate,sc_decimal_allow";
                    strsql = strsql + " order by rtrim(sc_company_name) +' ('+ rtrim(sc_isinname) + ')' ";
                }
            }
            if (Strings.Trim(BIOCode).Length > 8)
            { Acts = mylib.OpenDataTable(strsql, ObjConCross); }
            else
            { Acts = mylib.OpenDataTable(strsql, ObjConEstro); }
            if (Acts.Rows.Count > 0)
            {
                for (int i = 0; i < Acts.Rows.Count; i++)
                {

                    {
                        if (chkAsOn == "1" & PrpFromDate != "")
                        {
                            System.Web.HttpContext.Current.Session["Date"] = AsOnDate;
                        }
                        else if (Strings.Trim(BIOCode).Length > 8)
                        {

                            if (Acts.Rows[i]["Holding Date"].ToString().Trim() != "")
                            {
                                Acts.Rows[i]["Holding Date"] = myutil.DbToDate(Acts.Rows[i]["Holding Date"].ToString());
                                System.Web.HttpContext.Current.Session["Date"] = Acts.Rows[i]["Holding Date"].ToString();
                            }
                            if (Acts.Rows[i]["Rate Date"].ToString().Trim() != "")
                            {
                                Acts.Rows[i]["Rate Date"] = myutil.DbToDate(Acts.Rows[i]["Rate Date"].ToString().Trim());
                                System.Web.HttpContext.Current.Session["RateDate"] = Acts.Rows[i]["Rate Date"].ToString();
                            }
                        }
                        else
                        {
                            DataTable RateDate = null;

                            strsql = "select convert (char, convert(datetime,max(rm_trx_date)),103) from Rate_master";
                            RateDate = mylib.OpenDataTable(strsql, ObjConEstro);
                            System.Web.HttpContext.Current.Session["RateDate"] = RateDate.Rows[0][0].ToString().Trim();

                            Acts.Rows[i]["Holding Date"] = myutil.stod(Acts.Rows[i]["Holding Date"].ToString().Trim()).ToString("dd/MM/yyyy");
                            System.Web.HttpContext.Current.Session["Date"] = Acts.Rows[i]["Holding Date"].ToString();
                        }
                    }
                }
            }
            return Acts;
        }

        public DataTable ProcessBenHolding(string Code, bool blnSum)
        {
            DataTable dtHold = new DataTable();
            string[] Cross = new string[2];
            string[] Estro = new string[2];
            bool bCross = false;
            bool bEstro = false;

            LibraryModel mylib = new LibraryModel();

            strSQL = "Select OP_Server,OP_DataBase,OP_Owner,OP_User from Other_Products Where OP_Product = 'CROSS' and op_Status = 'A'";
            DataTable dtCross = mylib.OpenDataTable(strSQL);
            if (dtCross.Rows.Count > 0)
            {
                DataRow dr = dtCross.Rows[0];

                bCross = true;
                Cross = new string[3];
                Cross[0] = dr["OP_Server"].ToString().Trim();
                Cross[1] = dr["OP_DataBase"].ToString().Trim();
                Cross[2] = dr["OP_Owner"].ToString().Trim();
            }

            strSQL = "Select OP_Server,OP_DataBase,OP_Owner,OP_User,OP_PWD from Other_Products Where OP_Product = 'ESTRO' and op_Status = 'A'";
            DataTable dtEstro = mylib.OpenDataTable(strSQL);
            if (dtEstro.Rows.Count > 0)
            {
                DataRow dr = dtEstro.Rows[0];
                bEstro = true;
                Estro = new string[3];
                Estro[0] = dr["OP_Server"].ToString().Trim();
                Estro[1] = dr["OP_DataBase"].ToString().Trim();
                Estro[2] = dr["OP_Owner"].ToString().Trim();
            }
            if (!bCross && !bEstro)
            {
                return dtHold;
            }

            strSQL = "";

            if (blnSum)
            {
                if (bCross)
                {
                    strSQL = "select isnull(sum(cast((( a.hld_ac_pos * sc_Rate)) as decimal(15,2))),0)  as valuation" +
                            " from [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Holding a, [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Security b , " +
                            " [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Beneficiary_type d , dematact da " +
                            " where da.da_clientcd = '" + Code + "'  " +
                            " and a.hld_ac_code = da.da_actno " +
                            " and a.hld_isin_code = b.sc_isincode   " +
                            " and d.bt_code = a.hld_ac_type ";
                }
                if (bEstro)
                {
                    if (strSQL != "")
                    {
                        strSQL += " Union All ";
                    }
                    strSQL += "select isnull(sum(cast((( a.hld_ac_pos * sc_Rate)) as decimal(15,2))),0)  as valuation" +
                            " from [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].Holding a, [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].Security b , " +
                            " [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].Beneficiary_type d , dematact da " +
                            " where da.da_clientcd = '" + Code + "'  " +
                            " and a.hld_ac_code = da.da_actno " +
                            " and a.hld_isin_code = b.sc_isincode   " +
                            " and d.bt_code = a.hld_ac_type ";
                }

                strSQL = "Select isnull(Convert(decimal(15,2), sum(valuation)),0) from ( " + strSQL + " ) X ";
            }
            else
            {
                strSQL = "";
                if (bCross)
                {
                    strSQL = "select cm.cm_cd, cm.cm_name, a.hld_isin_code,b.sc_company_name,b.sc_isinname,cast((a.hld_ac_pos) as decimal(15,2)) hld_ac_pos,a.hld_ac_type, " +
                            " d.bt_description 'bt_description', hld_market_type,a.hld_settlement,cast((sc_rate) as decimal(15,2)) as sc_security_rate, " +
                            " cast(( ( a.hld_ac_pos * sc_Rate)) as decimal(15,2))  as valuation,bt_description as BType " +
                            " from [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Holding a, [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Security b , " +
                            " [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Beneficiary_type d , dematact da, " +
                            " [" + Cross[0] + "].[" + Cross[1] + "].[" + Cross[2] + "].Client_master cm " +
                            " where da.da_clientcd = '" + Code + "'  and cm.cm_cd = hld_ac_code " +
                            " and a.hld_ac_code = da.da_actno " +
                            " and a.hld_isin_code = b.sc_isincode   " +
                            " and d.bt_code = a.hld_ac_type ";
                }
                else if (bEstro)
                {
                    strSQL = "select a.hld_isin_code,b.sc_company_name,b.sc_isinname,cast((a.hld_ac_pos) as decimal(15,2)) hld_ac_pos,a.hld_ac_type, " +
                            " d.bt_description 'bt_description', hld_market_type,a.hld_settlement,cast((sc_rate) as decimal(15,2)) as sc_security_rate, " +
                            " cast(( ( a.hld_ac_pos * sc_Rate)) as decimal(15,2))  as valuation, bt_description as BType" +
                            " from " + Estro[0].Trim() + "." + Estro[1].Trim() + "." + Estro[2].Trim() + ".Holding a," +
                            Estro[0].Trim() + "." + Estro[1].Trim() + "." + Estro[2].Trim() + ".Security b ," +
                            Estro[0].Trim() + "." + Estro[1].Trim() + "." + Estro[2].Trim() + ".Beneficiary_type d, dematact da, " +
                            " [" + Estro[0] + "].[" + Estro[1] + "].[" + Estro[2] + "].Client_master cm " +
                            " where da.da_clientcd = '" + Code + "'  and cm.cm_cd = hld_ac_code " +
                            " and a.hld_ac_code = da.da_actno " +
                            " and a.hld_isin_code = b.sc_isincode   " +
                            " and d.bt_code = a.hld_ac_type ";
                }
            }
            dtHold = mylib.OpenDataTable(strSQL);
            return dtHold;
        }

        public void prCreateTempHolding()
        {

            //if (Trim(ObjLBl.Text).Length > 8)
            //{
            //    sqlConn = objApplicationUser.crostemp_conn("CROSS");
            //    if (sqlConn.State == ConnectionState.Closed)
            //    {
            //        sqlConn.Open();
            //    }
            //}
            //else
            //{
            //    sqlConnEstro = objApplicationUser.crostemp_conn("ESTRO");
            //    if (sqlConnEstro.State == ConnectionState.Closed)
            //    {
            //        sqlConnEstro.Open();
            //    }
            //}

            //try
            //{
            //    strsql = "Drop Table #TmpHolding";
            //    var ObjCmdIP = new SqlCommand(strsql, IIf(Trim(ObjLBl.Text).Length > 8, sqlConn, sqlConnEstro));
            //    ObjCmdIP.ExecuteNonQuery();
            //}
            //catch (Exception objError)
            //{
            //    strsql = "Create Table #TmpHolding (hld_ac_code Char(16),hld_isin_code Char(12),hld_ac_pos money,hld_ac_type char(4),hld_Rate Money,hld_hold_date char(8))";
            //    var ObjCmdIP = new SqlCommand(strsql, IIf(Trim(ObjLBl.Text).Length > 8, sqlConn, sqlConnEstro));
            //    ObjCmdIP.ExecuteNonQuery();
            //}
        }



        public DataTable GetNextAMCDueReport(string Code, string cmbRep)
        {
            DataTable dt = null;
            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strsql = "";

            string CMSCHEDULE = "49843750";

            string strAMCBASEOn = string.Empty;
            string strBilling = string.Empty;
            string strfreefor = string.Empty;
            string strOpendate = string.Empty;

            string strLastBillDate = DateTime.Now.ToString("yyyyMMdd");
            string gstrToday = DateTime.Now.ToString("dd/MM/yyyy");
            SqlConnection curCon = new SqlConnection(connectionstring);
            SqlConnection objconnection1 = null;
            if (curCon.State == ConnectionState.Closed)
            {
                curCon.Open();
            }


            SqlDataAdapter ObjAdapter1 = new SqlDataAdapter("select * from other_products where op_status='A' AND op_product='CROSS'", curCon);
            DataTable Objdt1 = new DataTable();
            ObjAdapter1.Fill(Objdt1);

            if (Objdt1.Rows.Count > 0)
            {
                objconnection1 = mydbutil.crostemp_conn("Cross"); ;

            }
            SqlDataAdapter ObjAdpEstro = new SqlDataAdapter("select * from other_products where op_status='A' AND op_product='Estro'", curCon);
            DataTable ObjdtEstro = new DataTable();
            ObjAdpEstro.Fill(ObjdtEstro);

            if (ObjdtEstro.Rows.Count > 0)
            {
                objconnection1 = mydbutil.crostemp_conn("Estro"); ;

            }


            try
            {
                strsql = " CREATE TABLE #TempClient ";
                strsql = strsql + "( tmp_cmcd Varchar(16) Not Null, ";
                strsql = strsql + " tmp_cmname varchar (100) Not Null ,";
                strsql = strsql + " tmp_yearmonth Char(8) Not Null) ";
                mylib.ExecSQL(strsql, curCon);

            }
            catch (Exception objError)
            {
                strsql = "Drop Table #TmpClient";
                mylib.ExecSQL(strsql, curCon);

            }

            strsql = " select cm_cd as Code, cm_name, cd_freq , cm_billcode , cm_opendate , cd_FreeFor , cd_ProRata , cd_BaseOn, cm_name ";
            strsql = strsql + " From Client_master, Chargesdetail  Where cm_chgsscheme = cd_scheme And cd_code = 4  ";
            strsql = strsql + " and cd_freq in ('Y','H','Q','M') and cm_schedule = " + CMSCHEDULE;  // & strAdvancefilter

            if (Code != "")
            {
                if (cmbRep == "BOId")
                {
                    strsql = strsql + " and cm_cd='" + Code.Trim() + "' ";
                }
                else if (cmbRep == "Group")
                {
                    strsql = strsql + " and cm_groupcd='" + Code.Trim() + "' ";
                }
                else if (cmbRep == "Family")
                {
                    strsql = strsql + " and cm_familycd='" + Code.Trim() + "' ";
                }
            }


            strsql = strsql + " and ((cm_active = '01' and cm_acc_closuredate = '') Or ( Left(cm_acc_closuredate,6) > '" + Strings.Left(strLastBillDate, 6) + "'))";

            strsql = strsql + " order by cm_cd";

            dt = mylib.OpenDataTable(strsql, objconnection1);


            if (dt.Rows.Count > 0)
            {
                string strAmtDt = string.Empty;
                short K = 0;
                var loopTo = dt.Rows.Count - 1;
                for (K = 0; K <= loopTo; K++)
                {
                    strAMCBASEOn = dt.Rows[K]["cd_BaseOn"].ToString();
                    strBilling = dt.Rows[K]["cd_freq"].ToString();
                    strfreefor = dt.Rows[K]["cd_FreeFor"].ToString();

                    if (dt.Rows.Count > 0)
                    {
                        strOpendate = dt.Rows[K]["cm_opendate"].ToString();
                    }

                    //if (ObjDataSetEstro1.Tables(0).Rows.Count > 0)
                    //{
                    //    strOpendate = objUtility.dtos(ObjDataSet.Tables(0).Rows(K).Item("cm_opendate"));
                    //}
                    // strOpendate = objUtility.dtos(ObjDataSet.Tables(0).Rows(K).Item("cm_opendate"))

                    if (strAMCBASEOn == "F" & Strings.InStr(1, "1Y,2Y,3Y,4Y,5Y", strfreefor) > 0)
                    {
                        //strAmtDt = mylib.mfnGetAccstartdatefromdate(strOpendate, objUtility.eNewDateformat.EDATABASE);
                        //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eDay, -1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                    }
                    else
                    {
                        strAmtDt = Strings.Left(strOpendate, 6) + "01";
                    }

                    var switchExpr = strfreefor;
                    switch (switchExpr)
                    {
                        //case "1M":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }

                        //case "1Q":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 3, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }

                        //case "1Y":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eYear, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }

                        //case "2Y":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eYear, 2, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }

                        //case "3Y":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eYear, 3, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }

                        //case "4Y":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eYear, 4, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }

                        //case "5Y":
                        //    {
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eYear, 5, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //        break;
                        //    }
                    }

                    if (strAMCBASEOn == "A")
                    {
                        //if (strBilling == "Y")
                        //{
                        //    while (strLastBillDate >= strAmtDt)
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eYear, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //}
                        //else if (strBilling == "H")
                        //{
                        //    while (strLastBillDate >= strAmtDt)
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 6, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //}
                        //else if (strBilling == "Q")
                        //{
                        //    while (strLastBillDate >= strAmtDt)
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 3, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                        //}
                        //else if (strBilling == "M")
                        //{
                        //    while (strLastBillDate >= strAmtDt)
                        //        strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);F:\tradenetX_sir\Final\TRADENET\TRADENET\Models\TradenetGlobalModel.cs
                        //}
                    }
                    else
                    {
                        var switchExpr1 = strBilling;
                        switch (switchExpr1)
                        {
                            case "Y":
                                {
                                    //strAmtDt = ObjUtility.mfnGetAccenddatefromdate(gstrToday, objUtility.eNewDateformat.EDATABASE);
                                    //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    strAmtDt = "20210430";
                                    break;
                                }

                            case "H":
                                {
                                    //strAmtDt = ObjUtility.mfnGetAccstartdatefromdate(gstrToday, objUtility.eNewDateformat.EDATABASE);
                                    //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eDay, -1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    //while (ObjUtility.mfnDateDiff(objUtility.eAddDate.eDay, strAmtDt, strLastBillDate) > 0)
                                    //    strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 6, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    break;
                                }

                            case "Q":
                                {
                                    //strAmtDt = ObjUtility.mfnGetAccstartdatefromdate(gstrToday, objUtility.eNewDateformat.EDATABASE);
                                    //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eDay, -1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    //while (ObjUtility.mfnDateDiff(objUtility.eAddDate.eDay, strAmtDt, strLastBillDate) > 0)
                                    //    strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 3, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    break;
                                }

                            case "M":
                                {
                                    //strAmtDt = ObjUtility.mfnFormatdate(gstrToday, objUtility.eNewDateformat.EDATABASE);
                                    //strAmtDt = ObjUtility.mfnFormatdate(ObjUtility.mfnDateAdd(objUtility.eAddDate.eMonth, 1, strAmtDt), objUtility.eNewDateformat.EDATABASE);
                                    break;
                                }
                        }

                    }



                    strAmtDt = strAmtDt.Substring(4, 2) + "-" + strAmtDt.Substring(0, 4);
                    strsql = "insert into #TempClient ";
                    strsql = strsql + " values ('" + dt.Rows[K]["Code"].ToString() + "','" + dt.Rows[K]["cm_name"].ToString() + "','" + strAmtDt + "')";
                    mylib.ExecSQL(strsql, curCon);
                }
            }

            strsql = "select * from #TempClient ";
            dt = mylib.OpenDataTable(strsql, curCon);
            return dt;

        }


        public DataTable GetCrossLedgerReport(string Code, string ChkListOfBal, string FromDate, string ToDate, string cmbClient, string DDlOnlyBal)
        {
            DataTable dt = new DataTable();
            string strsql = "";
            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strISIN = "";
            decimal strBal = 0;

            FromDate = myutil.dtos(FromDate);
            ToDate = myutil.dtos(ToDate);

            //if (ChkListOfBal.Checked == false)
            //{
            //    if (cmbClient.SelectedValue == "BackOfficeCd")
            //    {
            //        strsql = "Select count(0) from Client_master where cm_cd='" + Strings.Trim(TextClientCode.Text) + "'" + Session["LoginAcess"];
            //    }
            //    else
            //    {
            //        strsql = "Select count(0) from Dematact,Client_master where da_clientcd=cm_cd and da_actno='" + Strings.Trim(txtClFrom.Text) + "'" + Session["LoginAcess"];
            //    }

            //    strsql = strsql + Session["LoginAccess"];
            //    var Adpt1 = new SqlDataAdapter(strsql, ObjConnection);
            //    var ObjDs1 = new DataSet();
            //    Adpt1.Fill(ObjDs1);
            //    if (ObjDs1.Tables(0).Rows(0)(0) == 0)
            //    {
            //        if (cmbClient.SelectedValue == "BackOfficeCd")
            //        {
            //            strsql = Strings.Trim(TextClientCode.Text);
            //        }
            //        else
            //        {
            //            strsql = Strings.Trim(txtClFrom.Text);
            //        }

            //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Error", "alert('You dont have access to data of Client (" + strsql + ")');", true);
            //        if (cmbClient.SelectedValue == "BackOfficeCd")
            //        {
            //            TextClientCode.Text = "";
            //        }
            //        else
            //        {
            //            txtClFrom.Text = "";
            //        }

            //        TextClientName.Text = "";
            //        return;
            //    }
            //}
            SqlConnection curCon = new SqlConnection(connectionstring);
            SqlConnection objconnection1 = null;
            if (curCon.State == ConnectionState.Closed)
            {
                curCon.Open();
            }


            SqlDataAdapter ObjAdapter1 = new SqlDataAdapter("select * from other_products where op_status='A' AND op_product='Cross'", curCon);
            DataTable Objdt1 = new DataTable();
            ObjAdapter1.Fill(Objdt1);

            if (Objdt1.Rows.Count > 0)
            {
                objconnection1 = mydbutil.crostemp_conn("Cross"); ;

            }
            SqlDataAdapter ObjAdpEstro = new SqlDataAdapter("select * from other_products where op_status='A' AND op_product='Estro'", curCon);
            DataTable ObjdtEstro = new DataTable();
            ObjAdpEstro.Fill(ObjdtEstro);

            if (ObjdtEstro.Rows.Count > 0)
            {
                objconnection1 = mydbutil.crostemp_conn("Estro"); ;

            }


            if (ChkListOfBal == "0")
            {
                strsql = " select '0' as balance,cm_blsavingcd, cm_name + ' [ ' + ld_clientcd + ' ] ' as Client,cm_add1,cm_add2,cm_add3,cm_pin,ld_clientcd,ld_dt,ltrim(rtrim(convert(char,convert(datetime,ld_dt),103))) as ledgerdt,ld_particular,abs(cast(ld_amount as decimal(15,2))) as ld_amount,isnull(case when ld_amount > 0 then cast(ld_amount as decimal(15,2)) end,0) Debit,";
                strsql = strsql + " isnull(case when ld_amount < 0 then cast((ld_amount*-1) as decimal (15,2)) end,0)  Credit,ld_debitflag,ld_documenttype,ld_documentno, ";
                strsql = strsql + " 0 as opbal,cm_name,ld_documenttype + case when len(ltrim(Rtrim(ld_documentno))) < 6 then Replicate('0' , 6 - len(ltrim(Rtrim(ld_documentno)))) + ld_documentno else ld_documentno end as strVoucher,1 Flag  from Ledger,Client_master where cm_cd = ld_clientcd and ld_dt ";
                strsql = strsql + " between '" + FromDate + "' and '" + ToDate + "' ";

                if (cmbClient == "BackOfficeCd")
                {
                    strsql = strsql + " And cm_blsavingcd = '" + Strings.Trim(Code) + "'";
                }
                else
                {
                    strsql = strsql + " And  ld_clientcd = '" + Strings.Trim(Code) + "'";
                }

                strsql = strsql + " union all ";
                strsql = strsql + " select '0' as balance,cm_blsavingcd,cm_name + ' [ ' + ld_clientcd + ' ] ' as Client,cm_add1,cm_add2,cm_add3 ,cm_pin, ld_clientcd,'" + FromDate + "' as ld_dt,'" + FromDate + "' as ledgerdt,'',abs(cast(sum(ld_amount) as decimal(15,2))) , ";
                strsql = strsql + " isnull(case when sum(ld_amount) > 0 then cast(sum(ld_amount) as decimal(15,2)) end,0) Debit, ";
                strsql = strsql + " isnull(case when sum(ld_amount) < 0 then cast((sum(ld_amount)*-1) as decimal (15,2)) end,0)  Credit, ";
                strsql = strsql + " case sign(sum(ld_amount)) when -1 then 'C' else 'D' end as ld_debitflag , ";
                strsql = strsql + " '' as ld_documenttype,'' as ld_documentno, sum(ld_amount),cm_name,'' as strVoucher, 0 Flag    from Ledger, Client_master ";
                strsql = strsql + " where ld_dt< '" + FromDate + "' and ld_clientcd=cm_cd  "; // Trim(MyDatecontrol.PrpFromDateString)
                if (cmbClient == "BackOfficeCd")
                {
                    strsql = strsql + " And cm_blsavingcd = '" + Strings.Trim(Code) + "'";
                }
                else
                {
                    strsql = strsql + " And  ld_clientcd = '" + Strings.Trim(Code) + "'";
                }

                strsql = strsql + " group by cm_blsavingcd,cm_name,ld_clientcd, cm_add1,cm_add2,cm_add3,cm_pin having sum(ld_amount) <> 0 ";
                strsql = strsql + " order by ld_clientcd,Flag,ld_dt ,strvoucher  ";

                dt = mylib.OpenDataTable(strsql, objconnection1);
                System.Web.HttpContext.Current.Session["Data"] = dt;




                int intTotRowCnt;

                if (dt.Rows.Count > 0)
                {
                    short i;
                    string strcode = "";

                    intTotRowCnt = dt.Rows.Count - 1;

                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        if (strcode != dt.Rows[i]["cm_blsavingcd"].ToString().Trim())
                        {
                            strBal = 0;
                            strcode = dt.Rows[i]["cm_blsavingcd"].ToString().Trim();
                        }

                        strBal = strBal + Convert.ToDecimal(dt.Rows[i]["Debit"]) - Convert.ToDecimal(dt.Rows[i]["Credit"]);
                        dt.Rows[i]["Balance"] = strBal;
                        if (Convert.ToDecimal(dt.Rows[i]["Balance"]) > 0)
                        {
                            dt.Rows[i]["Balance"] = dt.Rows[i]["Balance"].ToString().Replace("-", "") + "Dr";
                        }
                        else if (Convert.ToDecimal(dt.Rows[i]["Balance"]) < 0)
                        {
                            dt.Rows[i]["Balance"] = dt.Rows[i]["Balance"].ToString().Replace("-", "") + "Cr";
                        }
                        else
                        {
                            dt.Rows[i]["Balance"] = "Nil";
                        }

                        if (dt.Rows[i]["Flag"].ToString() == "0")
                        {
                            dt.Rows[i]["ld_particular"] = "Opening Balance";
                        }


                    }

                }

                else
                {

                }


            }

            if (ChkListOfBal == "1")
            {

                strsql = "select ld_clientcd, cm_name, cm_BlSavingCd,";
                strsql = strsql + " cast((sum(case when ld_dt < '" + FromDate + "' then ld_Amount else 0 end)) as decimal(15,2)) as ld_OpenBal,cast((sum(ld_Amount)) as decimal (15,2)) as ld_CloseBal";
                strsql = strsql + " from ledger, client_master ";
                strsql = strsql + " where ld_clientcd=cm_cd and ld_dt <= '" + ToDate + "'";
                strsql = strsql + " and cm_schedule = " + mylib.GetSysPARM("CMSCHEDULE", curCon);


                if (cmbClient == "BackOfficeCd")
                {
                    strsql = strsql + " And cm_blsavingcd = '" + Strings.Trim(Code) + "'";
                }
                else
                {
                    strsql = strsql + " And  ld_clientcd = '" + Strings.Trim(Code) + "'";
                }

                strsql = strsql + " group by ld_clientcd, cm_name, cm_blsavingCd ";

                if (DDlOnlyBal == "Debit")
                {
                    strsql = strsql + " having sum(ld_amount) > 0";
                }
                else if (DDlOnlyBal == "Credit")
                {
                    strsql = strsql + " having sum(ld_amount) < 0";
                }
                else if (DDlOnlyBal == "All")
                {
                    strsql = strsql + " having sum(ld_amount) <= 0";
                }

                dt = mylib.OpenDataTable(strsql, objconnection1);
            }




            System.Web.HttpContext.Current.Session["Data"] = dt;








            return dt;


        }


        public DataTable GetTransactionStatementReport(string Code, string FromDate, string ToDate, string cmbClient)
        {
            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strISIN = "";
            decimal strBal = 0;
            decimal DblBal = 0;

            FromDate = myutil.dtos(FromDate);
            ToDate = myutil.dtos(ToDate);
            DataTable dt = new DataTable();
            string strsql = "";
            SqlConnection curCon = new SqlConnection(connectionstring);
            SqlConnection objconnection1 = null;
            if (curCon.State == ConnectionState.Closed)
            {
                curCon.Open();
            }


            SqlDataAdapter ObjAdapter1 = new SqlDataAdapter("select * from other_products where op_status='A' AND op_product='CROSS'", curCon);
            DataTable Objdt1 = new DataTable();
            ObjAdapter1.Fill(Objdt1);

            if (Objdt1.Rows.Count > 0)
            {
                objconnection1 = mydbutil.crostemp_conn("Cross"); ;

            }
            SqlDataAdapter ObjAdpEstro = new SqlDataAdapter("select * from other_products where op_status='A' AND op_product='Estro'", curCon);
            DataTable ObjdtEstro = new DataTable();
            ObjAdpEstro.Fill(ObjdtEstro);

            if (ObjdtEstro.Rows.Count > 0)
            {
                objconnection1 = mydbutil.crostemp_conn("Estro"); ;

            }



            if ((Strings.Left(Code, 2)) == "IN" || Code == "BackOfficeCd")

            {


                strsql = "select td_isin_code,'0' as Balance, 'BO Name    :  '+ ltrim(rtrim(cm_name))+' ['+ltrim(rtrim(td_ac_code)) +']' as BoId, td_booking_type, 'Client Name :- ' + ltrim(rtrim(cm_name))+' ['+ltrim(rtrim(cm_blsavingcd)) +']' as  Code,cm_name,cm_blsavingcd,cm_cd,cm_email,cm_add1,sc_decimal_allow,cm_add2,cm_groupcd,cm_familycd, cm_add3 ,bm_branchname,cm_pin, cm_tele1, cm_brboffcode, cm_sech_name , cm_thih_name,cm_pin, bc_description, bc_code 'cat' , bs_description,ltrim(rtrim(convert(char,convert(datetime,td_curdate),103)))as td_trxdate1 ,td_reference,td_beneficiery,td_countercmbpid,  td_market_type, td_description ,td_clear_corpn, ";
                strsql = strsql + "td_cds,td_curdate, ltrim(sc_company_name) +' ['+ltrim(rtrim(td_isin_code)) +']' as ISINCode,";
                strsql = strsql + " sc_company_name,sc_rate,td_settlement,td_ac_type,td_category,td_counterdp,td_narration,td_qty,td_debit_credit,  Case td_debit_credit  when 'C' then cast((td_qty)as decimal(15,3)) else 0 end 'credit', Case td_debit_credit  when 'D' then cast((td_qty)as decimal(15,3)) else 0 end 'debit',  sc_isinname , nr_description 'ndesc',bt_description 'acdesc',gr_email,fm_email,bm_email,  isnull((select sum(case td_debit_credit when 'C' then td_qty else td_qty * (-1) end)  From  Trxdetail   Where  td_ac_code = a.td_ac_code and td_booking_type not in ('13')   and  td_isin_code = a.td_isin_code ";
                strsql = strsql + " and  td_ac_type = a.td_ac_type and td_booking_type=a.td_booking_type and td_curdate < '" + Strings.Trim(FromDate) + "'),0) 'holding',td_totcert,td_amount  From    Trxdetail a, Security, Client_master left outer join DayEnd on cm_cd = de_cmcd , narration,  Beneficiary_type ,Beneficiary_category ,Beneficiary_status, group_master,family_master,branch_master  Where  td_ac_code = cm_cd And bc_code = td_category and td_category <> '03' and td_booking_type not in('02') and  td_narration not in('001') And bs_code = cm_active and td_isin_code = sc_isincode";
                strsql = strsql + " And a.td_ac_code = '" + Strings.Trim(Code) + "'";

                // '''' strsql = strsql & " and td_narration = nr_code   and  td_ac_type = bt_code  and td_trxdate between '" & Trim(MyDatecontrol.PrpFromDateString) & "' and '" & Trim(MyDatecontrol.PrpToDateString) & "' And cm_groupcd = gr_cd And cm_familycd = fm_cd And cm_brboffcode = bm_branchcd     Order By cm_cd, sc_company_name,sc_isinname , td_isin_code, td_ac_type, "
                strsql = strsql + " and td_narration = nr_code   and  td_ac_type = bt_code  and td_curdate between '" + Strings.Trim(FromDate) + "' and '" + Strings.Trim(ToDate) + "' And cm_groupcd = gr_cd And cm_familycd = fm_cd And cm_brboffcode = bm_branchcd     Order By cm_cd, sc_company_name,sc_isinname , td_isin_code, td_ac_type, ";
                strsql = strsql + " td_curdate, td_trxdate,td_debit_credit,td_market_type, td_settlement ";
                dt = mylib.OpenDataTable(strsql, objconnection1);

            }
            else
            {

                strsql = "select td_isin_code,'0' as Balance, 'BO Name    :  '+ ltrim(rtrim(cm_name))+' ['+ltrim(rtrim(td_ac_code)) +']' as BoId, td_booking_type, 'Client Name :- ' + ltrim(rtrim(cm_name))+' ['+ltrim(rtrim(cm_blsavingcd)) +']' as  Code,cm_name,cm_blsavingcd,cm_cd,cm_email,cm_add1,sc_decimal_allow,cm_add2,cm_groupcd,cm_familycd, cm_add3 ,bm_branchname,cm_pin, cm_tele1, cm_brboffcode, cm_sech_name , cm_thih_name,cm_pin, bc_description, bc_code 'cat' , bs_description,ltrim(rtrim(convert(char,convert(datetime,td_curdate),103)))as td_trxdate1 ,td_reference,td_beneficiery,td_countercmbpid,  td_market_type, td_description ,td_clear_corpn, ";
                strsql = strsql + " ltrim(sc_isinname) +' ['+ltrim(rtrim(td_isin_code)) +']' as ISINCode,";
                strsql = strsql + " sc_company_name,sc_rate,td_settlement,td_ac_type,td_category,td_counterdp,td_narration,td_qty,td_debit_credit,  Case td_debit_credit  when 'C' then cast((td_qty)as decimal(15,3)) else 0 end 'credit', Case td_debit_credit  when 'D' then cast((td_qty)as decimal(15,3)) else 0 end 'debit',  sc_isinname , nr_description 'ndesc',bt_description 'acdesc',gr_email,fm_email,bm_email,  isnull((select sum(case td_debit_credit when 'C' then td_qty else td_qty * (-1) end)  From  Trxdetail   Where  td_ac_code = a.td_ac_code and td_booking_type not in ('13')   and  td_isin_code = a.td_isin_code ";
                strsql = strsql + " and  td_ac_type = a.td_ac_type   and td_curdate < '" + Strings.Trim(FromDate) + "'),0) 'holding',td_totcert,td_amount  From    Trxdetail a, Security, Client_master left outer join DayEnd on cm_cd = de_cmcd , narration,  Beneficiary_type ,Beneficiary_category ,Beneficiary_status, group_master,family_master,branch_master  Where  td_ac_code = cm_cd And bc_code = td_category and td_category <> '03' and td_booking_type not in('02') and  td_narration not in('001') And bs_code = cm_active and td_isin_code = sc_isincode";


                strsql = strsql + " And a.td_ac_code = '" + Strings.Trim(Code) + "'";

                // '''' strsql = strsql & " and td_narration = nr_code   and  td_ac_type = bt_code  and td_trxdate between '" & Trim(MyDatecontrol.PrpFromDateString) & "' and '" & Trim(MyDatecontrol.PrpToDateString) & "' And cm_groupcd = gr_cd And cm_familycd = fm_cd And cm_brboffcode = bm_branchcd     Order By cm_cd, sc_company_name,sc_isinname , td_isin_code, td_ac_type, "
                strsql = strsql + " and td_narration = nr_code   and  td_ac_type = bt_code  and td_curdate between '" + Strings.Trim(FromDate) + "' and '" + Strings.Trim(ToDate) + "' And cm_groupcd = gr_cd And cm_familycd = fm_cd And cm_brboffcode = bm_branchcd     Order By cm_cd, sc_company_name,sc_isinname , td_isin_code, td_ac_type, ";
                strsql = strsql + " td_curdate, td_trxdate,td_debit_credit,td_market_type, td_settlement ";

                dt = mylib.OpenDataTable(strsql, objconnection1);
            }



            if (dt.Rows.Count > 0)
            {
                short i;
                string stracctype = "";
                short intDecimal;
                var loopTo = dt.Rows.Count - 1;
                for (i = 0; i <= loopTo; i++)
                {
                    intDecimal = Conversions.ToShort(Interaction.IIf(Convert.ToDecimal(dt.Rows[i]["sc_decimal_allow"]) > 0, 3, 0));
                    if (strISIN != dt.Rows[i]["ISINcode"].ToString().Trim() | stracctype != dt.Rows[i]["acdesc"].ToString().Trim())
                    {

                        DblBal = 0;
                        dt.Rows[i]["Balance"] = Math.Round(Convert.ToDecimal(dt.Rows[i]["holding"]) - Convert.ToDecimal(dt.Rows[i]["Debit"]) + Convert.ToDecimal(dt.Rows[i]["Credit"]), intDecimal);
                        DblBal = Convert.ToDecimal(dt.Rows[i]["Balance"]);
                        strISIN = dt.Rows[i]["ISINCode"].ToString().Trim();
                        stracctype = dt.Rows[i]["acdesc"].ToString().Trim();
                    }
                    else
                    {

                        dt.Rows[i]["Balance"] = Math.Round(DblBal - Convert.ToDecimal(dt.Rows[i]["Debit"]) + Convert.ToDecimal(dt.Rows[i]["Credit"]), intDecimal);
                        DblBal = Convert.ToDecimal(dt.Rows[i]["Balance"]);
                    }

                    dt.Rows[i]["Debit"] = Convert.ToDecimal(dt.Rows[i]["Debit"]).ToString("0.00");
                    dt.Rows[i]["Credit"] = Convert.ToDecimal(dt.Rows[i]["Credit"]).ToString("0.00");
                    dt.Rows[i]["Balance"] = Convert.ToDecimal(dt.Rows[i]["Balance"]).ToString("0.00");

                    UtilityDBModel.urdata GBp;
                    string strParticular = "";
                    string strMarkettype = "";
                    string strCode = "";

                    if ((Strings.Left(Code, 2)) == "IN" || Code == "BackOfficeCd")
                    {

                        // If ObjDataSetESTRO.Tables(0).Rows.Count > 0 Then 'Estro
                        string strParticularEstro = "";
                        if (((dt.Rows[i]["td_narration"]).ToString() == "042" | (dt.Rows[i]["td_narration"]).ToString() == "044") & !string.IsNullOrEmpty(Strings.Trim(dt.Rows[i]["td_market_type"].ToString())) & dt.Rows[i]["td_settlement"].ToString() != "  " & (Strings.Trim(dt.Rows[i]["td_booking_type"].ToString()) != "02" | Strings.Trim(dt.Rows[i]["td_booking_type"].ToString()) != "03"))
                        {
                            if (Information.IsDBNull(dt.Rows[i]["td_countercmbpid"]) | string.IsNullOrEmpty((dt.Rows[i]["td_countercmbpid"]).ToString()))
                            {
                                GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_counterdp"]).ToString());
                            }
                            else
                            {
                                GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_countercmbpid"]).ToString());
                            }

                            if (GBp.bprole == "03")
                            {
                                strParticular = "CM " + Strings.Left(Strings.Trim(GBp.bpname), 25) + Strings.Space(1);
                                strMarkettype = mylib.fnFireEstroQuery("Market_type", "mt_description", "mt_code", (String)dt.Rows[i]["td_market_type"], false) + "/" + dt.Rows[i]["td_settlement"];
                            }
                            else
                            {
                                strParticular = GBp.bpname + "/ ";
                            }
                        }
                        else if ((((dt.Rows[i]["td_narration"]).ToString() ?? "") == "042" | ((dt.Rows[i]["td_narration"]).ToString() ?? "") == "044") & string.IsNullOrEmpty((dt.Rows[i]["td_market_type"]).ToString()) & string.IsNullOrEmpty((dt.Rows[i]["td_settlement"]).ToString()))
                        {
                            if (!string.IsNullOrEmpty((dt.Rows[i]["Counterdp "]).ToString()))
                            {
                                GBp = mydbutil.fnFindBpName(dt.Rows[i]["td_counterdp"].ToString());
                                strParticular = GBp.bpname;
                            }
                            else
                            {
                                strParticular = (dt.Rows[i]["ndesc"]).ToString();
                            }
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "307" & ((dt.Rows[i]["td_ac_type"]).ToString() ?? "") == "22")
                        {
                            GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_counterdp"]).ToString());
                            strParticular = "Pledge Request " + GBp.bpname;
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "307" & ((dt.Rows[i]["td_ac_type"]).ToString() ?? "") == "26")
                        {
                            strParticular = "Pledge Request";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "307" & ((dt.Rows[i]["td_ac_type"]).ToString() ?? "") == "29")
                        {
                            strParticular = "Pledge Request";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "091" & ((dt.Rows[i]["td_ac_type"]).ToString() ?? "") == "26")
                        {
                            GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_counterdp"]).ToString());
                            strParticular = "Pledge " + GBp.bpname;
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "091" & ((dt.Rows[i]["td_ac_type"]).ToString() ?? "") != "22")
                        {
                            GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_counterdp"]).ToString());
                            strParticular = "Pledge " + GBp.bpname;
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "092")
                        {
                            strParticular = "Pledge Closure";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "093")
                        {
                            GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_counterdp"]).ToString());
                            strParticular = dt.Rows[i]["ndesc"] + " " + GBp.bpname;
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "201" | ((dt.Rows[i]["td_narration"]).ToString() ?? "") == "202")
                        {
                            if (((dt.Rows[i]["td_booking_type"]).ToString() ?? "") == "04")
                            {
                                GBp = mydbutil.fnFindBpName((dt.Rows[i]["td_counterdp"]).ToString());
                                strParticular = GBp.bpname;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "203" & ((dt.Rows[i]["td_booking_type"]).ToString() ?? "") == "02")
                        {
                            strParticular = "Inter Dp Rejection";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "012" & (dt.Rows[i]["td_debit_credit"]).ToString() == "C")
                        {
                            strParticular = "Dematerilization";
                        }
                        else if (((dt.Rows[i]["td_narration"].ToString() ?? "") == "012" & (dt.Rows[i]["td_debit_credit"]).ToString() == "D"))
                        {
                            strParticular = "Dematerialisation request confirmation";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "011" & (dt.Rows[i]["td_debit_credit"]).ToString() == "C")
                        {
                            strParticular = "Dematerilization Request";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "204")
                        {
                            strParticular = "Inter Depository transfer CDS";
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "082")
                        {
                            strParticular = mydbutil.mfnFindCorporateAction(dt.Rows[i]["td_isin_code"].ToString(), "082", "00", "", dt.Rows[i]["td_curdate"].ToString(), dt.Rows[i]["td_debit_credit"].ToString());
                        }
                        else if (((dt.Rows[i]["td_narration"]).ToString() ?? "") == "083")
                        {
                            strParticular = mydbutil.mfnFindCorporateAction(dt.Rows[i]["td_isin_code"].ToString(), "083", "00", "", dt.Rows[i]["td_curdate"].ToString(), dt.Rows[i]["td_debit_credit"].ToString());
                        }
                        else
                        {
                            strParticular = (dt.Rows[i]["ndesc"]).ToString();
                        }

                        if ((dt.Rows[i]["td_category"]).ToString() == "03")
                        {
                            if ((Strings.Trim(dt.Rows[i]["td_beneficiery"].ToString()) ?? "") != "00000000")
                            {
                                strCode = (String)dt.Rows[i]["td_beneficiery"];
                            }
                            else if ((dt.Rows[i]["td_counterdp"]).ToString() == "IN000026" & (dt.Rows[i]["td_cds"]).ToString() != "")
                            {
                                strCode = (String)dt.Rows[i]["td_cds"];
                                if (Strings.Len(Strings.Trim(strParticular)) > 30)
                                    strParticular = Strings.Left(Strings.Trim(strParticular), 30) + @"\";
                            }
                        }
                        else if ((String)dt.Rows[i]["td_beneficiery"] != "00000000")
                        {
                            if (!string.IsNullOrEmpty(Strings.Trim(dt.Rows[i]["td_clear_corpn"].ToString())))
                            {
                                strCode = "";
                            }
                            else
                            {
                                strCode = (String)dt.Rows[i]["td_beneficiery"];
                            }
                        }
                        else if (dt.Rows[i]["td_counterdp"].ToString() == "IN000026" & dt.Rows[i]["td_cds"].ToString() != "")
                        {
                            strCode = (String)dt.Rows[i]["td_cds"];
                            if (Strings.Len(Strings.Trim(strParticular)) > 30)
                                strParticular = Strings.Left(Strings.Trim(strParticular), 30) + @"\";
                        }

                        if (dt.Rows[i]["td_debit_credit"].ToString() == "D")
                        {
                            strParticular = " To " + strParticular + " " + strCode + " " + strMarkettype;
                        }
                        else
                        {
                            strParticular = " By " + strParticular + " " + strCode + " " + strMarkettype;
                        }

                        dt.Rows[i]["td_description"] = Strings.Trim(strParticular);
                    }
                    else
                    {
                        // ElseIf ObjDataSet2.Tables(0).Rows.Count > 0 Then ''''Cross''''''''''''''''''''
                        strParticular = dt.Rows[i]["td_description"].ToString();
                        if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString()) == "044" || Strings.Trim(dt.Rows[i]["td_narration"].ToString()) == "042"))
                        {
                            strParticular = strParticular + " / " + dt.Rows[i]["td_beneficiery"].ToString();
                            if (Information.IsDBNull(dt.Rows[i]["td_settlement"]) == false)
                            {
                                if (dt.Rows[i]["td_settlement"].ToString() != "")
                                    strParticular = strParticular + " / " + dt.Rows[i]["td_settlement"].ToString();
                            }
                        }
                        else if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString())) == "204")
                        {
                            if ((Strings.UCase(Strings.Left(strParticular, 5))) == "INTER")
                                strParticular = "INTDEP-CR";
                            strParticular = strParticular + "/" + Strings.Trim(dt.Rows[i]["td_counterdp"].ToString()) + " " + Strings.Trim(dt.Rows[i]["td_beneficiery"].ToString());
                            if (!string.IsNullOrEmpty(Strings.Trim(dt.Rows[i]["td_settlement"].ToString())))
                                strParticular = strParticular + "/" + Strings.Trim(dt.Rows[i]["td_settlement"].ToString());
                        }
                        else if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString())) == "202")
                        {
                            if ((Strings.UCase(Strings.Left(strParticular, 5)) ?? "") == "INTER")
                                strParticular = "INTDEP-DR";
                            strParticular = strParticular + "/" + Strings.Trim(dt.Rows[i]["td_counterdp"].ToString()) + " " + Strings.Trim(dt.Rows[i]["td_beneficiery"].ToString());
                            if (!string.IsNullOrEmpty(Strings.Trim(dt.Rows[i]["td_settlement"].ToString())))
                                strParticular = strParticular + "/" + Strings.Trim(dt.Rows[i]["td_settlement"].ToString());
                        }
                        else if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString())) == "052")
                        {
                            if (!string.IsNullOrEmpty(Strings.Trim(dt.Rows[i]["td_settlement"].ToString())))
                                strParticular = strParticular + "/" + dt.Rows[i]["td_settlement"].ToString() + "/" + mylib.fnFireCrossQuery("Market_type", "mt_description", "mt_code", Strings.Trim(dt.Rows[i]["td_market_type"].ToString()), true);
                            if (dt.Rows[i]["td_beneficiery"].ToString() != "")
                                strParticular = strParticular + "/" + dt.Rows[i]["td_beneficiery"].ToString();
                        }
                        else if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString())) == "054")
                        {
                            if ((Strings.Trim(dt.Rows[i]["td_settlement"].ToString())).Length == 13)
                            {
                                strParticular = strParticular + "/" + dt.Rows[i]["td_settlement"].ToString() + "/" + mylib.fnFireCrossQuery("Market_type", "mt_description", "mt_code", Strings.Trim(dt.Rows[i]["td_market_type"].ToString()), true);
                            }
                            else
                            {
                                strParticular = strParticular + "/" + dt.Rows[i]["td_settlement"];
                                if (!string.IsNullOrEmpty(Strings.Trim(dt.Rows[i]["td_beneficiery"].ToString())))
                                    strParticular = strParticular + "/" + Strings.Trim(dt.Rows[i]["td_beneficiery"].ToString());
                            }
                        }
                        else if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString())) == "011")
                        {
                            strParticular = mylib.fnFireCrossQuery("Narration", "nr_description", "nr_code", Strings.Trim(dt.Rows[i]["td_narration"].ToString()), true);
                        }
                        else if ((Strings.Trim(dt.Rows[i]["td_narration"].ToString())) == "013")
                        {
                            strParticular = mylib.fnFireCrossQuery("Narration", "nr_description", "nr_code", Strings.Trim(dt.Rows[i]["td_narration"].ToString()), true);
                        }

                        if ((Strings.Left(Strings.UCase(strParticular), 7) ?? "") == "OVERDUE")
                            strParticular = strParticular + " " + Strings.Trim(dt.Rows[i]["td_beneficiery"].ToString());
                        if (dt.Rows[i]["td_debit_credit"].ToString() == "D")
                        {
                            strParticular = " To " + strParticular;
                        }
                        else
                        {
                            strParticular = " By " + strParticular;
                        }

                        if ((Strings.Right(Strings.Trim(strParticular), 1) ?? "") == "/")
                            strParticular = Strings.Left(Strings.Trim(strParticular), Strings.Len(Strings.Trim(strParticular)) - 1);
                        dt.Rows[i]["td_description"] = Strings.Trim(strParticular);
                    }
                }

            }

            else
            {

            }








            return dt;


        }

        public DataTable GetPledgeRequestReport(string Code, string cmbSelect)
        {
            Boolean blnNSDL = false;
            Boolean blnCDSL = false;
            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            string gValuation = Convert.ToString(Conversion.Val(Strings.Trim(mylib.fnGetSysParam("RMSVALATLTRT"))));
            string gHAIRCUT = Convert.ToString(Conversion.Val(Strings.Trim(mylib.fnGetSysParam("HAIRCUTVAL"))));
            double gAddHairCut = Conversion.Val(Strings.Trim(mylib.fnGetSysParam("FMRADDHRCUT")));
            DataTable dtX = null;

            var strHoldingWhere = default(string);
            string strSql = "";

            string todaydate = DateTime.Now.ToString("yyyyMMdd");

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                if (Code != "")
                {
                    if (cmbSelect == "CL")
                    {
                        strHoldingWhere = strHoldingWhere + " and cm_cd = '" + Code + "'";
                    }

                    else if (cmbSelect == "GR")
                    {
                        strHoldingWhere = strHoldingWhere + " and cm_groupcd = '" + Code + "'";
                    }
                    else if (cmbSelect == "FM")
                    {
                        strHoldingWhere = strHoldingWhere + " and cm_familycd = '" + Code + "'";
                    }
                    else if (cmbSelect == "SB")
                    {
                        strHoldingWhere = strHoldingWhere + " and cm_subbroker = '" + Code + "'";
                    }
                    else if (cmbSelect == "All")
                    {
                        strHoldingWhere = strHoldingWhere + "";
                    }

                }

                try
                {
                    strSQL = "Drop Table #TblHolding";
                    mylib.ExecSQL(strSQL, curCon);
                }
                catch (Exception ex)
                {


                }
                finally
                {

                    strSql = "Create Table #TblHolding ( " + Constants.vbNewLine + " th_Identity Numeric Identity(1,1) , " + Constants.vbNewLine + " th_cmCd VarChar(8), "
                        + Constants.vbNewLine + " th_DematActNo Char(16), " + Constants.vbNewLine + " th_Scripcd Char(8), " + Constants.vbNewLine + " th_ISIN Char(12), "
                        + Constants.vbNewLine + " th_Qty Numeric , " + Constants.vbNewLine + " th_Rate Money  , " + Constants.vbNewLine + " th_MrgShortFall money,"
                        + Constants.vbNewLine + " th_PorjectedRisk money," + Constants.vbNewLine + " th_Haircut money," + Constants.vbNewLine + " th_NetRate money,"
                        + Constants.vbNewLine + " th_netValue money, " + Constants.vbNewLine + " th_Retain money, " + Constants.vbNewLine + " th_Approved varchar(12) , "
                        + Constants.vbNewLine + " th_RegForFO char(1) " + Constants.vbNewLine + " ) ";

                    mylib.ExecSQL(strSql, curCon);
                }
                string strServer = "";
                string strDPs;
                strDPs = Strings.Trim(mylib.fnFireQuery("other_products", "sum(case op_product when 'Cross' then 1 else 2 end)", "op_product in ('Cross','Estro') and op_status", "A", true));
                blnNSDL = Strings.InStr(1, "23", strDPs) > 0;
                HttpContext.Current.Session["blnNSDL"] = blnNSDL;
                blnCDSL = Strings.InStr(1, "13", strDPs) > 0;
                HttpContext.Current.Session["blnCDSL"] = blnCDSL;


                if (blnCDSL)
                {
                    strSql = "select OP_Server,OP_DataBase,OP_Owner,OP_User,OP_PWD ";
                    strSql += " from OTher_products Where OP_PRoDUCT = 'Cross' and OP_Status = 'A'";
                    DataTable dtTemp = mylib.OpenDataTable(strSql, curCon);
                    if (dtTemp.Rows.Count > 0)
                    {
                        strServer = "[" + (dtTemp.Rows[0]["OP_Server"]).ToString().Trim() + "].[" + (dtTemp.Rows[0]["OP_DataBase"]).ToString().Trim() + "].[dbo]";
                          //  strServer = "[" + (dtTemp.Rows[0]["OP_DataBase"]).ToString().Trim() + "].[dbo]";
                        strSql = "Insert into #TblHolding ";
                        strSql += " select '',hld_ac_code,'',hld_isin_code,hld_ac_pos,0,0,0,0,0,0,0,'','' ";
                        strSql += " from " + strServer.Trim() + ".Holding , ";
                        strSql += " " + strServer.Trim() + ".Client_master ";
                        strSql += " where cm_cd = hld_ac_code and hld_ac_type = '11' and cm_active = '01'";
                        strSql += " and exists (select da_actno from Dematact,Client_master Where cm_cd = da_clientcd and da_actno = hld_ac_code  and da_defaultyn = 'Y' and DA_STATUS = 'A' and left(da_dpid,2) <> 'IN' " + strHoldingWhere + HttpContext.Current.Session["LoginAccessOld"];
                        strSql += " and exists (select cud_boid from " + strServer.Trim() + ".Client_UCC_Details ";
                        strSql += " Where cud_boid=hld_ac_code ";
                        //if (Code.Trim() != "")
                        //{
                        //strSql += " and  cud_tmid = '" + mfnGetTMID(Code.Trim()) + "'";
                        strSql += " and  cud_tmid in (" + mfnGetTMID() + ")";
                        //}

                        strSql += " )) ";
                        mylib.ExecSQL(strSql, curCon);
                        strSql = "Update #TblHolding set th_cmCd = cud_UCC from " + strServer.Trim() + ".Client_UCC_Details ";
                        strSql += " Where cud_boid=th_DematActNo ";
                        //if (Code != "")
                        //{
                        //    strSql += " and cud_tmid = '" + mfnGetTMID(Code.Trim()) + "'";
                            strSql += " and cud_tmid in (" + mfnGetTMID() + ")";
                        //}

                        mylib.ExecSQL(strSql, curCon);
                    }
                }


                if (blnNSDL)
                {
                    strSql = "select OP_Server,OP_DataBase,OP_Owner,OP_User,OP_PWD from OTher_products Where OP_PRoDUCT = 'Estro' and OP_Status = 'A'";
                    DataTable dtTemp = mylib.OpenDataTable(strSql, curCon);
                    if (dtTemp.Rows.Count > 0)
                    {
                        strServer = "[" + (dtTemp.Rows[0]["OP_Server"]).ToString().Trim() + "].[" + (dtTemp.Rows[0]["OP_DataBase"]).ToString().Trim() + "].[dbo]";
                       // strServer = "[" + (dtTemp.Rows[0]["OP_DataBase"]).ToString().Trim() + "].[dbo]";
                        strSql = "Select sp_sysvalue from " + strServer + ".Sysparameter where sp_parmcd = 'DPID'";

                        dtX = mylib.OpenDataTable(strSql, curCon);
                        string strDpid = (dtX.Rows[0][0]).ToString();
                        strSql = "Insert into #TblHolding ";
                        strSql += " select '','" + strDpid + "'+hld_ac_code,'',hld_isin_code,hld_ac_pos,0,0,0,0,0,0,0,'','' ";
                        strSql += " from " + strServer.Trim() + ".Holding , " + strServer.Trim() + ".Client_master ";
                        strSql += " where cm_cd = hld_ac_code and hld_ac_type = '22' and cm_active = '01'";
                        strSql += " and ltrim(rtrim(cm_blsavingcd)) <> '' ";
                        strSql += " and '" + strDpid + "'+hld_ac_code in (select da_dpid+da_actno from Dematact,Client_master Where cm_cd=da_clientcd and da_defaultyn = 'Y' and DA_STATUS = 'A' and left(da_dpid,2) = 'IN' " + strHoldingWhere + HttpContext.Current.Session["LoginAccessOld"];
                        strSql += " ) ";
                        mylib.ExecSQL(strSql, curCon);
                        strSql = "Update #TblHolding set th_cmCd = da_clientcd from Dematact Where da_defaultyn = 'Y' and DA_STATUS = 'A' and case When left(da_dpid,2) = 'IN' Then da_dpid+da_actno else da_actno end = th_DematActNo ";
                        mylib.ExecSQL(strSql, curCon);
                    }
                }
                strSql = "Update #TblHolding set th_Scripcd =  im_scripcd From ISIN Where im_isin = th_ISIN and im_active = 'Y' and im_scripcd not Between '600000' and '699999' ";
                mylib.ExecSQL(strSql, curCon);



                if (mylib.fnGetSysParam("PRMISECURITY") == "Y")
                {
                    strSql = " Delete From #TblHolding ";
                    strSql += " from securities Where th_scripcd =ss_cd and ss_Permscm <> 'Y' ";
                    mylib.ExecSQL(strSql, curCon);
                }

                strSql = "Update #TblHolding set th_haircut = 100 ";
                mylib.ExecSQL(strSql, curCon);

                strSql = " update #TblHolding set th_haircut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin ";
                strSql = strSql + " where vm_exchange = 'B' and vm_scripcd = th_scripcd ";
                strSql = strSql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'B' and vm_scripcd = th_scripcd ";
                //  strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + HttpContext.Current.Session["gstrtoday"] + "')"; 20200821
                strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + "'" + todaydate + "' )";
                mylib.ExecSQL(strSql, curCon);

                if (Conversion.Val(gValuation) > 0)
                {
                    strSql = " update #TblHolding set th_haircut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin,Securities ";
                    strSql = strSql + " where vm_exchange = 'B' and vm_scripcd = th_scripcd And vm_scripcd = ss_cd And ss_group = 'F' ";
                    strSql = strSql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'B' and vm_scripcd = th_scripcd ";
                    //strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT=="0", "<=", "<") + " '" + HttpContext.Current.Session["gstrtoday"] + "')";
                    strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + todaydate + "')";
                    mylib.ExecSQL(strSql, curCon);
                }


                strSql = " update #TblHolding set th_haircut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin ";
                strSql = strSql + " where vm_exchange = 'N' and vm_scripcd = th_scripcd ";
                strSql = strSql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'N' and vm_scripcd = th_scripcd ";
                // strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + HttpContext.Current.Session["gstrtoday"] + "')";
                strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + todaydate + "')";
                mylib.ExecSQL(strSql, curCon);


                if (Conversion.Val(gValuation) > 0)
                {
                    strSql = " update #TblHolding set th_haircut = case vm_exchange When 'B' then vm_margin_rate  else vm_applicable_var end from VarMargin,Securities ";
                    strSql = strSql + " where vm_exchange = 'N' and vm_scripcd = th_scripcd And vm_scripcd = ss_cd And ss_group = 'F'";
                    strSql = strSql + " and vm_dt =(select max(vm_dt) from VarMargin where vm_exchange = 'N' and vm_scripcd = th_scripcd ";
                    // strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + HttpContext.Current.Session["gstrtoday"] + "')";
                    strSql = strSql + " and vm_dt " + Interaction.IIf(gHAIRCUT == "0", "<=", "<") + " '" + todaydate + "')";
                    mylib.ExecSQL(strSql, curCon);
                }

                if (gAddHairCut > 0)
                {
                    strSql = " update #TblHolding set th_haircut = th_haircut + " + gAddHairCut + " Where th_haircut <= 100 - " + gAddHairCut;
                    mylib.ExecSQL(strSql, curCon);
                }


                strSql = " update #TblHolding set th_Rate = mk_closerate from Market_Rates ";
                strSql = strSql + " where mk_exchange = 'B' and mk_scripcd = th_scripcd ";
                strSql = strSql + " and mk_dt =(select max(mk_dt) from Market_Rates  where mk_exchange = 'B'";
                strSql = strSql + " and mk_scripcd = th_scripcd ";
                //strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + HttpContext.Current.Session["gstrtoday"] + "')";
                strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + todaydate + "')";
                mylib.ExecSQL(strSql, curCon);

                if (Conversion.Val(gValuation) > 0)
                {
                    strSql = " update #TblHolding set th_Rate = mk_closerate from Market_Rates ,Securities";
                    strSql = strSql + " where mk_exchange = 'B' and mk_scripcd = th_scripcd And mk_scripcd = ss_cd And ss_group = 'F'";
                    strSql = strSql + " and mk_dt =(select max(mk_dt) from Market_Rates  where mk_exchange = 'B'";
                    strSql = strSql + " and mk_scripcd = th_scripcd ";
                    //strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + HttpContext.Current.Session["gstrtoday"] + "')";
                    strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + todaydate + "')";
                    mylib.ExecSQL(strSql, curCon);
                }

                strSql = " update #TblHolding set th_Rate = mk_closerate from Market_Rates ";
                strSql = strSql + " where mk_exchange = 'N' and mk_scripcd = th_scripcd ";
                strSql = strSql + " and mk_dt =(select max(mk_dt) from Market_Rates  where mk_exchange = 'N'";
                strSql = strSql + " and mk_scripcd = th_scripcd ";
                // strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT =="0", " <= ", " < ") + "'" + HttpContext.Current.Session["gstrtoday"] + "')";
                strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + todaydate + "')";
                mylib.ExecSQL(strSql, curCon);
                if (Conversion.Val(gValuation) > 0)
                {
                    strSql = " update #TblHolding set th_Rate = mk_closerate from Market_Rates ,Securities";
                    strSql = strSql + " where mk_exchange = 'N' and mk_scripcd = th_scripcd And mk_scripcd = ss_cd And ss_group = 'F'";
                    strSql = strSql + " and mk_dt =(select max(mk_dt) from Market_Rates  where mk_exchange = 'N'";
                    strSql = strSql + " and mk_scripcd = th_scripcd ";
                    //  strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + HttpContext.Current.Session["gstrtoday"] + "')";
                    strSql = strSql + " and mk_dt " + Interaction.IIf(gHAIRCUT == "0", " <= ", " < ") + "'" + todaydate + "')";
                    mylib.ExecSQL(strSql, curCon);
                }

                strSql = "update #TblHolding set th_NetRate =  th_Rate - (Round(th_Rate * ((th_Haircut) / 100), 2)) ";
                mylib.ExecSQL(strSql, curCon);
                mylib.ExecSQL("Delete from #TblHolding where th_NetRate <= 0 ", curCon);
                mylib.ExecSQL("Update #TblHolding  set th_netValue= Round(th_Qty*th_NetRate,2) ", curCon);



                DataTable dsOpen = new DataTable();
                strSql = " select * from PledgeRequest where rq_clientcd in (select th_cmcd from #TblHolding) and Rq_Status1='P' ";
                dsOpen = mylib.OpenDataTable(strSql, curCon);
                if (dsOpen.Rows.Count > 0)
                {
                    strSql = "update #TblHolding set th_retain = isNull((select sum(Rq_Qty) From PledgeRequest Where Rq_Clientcd = th_cmcd  and Rq_Scripcd=th_scripcd and Rq_Status1 = 'P'),0) ";
                    mylib.ExecSQL(strSql, curCon);
                }

                strSql = "select th_cmcd,cm_Name,th_MrgShortFall,";
                strSql += " th_scripcd,ss_lname,cast(th_qty as decimal(15,0)) as th_qty,cast(th_rate as decimal(15,2)) th_rate,cast(th_HairCut as decimal(15,2)) th_HairCut,th_ISIN,";
                strSql += " cast(th_netValue as decimal(15,2)) th_netValue,cast(th_retain as decimal(15,0)) Retain,th_DematActNo,th_ISIN,case when th_qty > 0 then cast((th_rate*th_qty) as decimal(15,2)) else 0 end th_Value,th_NetRate,";
                strSql += " case when th_retain > 0 then cast(((th_netValue/th_retain)* th_retain ) as decimal(15,2)) else 0 end th_ReqValue from #TblHolding,Securities,Client_master ";
                strSql += " where th_scripcd = ss_cd and th_cmcd = cm_cd ";
                strSql += " order by th_retain desc,th_netValue desc,ss_lname";


                DataTable ObjDataSet = new DataTable();
                ObjDataSet = mylib.OpenDataTable(strSql, curCon);
                HttpContext.Current.Session["DtlReport"] = ObjDataSet;

                strSql = " select th_cmcd,th_dematactno,cm_Name,SUM(cast(th_qty as decimal (15,0))) th_qty,sum(cast(th_netValue as decimal(15,2))) th_netValue, ";
                strSql = strSql + " SUM(cast(th_retain as decimal(15,0))) th_retain,SUM(case when th_retain>0 then cast((th_retain * th_netrate) as decimal(15,2)) else 0 end ) ReqValue";
                strSql = strSql + " from #TblHolding,Client_master where th_cmcd = cm_cd";
                strSql = strSql + " group by th_dematactno,cm_Name,th_cmcd";
                strSql = strSql + " order by th_retain desc,th_netValue desc,cm_Name";
                ObjDataSet = mylib.OpenDataTable(strSql, curCon);

                return ObjDataSet;

            }
        }

        //public string mfnGetTMID(string strExchange)
        //{
        //    string mfnGetTMIDRet = "";
        //    string strsql = "";
        //    LibraryModel mylib = new LibraryModel();
        //    UtilityModel myutil = new UtilityModel();
        //    strsql = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("select ", Interaction.IIf(strExchange == "BSE", "em_bclearingno", "em_nclearingno")), " from Entity_master where em_cd = '")) + HttpContext.Current.Session["CompanyCode"] + "'";
        //    mfnGetTMIDRet = Convert.ToString(mylib.OpenDataTable(strsql).Rows[0][0]);
        //    return mfnGetTMIDRet;
        //}
        public string mfnGetTMID()
        {
            string mfnGetTMIDRet = "";
            string strExchange = "";
            string strSql = "";
            string strValue = "";
            LibraryModel mylib = new LibraryModel();
            UtilityModel ObjUtility = new UtilityModel();
            var ds = new DataTable();

            strSql = " select distinct ces_exchange from CompanyExchangeSegments ";
            ds = mylib.OpenDataTable(strSql);
            strSql = "";
            {

                if (ds.Rows.Count > 0)
                {
                    for (int p = 0, loopTo = ds.Rows.Count - 1; p <= loopTo; p++)
                    {
                        switch (ds.Rows[p]["CES_Exchange"].ToString().Trim())
                        {
                            case "BSE":
                                {
                                    if (strSql != "")
                                    {
                                        strSql += "Union All ";
                                    }

                                    strSql += "select 'BSE' Exch, em_bclearingno clearingno  from Entity_master where em_cd = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                                    break;
                                }

                            case "NSE":
                                {
                                    if (strSql != "")
                                    {
                                        strSql += "Union All ";
                                    }

                                    strSql += "select 'NSE' Exch, em_nclearingno clearingno  from Entity_master where em_cd = '" + HttpContext.Current.Session["CompanyCode"] + "' ";
                                    break;
                                }
                        }
                    }
                }
            }

            ds = new DataTable();
            ds = mylib.OpenDataTable(strSql);
            for (int p = 0, loopTo1 = ds.Rows.Count - 1; p <= loopTo1; p++)
            {
                if (ds.Rows[p]["Exch"].ToString().Trim() == "BSE")
                {
                    strValue += "'" + Conversion.Val(ds.Rows[p]["clearingno"].ToString().Trim()) + "',";
                }
                else
                {
                    strValue += "'" + ObjUtility.LPad(ds.Rows[p]["clearingno"].ToString().Trim(), 5, "0") + "',";
                }
            }

            mfnGetTMIDRet = Strings.Left(strValue, strValue.Length - 1);
            return mfnGetTMIDRet;
        }

        public void SavePledgeRequest(IEnumerable<BulkPledgeRequestModel> BulkPledgeRequest)
        {
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel(true);
            DataTable dt = new DataTable();
            string strsql = "";
            string code = "";
            Boolean blnIdentityOn = false;
            try
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();

                    strsql = "delete from PledgeRequest where Rq_Clientcd='" + HttpContext.Current.Session["ClientCode"] + "' and Rq_Status1='P'";
                    mylib.ExecSQL(strsql, curCon);


                    DataSet Dstemp = null;
                    Dstemp = mylib.OpenDataSet("SELECT isnull (IDENT_CURRENT('PledgeRequest'),0)", curCon);


                    int intcnt = 0;
                    if (Convert.ToInt32(Dstemp.Tables[0].Rows[0][0]) > 0)
                    {
                        blnIdentityOn = true;
                        DataSet DsReqId;
                        DsReqId = mylib.OpenDataSet("SELECT IDENT_CURRENT('PledgeRequest')", curCon);
                        intcnt = Convert.ToInt32(DsReqId.Tables[0].Rows[0][0]);
                    }

                    string strTime = mylib.fnGetTime(curCon);
                    string strSql = "";
                    foreach (var item in BulkPledgeRequest)
                    {
                        if (item.RequestQty != 0)
                        {
                            if (blnIdentityOn)
                            {
                                strSql = "insert into PledgeRequest values ( ";
                            }
                            else
                            {
                                strSql = "insert into PledgeRequest values ( " + intcnt + ",";
                            }

                            strSql += " '" + Convert.ToString(HttpContext.Current.Session["ClientCode"]) + "','" + Convert.ToString(HttpContext.Current.Session["code"]) + "','" + item.ClientCode.Trim() + "','" + item.RequestQty.ToString().Trim() + "','" + Environment.MachineName + "',";
                            strSql += " '" + myutil.gstrDBToday() + "',";
                            strSql += " convert(char(8),getdate(),108),";
                            strSql += " 'P','P','P','" + myutil.Encrypt(myutil.gstrDBToday().ToString().Trim()) + "','')";
                            mylib.ExecSQL(strSql, curCon);
                        }

                    }


                }
            }


            catch (Exception ex)
            {
                string strMsg = ex.Message;
                throw;
            }
        }


        public DataTable GetBuyBackOfferReport(string CmbSecurity, string lblisin, string lblRate, string lblstlmnt)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            UtilityModel myutil = new UtilityModel();
            string strSql = "";
            string msg;
            Double dblRate;
            DataTable dt = null;
            DataTable dt1 = null;
            string StrWhere = "";
            SqlConnection ObjCross = mydbutil.crostemp_conn("CROSS");
            

        dt = mylib.OpenDataTable("select * from other_products where op_status='A' AND op_product='CROSS'");
            if (dt.Rows.Count > 0)
            {
                ObjCross = mydbutil.crostemp_conn("CROSS");
                StrWhere = " and hld_ac_type = '11'";
            }
            
            dt= mylib.OpenDataTable("select * from other_products where op_status='A' AND op_product='ESTRO'");
            if (dt.Rows.Count > 0)
            {
                ObjCross = mydbutil.crostemp_conn("ESTRO");
                StrWhere = " and hld_ac_type = '22'";
            }
            try
            {
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    curCon.Open();
                    mylib.ExecSQL("Drop Table #TblHolding", curCon);

                    strSql = "Create Table #TblHolding ( ";
                    strSql = strSql + " th_cmCd VarChar(8), ";
                    strSql = strSql + " th_DematAcc VarChar(16), ";
                    strSql = strSql + " th_Scripcd Char(6), ";
                    strSql = strSql + " th_Qty Numeric,  ";
                    strSql = strSql + " th_Rate Money, ";
                    strSql = strSql + " th_Offered Numeric,  ";
                    strSql = strSql + " th_status Varchar (1)  ";
                    strSql = strSql + " ) ";
                    mylib.ExecSQL(strSql, curCon);

                    string[] arrScr = new string[0];

                    string loginaccess;
                    if (HttpContext.Current.Session["LoginAccessOld"] != null)
                    {
                        loginaccess = HttpContext.Current.Session["LoginAccessOld"].ToString().Replace("cm_", "CM2.cm_");
                    }
                    else
                    {
                        loginaccess = "";

                    }
                    arrScr = CmbSecurity.Split('/');

                    if (ObjCross == null)
                    {
                        msg = "Could Not Established Connection.";
                    }
                    else
                    {

                          SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString);

                            string StrConn;
                            if (ObjCross.DataSource.ToUpper().ToString().Trim() == builder.DataSource.ToUpper().ToString().Trim())
                            {
                                StrConn = ObjCross.Database + ".dbo.";
                            }
                            else
                            {
                                StrConn = "[" + ObjCross.DataSource + "]" + "." + ObjCross.Database + ".dbo.";
                            }
                    

                        strSql = "Insert into #TblHolding ";
                        strSql = strSql + " Select Left(Rtrim(rtrim(cm_blsavingcd)),8) as pcode,hld_ac_code,'" + arrScr[0] + "',hld_ac_pos as DPHold," + lblRate + ",0,'X'";
                        strSql = strSql + " from " + StrConn + " Holding," + StrConn + "Client_master CM1,Client_master CM2";
                        strSql = strSql + " where CM1.cm_cd = hld_ac_code  and CM2.cm_cd = CM1.cm_blsavingcd ";
                        strSql = strSql + StrWhere;
                        strSql = strSql + " and ltrim(rtrim(cm_blsavingcd)) <> '' and cm_PoaForPayin = 'Y' and CM2.cm_type <> 'C' and CM2.cm_schedule = 49843750";
                        strSql = strSql + " " + loginaccess;
                        strSql = strSql = strSql + " and hld_isin_code='" + lblisin + "' ";

                        mylib.ExecSQL(strSql, curCon);

                        
                    }
                    strSql = " update #TblHolding set th_Offered=BB_Qty,th_status = BB_status from Buybacks where th_cmcd = BB_Clientcd and BB_Stlmnt ='" + lblstlmnt + "'";
                    mylib.ExecSQL(strSql, curCon);

                    dblRate = Convert.ToDouble(lblRate);
                    strSql = "select th_cmcd,cm_name,th_DematAcc,abs(th_qty) Qty ,th_Offered Offered ,case when th_Offered>0 then (th_Offered*" + dblRate + ") else 0 end Value,th_status Status,case th_status when 'P' then 'POA File Generated' when 'O' then 'Order File Generated' when 'T' then 'Request By Branch' when 'X' then '' else 'Request Earlier' end th_status ";
                    strSql = strSql + " from #TblHolding,client_master";
                    strSql = strSql + " where cm_cd=th_cmcd ";
                    strSql = strSql + " order by cm_name,th_cmcd";
                    dt = mylib.OpenDataTable(strSql, curCon);
                }

            }
            catch (Exception ex)
            {


            }

            return dt;
        }

        public string GetISINDetails(string isincode)
        {
            UtilityDBModel mydbutil = new UtilityDBModel();
            UtilityModel myutil = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            string strISINName = "";
            DataTable dtSecurity = new DataTable();
            if (isincode.Trim() != "")
            {
                strSQL = "select * from other_products where op_status='A' AND op_product in ('Cross','Estro')";
                DataTable dtProduct = mylib.OpenDataTable(strSQL);
                for (int i = 0; i <= dtProduct.Rows.Count - 1; i++)
                {
                    if (dtProduct.Rows[i]["op_product"].ToString().Trim() == "Cross")
                    {
                        strSQL = "select sc_isincode,sc_company_name from security where sc_isincode = '" + isincode.Trim() + "'";
                        SqlConnection SQLConcros = mydbutil.crostemp_conn("Cross");
                        if (SQLConcros.State == ConnectionState.Closed)
                        {
                            SQLConcros.Open();
                        }
                        dtSecurity = mylib.OpenDataTable(strSQL, SQLConcros);
                    }
                    else if (dtProduct.Rows[i]["op_product"].ToString().Trim() == "Estro")
                    {
                        strSQL = "select sc_isincode,sc_company_name from security where sc_isincode = '" + isincode.Trim() + "'";
                        SqlConnection SQLConcros = mydbutil.crostemp_conn("Estro");
                        if (SQLConcros.State == ConnectionState.Closed)
                        {
                            SQLConcros.Open();
                        }
                        dtSecurity = mylib.OpenDataTable(strSQL, SQLConcros);
                    }
                }
                if (dtSecurity.Rows.Count > 0)
                {
                    strISINName = dtSecurity.Rows[0]["sc_company_name"].ToString().Trim();
                }
            }
            return strISINName;
        }


    }
}