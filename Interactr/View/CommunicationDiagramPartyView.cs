using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.View
{
    public class CommunicationDiagramPartyView : PartyView
    {

        #region MessageArrowStacks

        public MessageArrowStack LeftArrowStack { get; set; }
        public MessageArrowStack RighArrowStack { get; set; }
        
        #endregion

        public CommunicationDiagramPartyView() : base()
        {
            //Set the arrow stack size;
            LeftArrowStack.PreferredWidth = 3;
            LeftArrowStack.PreferredHeight = Height;
            RighArrowStack.PreferredWidth = 3;
            RighArrowStack.PreferredHeight = Height;

            // Define the left and right arrow stack to be the height of the partyview.
            HeightChanged.Subscribe(newHeight =>
            {
                LeftArrowStack.Height = newHeight;
                RighArrowStack.Height = newHeight;
            });
        }

        public class MessageArrowStack : StackPanel
        {
            /// <summary>
            /// Add a new anchor element to attack an arrow start or end point to in the MessageArrowStack.
            /// </summary>
            /// <returns> The new UIElement that was added to the MessageArrowStack</returns>
            public UIElement AddArrowAnchorElement()
            {
                // A UIElement that can be used to attach a message arrow to.
                var arrowAnchorElement = new UIElement();

                // Add the element to the MessageArrow StackPanel
                Children.Add(arrowAnchorElement);

                return arrowAnchorElement;
            }
        }
    }
}
