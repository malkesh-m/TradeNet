
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;


namespace TRADENET.Models
{
    public class UtilityModel : ConnectionModel
    {
        public string DbToDate(string dDate)

        {

            string dtos = dDate.Substring(6, 2) + '/' + dDate.Substring(4, 2) + '/' + dDate.Substring(0, 4);
            return dtos;
        }

        public string dtos(string dDate)
        {    //for converting date to string format
            string dtos = dDate.Substring(6, 4) + dDate.Substring(3, 2) + dDate.Substring(0, 2);

            return dtos;
        }

        public DateTime ConvertDT(string Date)
        {
            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));
            return new DateTime(Year, Month, Day);
        }
        public string fnGetTime()
        {
            LibraryModel myLib = new LibraryModel();
            string fnGetTimeRet = "";
            DataTable dt = new DataTable();
            var dbCommand = "Select convert(char(8),getdate(),108) curtm";
            dt = myLib.OpenDataTable(dbCommand);
            fnGetTimeRet = Strings.Trim(dt.Rows[0]["curtm"].ToString());
            return fnGetTimeRet;
        }
        public DateTime stod(string Date)
        {
            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));
            return new DateTime(Year, Month, Day);
        }
        public static DateTime strtod(string Date)
        {
            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));
            return new DateTime(Year, Month, Day);
        }
        public string GetFormattedDate(string Date)
        {
            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));

            return new DateTime(Year, Month, Day).ToString("dd mm yyyy");
        }

        public string GetAccyearFromDate(string strDate)
        {
            if (strDate.Length == 10)
            {
                strDate = dtos(strDate);
            }
            int strYear = int.Parse(Strings.Left(strDate, 4));
            int intMonth = int.Parse(strDate.Substring(4, 2));
            if (intMonth < 4)
            {
                return Strings.Right((strYear - 1).ToString(), 2) + "04" + Strings.Right(strYear.ToString(), 2) + "03";
            }
            else
            {
                return Strings.Right(strYear.ToString(), 2) + "04" + Strings.Right((strYear + 1).ToString(), 2) + "03";
            }

        }

        public string GetTimeNow()
        {
            return System.DateTime.Now.ToString("HH:mm:ss");
        }

        public string gstrToday()
        {
            return DateTime.Today.ToString("dd/MM/yyyy");
        }
        public string gstrDBToday()
        {
            return DateTime.Today.ToString("yyyyMMdd");
        }

        public string gstrPCNname()
        {
            return System.Environment.MachineName;
        }

        public string gstrUserCd()
        {
            return System.Web.HttpContext.Current.User.Identity.Name.Trim();
        }


        public string LoginAccess(string ClientCodeField)
        {
            string strSession = "(select cm_cd from client_master, LoginAccess where((la_grouping = 'B' and LA_GrCode = cm_brboffcode) or(LA_grouping = 'G' and la_grcode = cm_groupcd) or(LA_grouping = 'A' and cm_cd = cm_cd) or(LA_grouping = 'C' and la_grcode = cm_cd) or(LA_grouping = 'F' and LA_GrCode = cm_familycd) or(LA_grouping = 'R' and LA_GrCode = cm_subbroker) or(LA_grouping = 'D' and LA_GrCode = cm_margintype)) and cm_cd = " + ClientCodeField + " and LA_UserId = '" + gstrUserCd() + "' ) ";
            return strSession;
        }

        public void LoginAccessOld()
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();

                string strsql = "if (select count(*) from sysobjects where name='LoginAccess')>0 select * from loginAccess where la_userID='" + gstrUserCd() + "' order by la_grouping else select * from group_master where 1=2";
                string AccessFilter = "";
                DataTable dtLogin = myLib.OpenDataTable(strsql, curCon);
                if (dtLogin.Rows.Count > 0)
                {
                    short j;
                    string strCatg = "";
                    string strTmp;
                    j = 0;
                    strsql = "";
                    while (j < dtLogin.Rows.Count)
                    {
                        strCatg = dtLogin.Rows[j]["La_grouping"].ToString();
                        strTmp = "";
                        while (strCatg == dtLogin.Rows[j]["La_grouping"].ToString())
                        {
                            strTmp = strTmp + "'" + Strings.Trim(dtLogin.Rows[j]["la_grcode"].ToString()) + "',";
                            j = Conversions.ToShort(j + 1);
                            if (j >= dtLogin.Rows.Count)
                            {
                                break;
                            }
                        }

                        strsql = strTmp;    // for Branches 
                        strTmp = Strings.Mid(strTmp, 1, Strings.Len(strTmp) - 1);
                        var switchExpr = Strings.UCase(strCatg);
                        switch (switchExpr)
                        {
                            case "B":
                                {
                                    strCatg = "cm_brboffcode";

                                    strsql = strsql.Replace("'", "").Replace(",", "|");
                                    HttpContext.Current.Session["Branch"] = strsql;
                                    break;
                                }

                            case "G":
                                {
                                    strCatg = "cm_groupcd";
                                    break;
                                }

                            case "F":
                                {
                                    strCatg = "cm_familycd";
                                    break;
                                }

                            case "R":
                                {
                                    strCatg = "cm_Subbroker";
                                    break;
                                }

                            case "A":
                                {
                                    break;
                                }

                            case "C":
                                {
                                    strCatg = "cm_cd";
                                    break;
                                }

                            case "M":
                                {
                                    strCatg = "cm_dpactno";
                                    break;
                                }
                            case "D":
                                {
                                    strCatg = "cm_margintype";
                                    break;
                                }
                        }

                        AccessFilter = Conversions.ToString(Conversions.ToString(AccessFilter + Interaction.IIf(Strings.Len(AccessFilter) > 0, " or ", "") + strCatg) + Interaction.IIf(Strings.InStr(1, strTmp, ",") > 0, " in (" + strTmp + ")", "=" + strTmp));
                    }

                    if ((Strings.UCase(strCatg) ?? "") == "A")
                    {
                        HttpContext.Current.Session["LoginAccessOld"] = "";
                    }
                    else
                    {
                        HttpContext.Current.Session["LoginAccessOld"] = " and ( " + AccessFilter + " )";
                    }
                }
                else
                {
                    strsql = Strings.Trim(HttpContext.Current.Session["Branch"].ToString()).Replace("|", "','");
                    if ((Strings.Right(strsql, 3) ?? "") == "','")
                        strsql = Strings.Left(strsql, strsql.Length - 3);
                    HttpContext.Current.Session["LoginAccessOld"] = " and ( cm_brboffcode " + Interaction.IIf(String.Compare("1", "strsql") > 0, " in ('" + strsql + "')", "=" + ("'" + strsql + "'")) + " )";

                }
            }

        }
        public string LoginAccessCommex(string ClientCodeField)
        {
            string ServerName = "";
            string Catalog = "";
            string strcon = ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString;

            string[] Arrconns = strcon.Split(';');

            foreach (string Arrconn in Arrconns)
            {
                if (Arrconn.Contains("Data Source"))
                {
                    string[] Arrconn1 = Arrconn.Split('=');
                    ServerName = Arrconn1[1];

                }

                if (Arrconn.Contains("Initial Catalog"))
                {
                    string[] Arrconn1 = Arrconn.Split('=');
                    Catalog = Arrconn1[1];

                }

            }
            string strSession = "(select cm_cd from client_master, [" + ServerName + "].[" + Catalog + "].[dbo].LoginAccess where((la_grouping = 'B' and LA_GrCode = cm_brboffcode) or(LA_grouping = 'G' and la_grcode = cm_groupcd) or(LA_grouping = 'A' and cm_cd = cm_cd) or(LA_grouping = 'C' and la_grcode = cm_cd) or(LA_grouping = 'F' and LA_GrCode = cm_familycd) or(LA_grouping = 'R' and LA_GrCode = cm_subbroker) or(LA_grouping = 'D' and LA_GrCode = cm_margintype)) and cm_cd = " + ClientCodeField + " and LA_UserId = '" + gstrUserCd() + "' ) ";
            return strSession;
        }

        public string LoginAccessgroupcd()
        {
            string strSession = "and ( cm_groupcd=(select LA_GrCode from  LoginAccess where LA_UserId='" + gstrUserCd() + "')) ";
            return strSession;
        }

        public string RPlace(string strString, string StrFind, string StrReplace)
        {
            string strReturn;
            strReturn = strString;
            if (!string.IsNullOrEmpty(strReturn))
            {
                strReturn = strReturn.Replace(StrFind, StrReplace);
            }

            return strReturn;
        }

        public string getBankSql(string strDpId)
        {
            string strRtn;
            LibraryModel myLib = new LibraryModel();
            string strExchSeg = Strings.Mid(strDpId, 2, 2);
            strRtn = "select cm_cd, cm_name, 0 Amt from Client_master, Schedule where cm_schedule= sc_cd and sc_bankflag='B' ";
            strRtn = strRtn + "  and (cm_dpid='' or cm_dpid='" + HttpContext.Current.Session["CompanyCode"] + "') and cm_freezeyn='N'";
            if (!string.IsNullOrEmpty(Strings.Trim(strExchSeg)))
            {
                strRtn = strRtn + " and ( cm_occup='' or cm_occup like '%" + Strings.Mid(strExchSeg, 1, 1) + Strings.Mid(strExchSeg, 2, 1) + "%') ";
            }

            string strBanks = " and cm_cd = 'XXXXXXXXX' ";
            string strQuery = "";
            DataSet dsLoginAccess = myLib.OpenDataSet("select LA_grouping from LoginAccess Where LA_UserId ='" + HttpContext.Current.User.Identity.Name + "' and LA_grouping in ('A','B') Order by LA_grouping");
            DataSet dsBranches;
            if (dsLoginAccess.Tables[0].Rows.Count > 0)
            {
                if ((dsLoginAccess.Tables[0].Rows[0][0]).ToString() == "A")
                {
                    strQuery = "select isNull(bm_server,'') from Branch_master Where isNull(bm_server,'') <> '' Order by isNull(bm_server,'')";
                }
                else
                {
                    strQuery = "select isNull(bm_server,'') from Branch_master Where bm_Branchcd in (select LA_GrCode from LoginAccess Where LA_UserId ='" + HttpContext.Current.User.Identity.Name + "' and LA_grouping = 'B') Order by isNull(bm_server,'') ";
                }

                dsBranches = myLib.OpenDataSet(strQuery);
                if (dsBranches.Tables[0].Rows.Count > 0)
                {
                    strBanks = "";
                    if ((dsBranches.Tables[0].Rows[0][0]).ToString() == "")
                    {
                    }
                    else
                    {
                        for (int intBranch = 0; intBranch < dsBranches.Tables[0].Rows.Count; intBranch++)
                        {
                            for (int intItems = 0; intItems < Strings.Split((dsBranches.Tables[0].Rows[intBranch][0]).ToString(), ",").Length; intItems++)
                                strBanks += "'" + Strings.Split((dsBranches.Tables[0].Rows[intBranch][0]).ToString(), ",")[intItems] + "',";
                        }
                    }

                    if (strBanks.Length > 0)
                    {
                        strBanks = " and cm_cd in (" + Strings.Left(strBanks, strBanks.Length - 1) + ")";
                    }
                }

                strRtn += strBanks;
            }

            return strRtn;
        }


        public string getBankSqlCommex(string strDpId)
        {
            string strRtn;
            string strExchSeg = Strings.Mid(strDpId, 2, 2);
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            strRtn = "select cm_cd, cm_name, 0 Amt from Client_master, Schedule where cm_schedule= sc_cd and sc_bankflag='B' ";
            strRtn += " and (cm_dpid='' or cm_dpid='" + HttpContext.Current.Session["CompanyCode"] + "') and cm_freezeyn='N' ";
            if (!string.IsNullOrEmpty(Strings.Trim(strExchSeg)))
            {
                strRtn += " and ( cm_occup='' or cm_occup like '%" + Strings.Left(strExchSeg, 1) + "," + "%') ";
            }

            string strBanks = " and cm_cd = 'XXXXXXXXX' ";
            string strQuery = "";
            DataSet dsLoginAccess = myLib.OpenDataSet("select LA_grouping,LA_GrCode from LoginAccess Where LA_UserId ='" + HttpContext.Current.Session["gstrUsercd"] + "' and LA_grouping in ('A','B') Order by LA_grouping");
            DataSet dsBranches;
            if (dsLoginAccess.Tables[0].Rows.Count > 0)
            {
                if ((dsLoginAccess.Tables[0].Rows[0][0]).ToString() == "A")
                {
                    strQuery = "select isNull(bm_server,'') from Branch_master Where isNull(bm_server,'') <> '' Order by isNull(bm_server,'')";
                }
                else
                {
                    string strBranches = "";
                    for (int intLoginAccess = 0, loopTo = dsLoginAccess.Tables[0].Rows.Count - 1; intLoginAccess <= loopTo; intLoginAccess++)
                        strBranches = strBranches + "'" + dsLoginAccess.Tables[0].Rows[intLoginAccess][1] + "',";
                    strQuery = "select isNull(bm_server,'') from Branch_master Where bm_Branchcd in (" + Strings.Left(strBranches, strBranches.Length - 1) + ")";
                }

                dsBranches = myLib.OpenDataSetCommex(strQuery);
                if (dsBranches.Tables[0].Rows.Count > 0)
                {
                    strBanks = "";
                    if ((dsBranches.Tables[0].Rows[0][0]).ToString() == "")
                    {
                    }
                    else
                    {
                        for (int intBranch = 0, loopTo1 = dsBranches.Tables[0].Rows.Count - 1; intBranch <= loopTo1; intBranch++)
                        {
                            for (int intItems = 0, loopTo2 = Strings.Split((dsBranches.Tables[0].Rows[intBranch][0]).ToString(), ",").Length - 1; intItems <= loopTo2; intItems++)
                                strBanks += "'" + Strings.Split((dsBranches.Tables[0].Rows[intBranch][0]).ToString(), ",")[intItems] + "',";
                        }
                    }

                    if (strBanks.Length > 0)
                    {
                        strBanks = " and cm_cd in (" + Strings.Left(strBanks, strBanks.Length - 1) + ")";
                    }
                }

                strRtn += strBanks;
            }

            return strRtn;
        }

        public string newline()
        {
            return Environment.NewLine;
        }

        public string mfnReplaceForSQLInjection(String strParms)

        {
            if (!string.IsNullOrEmpty(strParms))
            {
                return strParms.Replace("'", "");
            }
            return "";
        }

        public string Decrypt(string strenc)
        {
            //strenc = "’ˆŽ";
            int m = 0;
            string strEncKey = null;
            string gsEcDc = null;
            string gsFinal = null;
            string gsCompare = null;
            int glNumber = 0;
            strEncKey = "ASHOKKHE";
            gsFinal = "";
            glNumber = Strings.Len(Strings.Trim(strenc));
            for (m = 1; m <= Math.Round((decimal)glNumber / 8) + 1; m++)
            {
                strEncKey = strEncKey + strEncKey;
            }
            m = 0;
            for (m = 1; m <= glNumber; m++)
            {
                gsEcDc = Strings.Mid(strenc, m, 1);
                gsCompare = Strings.Mid(strEncKey, m, 1);
                gsFinal = gsFinal + Strings.Chr(Strings.Asc(gsEcDc) - Strings.Asc(gsCompare) - 13);
            }


            return gsFinal;
        }
        public string Encrypt(string strenc)
        {
            int m = 0;
            string strEncKey = null;
            string gsEcDc = null;
            string gsFinal = null;
            string gsCompare = null;
            int glNumber = 0;
            strEncKey = "ASHOKKHE";
            gsFinal = "";
            glNumber = Strings.Len(Strings.Trim(strenc));
            for (m = 1; m <= Math.Round((decimal)glNumber / 8) + 1; m++)
            {
                strEncKey = strEncKey + strEncKey;
            }
            m = 0;
            for (m = 1; m <= glNumber; m++)
            {
                gsEcDc = Strings.Mid(strenc, m, 1);
                gsCompare = Strings.Mid(strEncKey, m, 1);
                gsFinal = gsFinal + Strings.Chr(Strings.Asc(gsEcDc) + Strings.Asc(gsCompare) + 13);
            }
            return gsFinal;
        }

        public bool fnisBPT()
        {
            bool isbpt = false;
            string strSQL = "Select Em_Name from Entity_master Where em_name like 'BP EQUITIES%'";
            LibraryModel mylib = new LibraryModel();
            DataTable dt = mylib.OpenDataTable(strSQL);
            if (dt.Rows.Count > 0)
            {
                isbpt = true;
            }
            return isbpt;
        }

        //public string Encrypt(string strenc)
        //{
        //    int m;
        //    string strEncKey, gsEcDc, gsFinal, gsCompare;
        //    strEncKey = "ASHOKKHE";
        //    gsFinal = "";
        //    gsFinal = "";

        //    int glNumber = 0;
        //    glNumber = Strings.Len(Strings.Trim(strenc));
        //    for (m = 1; m <= Math.Round((decimal)glNumber / 8) + 1; m++)
        //    {
        //        strEncKey = strEncKey + strEncKey;
        //    }
        //    for (m = 1; m <= glNumber; m++)
        //    {
        //        gsEcDc = Strings.Mid(strenc, m, 1);
        //        gsCompare = Strings.Mid(strEncKey, m, 1);
        //        gsFinal = gsFinal + Strings.Chr(Strings.Asc(gsEcDc) - Strings.Asc(gsCompare) - 13);
        //    }
        //    return gsFinal;

        //}
        public DateTime AddDayDT(string Date, int Dayvalue)
        {
            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));
            return new DateTime(Year, Month, Day).AddDays(Dayvalue);
        }
        public DateTime SubDayDT(string Date, int Dayvalue)
        {
            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));
            return new DateTime(Year, Month, Day).AddDays(-Dayvalue);
        }
        public static string mfnGetExchangeCode2Desc(string strExchangeCode)
        {
            string mfnGetExchangeCode2DescRet = "";
            switch (Strings.UCase(strExchangeCode))
            {
                case "F":
                    {
                        mfnGetExchangeCode2DescRet = "NCDEX";
                        break;
                    }

                case "N":
                    {
                        mfnGetExchangeCode2DescRet = "NCDEX";
                        break;
                    }

                case "M":
                    {
                        mfnGetExchangeCode2DescRet = "MCX";
                        break;
                    }

                case "A":
                    {
                        mfnGetExchangeCode2DescRet = "AHM-NMCE";
                        break;
                    }

                case "S":
                    {
                        mfnGetExchangeCode2DescRet = "NSEL";
                        break;
                    }

                case "C":
                    {
                        mfnGetExchangeCode2DescRet = "ICEX";
                        break;
                    }

                case "D":
                    {
                        mfnGetExchangeCode2DescRet = "NSX";
                        break;
                    }

                case "E":
                    {
                        mfnGetExchangeCode2DescRet = "ACE";
                        break;
                    }

                default:
                    {
                        mfnGetExchangeCode2DescRet = "";
                        break;
                    }
            }

            return mfnGetExchangeCode2DescRet;
        }

        public static string mfnGetSegmentCode2Desc(string strSegment)
        {
            string mfnGetSegmentCode2DescRet = default(string);
            if ((strSegment ?? "") == "K")
                mfnGetSegmentCode2DescRet = "FX";
            else if ((strSegment ?? "") == "C")
                mfnGetSegmentCode2DescRet = "CASH";
            else if ((strSegment ?? "") == "X")
                mfnGetSegmentCode2DescRet = "Comm";
            else if ((strSegment ?? "") == "M")
                mfnGetSegmentCode2DescRet = "MF";
            else
                mfnGetSegmentCode2DescRet = "F&O";
            return mfnGetSegmentCode2DescRet;
        }

        public static string mfnGetExchangeCode2DescEquity(string strExchangeCode)
        {
            string mfnGetExchangeCode2DescEquityRet = default(string);
            if ((strExchangeCode ?? "") == "M")
                mfnGetExchangeCode2DescEquityRet = "MCX";
            else
                mfnGetExchangeCode2DescEquityRet = strExchangeCode + "SE";
            return mfnGetExchangeCode2DescEquityRet;
        }

        public string mfnGetExchangeDesc2Code(string strExchangeDesc)
        {
            string mfnGetExchangeDesc2CodeRet = "";
            LibraryModel mylib = new LibraryModel();
            switch (Strings.UCase(strExchangeDesc) ?? "")
            {
                case "NCDEX":
                    {
                        mfnGetExchangeDesc2CodeRet = Conversions.ToString((mylib.fnGetSysParam("CHGNCDEXCD") == "Y" ? "F" : "N"));
                        break;
                    }

                case "MCX":
                    {
                        mfnGetExchangeDesc2CodeRet = "M";
                        break;
                    }

                case "AHM-NMCE":
                    {
                        mfnGetExchangeDesc2CodeRet = "A";
                        break;
                    }

                case "NSEL":
                    {
                        mfnGetExchangeDesc2CodeRet = "S";
                        break;
                    }

                case "ICEX":
                    {
                        mfnGetExchangeDesc2CodeRet = "C";
                        break;
                    }

                case "NSX":
                    {
                        mfnGetExchangeDesc2CodeRet = "D";
                        break;
                    }

                case "ACE":
                    {
                        mfnGetExchangeDesc2CodeRet = "E";
                        break;
                    }

                default:
                    {
                        mfnGetExchangeDesc2CodeRet = "";
                        break;
                    }
            }

            return mfnGetExchangeDesc2CodeRet;
        }

        public object LPad(string str, int intLen, string strChar)
        {
            object LPadRet = "";
            object intPadstr;
            object intDiff;
            int m;
            object strFinPad;
            if (string.IsNullOrEmpty(Strings.Trim(str)) | Strings.Trim(str) == "&nbsp;")
            {
                str = "";
            }

            intPadstr = Strings.Len(Strings.Trim(str));
            strFinPad = "";
            intDiff = Operators.SubtractObject(intLen, intPadstr);
            int loopTo = Conversions.ToInteger(intDiff);
            for (m = 1; m <= loopTo; m++)
                strFinPad = Operators.ConcatenateObject(strFinPad, strChar);
            LPadRet = Operators.ConcatenateObject(strFinPad, Strings.Trim(str));
            return LPadRet;
        }
        public string mfnGetT2Dt(string strExchange, string strDT)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel util = new UtilityModel();
            string strT2 = strDT;
            if (strDT.Length > 8)
            { strT2 = util.dtos(strDT); }
            string strsql;

            {

                //strT2 = mfnFormatdate(mfnDateAdd(eAddDate.eDay, 1, strT2), eNewDateformat.EDATABASE);
                strT2 = util.dtos(mylib.AddDayDT(strT2, 1).ToString());
            AgaindtT2:
                ;
                strsql = "select * from Tholiday_master " + Constants.vbNewLine;
                strsql += " where hm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' " + Constants.vbNewLine;
                if (strExchange != "")
                    strsql += " and hm_exchange = '" + strExchange + "' " + Constants.vbNewLine;
                strsql += " and Replace(hm_dt,'BH','20') = '" + strT2 + "'" + Constants.vbNewLine;

                DataTable dtT2 = mylib.OpenDataTable(strsql);
                if (dtT2.Rows.Count > 0)
                {
                    strT2 = util.dtos(mylib.AddDayDT(strT2, 1).ToString());
                    goto AgaindtT2;
                }
            }
            return strT2;
        }

        public string mfnGetTPlusDt(string strExchange, string strDT, int intDays)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel util = new UtilityModel();
            string strSql = "";
            string strT2 = strDT;
            for (int intLoop = 1; intLoop <= intDays; intLoop++)
                strT2 = util.dtos(mylib.AddDayDT(strT2, 1).ToString());
            AgaindtT2:
            strSql = "select * from Tholiday_master " + Constants.vbNewLine;
            strSql += " where hm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' " + Constants.vbNewLine;
            if (strExchange != "")
            {
                strSql += " and hm_exchange = '" + strExchange + "' " + Constants.vbNewLine;
            }
            strSql += " and Replace(hm_dt,'BH','20') = '" + strT2 + "'" + Constants.vbNewLine;
            DataTable dtT2 = mylib.OpenDataTable(strSql);
            if (dtT2.Rows.Count > 0)
            {
                strT2 = util.dtos(mylib.AddDayDT(strT2, 1).ToString());
                goto AgaindtT2;
            }
            return strT2;
        }

        public Boolean fnchkTable(string strTable)
        {
            Boolean fnchkTableRet;

            LibraryModel mylib = new LibraryModel();
            DataTable dtChk = mylib.OpenDataTable("select count(*) from sysobjects where name ='" + strTable + "'");
            fnchkTableRet = (Convert.ToInt16(dtChk.Rows[0][0]) > 0);
            return fnchkTableRet;
        }

        public string FnCheckDt(string StrID = "")
        {
            string strDate = "20201101";
            return strDate;
        }
        public void prCreateTableHolding(SqlConnection curCon)
        {
            string strSql;
            LibraryModel mylib = new LibraryModel();

            strSql = "Drop table #tmpHoldingrepM";
            mylib.ExecSQL(strSql, curCon);
            strSql = "Create table  #tmpHoldingrepM (";
            strSql = strSql + " dm_Type Varchar(9),";
            strSql = strSql + " dm_clientcd char(8),";
            strSql = strSql + " dm_ActNo char(16),";
            strSql = strSql + " dm_scripcd char(6),";
            strSql = strSql + " dm_ISIN char(12),";
            strSql = strSql + " dm_bcqty Numeric,";
            strSql = strSql + " dm_lastprice money,";
            strSql = strSql + " dm_approved Varchar(20),";
            strSql = strSql + " dm_grossvalue money,";
            strSql = strSql + " dm_haircut money,";
            strSql = strSql + " dm_haircutvalue money,";
            strSql = strSql + " dm_netvalue money) ";
            mylib.ExecSQL(strSql, curCon);


        }

        public void prCreateTempTable(SqlConnection curCon, bool blnCollExcessOppExch = false)
        {
            string strsql;
            LibraryModel mylib = new LibraryModel();
            string strTempRMSSummary = "#TmpRMSSummaryReport";
            string strTempRMSDetail = "#TmpRMSDetailReport";

            strsql = "Drop Table " + strTempRMSSummary;
            mylib.ExecSQL(strsql, curCon);

            strsql = "Create Table " + strTempRMSSummary + " (";
            strsql += " Tmp_Clientcd VarChar(8),";
            strsql += " Tmp_Limit Money,";
            strsql += " Tmp_TplusBal Money,";
            strsql += " Tmp_LoanBal Money,";
            strsql += " Tmp_FundedAmount Money,";
            strsql += " Tmp_FundedMrgReq Money,";
            strsql += " Tmp_CollateralFund Money,";
            strsql += " Tmp_CollateralValue Money,";
            strsql += " Tmp_ShortFallExcess Money,";
            strsql += " Tmp_TradeValue Money,";
            strsql += " Tmp_M2MLoss Money,";
            strsql += " Tmp_Tplus1Bal Money,";
            strsql += " Tmp_Tplus2Bal Money,";
            strsql += " Tmp_InitMRG Money,";
            strsql += " Tmp_RMSNetVal Money)";
            mylib.ExecSQL(strsql, curCon);

            strsql = "Drop Table " + strTempRMSDetail;
            mylib.ExecSQL(strsql, curCon);

            strsql = "Create Table " + strTempRMSDetail + " (";
            strsql += " Tmp_Type Char(1),";
            strsql += " Tmp_Exchange Char(1),";
            strsql += " Tmp_stlmnt VarChar(9),";
            strsql += " Tmp_Clientcd VarChar(8),";
            strsql += " Tmp_Scripcd VarChar(6),";
            strsql += " Tmp_RegForFO VarChar(1),";
            strsql += " Tmp_Qty Numeric,";
            strsql += " Tmp_Rate Money,";
            strsql += " Tmp_MarketRate Money,";
            strsql += " Tmp_Value Money,";
            strsql += " Tmp_MrgHairCut Money,";
            strsql += " Tmp_NetValue Money)";
            mylib.ExecSQL(strsql, curCon);



        }

        public string GetTMinusPlusdt(string strDate, int intDay, string strExch)
        {
            LibraryModel mylib = new LibraryModel();
            UtilityModel util = new UtilityModel();
            //strDate = util.dtos(strDate);
            for (int intCnt = 0, loopTo = Math.Abs(intDay) - 1; intCnt <= loopTo; intCnt++)
            {
            Againdt:

                strDate = util.dtos(mylib.AddDayDT(strDate, (Math.Sign(intDay) * 1)).ToString());


                if (Convert.ToInt32(mylib.fnFireQuery("THoliday_master", "count(0)", "hm_exchange = '" + strExch + "' and hm_dt", strDate, true)) > 0)
                {
                    goto Againdt;
                }
            }

            return strDate;
        }

        public Boolean ckyc()
        {
            UtilityDBModel objutility = new UtilityDBModel();
            LibraryModel lib = new LibraryModel();
            string strLicenseKey;
            Boolean blnCKYC;
            blnCKYC = objutility.mfnGetSysSplFeature("CKC") | objutility.mfnGetSysSplFeature("CKE");
            HttpContext.Current.Session["VeriyfyCkycMenu"] = "False";
            strLicenseKey = (lib.A2N(Strings.Split(lib.fnFireQuery("sysSetting", "sys_Value", "'A'", "A", true), "-")[0])).ToString();
            if (Strings.Mid(strLicenseKey, 3, 1) == "0")
            {
                HttpContext.Current.Session["VeriyfyCkycMenu"] = "True";
                if (blnCKYC == true)
                {
                    return true;
                }
                else
                {

                    return false;
                }
            }
            else
            { return true; }

        }


        public void prNotTradedClnt(string strDt, string strApp, SqlConnection curCon)
        {
            string strsql;

            LibraryModel mylib = new LibraryModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            //HttpContext.Current.Session["IsTplusCommex"] = "N";
            SqlConnection dbsCommex = mydbutil.commexTemp_conn("Commex");
            curCon.Open();
            strsql = "if OBJECT_ID('tempdb..#TmpClnt') is not null Drop Table #TmpClnt";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

            strsql = "Create table #TmpClnt (tmp_Clnt VarChar(8) not null)";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));



            strsql = " Insert into #TmpClnt ";
            strsql += " select Distinct td_clientcd from trx Where td_dt between '" + Strings.Format(stod(strDt).AddDays(-30), "yyyyMMdd") + "' and '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and td_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
            strsql += " Union";
            strsql += " select Distinct td_clientcd from trades Where td_dt between '" + Strings.Format(stod(strDt).AddDays(-30), "yyyyMMdd") + "' and '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and td_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
            if (strApp == "T")
            {
                if (fnchkTable("MFTrades"))
                {
                    strsql += " Union";
                    strsql += " select Distinct MTd_ClientCd from MFTrades Where MTd_dt between '" + Strings.Format(stod(strDt).AddDays(-30), "yyyyMMdd") + "' and '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and MTd_CompanyCode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                }

                if (HttpContext.Current.Session["IsTplusCommex"].ToString() == "Y")
                {
                    strsql += " Union ";
                    strsql += " select Distinct td_clientcd from " + dbsCommex + "trades Where td_dt between '" + Strings.Format(stod(strDt).AddDays(-30), "yyyyMMdd") + "' and '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and td_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                }
            }
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));
        }

        public void pr30DaysNotTradeClnt(string strDt, string strApp, SqlConnection curCon)
        {

            string strsql;

            LibraryModel mylib = new LibraryModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            HttpContext.Current.Session["IsTplusCommex"] = "N";
            SqlConnection dbsCommex = mydbutil.commexTemp_conn("Commex");
            Boolean blnQSRCompWiseQtr = false;
            string strQSRWhere = "";
            string strQSRAdvanceFilter = "";
            strsql = "if OBJECT_ID('tempdb..#TmpQuarterlySquareoff') is not null Drop Table #TmpQuarterlySquareoff";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));
            strsql = "Create table #TmpQuarterlySquareoff (tqs_cmcd VarChar(8) not null,tqs_dt VarChar(8) not null,tqs_LastQSdt VarChar(8) not null,tqs_receiptdt VarChar(8) not null)";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));


            strsql = " Insert into #TmpQuarterlySquareoff ";
            strsql += " select qs_cmcd,qs_dt,'','' ";
            strsql += " from QuarterlySquareoff, Client_master,Branch_master where qs_cmcd=cm_cd and cm_brboffCode = bm_branchcd and qs_dt = '" + strDt + "' and qs_netvalue < 0 ";
            strsql = (strsql + (blnQSRCompWiseQtr ? " and isNull(qs_CompanyCode,'') = '" + HttpContext.Current.Session["CompanyCode"] + "' " : ""));
            strsql += strQSRWhere + strQSRAdvanceFilter;
            strsql += " and Cm_opendt < '" + Strings.Format(stod(strDt).AddDays(-30), "yyyyMMdd") + "'";
            strsql += " and cm_type <> 'P' ";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));


            strsql = " Update #TmpQuarterlySquareoff set tqs_LastQSdt = cq_qsdt from (";
            strsql += " select cq_cmcd,max(cq_qsdt) cq_qsdt from Client_QtrSqrOFF ,#TmpQuarterlySquareoff";
            strsql += " Where cq_qsdt <= '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and tqs_cmcd = cq_cmcd  Group by cq_cmcd ) a Where tqs_cmcd  = cq_cmcd";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

            strsql = " Delete a from #TmpQuarterlySquareoff a , QuarterlySquareoff b";
            strsql += " Where tqs_LastQSdt = qs_dt and tqs_cmcd = b.qs_cmcd and ( qs_TotalRetain > 0 Or abs(qs_LedBal) > 0 ) ";


            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

            strsql = " Delete #TmpQuarterlySquareoff Where Tqs_cmcd  in ( select qs_cmcd  ";
            strsql += " from QuarterlySquareoff , #TmpQuarterlySquareoff ";
            strsql += " Where qs_Dt between tqs_LastQSdt and '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and qs_cmcd = tqs_cmcd ";
            strsql += " Group by qs_cmcd ";
            strsql += " Having Sum(case When qs_TotalRetain > 0 Then 1 else 0 end) > 0 ) ";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

            strsql = " Update #TmpQuarterlySquareoff set tqs_receiptdt = receiptdt from (";
            strsql += " select rc_clientcd,min(rc_receiptdt) receiptdt from Receipts ,#TmpQuarterlySquareoff";
            strsql += " Where rc_debitflag = 'C' and rc_status = 'Y' and rc_receiptdt between tqs_LastQSdt and '" + Strings.Format(stod(strDt), "yyyyMMdd") + "' and rc_clientcd = tqs_cmcd Group by rc_clientcd ) a";
            strsql += " Where tqs_cmcd  = rc_clientcd ";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

            strsql = " Delete #TmpQuarterlySquareoff Where tqs_receiptdt = ''";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

            strsql = " Delete #TmpQuarterlySquareoff Where datediff(d,tqs_receiptdt,'" + Strings.Format(stod(strDt), "yyyyMMdd") + "') > 75";
            mylib.ExecSQL(strsql, (strApp == "C" ? dbsCommex : curCon));

        }


    }



    public class UtilityDBModel : ConnectionModel
    {
        public string GetMaxBillDate(bool blnDBformat = false)
        {
            UtilityModel myutil = new UtilityModel();
            strSQL = @"Select MAX(BillDate) From" +
                        "(" +
                           " select MAX(se_stdt) BillDate from Bills ,Settlements Where  bl_billdt=se_Stlmnt and  exists " + myutil.LoginAccess("bl_clientcd") +
                          "  Union All " +
                          "  select MAX(fb_billdt) BillDate from FBills Where exists " + myutil.LoginAccess("fb_clientcd") +
                       " ) X";
            LibraryModel mylib = new LibraryModel();
            string strMaxDate = mylib.GetScalarValueString(strSQL);
            if (strMaxDate.Trim() != "")
            {
                strMaxDate = myutil.DbToDate(strMaxDate);
                if (blnDBformat)
                {
                    UtilityModel util = new UtilityModel();
                    strMaxDate = util.dtos(strMaxDate);
                }
            }
            return strMaxDate;
        }


        public string GetT2BillDate()
        {
            string T2Date = "";
            UtilityModel myutil = new UtilityModel();
            UtilityModel util = new UtilityModel();
            LibraryModel mylib = new LibraryModel();

            strSQL = @"Select MAX(BillDate) From" +
                        "(" +
                           " select MAX(se_stdt) BillDate from Bills ,Settlements Where  bl_billdt=se_Stlmnt and  exists " + myutil.LoginAccess("bl_clientcd") +
                          "  Union All " +
                          "  select MAX(fb_billdt) BillDate from FBills Where exists " + myutil.LoginAccess("fb_clientcd") +
                       " ) X";

            string strMaxDate = mylib.GetScalarValueString(strSQL);
            if (strMaxDate == "" || strMaxDate == null)
            {
                strMaxDate = DateTime.Now.ToString("yyyyMMdd");
            }

            T2Date = util.DbToDate(GetTPlusDate(strMaxDate, 2)).ToString();

            return T2Date;
        }

        public string GetMinusT2BillDate()
        {
            string T2Date = "";
            UtilityModel myutil = new UtilityModel();
            UtilityModel util = new UtilityModel();
            LibraryModel mylib = new LibraryModel();

            strSQL = @"Select MAX(BillDate) From" +
                        "(" +
                           " select MAX(se_stdt) BillDate from Bills ,Settlements Where  bl_billdt=se_Stlmnt and  exists " + myutil.LoginAccess("bl_clientcd") +
                          "  Union All " +
                          "  select MAX(fb_billdt) BillDate from FBills Where exists " + myutil.LoginAccess("fb_clientcd") +
                       " ) X";

            string strMaxDate = mylib.GetScalarValueString(strSQL);
            if (strMaxDate == "" || strMaxDate == null)
            {
                strMaxDate = DateTime.Now.ToString("yyyyMMdd");
            }

            T2Date = mylib.AddDayDT(strMaxDate, -2).ToString();
            //T2Date = util.DbToDate(GetMinusDate(strMaxDate, -2)).ToString();

            return T2Date;
        }




        public string GetTPlusDate(string currentdate, int day)
        {

            UtilityModel util = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            string strMaxDate = currentdate;
            string strMaxDateNext = util.dtos(mylib.AddDayDT(strMaxDate, 1).ToString("dd/MM/yyyy"));
            int countdate = 0;
            if (strMaxDateNext.Trim() != "")
            {

                while (countdate < day)
                {
                    strSQL = "select * from FHoliday_master where hm_dt='" + strMaxDateNext + "'";
                    string IsdateHD = mylib.GetScalarValueString(strSQL);
                    if (IsdateHD == "")
                    {
                        countdate = countdate + 1;

                    }

                    if (countdate != day)
                    {
                        strMaxDateNext = util.dtos(mylib.AddDayDT(strMaxDateNext, 1).ToString("dd/MM/yyyy"));
                    }


                }

                // strMaxDate = util.dtos(strMaxDate);

            }
            return strMaxDateNext;
        }

        public string GetMinusDate(string currentdate, int day)
        {

            UtilityModel util = new UtilityModel();
            LibraryModel mylib = new LibraryModel();
            string strMaxDate = currentdate;
            string strMaxDateNext = util.dtos(mylib.AddDayDT(strMaxDate, -2).ToString("dd/MM/yyyy"));
            int countdate = 0;
            if (strMaxDateNext.Trim() != "")
            {

                while (countdate < day)
                {
                    strSQL = "select * from FHoliday_master where hm_dt='" + strMaxDateNext + "'";
                    string IsdateHD = mylib.GetScalarValueString(strSQL);
                    if (IsdateHD == "")
                    {
                        countdate = countdate - 1;

                    }

                    if (countdate != day)
                    {
                        strMaxDateNext = util.dtos(mylib.AddDayDT(strMaxDateNext, -1).ToString("dd/MM/yyyy"));
                    }


                }

                // strMaxDate = util.dtos(strMaxDate);

            }
            return strMaxDateNext;
        }


        public string GetMaxTrxDate(bool blnDBformat = false)
        {
            UtilityModel myutil = new UtilityModel();
            strSQL = "Select MAX(trxdate) From " +
                    " ( " +
                    " select MAX(td_dt) trxdate from trx " +
                     " union all " +
                     " select MAX(td_dt) trxdate from Trades " +
                      " ) X ";
            LibraryModel mylib = new LibraryModel();
            string strMaxDate = mylib.GetScalarValueString(strSQL);
            if (strMaxDate.Trim() != "")
            {
                strMaxDate = myutil.DbToDate(strMaxDate);
                if (blnDBformat)
                {
                    UtilityModel util = new UtilityModel();
                    strMaxDate = util.dtos(strMaxDate);
                }
            }
            return strMaxDate;
        }

        public SqlConnection commexTemp_conn(string strProduct)
        {
            LibraryModel mylib = new LibraryModel();
            var i = default(int);
            string strSlipScan;
            SqlConnection sqlSlipScanConn = null;
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                string stringSQL = "select * from other_products where op_status='A' AND op_product='" + strProduct + "'";
                curCon.Open();
                DataSet ObjDataSet = new DataSet();
                ObjDataSet = mylib.OpenDataSet(stringSQL, curCon);
                if (ObjDataSet.Tables[0].Rows.Count > 0)
                {
                    strSlipScan = "Initial Catalog=" + ObjDataSet.Tables[0].Rows[i]["op_database"].ToString().Trim() + ";Data Source=" + (ObjDataSet.Tables[0].Rows[i]["OP_server"]).ToString().Trim() + ";UID=" + (ObjDataSet.Tables[0].Rows[i]["op_user"]).ToString().Trim() + ";PWD=" + (ObjDataSet.Tables[0].Rows[i]["op_pwd"]).ToString().Trim() + ";Max Pool Size=2000;pooling='true' ;Connect Timeout=5000";
                    HttpContext.Current.Application["ConnectionSlipScane"] = strSlipScan;
                    sqlSlipScanConn = new SqlConnection(strSlipScan);
                }


                return sqlSlipScanConn;

            }
        }
        public SqlConnection SlipScanTemp_conn(string strProduct)
        {
            SqlConnection sqlConnTplusImages = null;

            try
            {
                LibraryModel mylib = new LibraryModel();
                var i = default(int);
                string strCommex;
                using (SqlConnection curCon = new SqlConnection(connectionstring))
                {
                    string stringSQL = "select * from other_products where op_status='A' AND op_product='" + strProduct + "'";
                    curCon.Open();
                    DataSet ObjDataSet = new DataSet();
                    ObjDataSet = mylib.OpenDataSet(stringSQL, curCon);
                    if (ObjDataSet.Tables[0].Rows.Count > 0)
                    {
                        strCommex = "Initial Catalog=" + ObjDataSet.Tables[0].Rows[i]["op_database"].ToString().Trim() + ";Data Source=" + (ObjDataSet.Tables[0].Rows[i]["OP_server"]).ToString().Trim() + ";UID=" + (ObjDataSet.Tables[0].Rows[i]["op_user"]).ToString().Trim() + ";PWD=" + (ObjDataSet.Tables[0].Rows[i]["op_pwd"]).ToString().Trim() + ";Max Pool Size=2000;pooling='true' ;Connect Timeout=5000";
                        HttpContext.Current.Application["ConnectionSlipScane"] = strCommex;
                        sqlConnTplusImages = new SqlConnection(strCommex);

                    }
                    return sqlConnTplusImages;
                }
            }

            catch (SqlException ex)
            {
                return sqlConnTplusImages;
            }





        }

        public Boolean mfnGetSysSplFeature(string strKeyCode)
        {
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                string strsql;
                string strcomname;
                Boolean mfnGetSysSplFeature = false;
                DataTable dtfind = new DataTable();
                LibraryModel mylib = new LibraryModel(true);
                UtilityModel myutil = new UtilityModel();
                UtilityDBModel mydbutil = new UtilityDBModel();


                strsql = "select st_KeyCode ,st_KeyVal From sysTable Where st_KeyCode  = '" + strKeyCode + "'";
                dtfind = mylib.OpenDataTable(strsql, curCon);
                if (dtfind.Rows.Count > 0)
                {
                    strsql = "select em_Name from Entity_master where em_cd =(select min(em_cd) from Entity_master)";
                    DataTable dscomp = new DataTable();
                    dscomp = mylib.OpenDataTable(strsql, curCon);
                    // strcomname = Tostring().Trim(Left(UCase(dscomp.Rows[0]["em_Name"]), 20));
                    strcomname = Strings.Left(Convert.ToString(dscomp.Rows[0]["em_Name"]), 20);
                    if (myutil.Decrypt(dtfind.Rows[0]["st_KeyVal"].ToString()) == (strKeyCode + strcomname).ToString().Trim())
                    {
                        mfnGetSysSplFeature = true;
                    }
                }
                return mfnGetSysSplFeature;
            }
        }
        public string GetWebParameter(string strParmcd)
        {
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                LibraryModel mylib = new LibraryModel();
                string strsql = string.Empty;
                string strReturn = string.Empty;
                strsql = "select sp_sysvalue  from WebParameter where sp_parmcd ='" + strParmcd + "'";
                DataSet ds = new DataSet();
                ds = mylib.OpenDataSet(strsql, curCon);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strReturn = Convert.ToString(ds.Tables[0].Rows[0]["sp_sysvalue"]);
                    return strReturn;
                }
                else
                {
                    return "";
                }
            }

        }
        public string AddBracket(string strparm)
        {
            string temp = "";
            if (strparm != "")
            {
                int count = 0;
                string[] Arrstring = strparm.Split('/');
                foreach (string str in Arrstring)
                {
                    if (count < 3)
                    { temp += "[" + str + "]"; }
                    else { temp += str; }
                    if (count != Arrstring.Length - 1)
                    { temp = temp + "/"; }
                    count++;
                }
            }
            return temp;
        }
        public void getallwebPeram()
        {
            HttpContext.Current.Application["CommDatabase"] = "Commex";
            HttpContext.Current.Application["ChangeForNCDEX"] = "N";
            HttpContext.Current.Session["IsTplusCommex"] = "N";
            LibraryModel mylib = new LibraryModel();
            DataSet Dstpluscommex = new DataSet();
            HttpContext.Current.Application["CMSCHEDULE"] = mylib.fnGetSysParam("CMSCHEDULE");
            // ConfigurationManager.AppSettings["Commex"] = AddBracket(GetWebParameter("Commex"));
            ConfigurationManager.AppSettings["Commex"] = AddBracket(commexTemp_conn("Commex").ToString());

            Dstpluscommex = mylib.OpenDataSet("select count(0) from CompanyExchangeSegments where right(rtrim(ces_cd),1)='X'");
            if (Dstpluscommex.Tables[0].Rows.Count > 0)
            {
                if (mylib.mfnGetSysSplFeatureCommodity("TCM", (string)HttpContext.Current.Application["CommDatabase"]))
                {
                    HttpContext.Current.Session["IsTplusCommex"] = "Y";
                    if (mylib.fnGetSysParamComm("CHGNCDEXCD", (string)HttpContext.Current.Application["CommDatabase"]) == "Y")
                    {
                        HttpContext.Current.Application["ChangeForNCDEX"] = "Y";
                    }
                }
            }

        }
        public void getCompanyDetail()
        {
            string strsql = "";
            DataTable dt = new DataTable();
            LibraryModel mylib = new LibraryModel();

            SqlConnection SQLConnComex = commexTemp_conn("Commex");
            if (SQLConnComex != null)

            {

                strsql = "select em_cd,em_name,em_add1,em_add2,em_add3,em_add4,em_panno from Entity_master ";
                strsql += "where em_cd in (select min(em_cd) from Entity_master) ";
                dt = mylib.OpenDataTable(strsql, SQLConnComex);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["CommCompanyCode"] = (dt.Rows[0]["em_cd"]).ToString();
                    HttpContext.Current.Session["strCommCompName"] = (dt.Rows[0]["em_name"]).ToString();
                    HttpContext.Current.Session["strCommCompAdd1"] = (dt.Rows[0]["em_add1"]).ToString();
                    HttpContext.Current.Session["strCommCompAdd2"] = (dt.Rows[0]["em_add2"]).ToString();
                    HttpContext.Current.Session["strCommCompAdd3"] = (dt.Rows[0]["em_add3"]).ToString();
                    HttpContext.Current.Session["strCommCompAdd4"] = (dt.Rows[0]["em_add4"]).ToString();
                    HttpContext.Current.Session["CommPanNo"] = (dt.Rows[0]["em_panno"]).ToString();

                }
            }
        }
        public SqlConnection crostemp_conn(string strProduct)
        {
            LibraryModel mylib = new LibraryModel();
            var i = default(int);
            string strCross;
            string strEstro;
            SqlConnection sqlSlipScanConn = null;
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                curCon.Open();
                string stringSQL = "select * from other_products where op_status='A' AND op_product='" + strProduct + "'";
                SqlCommand objSqlCommand = new SqlCommand(stringSQL, curCon);


                DataSet ObjDataSet = new DataSet();
                ObjDataSet = mylib.OpenDataSet(stringSQL, curCon);
                SqlDataAdapter ObjAdapter = new SqlDataAdapter(objSqlCommand);

                ObjAdapter.Fill(ObjDataSet);

                for (i = 0; i <= ObjDataSet.Tables[0].Rows.Count - 1; i++)
                {
                    if ((Strings.Trim(ObjDataSet.Tables[0].Rows[i]["op_product"].ToString().Trim())) == "Cross")
                    {
                        strCross = "Initial Catalog=" + ObjDataSet.Tables[0].Rows[i]["op_database"].ToString().Trim() + ";Data Source=" + (ObjDataSet.Tables[0].Rows[i]["OP_server"]).ToString().Trim() + ";UID=" + (ObjDataSet.Tables[0].Rows[i]["op_user"]).ToString().Trim() + ";PWD=" + (ObjDataSet.Tables[0].Rows[i]["op_pwd"]).ToString().Trim() + ";Max Pool Size=2000;pooling='true' ;Connect Timeout=5000";
                        HttpContext.Current.Application["ConnectionSlipScane"] = strCross;
                        sqlSlipScanConn = new SqlConnection(strCross);
                    }
                    else if ((Strings.Trim(ObjDataSet.Tables[0].Rows[i]["op_product"].ToString().Trim().ToUpper())) == "ESTRO")
                    {
                        strEstro = "Initial Catalog=" + ObjDataSet.Tables[0].Rows[i]["op_database"].ToString().Trim() + ";Data Source=" + (ObjDataSet.Tables[0].Rows[i]["OP_server"]).ToString().Trim() + ";UID=" + (ObjDataSet.Tables[0].Rows[i]["op_user"]).ToString().Trim() + ";PWD=" + (ObjDataSet.Tables[0].Rows[i]["op_pwd"]).ToString().Trim() + ";Max Pool Size=2000;pooling='true' ;Connect Timeout=5000";
                        HttpContext.Current.Application["ConnectionSlipScane"] = strEstro;
                        sqlSlipScanConn = new SqlConnection(strEstro);
                    }
                }


                return sqlSlipScanConn;

            }



        }
        public string CheckConnection(string strProduct)
        {

            LibraryModel myLib = new LibraryModel();

            var i = default(int);
            string strSlipScan;
            string StrSQL = "";
            string strValidConn = "0";
            var dsDT = new DataTable();

            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                StrSQL = "select * from other_products where op_status='A' AND op_product='" + strProduct + "'";
                dsDT = myLib.OpenDataTable(StrSQL, curCon);
            }
            if (dsDT.Rows.Count > 0)
            {
                strSlipScan = "Initial Catalog=" + dsDT.Rows[i]["op_database"].ToString().Trim() + ";Data Source=" + dsDT.Rows[i]["OP_server"].ToString().Trim() + ";UID=" + dsDT.Rows[i]["op_user"].ToString().Trim() + ";PWD=" + dsDT.Rows[i]["op_pwd"].ToString().Trim() + ";   Connect Timeout=5000";
                // HttpContext.Current.Application["ConnectionSlipScane"] = strSlipScan;
                strValidConn = IsServerConnected(strSlipScan);
                return strValidConn;
            }
            else
            {

                return strValidConn;
            }
        }

        public string IsServerConnected(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                    return "1";
                }
                catch (SqlException ex)
                {
                    return ex.Message;
                }
                catch (Exception generatedExceptionName)
                {
                    return generatedExceptionName.Message;
                }
            }
        }



        public void prProcess(string strDT, string strExchange, string strWhere, bool blnCollExcessOppExch = false)
        {
            LibraryModel myLib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();
            string strTempRMSSummary = "#TmpRMSSummaryReport";
            string strTempRMSDetail = "#TmpRMSDetailReport";

            // SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");

            double dblHairCut = Convert.ToDouble(myLib.fnGetSysParam("MTFP_HAIRCUT"));

            Boolean blnBSE;
            Boolean blnNSE;
            blnBSE = myLib.fnFireQuery("Sysparameter", "sp_sysvalue", "sp_parmcd", "MTFP_LICBSE", false) == "Y";
            blnNSE = myLib.fnFireQuery("Sysparameter", "sp_sysvalue", "sp_parmcd", "MTFP_LICNSE", false) == "Y";



            DataTable dtSum = null;
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {

                curCon.Open();
                string strsql;

                //----------------------create temp table---------------------------

                strsql = "Drop Table " + strTempRMSSummary;
                myLib.ExecSQL(strsql, curCon);

                strsql = "Create Table " + strTempRMSSummary + " (";
                strsql += " Tmp_Clientcd VarChar(8),";
                strsql += " Tmp_Limit Money,";
                strsql += " Tmp_TplusBal Money,";
                strsql += " Tmp_LoanBal Money,";
                strsql += " Tmp_FundedAmount Money,";
                strsql += " Tmp_FundedMrgReq Money,";
                strsql += " Tmp_CollateralFund Money,";
                strsql += " Tmp_CollateralValue Money,";
                strsql += " Tmp_ShortFallExcess Money,";
                strsql += " Tmp_TradeValue Money,";
                strsql += " Tmp_M2MLoss Money)";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Drop Table " + strTempRMSDetail;
                myLib.ExecSQL(strsql, curCon);

                strsql = "Create Table " + strTempRMSDetail + " (";
                strsql += " Tmp_Type Char(1),";
                strsql += " Tmp_Exchange Char(1),";
                strsql += " Tmp_Clientcd VarChar(8),";
                strsql += " Tmp_Scripcd VarChar(6),";
                strsql += " Tmp_RegForFO VarChar(1),";
                strsql += " Tmp_Qty Numeric,";
                strsql += " Tmp_Rate Money,";
                strsql += " Tmp_MarketRate Money,";
                strsql += " Tmp_Value Money,";
                strsql += " Tmp_MrgHairCut Money,";
                strsql += " Tmp_NetValue Money)";
                myLib.ExecSQL(strsql, curCon);


                //-----------------------------------------------------------------



                strsql = "Insert into " + strTempRMSSummary;
                strsql += " select MTFC_CMcd,MTFC_AllowLimit,0,0,0,0,0,0,0,0,0";
                strsql += " from MrgTdgFin_Clients,Client_master ";
                strsql += " Where cm_cd = MTFC_CMcd and MTFC_Status='A' and MTFC_RegDt<='" + strDT + HttpContext.Current.Session["LoginAccessOld"] + "'" + strWhere + "";

                myLib.ExecSQL(strsql, curCon);

                strsql = "Update " + strTempRMSSummary;
                strsql += " set Tmp_TplusBAL = A.ld_amount ";
                strsql += " from (select MTFC_CMcd,Sum(ld_amount) ld_amount ";
                strsql += " from Ledger,MrgTdgFin_Clients,Client_master";
                strsql += " Where cm_cd = MTFC_CMcd and (ld_clientcd = cm_cd " + (myLib.fnGetSysParam("MTFP_MRGNBAL") == "Y" ? "or ld_clientcd= cm_brkggroup" : "") + ")";
                strsql += " and ld_dt <='" + strDT + "' "; // and " & mfnGetLedType(cmbTPLedgerBal)
                strsql += " Group By MTFC_CMcd ) a ";
                strsql += " Where MTFC_CMcd = Tmp_Clientcd ";

                myLib.ExecSQL(strsql, curCon);

                // T+2 day loan balance
                //string strT2 = mfnGetT2Dt[strExchange, strDT];
                //string strT2 = myutil.AddDayDT(strDT, 2).ToString("yyyyMMdd");
                //string strT2 = myLib.mfnGetT2Dt("", strDT);

                string strT2 = myutil.mfnGetT2Dt("", strDT);


                strsql = "Update " + strTempRMSSummary + Constants.vbNewLine;
                strsql += " set Tmp_LoanBal = A.ld_amount " + Constants.vbNewLine;
                strsql += " from (select MTFC_CMcd,Sum(ld_amount) ld_amount " + Constants.vbNewLine;
                strsql += " from Ledger,MrgTdgFin_Clients" + Constants.vbNewLine;
                strsql += " Where ld_clientcd = Rtrim(MTFC_CMcd) + '" + myLib.fnGetSysParam("MTFP_SUFFIX") + "' and ld_documentType not in ('P','R') ";
                strsql += " and ld_dt <= '" + strT2 + "' " + Constants.vbNewLine;
                strsql += " and left(ld_DPID,1) = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                strsql += " Group By MTFC_CMcd ) a " + Constants.vbNewLine;
                strsql += " Where MTFC_CMcd = Tmp_Clientcd " + Constants.vbNewLine;

                myLib.ExecSQL(strsql, curCon);

                strsql = "Update " + strTempRMSSummary;
                strsql += " set Tmp_CollateralFund = A.ld_amount ";
                strsql += " from (select MTFC_CMcd,Sum(-ld_amount) ld_amount ";
                strsql += " from Ledger,MrgTdgFin_Clients";
                strsql += " Where ld_clientcd = Rtrim(MTFC_CMcd) + '" + myLib.fnGetSysParam("MTFP_SUFFIX") + "' and ld_documentType in ('P','R') ";
                strsql += " and ld_dt <='" + strDT + "'";
                strsql += " and left(ld_DPID,1) = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                strsql += " Group By MTFC_CMcd Having Sum(-ld_amount) >  0 ) a ";
                strsql += " Where MTFC_CMcd = Tmp_Clientcd ";

                myLib.ExecSQL(strsql, curCon);

                strsql = "Update " + strTempRMSSummary;
                strsql += " set Tmp_TradeValue = A.TradeValue ";
                strsql += " from (select MTtd_clientcd, Round(Sum((MTtd_bqty-MTtd_sqty)*MTtd_Rate),2) TradeValue ";
                strsql += " from MrgTdgFin_TRX,Settlements ";
                strsql += " Where MTtd_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and MTTd_stlmnt = se_stlmnt and se_shpayoutdt ='" + strDT + "'";
                strsql += " Group By MTtd_clientcd ) a ";
                strsql += " Where MTtd_clientcd = Tmp_Clientcd ";
                myLib.ExecSQL(strsql, curCon);

                strsql = " Insert into " + strTempRMSDetail;
                strsql += " select type,";
                strsql += " case dm_OurDP When '" + myLib.fnGetSysParam("MTFP_DMTBSE").Trim().Trim() + "' Then 'B' When '" + myLib.fnGetSysParam("MTFP_DMTNSE").Trim().Trim() + "' Then 'N' else '' end,";
                strsql += " dm_clientcd,dm_Scripcd,'N',Sum(Qty),0,0,0,0,0 from ( ";

                strsql += " select 'X' type,dm_OurDP,dm_clientcd,dm_Scripcd,Sum(-dm_qty) Qty";
                strsql += " from Demat,Settlements,client_master";
                strsql += " Where dm_stlmnt=se_stlmnt and dm_clientcd=cm_cd ";
                strsql += " and se_shpayoutdt <='" + strDT + HttpContext.Current.Session["LoginAccessOld"] + "'" + strWhere + "";

                strsql += " and dm_ourDP in ('" + myLib.fnGetSysParam("MTFP_DMTBSE") + "','" + myLib.fnGetSysParam("MTFP_DMTNSE") + "')";

                strsql += " and dm_type ='BC' and dm_locked ='N' and dm_transfered = 'N'";
                strsql += " Group By dm_OurDP,dm_clientcd,dm_Scripcd";

                // Future Date Payout to Client & Benf to Pool
                strsql += " union all ";
                strsql += " select 'X' type,dm_OurDP,dm_clientcd,dm_Scripcd,Sum(-dm_qty) Qty";
                strsql += " from Demat,Settlements,client_master";
                strsql += " Where dm_stlmnt=se_stlmnt and dm_clientcd=cm_cd ";
                strsql += " and se_shpayoutdt <='" + strDT + "'";
                strsql += " and dm_execdt > '" + strDT + HttpContext.Current.Session["LoginAccessOld"] + strWhere + "'";


                strsql += " and dm_ourDP in ('" + myLib.fnGetSysParam("MTFP_DMTBSE") + "','" + myLib.fnGetSysParam("MTFP_DMTNSE") + "')";
                strsql += " and dm_type ='BC' and (dm_locked <>'N' Or dm_transfered <> 'N') ";
                strsql += " Group By dm_OurDP,dm_clientcd,dm_Scripcd";

                // Expected
                strsql += " union all ";
                strsql += " select 'X',dm_OurDP,dm_clientcd,dm_Scripcd,Sum(-dm_qty)";
                strsql += " from Demat,Settlements,client_master";
                strsql += " Where dm_stlmnt=se_stlmnt and dm_clientcd=cm_cd ";
                strsql += " and se_stdt <= '" + strDT + "'";
                strsql += " and se_shpayoutdt > '" + strDT + HttpContext.Current.Session["LoginAccessOld"] + strWhere + "'";


                strsql += " and dm_ourDP in ('" + myLib.fnGetSysParam("MTFP_DMTBSE") + "','" + myLib.fnGetSysParam("MTFP_DMTNSE") + "')";

                strsql += " and dm_type ='BC'";
                strsql += " Group By dm_OurDP,dm_clientcd,dm_Scripcd";


                strsql += " ) a Group By type,dm_OurDP,dm_clientcd,dm_Scripcd ";

                myLib.ExecSQL(strsql, curCon);


                strsql = "Insert into " + strTempRMSDetail;
                strsql += " select 'M',MTtd_Exchange,MTtd_clientcd,MTtd_scripcd,'N',Sum(MTtd_bqty-MTtd_sqty),Round(Sum((MTtd_bqty-MTtd_sqty)*MTtd_rate)/Sum(MTtd_bqty-MTtd_sqty),2),0,0,0,0";
                strsql += " from MrgTdgFin_TRX,client_master ";
                strsql += " Where MTtd_clientcd=cm_cd and MTtd_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' and MTtd_dt <= '" + strDT + HttpContext.Current.Session["LoginAccessOld"] + "'" + strWhere + "";

                strsql += " Group By MTtd_Exchange,MTtd_clientcd,MTtd_scripcd";   // ,MTtd_CntStlmnt
                strsql += " Having Sum(MTtd_bqty-MTtd_sqty) <> 0 ";
                myLib.ExecSQL(strsql, curCon);


                strsql = "Insert into " + strTempRMSDetail;
                strsql += " select 'C',Tmp_Exchange,Tmp_Clientcd,Tmp_Scripcd,'N',Sum(case Tmp_Type When 'X' Then Tmp_Qty else -Tmp_Qty end),0,0,0,0,0";
                strsql += " from " + strTempRMSDetail;
                strsql += " Group By Tmp_Exchange,Tmp_Clientcd,Tmp_Scripcd ";
                strsql += " Having Sum(case Tmp_Type When 'X' Then Tmp_Qty else -Tmp_Qty end) > 0 ";
                myLib.ExecSQL(strsql, curCon);

                strsql = " Delete " + strTempRMSDetail + " where Tmp_Type = 'X' ";
                myLib.ExecSQL(strsql, curCon);


                strsql = "Delete " + strTempRMSDetail + Constants.vbNewLine + " Where Tmp_Type = 'C' and Tmp_Scripcd not in (select MTFD_SecCD from MrgTdgFin_SecuritiesDt ";
                strsql += " Where MTFD_SecDt = (Select max(MTFD_SecDt) from MrgTdgFin_SecuritiesDt where MTFD_SecDt <= '" + strDT + "') and (MTFD_BSE = 'Y' Or MTFD_NSE = 'Y'))";
                myLib.ExecSQL(strsql, curCon);


                //string[] arrExch;

                // arrExch = ((IIf(blnBSE, "B/", "") + IIf(blnNSE, "N/", "")).s("/");// BSE, NSE

                string[] arrExch = ((blnBSE ? "B/" : "") + (blnNSE ? "N/" : "")).Split('/');


                for (int i = 0; i < arrExch.Length - 1; i++)
                {
                    string line = arrExch[i];
                    if (line != "")
                    {
                        strsql = "Update " + strTempRMSDetail;
                        strsql += " set Tmp_MarketRate = mk_closerate from market_rates a with (nolock) where mk_exchange = '" + line + "' and mk_scripcd=Tmp_Scripcd ";
                        strsql += " and mk_dt= (select max(mk_dt) from market_rates with(nolock) where mk_exchange = '" + line + "'";
                        strsql += " and mk_dt <='" + strDT + "') ";
                        strsql += " and Tmp_Type= 'M' and Tmp_Exchange = '" + line + "'";
                        myLib.ExecSQL(strsql, curCon);

                        strsql = "Update " + strTempRMSDetail;
                        strsql += " set Tmp_MarketRate = mk_closerate from market_rates a with (nolock) where mk_exchange = '" + line + "' and mk_scripcd=Tmp_Scripcd ";
                        strsql += " and mk_dt= (select max(mk_dt) from market_rates with(nolock) where mk_exchange = '" + line + "'";
                        strsql += " and mk_dt <'" + strDT + "') ";
                        strsql += " and Tmp_Type <> 'M' and Tmp_Exchange = '" + line + "'";
                        myLib.ExecSQL(strsql, curCon);

                        strsql = "Update " + strTempRMSDetail;
                        strsql += " Set Tmp_RegForFO = 'Y'  ";
                        strsql += " From Product_master, Securities ";
                        strsql += " Where pm_Exchange = '" + line + "' and pm_assetcd = " + (line == "B" ? "ss_Bsymbol" : "ss_nsymbol");

                        strsql += " and Tmp_Scripcd  = ss_cd and Tmp_Exchange = '" + line + "'";
                        if (Convert.ToInt32(myLib.fnFireQuery("Product_Expiry", "Count(0)", "pe_expirydt >= '" + strDT + "' and pe_exchange", line, false)) > 0)
                            strsql += " and pm_assetcd in ( select pe_assetcd from Product_Expiry Where pe_exchange = '" + line + "' and pe_expirydt >= '" + strDT + "')";
                        myLib.ExecSQL(strsql, curCon);

                        strsql = "update " + strTempRMSDetail;
                        strsql += " set Tmp_MrgHairCut = ";
                        strsql += " case Tmp_Type When 'C' Then Case When vm_exchange = 'N' then vm_applicable_var else vm_margin_rate end else (Case When vm_exchange = 'N' then vm_applicable_var else vm_margin_rate end * " + myLib.fnGetSysParam("MTFP_VARMAR") + " ) + (vm_max_loss * case Tmp_RegForFO When 'Y' Then " + myLib.fnGetSysParam("MTFP_ELMRGFO") + " else " + myLib.fnGetSysParam("MTFP_ELMNTFO") + " end ) end ";
                        strsql += " from VarMargin  ";
                        strsql += " where vm_scripcd = Tmp_Scripcd and vm_exchange = '" + line + "'";
                        strsql += " and vm_dt =(select max(vm_dt) from VarMargin ";
                        strsql += " Where vm_exchange = '" + line + "' ";
                        strsql += " and vm_dt <'" + strDT + "')";
                        strsql += " and Tmp_Exchange = '" + line + "'";
                        myLib.ExecSQL(strsql, curCon);
                    }

                }

                strsql = "update " + strTempRMSDetail + "  set Tmp_MrgHairCut = " + dblHairCut + " Where Tmp_MrgHairCut < " + dblHairCut;
                myLib.ExecSQL(strsql, curCon);

                strsql = "update " + strTempRMSDetail + "  set Tmp_Value = Round(Tmp_Qty*Tmp_Rate,2) Where Tmp_Type= 'M' ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "update " + strTempRMSDetail + "  set Tmp_Value = Round(Tmp_Qty*Tmp_MarketRate,2) Where Tmp_Type <> 'M' ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "update " + strTempRMSDetail + "  set Tmp_NetValue = Round(Tmp_Value*((Tmp_MrgHairCut)/100),2) Where Tmp_Type = 'M' ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "update " + strTempRMSDetail + "  set Tmp_NetValue = Round(Tmp_Value*((100-Tmp_MrgHairCut)/100),2) Where Tmp_Type <> 'M' ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Update " + strTempRMSSummary + "  set Tmp_FundedAmount = A.Tmp_FundedAmount, Tmp_FundedMrgReq  = A.Tmp_FundedMrgReq, ";
                strsql += " Tmp_CollateralValue = A.Tmp_CollateralValue ";
                strsql += " from (select Tmp_Clientcd Clientcd,";
                strsql += " Round(Sum(case Tmp_Type When 'M' Then Tmp_Value else 0 end),2) Tmp_FundedAmount, ";
                strsql += " Round(Sum(case Tmp_Type When 'M' Then Tmp_NetValue else 0 end),2) Tmp_FundedMrgReq , ";
                strsql += " Round(Sum(case Tmp_Type When 'C' Then Tmp_NetValue else 0 end),2) Tmp_CollateralValue ";
                strsql += " from " + strTempRMSDetail;
                strsql += " Group By Tmp_Clientcd ) a ";
                strsql += " Where Clientcd = Tmp_Clientcd ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Update " + strTempRMSSummary + "  set Tmp_M2MLoss = A.M2MLoss ";
                strsql += " From ( select Tmp_clientcd Tmpclientcd, Round(Sum((Tmp_Rate-Tmp_MarketRate)*Tmp_Qty),2) M2MLoss ";
                strsql += " From " + strTempRMSDetail;
                strsql += " Where Tmp_type = 'M' and Tmp_Rate > Tmp_MarketRate  ";
                strsql += " Group By Tmp_clientcd) A ";
                strsql += " Where Tmp_Clientcd = Tmpclientcd ";
                myLib.ExecSQL(strsql, curCon);

                strsql = "Update " + strTempRMSSummary + "  set Tmp_ShortFallExcess = (Tmp_CollateralFund+Tmp_CollateralValue)-(Tmp_FundedMrgReq+Tmp_M2MLoss) ";


                myLib.ExecSQL(strsql, curCon);



            }


        }

        public SqlConnection EsignConnectionString(string strProduct)
        {
            LibraryModel myLib = new LibraryModel();
            SqlConnection sqlSlipScanConn;
            var i = default(int);
            string strSlipScan = "";
            string StrSQL = "";
            var dsDT = new DataTable();
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                StrSQL = "select * from other_products where op_status='A' AND op_product='" + strProduct + "'";
                dsDT = myLib.OpenDataTable(StrSQL, curCon);
            }
            if (dsDT.Rows.Count > 0)
            {
                strSlipScan = "Initial Catalog=" + dsDT.Rows[i]["op_database"].ToString().Trim() + ";Data Source=" + dsDT.Rows[i]["OP_server"].ToString().Trim() + ";UID=" + dsDT.Rows[i]["op_user"].ToString().Trim() + ";PWD=" + dsDT.Rows[i]["op_pwd"].ToString().Trim() + ";   Connect Timeout=5000";
                HttpContext.Current.Application["ConnectionSlipScane"] = strSlipScan;
                sqlSlipScanConn = new SqlConnection(strSlipScan);
                return sqlSlipScanConn;
            }
            else
            {
                return null;
            }

        }

        public string getlogoimageURL()
        {
            LibraryModel myLib = new LibraryModel();
            string LogoUrl = "";

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/CompanyLogo.BMP")))
            {
                string strsql;
                strsql = "Select img_code, img_logo from Images where img_desc = 'Company Logo' and img_logo is not null order by img_code";
                DataTable dtLogo = new DataTable();
                var ObjConn = new SqlConnection();
                ObjConn = new SqlConnection(connectionstring);
                if (ObjConn.State == ConnectionState.Closed)
                    ObjConn.Open();
                dtLogo = myLib.OpenDataTable(strsql, ObjConn);
                if (dtLogo.Rows.Count > 0)
                {
                    byte[] buff = (byte[])dtLogo.Rows[0]["img_logo"];
                    if (buff is object)
                    {
                        var fs = new FileStream(HttpContext.Current.Server.MapPath("~/CompanyLogo.BMP"), FileMode.OpenOrCreate, FileAccess.Write);
                        fs.Write(buff, 0, buff.Length);
                        fs.Flush();
                        fs.Close();
                        LogoUrl = "~/CompanyLogo.BMP";
                    }
                }
                else
                {
                    LogoUrl = "";
                }
            }
            else
            {
                LogoUrl = "~/CompanyLogo.BMP";
            }
            return LogoUrl;
        }

        public void GetXCompanylogo()
        {
            string ImageUrl = "";
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/CompanyLogo.BMP")))
            {
                string strsql;
                strsql = "Select img_code, img_logo from Images where img_desc = 'Company Logo' and img_logo is not null  order by img_code";
                var dsLogo = new DataTable();
                var ObjConn = new SqlConnection();
                ObjConn = new SqlConnection();
                if (ObjConn.State == ConnectionState.Closed)
                    ObjConn.Open();
                var SqlDataAdapter = new SqlDataAdapter(strsql, ObjConn);

                if (dsLogo.Rows.Count > 0)
                {
                    byte[] buff = (byte[])dsLogo.Rows[0]["img_logo"];
                    if (buff is object)
                    {
                        var fs = new FileStream(HttpContext.Current.Server.MapPath("~/CompanyLogo.BMP"), FileMode.OpenOrCreate, FileAccess.Write);
                        fs.Write(buff, 0, buff.Length);
                        fs.Flush();
                        fs.Close();
                        ImageUrl = "~/CompanyLogo.BMP";
                    }
                }
                else
                {
                    ImageUrl = "";
                }
            }
            else
            {
                ImageUrl = "~/CompanyLogo.BMP";
            }
        }




        public partial struct urdata
        {
            public string bprole;
            public string bpname;
        }

        //public void run_menuQuery()
        //{
        //    LibraryModel mylib = new LibraryModel(true);
        //    string strSQL = "if (select count(*) from wmenu_master where wmn_modulecd = 'H0000000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0000000' insert into wmenu_master select '000','SUPER','mnuDashBoard','H0000000','DASHBOARD','M',0,'1111','A','#',1,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDashBoard','H0000000','DASHBOARD','M',0,'1111','A','#',1,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDashBoard','H0000000','DASHBOARD','M',0,'1111','A','#',1,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0000000')"
        //     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0100000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0100000' insert into wmenu_master select '000','SUPER','mnuDBClients','H0100000','     - CLIENTS','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBClients','H0100000','     - CLIENTS','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBClients','H0100000','     - CLIENTS','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0100000')"
        //     + "if (select count(*) from wmenu_master where wmn_modulecd=  'H0200000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0200000' insert into wmenu_master select '000','SUPER','mnuDBBills','H0200000','     - BILLS','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBBills','H0200000','     - BILLS','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup  end else  insert into wmenu_master select '000',ug_cd, 'mnuDBBills','H0200000','     - BILLS','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd='H0200000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0300000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0300000' insert into wmenu_master select '000','SUPER','mnuDBTrades','H0300000','     - TRADES','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBTrades','H0300000','     - TRADES','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBTrades','H0300000','     - TRADES','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0300000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0400000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0400000' insert into wmenu_master select '000','SUPER','MnuDBRiskMgmt','H0400000','     - RISK MANAGEMENT','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'MnuDBRiskMgmt','H0400000','     - RISK MANAGEMENT','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'MnuDBRiskMgmt','H0400000','     - RISK MANAGEMENT','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0400000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0500000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0500000' insert into wmenu_master select '000','SUPER','mnuDBSearch','H0500000','     - SEARCH','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBSearch','H0500000','     - SEARCH','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBSearch','H0500000','     - SEARCH','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0500000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0600000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0600000' insert into wmenu_master select '000','SUPER','mnuDBCharts','H0600000','     - Charts','M',0,'1111','A','#',1,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBCharts','H0600000','     - Charts','M',0,'1111','A','#',1,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBCharts','H0600000','     - Charts','M',0,'1111','A','#',1,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0600000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0601000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0601000' insert into wmenu_master select '000','SUPER','mnuDBCHartBroKeage','H0601000','           -Brokerages Analysis','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBCHartBroKeage','H0601000','           -Brokerages Analysis','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBCHartBroKeage','H0601000','           -Brokerages Analysis','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0601000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0602000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0602000' insert into wmenu_master select '000','SUPER','mnuDBChartRM','H0602000','           -Risk Management','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBChartRM','H0602000','           -Risk Management','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBChartRM','H0602000','           -Risk Management','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0602000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0603000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0603000' insert into wmenu_master select '000','SUPER','mnuDBChartDebtor','H0603000','           -Client Debtors','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBChartDebtor','H0603000','           -Client Debtors','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBChartDebtor','H0603000','           -Client Debtors','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0603000')"
        //    + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0604000' and wmn_groupcd = 'Super')= 0  Begin delete from wmenu_master where wmn_modulecd = 'H0604000' insert into wmenu_master select '000','SUPER','mnuDBChartCreditor','H0604000','           -Client Creditors','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  insert into wmenu_master select '000',ug_cd, 'mnuDBChartCreditor','H0604000','           -Client Creditors','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end else  insert into wmenu_master select '000',ug_cd, 'mnuDBChartCreditor','H0604000','           -Client Creditors','I',0,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup where ug_cd not in (select wmn_groupcd from wmenu_master where wmn_modulecd = 'H0604000')";
        //    var ObjConn = new SqlConnection();
        //    ObjConn = new SqlConnection(connectionstring);
        //    if (ObjConn.State == ConnectionState.Closed)
        //        ObjConn.Open();
        //    mylib.ExecSQL(strSQL, ObjConn);


        //}
        public void run_menuQuery()
        {
            LibraryModel mylib = new LibraryModel(true);
            modUserDetailsModel ud = new modUserDetailsModel();
            var ObjConn = new SqlConnection();
            ObjConn = new SqlConnection(connectionstring);
            //DataTable dt = mylib.OpenDataTable("SELECT count (DISTINCT wmn_modulecd) FROM WMenu_master t1 WHERE EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0000000') AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0100000') AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0200000')  AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0300000')"
            //      + "AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0400000')   AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0500000')   AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0100000')"
            //      + "AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0600000')   AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0601000')   AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0100000')"
            //      + "AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0602000')   AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0603000')   AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0100000')"
            //      + "AND EXISTS(SELECT * FROM WMenu_master WHERE wmn_modulecd = 'H0604000')", ObjConn);
            DataTable dt = mylib.OpenDataTable("SELECT count(0) FROM WMenu_master WHERE wmn_modulecd in ('H0000000','H0100000','H0200000','H0300000','H0400000', 'H050000', 'H0600000', 'H0601000', 'H0602000', 'H0603000', 'H0604000')", ObjConn);


            if (dt.Rows.Count <0 )
            {

                strSQL = "if (select count(*) from wmenu_master where wmn_modulecd = 'H0000000' )= 0  Begin insert into wmenu_master select '000',ug_cd, 'mnuDashBoard','H0000000','DASHBOARD','M',1,'1111','A','#',1,'',0,0,'Acer','20200518'  from usergroup end  "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0100000')= 0  Begin insert into wmenu_master select '000',ug_cd, 'mnuDBClients','H0100000',' - CLIENTS','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0200000')= 0  Begin insert into wmenu_master select '000',ug_cd, 'mnuDBBills','H0200000','- BILLS','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'  from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0300000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBTrades','H0300000','- TRADES','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'   from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0400000')= 0  Begin insert into wmenu_master select '000',ug_cd,'MnuDBRiskMgmt','H0400000','- RISK MANAGEMENT','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'    from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0500000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBSearch','H0500000','- SEARCH','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'    from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0600000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBCharts','H0600000','- Charts','M',1,'1111','A','#',1,'',0,0,'Acer','20200518'   from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0601000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBCHartBroKeage','H0601000','-Brokerages Analysis','I',1,'1111','A','#',0,'',0,0,'Acer','20200518' from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0602000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBChartRM','H0602000','-Risk Management','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'          from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0603000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBChartDebtor','H0603000',' -Client Debtors','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'   from usergroup end "
     + "if (select count(*) from wmenu_master where wmn_modulecd = 'H0604000')= 0  Begin insert into wmenu_master select '000',ug_cd,'mnuDBChartCreditor','H0604000',' -Client Creditors','I',1,'1111','A','#',0,'',0,0,'Acer','20200518'   from usergroup end ";


                if (ObjConn.State == ConnectionState.Closed)
                    ObjConn.Open();
                mylib.ExecSQL(strSQL, ObjConn);

            }
        }

        public urdata fnFindBpName(string strDPID)  // 
        {
            urdata fnFindBpNameRet = default(urdata);
            // To find DP Name
            var objconnection = new SqlConnection();
            //var objApplicationUser = new ApplicationUser();
            LibraryModel mylib = new LibraryModel(true);
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            SqlConnection ObjConCross = mydbutil.crostemp_conn("Cross");
            objconnection = mydbutil.crostemp_conn("ESTRO");
            if (objconnection.State == ConnectionState.Closed)
            {
                objconnection.Open();
            }

            var GBp = default(urdata);
            DataTable rsbpname = new DataTable();
            string strsql;
            strsql = "select bp_name,bp_role from Bpmaster where bp_id = '" + strDPID + "'";

            rsbpname = mylib.OpenDataTable(strsql);
            if (rsbpname.Rows.Count > 0)
            {
                GBp.bpname = rsbpname.Rows[0]["bp_name"].ToString();
                GBp.bprole = rsbpname.Rows[0]["bp_role"].ToString();
                fnFindBpNameRet = GBp;
            }
            else
            {
                fnFindBpNameRet = default(urdata);
            }

            return fnFindBpNameRet;
        }

        public string mfnFindCorporateAction(string strISin, string strNarration, string strMkttype, string strSettlement, string strExecdt, string strDrCr)
        {
            string mfnFindCorporateActionRet = "";
            DataTable rsAca = new DataTable();
            UtilityDBModel mydbutil = new UtilityDBModel();
            SqlConnection objconnection = new SqlConnection();
            LibraryModel mylib = new LibraryModel(true);
            objconnection = mydbutil.crostemp_conn("ESTRO");
            string strsql;
            strsql = "select cr_desc from CorporateAction_master where cr_isin = '" + strISin + "' and cr_execdt = '" + strExecdt + "'";
            strsql = strsql + " and cr_drcrind = '" + strDrCr + "'";
            if (objconnection.State == ConnectionState.Closed)
            {
                objconnection.Open();
            }

            rsAca = mylib.OpenDataTable(strsql);


            if (rsAca.Rows.Count > 0)
            {
                mfnFindCorporateActionRet = (rsAca.Rows[0]["cr_desc"]).ToString();
            }
            else
            {
                strsql = "select cr_description from Corporate_master where cr_isin = '" + strISin + "' and cr_date = '" + strExecdt + "'";
                DataTable rsAca1 = new DataTable();
                var ObjCommand1 = new SqlCommand(strsql, objconnection);
                var ObjAdapter1 = new SqlDataAdapter(ObjCommand1);
                ObjAdapter1.Fill(rsAca1);
                ObjAdapter1.Dispose();
                ObjCommand1.Dispose();
                if (rsAca1.Rows.Count > 0)
                {
                    mfnFindCorporateActionRet = (rsAca1.Rows[0]["cr_description"]).ToString();
                }
                else
                {
                    if (strNarration == "082" | strNarration == "088")
                    {
                        strsql = "Select ac_desc from Auto_corporate where ac_creditisin = '" + strISin + "' and ac_executiondt = '" + strExecdt + "'";
                    }
                    else
                    {
                        strsql = "Select ac_desc from Auto_corporate where ac_debitisin = '" + strISin + "'  and ac_executiondt = '" + strExecdt + "'";
                    }

                    DataTable rsAca2 = new DataTable();
                    var ObjCommand2 = new SqlCommand(strsql, objconnection);
                    ObjCommand2.CommandTimeout = 5000;
                    var ObjAdapter2 = new SqlDataAdapter(ObjCommand2);
                    ObjAdapter2.Fill(rsAca2);
                    ObjAdapter2.Dispose();
                    ObjCommand2.Dispose();
                    if (rsAca2.Rows.Count > 0)
                    {
                        mfnFindCorporateActionRet = (rsAca2.Rows[0]["ac_desc"]).ToString();
                    }
                    else
                    {
                        mfnFindCorporateActionRet = "Corporate Action";
                    }
                }
            }

            return mfnFindCorporateActionRet;
        }


        //public void LogingAccess(string gClientCodeField)
        //{
        //    string strsql = "";

        //    strsql = "if (select count(*) from sysobjects where name='LoginAccess')>0 select * from loginAccess where la_userID='" + gClientCodeField + "' order by la_grouping else select * from group_master where 1=2";
        //    string AccessFilter = "";
        //    var rsLogin = new SqlCommand(strsql, ObjConnection);
        //    var dbLogin = new SqlDataAdapter(rsLogin);
        //    var dsLogin = new DataSet();
        //    dbLogin.Fill(dsLogin);
        //    if (dsLogin.Tables(0).Rows.Count > 0)
        //    {
        //        short j; string strCatg = ""; string strTmp;
        //        j = Conversions.ToShort(0);
        //        strsql = "";
        //        while (j < dsLogin.Tables(0).Rows.Count)
        //        {
        //            strCatg = dsLogin.Tables(0).Rows(j).Item("La_grouping");
        //            strTmp = "";
        //            while (strCatg == dsLogin.Tables(0).Rows(j).Item("La_grouping"))
        //            {
        //                strTmp = strTmp + "'" + Trim(dsLogin.Tables(0).Rows(j).Item("la_grcode")) + "',";
        //                j = Conversions.ToShort(j + 1);
        //                if (j >= dsLogin.Tables(0).Rows.Count)
        //                    break;
        //            }
        //            strsql = strTmp;    // for Branches 
        //            strTmp = Strings.Mid(strTmp, 1, Strings.Len(strTmp) - 1);
        //            switch (Strings.UCase(strCatg))
        //            {
        //                case "B":
        //                    {
        //                        strCatg = "cm_brboffcode";
        //                        strsql = objutility.RPlace(objutility.RPlace(strsql, "'", ""), ",", "|");
        //                        Session["Branch"] = strsql;
        //                        break;
        //                    }

        //                case "G":
        //                    {
        //                        strCatg = "cm_groupcd";
        //                        break;
        //                    }

        //                case "F":
        //                    {
        //                        strCatg = "cm_familycd";
        //                        break;
        //                    }

        //                case "R":
        //                    {
        //                        strCatg = "cm_Subbroker";
        //                        break;
        //                    }

        //                case "A":
        //                    {
        //                        break;
        //                        break;
        //                    }

        //                case "C":
        //                    {
        //                        strCatg = "cm_cd";
        //                        break;
        //                    }

        //                case "M":
        //                    {
        //                        strCatg = "cm_dpactno";
        //                        break;
        //                    }
        //            }
        //            AccessFilter = Conversions.ToString(Conversions.ToString(AccessFilter + Interaction.IIf(Strings.Len(AccessFilter) > 0, " or ", "") + strCatg) + IIf(InStr(1, strTmp, ",") > 0, " in (" + strTmp + ")", "=" + strTmp));
        //        }
        //        if ((Strings.UCase(strCatg) ?? "") == "A")
        //            Session["LoginAccess"] = "";
        //        else
        //            Session["LoginAccess"] = " and ( " + AccessFilter + " )";
        //    }
        //    else
        //    {
        //        strsql = objutility.RPlace(Trim(HttpContext.Current.Session("Branch")), "|", "','");
        //        if ((Right(strsql, 3) ?? "") == "','")
        //            strsql = Left(strsql, Len(strsql) - 3);
        //        Session["LoginAccess"] = " and ( cm_brboffcode " + IIf(InStr(1, strsql, ",") > 0, " in ('" + strsql + "')", "=" + ("'" + strsql + "'")) + " )";
        //    }
        //}


       

    }
}