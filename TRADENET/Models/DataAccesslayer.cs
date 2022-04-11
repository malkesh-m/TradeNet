using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml;

namespace TRADENET.Models
{
    public class DataAccesslayer 
    {

        private SqlConnection _conn = new SqlConnection();
        private SqlTransaction _trans;
        protected SqlConnection _connectionstring = new SqlConnection(ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString);
        private SqlCommand _command = new SqlCommand();
        protected SqlConnection ObjConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TradenetConnectionString"].ConnectionString);

       
            public SqlCommand SqlCommand
            {
                get
                {
                    return _command;
                }

                set
                {
                    _command = value;
                }
            }

            public SqlConnection SqlConnection
            {
                get
                {
                    return _conn;
                }

                set
                {
                    _conn = value;
                }
            }

            public SqlTransaction Transaction
            {
                get
                {
                    return _trans;
                }

                set
                {
                    _trans = value;
                }
            }
  
        public DataSet OpenDataSet(string StrSql, bool blncon, int cmdtimeout)
        {
            if (ObjConnection.State == ConnectionState.Closed)
            {
                ObjConnection.Open();
            }

            var ObjDataSetOpen = new DataSet();
            using (var SqlDataAdapter = new SqlDataAdapter(StrSql, ObjConnection))
            {
                SqlDataAdapter.Fill(ObjDataSetOpen);
                return ObjDataSetOpen;
              //  SqlDataAdapter.Dispose();
                if(blncon == true)
                {
                    ObjConnection.Close();
                }
            }
        }


        public void Executesql(string StrSql, bool blncon, int cmdtimeout)
        {
            if (ObjConnection.State == ConnectionState.Closed)
            {
                ObjConnection.Open();
            }

            using (var SqlCommand = new SqlCommand())
            {
                SqlCommand.Connection = ObjConnection;
                SqlCommand.CommandTimeout = cmdtimeout;
                SqlCommand.CommandType = CommandType.Text;
                SqlCommand.CommandText = StrSql;
                SqlCommand.ExecuteNonQuery();
                SqlCommand.Dispose();
                if (blncon == true)
                {
                    ObjConnection.Close();
                }
            }
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

        public int ExecutesqlXml(string StrSql)
        {
            if (ObjConnection.State == ConnectionState.Closed)
            {
                ObjConnection.Open();
            }

            int rowsAffected;
            using (var SqlCommand = new SqlCommand())
            {
                SqlCommand.Connection = ObjConnection;
                SqlCommand.CommandType = CommandType.Text;
                SqlCommand.CommandText = StrSql;
                rowsAffected = SqlCommand.ExecuteNonQuery();
                return rowsAffected;
               // SqlCommand.Dispose();
            }
        }

        public string GetSysParmSt(string strParmcd, string strTableName = "", string strExchSeg = "")
        {
            string GetSysParmStRet = "";
            DataTable dsSys = new DataTable();
            //var ObjConnection = new SqlConnection(ConfigurationManager.ConnectionStrings("TradenetDefaultConnectionString").ConnectionString);
            GetSysParmStRet = "";
            string strSysSQL = "";
            if (string.IsNullOrEmpty(Strings.Trim(strTableName)))
            {
                strSysSQL = "Select sp_sysvalue from Sysparameter where sp_parmcd= '" + strParmcd + "'";
            }
            else if (Strings.Trim(Strings.UCase(strTableName)) == "STATIONARY")
            {
                strSysSQL = "Select st_sysvalue from Stationary where st_parmcd= '" + strParmcd + "'";
                strSysSQL = strSysSQL + " and st_companycode = '" + HttpContext.Current.Session["CompanyCode"] + "'";
                if (string.IsNullOrEmpty(strExchSeg))
                {
                }
                else
                {
                    strSysSQL = strSysSQL + "  and st_exchange+st_segment = '" + strExchSeg + "'";
                }
            }

            if (string.IsNullOrEmpty(Strings.Trim(strTableName)) | Strings.Trim(Strings.UCase(strTableName)) == "STATIONARY")
            {
                //dsSys = lib.OpenDataTable(strSysSQL);
                using (dsSys)
                    if (string.IsNullOrEmpty(Strings.Trim(strTableName)))
                    {
                        if (dsSys.Rows.Count <= 0)
                        {
                            GetSysParmStRet = "";
                        }
                        else
                        {
                            GetSysParmStRet = dsSys.Rows[0]["sp_sysvalue"].ToString();
                        }
                    }
                    else if (Strings.Trim(Strings.UCase(strTableName)) == "STATIONARY")
                    {
                        if (dsSys.Rows.Count <= 0)
                        {
                            GetSysParmStRet = "";
                        }
                        else
                        {
                            GetSysParmStRet = dsSys.Rows[0]["st_sysvalue"].ToString();
                        }
                    }
            }

            return GetSysParmStRet;
        }

        public string fnFireQuery(string strTable, string strSelect, string strParam1, string strParam2, bool strInt)
        {
            string fnFireQueryRet = "";
            string strsql;
            if (Strings.Len(strParam1) == 0 | Strings.Len(strParam2) == 0)
            {
                return "";
            }

            if (strInt == true)
            {
                strsql = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + Strings.Trim(strParam2) + "'";
            }
            else
            {
                strsql = "select " + strSelect + " from " + strTable + " where " + strParam1 + " = '" + Strings.Trim(strParam2) + "'";
            }

            DataSet ObjDataSet;
            ObjDataSet = OpenDataSet(strsql, false, 100);
            if (ObjDataSet.Tables[0].Rows.Count < 1)
            {
                if (strInt == true)
                {
                    fnFireQueryRet = "";
                }
                else
                {
                    fnFireQueryRet = 0.ToString();
                }
            }
            else
            {
                fnFireQueryRet = Strings.Trim(ObjDataSet.Tables[0].Rows[0][0].ToString());
            }

            return fnFireQueryRet;
        }

    }
}