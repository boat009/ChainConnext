using ChainConnext.Shared.Authen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChainConnext.Shared
{
    public class DataLeakApi
    {
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public string? OSUser { get; set; }
        public string? HostName { get; set; }
        public string? HostIP { get; set; }
        public string? ApplicationName { get; set; }
        public string? ApplicationURL { get; set; }
        public string? ServerIP { get; set; }
        public string? ServerName { get; set; }
        public string? UserDatabase { get; set; }
        public string? DataName { get; set; }
        public string? ActionName { get; set; }
        public string? ActionParameter { get; set; }
        public string? ActionNote { get; set; }
        public string? ActionScript { get; set; }
        public string? RefNo { get; set; }
        public string? ContNo { get; set; }

        public string? status { get; set; }
        public string? message { get; set; }

        public async Task<DataLeakApi> SentApi(DataLeakApi api)
        {
            try
            {
                var values = new Dictionary<string, string>
              {
                { "UserCode", api.UserCode},
                { "UserName", api.UserName},
                { "OSUser", api.OSUser},
                { "HostName", api.HostName},
                { "HostIP", api.HostIP},
                { "ApplicationName", api.ApplicationName},
                { "ApplicationURL", api.ApplicationURL},
                { "ServerIP", api.ServerIP},
                { "ServerName", api.ServerName},
                { "UserDatabase", api.UserDatabase},
                { "DataName", api.DataName},
                { "ActionName", api.ActionName},
                { "ActionParameter", api.ActionParameter},
                { "ActionNote", api.ActionNote},
                { "ActionScript", api.ActionScript},
                { "RefNo", api.RefNo==null?"-":string.IsNullOrEmpty(api.RefNo.Trim())?"-":api.RefNo },
                { "ContNo", api.ContNo==null?"-":string.IsNullOrEmpty(api.ContNo.Trim())?"-":api.ContNo }
            };

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.sabuyconnext.com/sandbox/DataLeak/Save");
                request.Method = "POST";
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(JsonConvert.SerializeObject(values));

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream())
                {
                    await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DataLeakApi>(responseString);
                api.status = result.status;
                api.message = result.message;
            }
            catch (Exception ex)
            {
                api.status = "Api DataLeak Error";
                api.message = ex.Message;
            }
            return api;
        }
    }
}
