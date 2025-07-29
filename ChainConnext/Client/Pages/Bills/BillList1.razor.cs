using ChainConnext.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using ChainConnext.Shared.Payments;
using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;

namespace ChainConnext.Client.Pages.Bills
{
    public partial class BillList1
    {
        [Parameter]
        public Contract_Info? pConInf { get; set; }
        [Parameter]
        public int pHeight { get; set; }
        [Parameter]
        public int pWidth { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public List<Payment_Info> payment_Infos { get; set; } = new List<Payment_Info>();
        [Parameter]
        public bool isLoading { get; set; } = false;

        decimal SumPay = 0;
        int CountPay = 0;
        string SumPayColor = "black";

        //protected override async Task OnInitializedAsync()
        //{
        //    await PaymentListData();
        //}

        protected override async Task OnParametersSetAsync()
        {
            if (pConInf == null)
            {
                return;
            }
            if (payment_Infos == null)
            {
                return;
            }
            if (payment_Infos.Count == 0)
            {
                return;
            }
            Console.WriteLine($"parameters set {pConInf.ContractId} pConInf.AfterDiscount={pConInf.AfterDiscount}");
            await Task.Run(() =>
            {
                if (payment_Infos.Count > 0)
                {
                    SumPay = payment_Infos.Sum(x => x.PayAmt);
                    CountPay = payment_Infos.Count;

                    decimal afterDiscount = 0;
                    if (pConInf.AfterDiscount == 0)
                    {
                        afterDiscount = pConInf.Credit;
                    }
                    else
                    {
                        afterDiscount = pConInf.AfterDiscount;
                    }
                    if (afterDiscount > 0)
                    {
                        var p_sum = (SumPay / afterDiscount) * 100;
                        if (p_sum < 50)
                        {
                            SumPayColor = "red";
                        }
                        else if (p_sum >= 50 && p_sum < 90)
                        {
                            SumPayColor = "Orange";
                        }
                        else
                        {
                            SumPayColor = "green";
                        }
                    }
                }
            });
            //await PaymentListData();
        }

        private async Task PaymentListData()
        {
            if (pConInf == null)
            {
                return;
            }
            if(pConInf.ContractId == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(pConInf.ContractId.Trim()))
            {
                return;
            }

            Console.WriteLine("IsLoading");

            Authens userData = new Authens();
            userData = await _accountService.GetAuthensAsync(Navigation.Uri);

            var postBody = new Payment_Info { ContractId = pConInf.ContractId, UserData = userData };
            var response = await Http.PostAsJsonAsync("Payment/PaymentList", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                //Logger.LogInformation(Rs.Msg);
                if (Rs.Data != null)
                {
                    payment_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Payment_Info>>(Rs.Data.ToString());

                    SumPay = payment_Infos.Sum(x => x.PayAmt);
                    CountPay = payment_Infos.Count;
                    var p_sum = (SumPay / pConInf.AfterDiscount) * 100;
                    if (p_sum < 50)
                    {
                        SumPayColor = "red";
                    }
                    else if (p_sum >= 50 && p_sum < 90)
                    {
                        SumPayColor = "Orange";
                    }
                    else
                    {
                        SumPayColor = "green";
                    }
                }
            }
        }
    }
}
