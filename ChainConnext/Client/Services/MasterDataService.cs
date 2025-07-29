using ChainConnext.Shared.Authen;
using ChainConnext.Shared.BD;
using ChainConnext.Shared;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq.Dynamic.Core.Tokenizer;

namespace ChainConnext.Client.Services
{
    public interface IMasterDataService
    {
        Task GetProModelData();
        Task<List<ProModel>> GetProModelDatas();
    }
    public class MasterDataService: IMasterDataService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        public MasterDataService(ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }
        public async Task GetProModelData()
        {
            var postBody = new ProModel();
            var response = await _httpClient.PostAsJsonAsync("BD/ListProModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Data != null)
                {
                    List<ProModel> PmdData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProModel>>(Rs.Data.ToString());
                    string Data = Newtonsoft.Json.JsonConvert.SerializeObject(PmdData);
                    ShareValues.PmdData = PmdData;
                    await _localStorageService.SetItemAsync("BDProModel", Data);
                }
            }
        }
        public async Task<List<ProModel>> GetProModelDatas()
        {
            List<ProModel> PmdData = new List<ProModel>();
            var postBody = new ProModel();
            var response = await _httpClient.PostAsJsonAsync("BD/ListProModel", postBody);

            ExecResult? Rs = await response.Content.ReadFromJsonAsync<ExecResult>();
            if (Rs != null)
            {
                if (Rs.Data != null)
                {
                    PmdData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProModel>>(Rs.Data.ToString());
                }
            }
            return PmdData;
        }
    }
}
