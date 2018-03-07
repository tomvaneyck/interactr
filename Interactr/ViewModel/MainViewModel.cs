using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.ViewModel
{
    public class MainViewModel
    {
        public CommunicationDiagramViewModel CommDiagramVM { get; } = new CommunicationDiagramViewModel();
        public SequenceDiagramViewModel SeqDiagramVM { get; } = new SequenceDiagramViewModel();

        public void SwitchViews()
        {
            if (CommDiagramVM.IsVisible)
            {
                CommDiagramVM.IsVisible = false;
                SeqDiagramVM.IsVisible = true;
            }
            else //If SeqDiagramVM or neither is visible
            {
                CommDiagramVM.IsVisible = true;
                SeqDiagramVM.IsVisible = false;
            }
        }
    }
}
