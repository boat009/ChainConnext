using ChainConnext.Shared.BD;
using Microsoft.AspNetCore.Components;

namespace ChainConnext.Client.Pages.ARs
{
    public partial class ARContractDetailList
    {
        [Parameter]
        public BD_MastCont? pMastCont { get; set; }
        [Parameter]
        public List<BD_MastCont> ListDetailMastCont { get; set; } = new List<BD_MastCont>();
        [Parameter]
        public bool IsLoad { get; set; } = false;
        [Parameter]
        public int Height { get; set; }
        [Parameter]
        public int Width { get; set; }
        IList<BD_MastCont>? selectedDetailMastCont;
    }
}
