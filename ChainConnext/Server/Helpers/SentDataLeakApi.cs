using ChainConnext.Shared;
using Helpers;

namespace ChainConnext.Server.Helpers
{
    public class SentDataLeakApi
    {
        public static async Task<DataLeakApi> SentApiFormat(BaseShared x, string actionName, string actionNote, string actionParameter, string actionScript)
        {
            DataLeakApi dataLeak = new DataLeakApi();
            if (x.UserData != null)
            {
                return await dataLeak.SentApi(new DataLeakApi
                {
                    UserCode = x.UserData.UserID,
                    UserName = x.UserData.UserName,
                    ApplicationURL = x.UserData.AppUrl,
                    ApplicationName = $"{BaseSettup.ApplicationName}({BaseShared.AppVersion})",
                    ServerIP = BaseSettup.ServerName,
                    ServerName = BaseSettup.ServerName,
                    UserDatabase = BaseSettup.DatabaseUserName,
                    DataName = BaseSettup.DatabaseName,
                    ActionName = actionName,
                    ActionNote = actionNote,
                    ActionParameter = actionParameter,
                    ActionScript = actionScript
                });
            }
            else
            {
                return dataLeak;
            }
        }
    }
}
