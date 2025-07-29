using ChainConnext.Shared.Authen;
using ExcelDataReader;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared
{
    public class BaseShared
    {
        public Authens? UserData { get; set; }
        public string? SqlMessage { get; set; }

        public static string AppVersion = "";
        public static string ServerPrefix = "";

        public static string? HostUrl;
        public bool ForTest { get; set; } = false;
        public static string CheckInj(string sText)
        {
            return sText.Trim().Replace("=", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty).Replace("\'", string.Empty);
        }
        public static string ConvertPsw(string pass)
        {
            var bytes = Encoding.UTF8.GetBytes(pass);
            return Convert.ToBase64String(bytes);
        }
        public static string GetPsw(string pass)
        {
            var data = Convert.FromBase64String(pass);
            return Encoding.UTF8.GetString(data);
        }

        public static string Base64EncodeSession(string plainText)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            //return System.Convert.ToBase64String(plainTextBytes);

            //var plainTextBytes = Zip(plainText);
            //return System.Convert.ToBase64String(plainTextBytes);
            return Base64Zip(plainText);
        }
        public static string Base64DecodeSession(string base64EncodedData)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            //return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            //var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            //return Unzip(base64EncodedBytes);

            return Base64UnZip(base64EncodedData);
        }
        public static string Base64Zip(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            string B64 = System.Convert.ToBase64String(plainTextBytes);

            var pb = Zip(B64);
            return System.Convert.ToBase64String(pb);
        }
        public static string Base64UnZip(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            string unz = Unzip(base64EncodedBytes);

            var F64 = System.Convert.FromBase64String(unz);
            return System.Text.Encoding.UTF8.GetString(F64);
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static string FillRefNo(string value,int len)
        {
            string result = "";
            if (value.Contains("-") || value.Contains("+"))
            {
                string tmpRefNo = value.Replace("-", "|").Replace("+", "|").Trim();
                List<string> lplus = tmpRefNo.Split('|').ToList();
                if (lplus.Count > 1)
                {
                    string pre = lplus[0].Trim();
                    string lst = lplus[1].Trim();
                    while ((pre + lst).Length < len)
                    {
                        lst = "0" + lst;
                    }
                    tmpRefNo = pre + lst;
                }
                result = tmpRefNo.ToUpper();
            }
            else
            {
                result = value.ToUpper();
            }
            return result;
        }

        public static bool CheckCompareDate(DateTime dateFrom, DateTime dateTo)
        {
            dateFrom = Convert.ToDateTime(dateFrom.ToShortDateString());
            dateTo = Convert.ToDateTime(dateTo.ToShortDateString());
            if (dateFrom.CompareTo(dateTo) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string DataTableToJson(DataTable dt)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }
        public static DataTable JsonToDataTable(string json)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return JsonConvert.DeserializeObject<DataTable>(json);
        }
        public static string DataSetToJson(DataSet dt)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }
        public static DataSet JsonToDataSet(string json)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return JsonConvert.DeserializeObject<DataSet>(json);
        }

        public static ExcelWorksheet FormatExccel(ExcelWorksheet ws, DataTable dt, string[]? ColDate = null, string[]? ColDateTime = null, string[]? ColInt = null, string[]? ColNumber = null, string[]? ColString = null)
        {
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                if (ColDate != null)
                {
                    if (ColDate.Length > 0)
                    {
                        for (int i = 0; i < ColDate.Length; i++)
                        {
                            if (ColDate[i].Trim() == dt.Columns[c].ColumnName.Trim())
                            {
                                ws.Column(c + 1).Style.Numberformat.Format = "d/m/yyyy";
                            }
                        }
                    }
                }
                else
                {
                    if (dt.Columns[c].ColumnName.ToLower().Trim().Contains("date")
                        || dt.Columns[c].ColumnName.Trim().Contains("วันที่")
                        || dt.Columns[c].ColumnName.Trim().Contains("วัน"))
                    {
                        ws.Column(c + 1).Style.Numberformat.Format = "d/m/yyyy";
                    }
                }
                if (ColDateTime != null)
                {
                    if (ColDateTime.Length > 0)
                    {
                        for (int i = 0; i < ColDateTime.Length; i++)
                        {
                            if (ColDateTime[i].Trim() == dt.Columns[c].ColumnName.Trim())
                            {
                                ws.Column(c + 1).Style.Numberformat.Format = "d/m/yyyy hh:mm";
                            }
                        }
                    }
                }
                if (ColInt != null)
                {
                    if (ColInt.Length > 0)
                    {
                        for (int i = 0; i < ColInt.Length; i++)
                        {
                            if (ColInt[i].Trim() == dt.Columns[c].ColumnName.Trim())
                            {
                                ws.Column(c + 1).Style.Numberformat.Format = "#,##0";
                            }
                        }
                    }
                }
                if (ColNumber != null)
                {
                    if (ColNumber.Length > 0)
                    {
                        for (int i = 0; i < ColNumber.Length; i++)
                        {
                            if (ColNumber[i].Trim() == dt.Columns[c].ColumnName.Trim())
                            {
                                ws.Column(c + 1).Style.Numberformat.Format = "#,##0.00";
                            }
                        }
                    }
                }
                if (ColString != null)
                {
                    if (ColString.Length > 0)
                    {
                        for (int i = 0; i < ColString.Length; i++)
                        {
                            if (ColString[i].Trim() == dt.Columns[c].ColumnName.Trim())
                            {
                                ws.Column(c + 1).Style.Numberformat.Format = "@";
                            }
                        }
                    }
                }
            }
            return ws;
        }

        public static string ConvertFromOADate(string DatePara)
        {
            if (string.IsNullOrEmpty(DatePara.Trim()))
            {
                return string.Empty;
            }
            else
            {
                if (DatePara.Contains("/"))
                {
                    try
                    {
                        return CheckDateCalendar(Convert.ToDateTime(DatePara).ToString());
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    try
                    {
                        if (Convert.ToDouble(DatePara) != 0)
                        {
                            return CheckDateCalendar(DateTime.FromOADate(Convert.ToDouble(DatePara)).ToString());
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    catch
                    {
                        try
                        {
                            return CheckDateCalendar(Convert.ToDateTime(DatePara).ToString());
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                }
            }
        }

        static string CheckDateCalendar(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return string.Empty;
            }
            string DatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            DateTime dtNew = Convert.ToDateTime(date);
            int iYear = DateTime.Now.Year;
            string strCurrentCalendar = Thread.CurrentThread.CurrentCulture.Calendar.ToString();

            if (strCurrentCalendar == "System.Globalization.ThaiBuddhistCalendar")
            {
                iYear = iYear + 543;
                string sDateNew = string.Format(new CultureInfo("th-TH", true), @"{0:" + DatePattern + " HH:mm:ss}", dtNew);
                string cYear = string.Format(new CultureInfo("th-TH", true), @"{0:yyyy}", dtNew);
                int CheckYear = int.Parse(cYear);
                if (CheckYear > (iYear + 400))
                {
                    string YearOld = CheckYear.ToString();
                    string YearNew = (CheckYear - 543).ToString();
                    sDateNew = sDateNew.Replace(YearOld, YearNew);
                }
                else if (CheckYear <= (iYear - 400))
                {
                    string YearOld = CheckYear.ToString();
                    string YearNew = (CheckYear + 543).ToString();
                    sDateNew = sDateNew.Replace(YearOld, YearNew);
                }
                return sDateNew;
            }
            else
            {
                string sDateNew = string.Format(new CultureInfo("en-US", true), @"{0:" + DatePattern + " HH:mm:ss}", dtNew);
                string cYear = string.Format(new CultureInfo("en-US", true), @"{0:yyyy}", dtNew);
                int CheckYear = int.Parse(cYear);
                if (CheckYear > (iYear + 400))
                {
                    string YearOld = CheckYear.ToString();
                    string YearNew = (CheckYear - 543).ToString();
                    sDateNew = sDateNew.Replace(YearOld, YearNew);
                }
                else if (CheckYear <= (iYear - 400))
                {
                    string YearOld = CheckYear.ToString();
                    string YearNew = (CheckYear + 543).ToString();
                    sDateNew = sDateNew.Replace(YearOld, YearNew);
                }

                return sDateNew;
            }
        }

        public static DataTable FData(DataTable dt)
        {
            DataTable dtf = new DataTable();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string ClName = dt.Columns[i].ColumnName.Trim();
                dtf.Columns.Add(ClName, typeof(string));
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtf.Rows.Add(dt.Rows[i].ItemArray);
            }
            for (int i = 0; i < dtf.Rows.Count; i++)
            {
                for (int j = 0; j < dtf.Columns.Count; j++)
                {
                    if (dtf.Rows[i][j].ToString().Trim() == "(blank)")
                    {
                        dtf.Rows[i][j] = "";
                    }
                }
            }
            for (int i = 0; i < dtf.Rows.Count; i++)
            {
                for (int j = 0; j < dtf.Columns.Count; j++)
                {
                    if (dtf.Rows[i][j].ToString().Trim() == "-")
                    {
                        dtf.Rows[i][j] = "";
                    }
                }
            }
            for (int i = 0; i < dtf.Rows.Count; i++)
            {
                for (int j = 0; j < dtf.Columns.Count; j++)
                {
                    if (dtf.Rows[i][j].ToString().Trim() == "NULL")
                    {
                        dtf.Rows[i][j] = "";
                    }
                }
            }
            for (int i = 0; i < dtf.Rows.Count; i++)
            {
                for (int j = 0; j < dtf.Columns.Count; j++)
                {
                    if (dtf.Rows[i][j].ToString().Trim() == "-   -")
                    {
                        dtf.Rows[i][j] = "";
                    }
                }
            }
            for (int i = 0; i < dtf.Rows.Count; i++)
            {
                for (int j = 0; j < dtf.Columns.Count; j++)
                {
                    if (dtf.Rows[i][j].ToString().Trim().ToUpper() == "#N/A")
                    {
                        dtf.Rows[i][j] = "";
                    }
                }
            }
            return dtf;
        }
        public static bool CheckHeaderData(DataTable dt,DataTable dtCheckHeader)
        {
            bool ishash = true;
            for (int i = 0; i < dtCheckHeader.Columns.Count; i++)
            {
                string cname = dtCheckHeader.Columns[i].ColumnName.Trim();
                if (!string.IsNullOrEmpty(cname))
                {
                    if (!dt.Columns.Contains(cname))
                    {
                        ishash = false;
                        break;
                    }
                }
            }
            return ishash;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if (dr[column.ColumnName] != null)
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }
    }
}
