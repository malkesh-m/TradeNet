using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TRADENET.Const
{
    public class ColumnStyle
    {
        public string ColumnName { get; set; }
        public string HeaderName { get; set; }
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Right;
        public int Width { get; set; } = 80;
        public bool Visible { get; set; } = true;
        public bool IsGroup { get; set; } = false;
        public bool IsSum { get; set; } = false;
        public int DecimalPlaces { get; set; } = 0;
        public string PopupUrl { get; set; }
    }

    public class DevExreamData
    {
        public DataTable Data { get; set; }
        public List<ColumnStyle> ColumnStyles { get; set; }
        public bool IsZeroVisible { get; set; } = true;
        public bool IsThousandSep { get; set; } = false;
        public string CompanyLogo { get; set; }
        public string Header { get; set; }
        public string SubHeader1 { get; set; }
        public string SubHeader2 { get; set; }
    }
}