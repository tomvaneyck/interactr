using System;
using System.Drawing;
using Interactr.View.Controls;
using Interactr.View.Framework;

namespace Interactr.View
{
    public class CommunicationDiagramPartyView : PartyView
    {
        #region MessageArrowStacks

        /// <summary>
        /// The left arrow stack for attaching arrows to the party.
        /// </summary>
        public MessageArrowStack LeftArrowStack { get; set; } = new MessageArrowStack(Orientation.Vertical);

        /// <summary>
        /// The right arrow stack for attaching arrows to the party.
        /// </summary>
        public MessageArrowStack RightArrowStack { get; set; } = new MessageArrowStack(Orientation.Vertical);

        #endregion

        public CommunicationDiagramPartyView()
        {
            // Set layout
            MarginsProperty.SetValue(LeftArrowStack, new Margins(Width / 3, 0, 0, 0));
            MarginsProperty.SetValue(RightArrowStack, new Margins(0, 0, Width / 3, 0));

            AnchorsProperty.SetValue(LeftArrowStack, Anchors.Left);
            AnchorsProperty.SetValue(RightArrowStack, Anchors.Right);

            //Set the arrow stack size;
            LeftArrowStack.PreferredWidth = 3;
            LeftArrowStack.PreferredHeight = Height;

            RightArrowStack.PreferredWidth = 3;
            RightArrowStack.PreferredHeight = Height;

            // Define the left and right arrow stack to be the height of the partyview.
            HeightChanged.Subscribe(newHeight =>
            {
                LeftArrowStack.PreferredHeight = newHeight;
                RightArrowStack.PreferredHeight = newHeight;
            });

            // Change the margin size on a change of width
            WidthChanged.Subscribe(newWidth =>
            {
                MarginsProperty.SetValue(LeftArrowStack, new Margins(newWidth / 4, 0, 0, 0));
                MarginsProperty.SetValue(RightArrowStack, new Margins(0, 0, newWidth / 4, 0));
            });

            Children.Add(LeftArrowStack);
            Children.Add(RightArrowStack);
        }

        /// <summary>
        /// A StackPanel that is contains ArrowAnchor UIElements.
        /// The ArrowAnchors are used to attach a start or end point of an arrow too
        /// and the StackPanel stacks the ArrowAnchor elements vertically.
        /// </summary>
        public class MessageArrowStack : StackPanel
        {
            /// <summary>
            /// Add a new anchor element to attach an arrow start or end point to in the MessageArrowStack.
            /// </summary>
            /// <returns> The new UIElement that was added to the MessageArrowStack</returns>
            public ArrowAnchor AddArrowAnchorElement(int width, int height)
            {
                // A UIElement that can be used to attach a message arrow to.
                var arrowAnchorElement = new ArrowAnchor(width, height);

                // Add the element to the MessageArrow StackPanel
                Children.Add(arrowAnchorElement);

                return arrowAnchorElement;
            }

            public MessageArrowStack(Orientation orientation)
            {
                StackOrientation = orientation;
            }
        }

        /// <summary>
        /// An element used for attaching Arrows start and endpoints to.
        /// </summary>
        public class ArrowAnchor : UIElement
        {
            public ArrowAnchor(int width, int height)
            {
                PreferredWidth = width;
                PreferredHeight = height;
            }
        }
    }
}