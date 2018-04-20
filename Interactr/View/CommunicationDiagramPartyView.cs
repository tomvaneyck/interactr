using System;
using System.Drawing;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.View
{
    public class CommunicationDiagramPartyView : PartyView
    {
        #region MessageArrowStacks

        public MessageArrowStack LeftArrowStack { get; set; } = new MessageArrowStack();
        public MessageArrowStack RighArrowStack { get; set; } = new MessageArrowStack();

        #endregion

        public CommunicationDiagramPartyView()
        {
            // Set layout
            MarginsProperty.SetValue(LeftArrowStack, new Margins(0, 0, 0, 0));
            MarginsProperty.SetValue(RighArrowStack, new Margins(0, 0, 0, 0));
            AnchorsProperty.SetValue(LeftArrowStack, Anchors.Left);
            AnchorsProperty.SetValue(RighArrowStack, Anchors.Right);

            //Set the arrow stack size;
            LeftArrowStack.PreferredWidth = 3;
            LeftArrowStack.PreferredHeight = Height;
            RighArrowStack.PreferredWidth = 3;
            RighArrowStack.PreferredHeight = Height;

            // Define the left and right arrow stack to be the height of the partyview.
            HeightChanged.Subscribe(newHeight =>
            {
                LeftArrowStack.PreferredHeight = newHeight;
                RighArrowStack.PreferredHeight = newHeight;
            });

            Children.Add(LeftArrowStack);
            Children.Add(RighArrowStack);
        }

        /// <summary>
        /// A StackPanel that is contains ArrowAnchor UIElements.
        /// The ArrowAnchors are used to attach a start or end point of an arrow too
        /// and the StackPanel stacks the ArrowAnchor elements vertically.
        /// </summary>
        public class MessageArrowStack : StackPanel
        {
            /// <summary>
            /// Add a new anchor element to attack an arrow start or end point to in the MessageArrowStack.
            /// </summary>
            /// <returns> The new UIElement that was added to the MessageArrowStack</returns>
            public ArrowAnchor AddArrowAnchorElement()
            {
                // A UIElement that can be used to attach a message arrow to.
                var arrowAnchorElement = new ArrowAnchor();

                // Add the element to the MessageArrow StackPanel
                Children.Add(arrowAnchorElement);

                return arrowAnchorElement;
            }

            public MessageArrowStack()
            {
                // Stack the elements vertically.
                StackOrientation = Orientation.Vertical;
            }
        }

        /// <summary>
        /// An element used for attaching Arrows start and endpoints to.
        /// </summary>
        public class ArrowAnchor : UIElement
        {
            
            public ArrowAnchor()
            {
                PreferredWidth = 3;
                PreferredHeight = 10;
            }
        }
    }
}