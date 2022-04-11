using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace TRADENET.Models
{
    public class LibraryModel : ConnectionModel
    {

        private SqlConnection SqlCon;

        public LibraryModel(bool _blnConnectionOn = false)
        {

        }

        public LibraryModel()
        {
            SqlCon = new SqlConnection(connectionstring);
        }

        public void ExecSQL(string strSQL)
        {
            try
            {
                using (SqlCommand MyCmd = new SqlCommand())
                {
                    if (SqlCon.State == ConnectionState.Closed)
                    {
                        SqlCon.Open();
                    }
                    MyCmd.Connection = SqlCon;
                    MyCmd.CommandTimeout = 5000;
                    MyCmd.CommandText = strSQL;
                    MyCmd.ExecuteNonQuery();
                    MyCmd.Dispose();
                    SqlCon.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void ExecSQL(string strSQL, SqlConnection con)
        {
            try
            {
                using (SqlCommand MyCmd = new SqlCommand())
                {
                    MyCmd.Connection = con;
                    MyCmd.CommandTimeout = 5000;
                    MyCmd.CommandText = strSQL;
                    MyCmd.ExecuteNonQuery();
                    MyCmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                string strExcep = ex.Message;
            }

        }


        public void ExecSQL(string strSQL, SqlConnection con, SqlTransaction tran)
        {
            using (SqlCommand MyCmd = new SqlCommand())
            {
                MyCmd.Connection = con;
                MyCmd.Transaction = tran;
                MyCmd.CommandTimeout = 5000;
                MyCmd.CommandText = strSQL;
                MyCmd.ExecuteNonQuery();
                MyCmd.Dispose();
            }
        }

        public DataTable OpenDataTable(string strSQL)
        {
            DataTable dt = new DataTable();

            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }
            SqlCommand cmd = new SqlCommand(strSQL, SqlCon);
            cmd.CommandTimeout = 5000;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            //adapter.SelectCommand.CommandTimeout = 180;
            adapter.Fill(dt);
            SqlCon.Close();
            return dt;

        }
        public DataTable OpenDataTable(string strSQL, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand(strSQL, con);
            cmd.CommandTimeout = 5000;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        public DataTable OpenDataTable(string strSQL, SqlConnection con, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand(strSQL, con, tran);
            cmd.CommandTimeout = 5000;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataSet OpenDataSet(string strFireQry)
        {
            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }

            using (SqlCommand objSqlCommand = new SqlCommand(strFireQry, SqlCon))
            {
                objSqlCommand.CommandTimeout = 5000;
                using (SqlDataAdapter MyAdapter = new SqlDataAdapter(objSqlCommand))
                {
                    DataSet MyDataSet = new DataSet();
                    MyAdapter.Fill(MyDataSet);
                    MyAdapter.Dispose();
                    return MyDataSet;
                }
            }
        }



        public DataSet OpenDataSetCommex(string StrSql)
        {
            var ObjCommexCon = new SqlConnection();
            UtilityDBModel myutildb = new UtilityDBModel();
            // var objApplicationUser = new ApplicationUser();
            ObjCommexCon = myutildb.commexTemp_conn(Convert.ToString("Commex"));
            if (ObjCommexCon.State == ConnectionState.Closed)
            {
                ObjCommexCon.Open();
            }


            var ObjDataSetOpen = new DataSet();
            using (var objSqlCommand = new SqlCommand(StrSql, ObjCommexCon))
            {
                objSqlCommand.CommandTimeout = 5000;
                using (var SqlDataAdapter = new SqlDataAdapter(objSqlCommand))
                {
                    SqlDataAdapter.Fill(ObjDataSetOpen);
                    return ObjDataSetOpen;

                }
            }
        }

        public DataSet OpenDataSet(string strFireQry, SqlConnection SqlCon)
        {


            using (SqlCommand objSqlCommand = new SqlCommand(strFireQry, SqlCon))
            {
                using (SqlDataAdapter MyAdapter = new SqlDataAdapter(objSqlCommand))
                {
                    objSqlCommand.CommandTimeout = 5000;
                    DataSet MyDataSet = new DataSet();

                    MyAdapter.Fill(MyDataSet);
                    MyAdapter.Dispose();
                    return MyDataSet;
                }
            }
        }

        public string GetScalarValueString(string strSQL)
        {
            string strReturn = "";

            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, SqlCon);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    strReturn = reader.GetValue(0).ToString().Trim();
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlCon.Close();
            }
            return strReturn;
        }

        public string GetScalarValueString(string strSQL, SqlConnection con)
        {
            string strReturn = "";
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    strReturn = reader.GetValue(0).ToString().Trim();
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return strReturn;
        }
        public DataTable OpendatatableXml(string strDataXml)
        {
            DataTable ObjDataTable;
            var dsData = new DataSet();
            var doc = new XmlDocument();
            doc.LoadXml(strDataXml);
            using (var nodereader = new XmlNodeReader(doc))
            {
                dsData.ReadXml(nodereader);
                ObjDataTable = dsData.Tables[0];
            }

            return ObjDataTable;
        }

        public decimal GetScalarValueDecimal(string strSQL)
        {
            decimal decReturn = 0;

            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, SqlCon);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    decReturn = decimal.Parse(reader.GetValue(0).ToString());
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlCon.Close();
            }
            return decReturn;
        }

        public int GetScalarValueInt(string strSQL)
        {
            int intReturn = 0;
            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, SqlCon);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    intReturn = int.Parse(reader.GetValue(0).ToString());
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlCon.Close();
            }
            return intReturn;
        }
        public int GetScalarValueInt(string strSQL, SqlConnection con)
        {
            int intReturn = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    intReturn = int.Parse(reader.GetValue(0).ToString());
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return intReturn;
        }


        public long GetScalarValueLong(string strSQL)
        {
            long lngReturn = 0;
            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, SqlCon);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lngReturn = long.Parse(reader.GetValue(0).ToString());
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlCon.Close();
            }
            return lngReturn;
        }
        public long GetScalarValueLong(string strSQL, SqlConnection con)
        {
            long lngReturn = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.CommandTimeout = 5000;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lngReturn = long.Parse(reader.GetValue(0).ToString());
                }
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return lngReturn;
        }



        //public string dtos(string dDate)
        //{
        //    if (Strings.Len(Strings.Trim(dDate)) > 0)
        //        dtos = Strings.Mid(dDate, 7, 4) + Strings.Mid(dDate, 4, 2) + Strings.Mid(dDate, 1, 2);
        //    else
        //        dtos = "";
        //}


        private Func<string, string> Mid(string dDate, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public string GetSysPARM(string strParmcd)
        {
            strSQL = "Select sp_sysvalue from sysparameter where sp_parmcd= '" + strParmcd + "'";
            string strValue = GetScalarValueString(strSQL);
            return strValue;
        }

        public string GetSysPARM(string strParmcd, SqlConnection con)
        {
            string strValue = "";
            strSQL = "Select sp_sysvalue from sysparameter where sp_parmcd= '" + strParmcd + "'";
            DataTable dtParm = OpenDataTable(strSQL, con);
            if (dtParm.Rows.Count > 0)
            {
                strValue = dtParm.Rows[0][0].ToString().Trim();
            }
            return strValue.Trim();
        }

        public string fnGetSysParamComm(string strSysCd, string strConn, string strLogin = "", string strCompany = "", string strExchange = "", bool blnstationary = false, string strSegment = "")
        {
            // Dim strCon As String
            var ObjCommexCon = new SqlConnection();
            UtilityDBModel myutildb = new UtilityDBModel();
            ObjCommexCon = myutildb.commexTemp_conn(strConn.Trim());
            if (ObjCommexCon.State == System.Data.ConnectionState.Closed)
            {
                ObjCommexCon.Open();
            }

            string strsql;
            string strWhere = "";
            if (!string.IsNullOrEmpty(strCompany))
            {
                strWhere = " and st_companycode='" + strCompany + "'";
            }

            if (!string.IsNullOrEmpty(strExchange))
            {
                strWhere = strWhere + " and st_exchange='" + strExchange + "'";
            }

            if (!string.IsNullOrEmpty(strSegment))
            {
                strWhere = strWhere + " and st_Segment='" + strSegment + "'";
            }

            if (blnstationary)
            {
                strsql = "Select st_sysvalue from Stationary where st_parmcd= '" + strSysCd + "' " + strWhere;
            }
            else
            {
                strsql = "Select sp_sysvalue from sysparameter where sp_parmcd= '" + strSysCd + "'";
            }

            var dbsCommand = new System.Data.SqlClient.SqlCommand(strsql, ObjCommexCon);
            var adpReader = new System.Data.SqlClient.SqlDataAdapter(dbsCommand);
            var dsReader = new System.Data.DataSet();
            adpReader.Fill(dsReader);
            if (dsReader.Tables[0].Rows.Count > 0)
            {
                return Strings.Trim(Conversions.ToString(dsReader.Tables[0].Rows[0][0]));
            }
            else
            {
                return "";
            }
        }



        public string fnFireQuery(string strTable, string strSelect, string strParam1, string strParam2, bool blnisNumeric)
        {

            if (strParam1.Trim().Length == 0 || strParam2.Trim().Length == 0)
            {
                return "";
            }
            if (blnisNumeric)
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }
            else
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }


            DataTable dtQry = OpenDataTable(strSQL);
            if (dtQry.Rows.Count == 0)
            {
                if (blnisNumeric)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return dtQry.Rows[0][0].ToString().Trim();
            }
        }

        public string fnFireQuery(string strTable, string strSelect, string strParam1, string strParam2, bool blnisNumeric, SqlConnection con)
        {
            if (strParam1.Trim().Length == 0 || strParam2.Trim().Length == 0)
            {
                return "";
            }
            if (blnisNumeric)
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }
            else
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }
            DataTable dtQry = OpenDataTable(strSQL, con);
            if (dtQry.Rows.Count == 0)
            {
                if (blnisNumeric)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return dtQry.Rows[0][0].ToString().Trim();
            }
        }

        public string fnFireQueryCommex(string strTable, string strSelect, string strParam1, string strParam2, bool blnisNumeric)
        {
            UtilityDBModel mydbutil = new UtilityDBModel();
            if (strParam1.Trim().Length == 0 || strParam2.Trim().Length == 0)
            {
                return "";
            }
            if (blnisNumeric)
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = " + strParam2.Trim();
            }
            else
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }
            SqlConnection SQLConnComex = mydbutil.commexTemp_conn("Commex");
            if (SQLConnComex.State == ConnectionState.Closed)
            {
                SQLConnComex.Open();
            }
            DataTable dtQry = OpenDataTable(strSQL, SQLConnComex);
            if (dtQry.Rows.Count == 0)
            {
                if (blnisNumeric)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return dtQry.Rows[0][0].ToString().Trim();
            }
        }


        public string fnFireEstroQuery(string strTable, string strSelect, string strParam1, string strParam2, bool strInt)
        {

            string strSQL;
            UtilityDBModel mydbutil = new UtilityDBModel();

            if (strParam1.Trim().Length == 0 || strParam2.Trim().Length == 0)
            {
                return "";
            }
            if (strInt == true)

            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }
            else
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }

            SqlConnection SQLConEstro = mydbutil.crostemp_conn("ESTRO");

            if (SQLConEstro.State == ConnectionState.Closed)
            {
                SQLConEstro.Open();
            }
            DataTable dtQry = OpenDataTable(strSQL, SQLConEstro);
            if (dtQry.Rows.Count == 0)
            {
                if (strInt)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return dtQry.Rows[0][0].ToString().Trim();
            }
        }

        public string fnFireCrossQuery(string strTable, string strSelect, string strParam1, string strParam2, bool strInt)
        {

            string strSQL;
            UtilityDBModel mydbutil = new UtilityDBModel();

            if (strParam1.Trim().Length == 0 || strParam2.Trim().Length == 0)
            {
                return "";
            }
            if (strInt == true)

            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }
            else
            {
                strSQL = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + strParam2.Trim() + "'";
            }

            SqlConnection SQLConcros = mydbutil.crostemp_conn("Cross");

            if (SQLConcros.State == ConnectionState.Closed)
            {
                SQLConcros.Open();
            }
            DataTable dtQry = OpenDataTable(strSQL, SQLConcros);
            if (dtQry.Rows.Count == 0)
            {
                if (strInt)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return dtQry.Rows[0][0].ToString().Trim();
            }
        }
        public string fnFireQueryOnlyPayout(string strTable, string strSelect, string strParam1, string strParam2, SqlConnection con, bool blnisNumeric)
        {

            string strsql;
            if (strParam1.ToString().Trim() == "0" | strParam2.ToString().Trim() == "0")
            {
                return "";
            }

            if (blnisNumeric)
            {
                strsql = "select " + strSelect + " from " + strTable + " where " + strParam1 + " Like '" + strParam2.ToString().Trim() + "'";
            }
            else
            {
                strsql = "select " + strSelect + " from " + strTable + " where " + strParam1 + " Like '" + strParam2.ToString().Trim() + "'";
            }

            DataTable dtQry = OpenDataTable(strsql, con);
            if (dtQry.Rows.Count == 0)
            {
                if (blnisNumeric)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return dtQry.Rows[0][0].ToString().Trim();
            }
        }


        //public string FnFireQuery(string strTable, string strSelect, string strParam1, string strParam2, bool strInt, SqlConnection con)
        //{
        //    string strsql;
        //    if (Strings.Len(strParam1) == 0 | Strings.Len(strParam2) == 0)
        //        return "";
        //    if (strInt == true)
        //        strsql = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + Strings.Trim(strParam2) + "'";
        //    else
        //        strsql = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + Strings.Trim(strParam2) + "'";


        //    using (SqlCommand ObjCommand = new SqlCommand(strsql, con))
        //    {
        //        using (SqlDataAdapter ObjAdapter = new SqlDataAdapter(ObjCommand))
        //        {
        //            DataTable ObjDataSet = new DataTable();
        //            ObjAdapter.Fill(ObjDataSet);
        //            if (ObjDataSet.Rows.Count < 1)
        //            {
        //                if (strInt == true)
        //                    return "";
        //                else
        //                    return 0;
        //            }
        //            else
        //                return ObjDataSet.Rows[0][0].ToString().Trim();


        //        }
        //    }
        //}
        public string GetSysParmSt(string strParmcd, string strTableName)
        {
            string strsql = string.Empty;
            string strReturn = string.Empty;
            if (strTableName == "")
            {
                strsql = "Select sp_sysvalue from Sysparameter where sp_parmcd= '" + strParmcd + "'";
                DataTable dtSP = OpenDataTable(strsql);
                if (dtSP.Rows.Count > 0)
                {
                    strReturn = dtSP.Rows[0]["sp_sysvalue"].ToString().Trim();
                    return strReturn;
                }
                else
                {
                    return strReturn;
                }
            }
            else if (strTableName.Trim().ToUpper() == "STATIONARY")
            {
                strsql = "Select st_sysvalue from Stationary where st_parmcd= '" + strParmcd + "'";
                strsql = strsql + " and st_companycode = (select min(em_cd) from Entity_master Where Len(Ltrim(Rtrim(em_cd))) = 1) and st_exchange = 'N'";
                DataTable dtST = OpenDataTable(strsql);
                if (dtST.Rows.Count > 0)
                {
                    strReturn = dtST.Rows[0]["st_sysvalue"].ToString().Trim();
                    return strReturn;
                }
                else
                { return strReturn; }
            }
            return strReturn;
        }

        public string fnGetSysParam(string strSysCd, string strLogin = "", string strCompany = "", string strExchange = "", bool blnstationary = false, string strSegment = "")
        {
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                LibraryModel mylib = new LibraryModel();
                string strsql;
                string strWhere = "";
                if (!string.IsNullOrEmpty(strCompany))
                    strWhere = " and st_companycode='" + strCompany + "'";
                if (!string.IsNullOrEmpty(strExchange))
                    strWhere = strWhere + " and st_exchange='" + strExchange + "'";
                if (!string.IsNullOrEmpty(strSegment))
                    strWhere = strWhere + " and st_Segment='" + strSegment + "'";
                if (blnstationary)
                    strsql = "Select st_sysvalue from Stationary where st_parmcd= '" + strSysCd + "' " + strWhere;
                else
                    strsql = "Select sp_sysvalue from sysparameter where sp_parmcd= '" + strSysCd + "'";
                curCon.Open();
                DataSet ObjDataSet = new DataSet();
                ObjDataSet = mylib.OpenDataSet(strsql, curCon);

                if (ObjDataSet.Tables[0].Rows.Count > 0)
                {
                    return ((ObjDataSet.Tables[0].Rows[0][0]).ToString());
                }
                else
                {
                    return "";
                }
            }
        }

        public string mfnGetT2Dt(string strExchange, string strDT)
        {
            using (SqlConnection curCon = new SqlConnection(connectionstring))
            {
                string strT2 = strDT;
                curCon.Open();
                LibraryModel mylib = new LibraryModel();
                UtilityModel util = new UtilityModel();
                if (strDT.Length > 8)
                { strT2 = util.dtos(strDT); }

                string strsql;
                for (int intLoop = 1; intLoop <= 2; intLoop++)
                {
                    strT2 = util.dtos(mylib.AddDayDT(strT2, 1).ToString("dd\\/MM\\/yyyy"));
                    AgaindtT2:
                    ;
                    strsql = "select * from Tholiday_master " + Microsoft.VisualBasic.Constants.vbNewLine;
                    strsql += " where hm_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "' " + Microsoft.VisualBasic.Constants.vbNewLine;
                    if (strExchange != "")
                    {
                        strsql += " and hm_exchange = '" + strExchange + "' " + Microsoft.VisualBasic.Constants.vbNewLine;
                    }
                    strsql += " and Replace(hm_dt,'BH','20') = '" + strT2 + "'" + Microsoft.VisualBasic.Constants.vbNewLine;


                    DataSet ObjDataSet = new DataSet();
                    ObjDataSet = mylib.OpenDataSet(strsql, curCon);

                    if (ObjDataSet.Tables[0].Rows.Count > 0)
                    {
                        strT2 = util.dtos(mylib.AddDayDT(strT2, 1).ToString("dd\\/MM\\/yyyy"));
                        goto AgaindtT2;
                    }
                }
                return strT2;
            }

        }
        public DateTime AddDayDT(string Date, int Dayvalue)
        {

            int Year = int.Parse(Date.Substring(0, 4));
            int Month = int.Parse(Date.Substring(4, 2));
            int Day = int.Parse(Date.Substring(6, 2));
            return new DateTime(Year, Month, Day).AddDays(Dayvalue);
        }


        public string fnGetTime(SqlConnection con)
        {
            string fnGetTimeRet = default(string);

            strSQL = "Select convert(char(8),getdate(),108) curtm";
            DataTable dtParm = OpenDataTable(strSQL, con);
            if (dtParm.Rows.Count > 0)
            {
                fnGetTimeRet = dtParm.Rows[0][0].ToString().Trim();
            }

            return fnGetTimeRet;
        }

        public string fnCheckInterOperability(string strDate, string strSegment, SqlConnection con)
        {
            string fnCheckInterOperabilityRet;
            DataTable dtInterOP = new DataTable();
            string StrSql;
            if (Convert.ToInt32(fnFireQuery("sysobjects", "count(0)", "name", "ClearingHouse", true, con)) == 0)
            {
                fnCheckInterOperabilityRet = "";
                return fnCheckInterOperabilityRet;
            }

            // If strSegment = "C" Then
            // If strStlmnt <> "" Then
            // If InStr(1, ",BW,BC,NN,NZ,", "," & Left(strStlmnt, 2) & ",") > 0 Then
            // ElseIf InStr(1, ",L,M,", "," & fnFireQuery("Settlement_type", "sy_maptype", "sy_exchange+sy_type", Left(strStlmnt, 2), True) & ",") > 0 Then
            // fnCheckInterOperability = ""
            // Exit Function
            // End If
            // strDate = fnFireQuery("Settlements", "se_stdt", "se_stlmnt", strStlmnt, True)
            // End If
            // End If

            StrSql = " select * from ClearingHouse " + " Where CH_CompanyCode = '" + HttpContext.Current.Session["CompanyCode"] + "' and CH_Segment = '" + strSegment + "'" + " and CH_EffDt = (Select max(CH_EffDt) from ClearingHouse " + " Where CH_CompanyCode = '" + HttpContext.Current.Session["CompanyCode"] + "' and CH_Segment = '" + strSegment + "' and CH_EffDt <='" + strDate + "')";

            dtInterOP = OpenDataTable(StrSql, con);


            if (dtInterOP.Rows.Count == 0)
            {
                fnCheckInterOperabilityRet = "";
            }
            else if (string.IsNullOrEmpty(dtInterOP.Rows[0]["CH_ClgHs"].ToString().Trim()))
            {
                fnCheckInterOperabilityRet = "";
            }
            else
            {
                fnCheckInterOperabilityRet = "TRUE";
            }

            return fnCheckInterOperabilityRet;
        }

        public string fnGetInterOpStlmnts(string strStlmnt, bool blnIncludeT2T = false, SqlConnection con = null)
        {
            string fnGetInterOpStlmntsRet = "";
            DataTable dtInterOP;
            string StrSql;
            string strData;
            short j;
            string StrOrderBy;
            StrOrderBy = fnFireQuery("ClearingHouse", "CH_ClgHs", "CH_Segment", "C", true, con);
            if ((StrOrderBy ?? "") == "I")
            {
                StrOrderBy = " order by Case left(se_stlmnt,1) when 'B' then 0 else 1 end";
            }
            else if ((StrOrderBy ?? "") == "N")
            {
                StrOrderBy = " order by Case left(se_stlmnt,1) when 'N' then 0 else 1 end";
            }
            else if ((StrOrderBy ?? "") == "M")
            {
                StrOrderBy = " order by Case left(se_stlmnt,1) when 'M' then 0 else 1 end";
            }

            if (Strings.InStr(1, ",BR,BW,BC,NA,NN,NZ,MA,MN,MZ,", "," + Strings.Left(strStlmnt, 2) + ",") > 0)
            {
                strData = "";
                StrSql = " select * from SEttlements Where se_stdt = (Select se_stdt from Settlements " + " Where se_stlmnt = '" + strStlmnt + "')";
                if (Strings.InStr(1, ",BW,NN,MN,", "," + Strings.Left(strStlmnt, 2) + ",") > 0)
                {
                    StrSql = StrSql + " and left(se_stlmnt,2) in ('BW','NN','MN')";
                }
                else if (Strings.InStr(1, ",BC,NZ,MZ,", "," + Strings.Left(strStlmnt, 2) + ",") > 0)
                {
                    StrSql = StrSql + " and left(se_stlmnt,2) in ('BC','NZ','MZ')";
                }
                else
                {
                    StrSql = StrSql + " and left(se_stlmnt,2) in ('BR','NA','MA','BD','NE')";
                }

                StrSql = StrSql + StrOrderBy;
                dtInterOP = OpenDataTable(StrSql, con);

                if (dtInterOP.Rows.Count == 0)
                {
                    strData = strStlmnt + ",";
                }
                else
                {
                    strData = "";
                    j = 0;
                    while (j < dtInterOP.Rows.Count)
                    {
                        strData = strData + dtInterOP.Rows[j]["se_stlmnt"] + ",";
                        j = Conversions.ToShort(j + 1);
                    }
                }


                if (blnIncludeT2T)
                {
                    StrSql = " select * from SEttlements Where se_stdt = (Select se_stdt from Settlements " + " Where se_stlmnt = '" + strStlmnt + "')";
                    StrSql = StrSql + " and left(se_stlmnt,2) in ('BC','NZ')";
                    dtInterOP = OpenDataTable(StrSql, con);
                    if (dtInterOP.Rows.Count == 0)
                    {
                        strData = strData + strStlmnt + ",";
                    }
                    else
                    {
                        j = 0;
                        while (j < dtInterOP.Rows.Count)
                        {
                            strData = strData + dtInterOP.Rows[j]["se_stlmnt"] + ",";
                            j = Conversions.ToShort(j + 1);
                        }
                    }


                }

                strData = Strings.Left(strData, Strings.Len(strData) - 1);
                fnGetInterOpStlmntsRet = strData;
            }
            else
            {
                fnGetInterOpStlmntsRet = strStlmnt;
            }

            return fnGetInterOpStlmntsRet;
        }

        public string fnGetInterOpExchange(string strSegment)
        {
            string fnGetInterOpExchangeRet = "";
            LibraryModel mylib = new LibraryModel();
            DataTable DsInterOP;
            string StrSql;
            string strData;
            short j;
            StrSql = " select CES_Cd from CompanyExchangeSegments Where CES_CompanyCd = '" + HttpContext.Current.Session["CompanyCode"] + "' and Right(CES_Cd,1) = '" + strSegment + "'";
            DsInterOP = mylib.OpenDataTable(StrSql);
            if (DsInterOP.Rows.Count == 0)
            {
                strData = "";
            }
            else
            {
                strData = "";
                j = 0;
                while (j < DsInterOP.Rows.Count)
                {
                    strData = strData + Strings.Mid((DsInterOP.Rows[j]["CES_Cd"]).ToString(), 2, 1) + ",";
                    j = Conversions.ToShort(j + 1);
                }
            }

            DsInterOP = null;
            strData = Strings.Left(strData, Strings.Len(strData) - 1);
            fnGetInterOpExchangeRet = strData;
            return fnGetInterOpExchangeRet;
        }
        public bool mfnGetSysSplFeatureCommodity(string strKeyCode, string strConn)
        {
            bool mfnGetSysSplFeatureCommodityRet;
            string strsql;
            string strcomname;
            var Dsfind = new DataTable();
            var ObjCommexCon = new SqlConnection();
            UtilityDBModel myutildb = new UtilityDBModel();
            UtilityModel myutil = new UtilityModel();
            ObjCommexCon = myutildb.commexTemp_conn(strConn.Trim());
            mfnGetSysSplFeatureCommodityRet = false;
            if (ObjCommexCon != null)
            {
                strsql = "select st_KeyCode ,st_KeyVal From sysTable Where st_KeyCode  = '" + strKeyCode + "'";
                Dsfind = OpenDataTable(strsql, ObjCommexCon);

                if (Dsfind.Rows.Count > 0)
                {
                    strsql = "select em_Name from Entity_master where em_cd =(select min(em_cd) from Entity_master)";
                    var dscomp = new DataTable();
                    dscomp = OpenDataTable(strsql, ObjCommexCon);
                    strcomname = Strings.Trim(Strings.Left(dscomp.Rows[0]["em_Name"].ToString(), 20));
                    if (myutil.Decrypt(Dsfind.Rows[0]["st_KeyVal"].ToString()) == Strings.UCase(strKeyCode + strcomname))
                    {
                        mfnGetSysSplFeatureCommodityRet = true;
                    }
                }
            }
            return mfnGetSysSplFeatureCommodityRet;
        }

        public string fnGetInterOpStlmntsForMultipleExch(string strStlmnt, string strDate, Boolean blnIncludeT2T = false)
        {
            DataTable DsInterOP;
            string StrSql;
            string strData;
            int j = 0;
            string StrWhere;

            LibraryModel mylib = new LibraryModel();
            UtilityModel myutil = new UtilityModel();
            UtilityDBModel mydbutil = new UtilityDBModel();

            StrWhere = fnFireQuery("ClearingHouse", "CH_ClgHs", "CH_Segment", "C", true);
            strData = "";
            StrSql = " select * from SEttlements Where se_stdt = '" + strDate + "'";

            if (StrWhere == "I")
            {

                StrSql += " and left(se_stlmnt,2) in ('BW','BC') ";
                StrSql += " order by Case left(se_stlmnt,2) when 'BW' then 0 else 1 end";
            }
            else if (StrWhere == "N")
            {
                StrSql += " and left(se_stlmnt,2) in ('NN','NZ') ";
                StrSql += " order by Case left(se_stlmnt,2) when 'NN' then 0 else 1 end";
            }
            else if (StrWhere == "M")
            {
                StrSql += " and left(se_stlmnt,2) in ('MN','MZ') ";
                StrSql += " order by Case left(se_stlmnt,2) when 'MN' then 0 else 1 end";
            }
            DsInterOP = mylib.OpenDataTable(StrSql);
            if (DsInterOP.Rows.Count == 0)
            {
                strData = strStlmnt + ",";
            }
            else
            {
                strData = "";
                j = 0;
                while (j < DsInterOP.Rows.Count)
                {
                    strData = strData + DsInterOP.Rows[j]["se_stlmnt"] + ",";
                    j = j + 1;
                }

            }

            DsInterOP = null;

            if (blnIncludeT2T)
            {

                StrSql = " select * from SEttlements Where se_stdt = '" + strDate + "'";
                StrSql += " and left(se_stlmnt,2) in ('BC','NZ') ";

                StrSql = StrSql + " Order by Case left(se_stlmnt,1) When '" + StrWhere + "' then 0 else 1 end";
                DsInterOP = mylib.OpenDataTable(StrSql);


                if (DsInterOP.Rows.Count == 0)
                {
                    strData = strData + strStlmnt + ",";
                }
                else
                {
                    j = 0;
                    while (j < DsInterOP.Rows.Count)
                    {
                        strData = strData + DsInterOP.Rows[j]["se_stlmnt"] + ",";
                        j = j + 1;
                    }
                }
                DsInterOP = null;

            }
            strData = Strings.Left(strData, Strings.Len(strData) - 1);
            return strData;

        }

        public double fnPeakFactor(string strForDate)
        {
            double dblPeakFactor = 25;
            if (Conversion.Val(strForDate) >= 20210901)
                dblPeakFactor = 100;
            else if (Conversion.Val(strForDate) >= 20210601)
                dblPeakFactor = 75;
            else if (Conversion.Val(strForDate) >= 20210301)
                dblPeakFactor = 50;
            else if (Conversion.Val(strForDate) >= 20201201)
                dblPeakFactor = 25;
            return dblPeakFactor;
        }
        public bool fnisPeakMargin(string StrDt)
        {
            bool isPeakMargin = false;
            if (Conversion.Val(StrDt.Trim()) >= Conversion.Val("20201001"))
                isPeakMargin = true;
            return isPeakMargin;
        }


         public double A2N(string strAlp)
        {
            double A2NRet;
            double intRtn;
            double mult;
            string stra;
            intRtn = 0d;
            mult = 1d;
            while (Strings.Len(Strings.RTrim(strAlp)) > 0)
            {
                stra = Strings.Mid(strAlp, Strings.Len(Strings.RTrim(strAlp)), 1);
                intRtn = intRtn + (Strings.Asc(stra) - 65) * mult;
                mult = mult * 26d;
                strAlp = Strings.Mid(strAlp, 1, Strings.Len(Strings.RTrim(strAlp)) - 1);
            }

            A2NRet = intRtn;
            return A2NRet;
        }
      

    }
}