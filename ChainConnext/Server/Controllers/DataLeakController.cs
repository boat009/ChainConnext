using ChainConnext.Shared.Authen;
using ChainConnext.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChainConnext.Client;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace ChainConnext.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DataLeakController : ControllerBase
    {
        public async Task<DataLeakApi> SentDataLeakApi([FromBody] DataLeakApi api)
        {
            DataLeakApi dataLeak = new DataLeakApi();
            return await dataLeak.SentApi(api);
        }
    }
}
