using ChainConnext.Shared;
using ChainConnext.Shared.BD;
using Radzen;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Products
{
    public partial class ProModelDialog
    {
        List<BDProModel> proModels = new List<BDProModel>();
        IList<BDProModel>? selectedModel;
        bool IsModelLoading = false;
        string SearchValue = "";

        async Task Search()
        {
            IsModelLoading = true;
            if (string.IsNullOrEmpty(SearchValue.Trim()))
            {
                return;
            }
            var postBody = new BDProModel { ModelDesc = SearchValue };
            var response = await Http.PostAsJsonAsync("BD/GetProModelFind", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.IsSuccess)
                {
                    if (Rs.Rows > 0)
                    {
                        proModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BDProModel>>(Rs.Data.ToString());
                    }
                }
                else
                {
                    //Logger.LogInformation(Rs.Msg);
                    NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Rs.Msg, Duration = 5000 });
                }
            }
            IsModelLoading = false;
        }

        async Task Selected(BDProModel? daTa, string key)
        {
            if (daTa == null)
            {
                daTa = new BDProModel();
            }
            ExecResult Rs = new ExecResult();
            Rs.Data = daTa;
            Rs.Rows = 1;
            Rs.IsSuccess = true;
            dialogService.Close(Rs);
        }
    }
}
