using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using ExcelDataReader;
using System.Data;

namespace ChainConnext.Client.Services
{
    public class ShareValues
    {
        public static string CurrentURL = "";

        public static List<ProModel> PmdData = new List<ProModel>();

        public static string GetTokenUrl()
        {
            string url = BaseShared.ConvertPsw(ShareValues.CurrentURL.Replace("https://","").Replace("http://", "").Replace("/", "").Replace("/", ""));
            return url;
        }
    }

    public class WindowDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
