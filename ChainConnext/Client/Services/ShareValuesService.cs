using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChainConnext.Client.Services
{
    public class ShareValuesService
    {
        IWebAssemblyHostEnvironment _hostEnvironment;
        public ShareValuesService(IWebAssemblyHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }
    }
}
