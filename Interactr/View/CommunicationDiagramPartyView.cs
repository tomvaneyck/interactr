using System;
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
            LeftArrowStack = new MessageArrowStack();
            RighArrowStack = new MessageArrowStack();

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

            // Set the positions of the ArrowStacks relative to the party views.
            LeftArrowStack.Position = new Point(0, 0);
            RighArrowStack.Position = new Point(Width - 3, 0);

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

            public MessageArrowStack() : base()
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
            public ArrowAnchor() : base()
            {
                PreferredWidth = 3;
                PreferredHeight = 3;
            }
        }
    }
}
