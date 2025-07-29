namespace Helpers
{
    public static class BaseSettup
    {
        public static string ServerName;
        public static string DatabaseName;
        public static string DatabaseUserName;
        public static string DatabasePassword;
        public static bool PersistSecurityInfo = true;
        public static string CultureInfo = "en-US";
        public static string CultureInfoTH = "th-TH";
        public static string ApplicationName;

        public static string ApiLogin;
        public static string ApplicationURL;

        public static string SqlMassager;
        public static bool IsNotConnect = false;

        public static int dbYear;

        public static string CheckInj(string sText)
        {
            return sText.Trim().Replace("=", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty).Replace("\'", string.Empty);
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

        //public static string GetCurrentUser()
        //{
        //    return System.DirectoryServices.AccountManagement.UserPrincipal.Current.SamAccountName;
        //}

        public static int UserID = 0;
        public static string ProVersion = "";
        public static bool IsDeploy = false;

        public static bool SaveErrorData(int UsrID, string ErrorMsg, string ErrorDetail, string pgversion)
        {
            if (BaseSettup.IsDeploy)
            {
                using (SqlServerDataConnection sqlCon = new SqlServerDataConnection())
                {
                    sqlCon.SqlCommandType = System.Data.CommandType.StoredProcedure;
                    sqlCon.CommandString = "Log_Program_Error_Save";
                    sqlCon.AddParameter("@UsrID", UsrID);
                    sqlCon.AddParameter("@ErrorMsg", ErrorMsg);
                    sqlCon.AddParameter("@ErrorDetail", ErrorDetail);
                    sqlCon.AddParameter("@ProgamVersion", pgversion);
                    return sqlCon.ExecuteErrorSaveNonQuery();
                }
            }
            else
            {
                return true;
            }
        }
    }
}
