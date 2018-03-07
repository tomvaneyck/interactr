using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
    /// <summary>
    /// Layout UIElement that automatically resizes subelements as the size of this panel changes. 
    /// </summary>
    public class AnchorPanel : UIElement
    {
        public static AttachedProperty<Anchors> AnchorsProperty { get; }
            = new AttachedProperty<Anchors>(Anchors.Left | Anchors.Top | Anchors.Right | Anchors.Bottom);

        public static AttachedProperty<Margins> MarginsProperty { get; } 
            = new AttachedProperty<Margins>(new Margins());

        public AnchorPanel()
        {
            InitializeObservables();
        }

        /// <summary>
        /// Initialize the observables for this panel.
        /// </summary>
        /// <remarks>
        /// Observe:
        /// <list type="bullet">
        /// <item>a combination of WidthChanged and HeightChanged,</item>
        /// <item>the children of this panel</item>
        /// </list>
        /// </remarks>
        private void InitializeObservables()
        {
            // Update layout when the width or height of this panel changes.
            Observable.Merge(WidthChanged, HeightChanged).Subscribe(_ => UpdateLayout());

            // Update a child when its Anchors or Margins property changes.
            Children.ObserveEach(child => child.AttachedProperties.OnValueChanged)
                .Where(p => p.Value.Key == AnchorsProperty || p.Value.Key == MarginsProperty)
                .Subscribe(p => UpdateLayout(p.Element));

            // Update a child when its PreferredWidth/Height property changes.
            Observable.Merge(
                Children.ObserveEach(child => child.PreferredHeightChanged),
                Children.ObserveEach(child => child.PreferredWidthChanged)
            ).Subscribe(p => UpdateLayout(p.Element));
        }

        private void UpdateLayout()
        {
            foreach (UIElement child in Children)
            {
                UpdateLayout(child);
            }
        }

        private void UpdateLayout(UIElement child)
        {
            Anchors childAnchors = AnchorsProperty.GetValue(child);
            Margins childMargins = MarginsProperty.GetValue(child);

            int x = childMargins.Left;
            int y = childMargins.Top;

            if (childAnchors.HasFlag(Anchors.Right))
            {
                if (childAnchors.HasFlag(Anchors.Left))
                {
                    // Stretch element horizontally
                    x = childMargins.Left;
                    child.Width = this.Width - (childMargins.Left + childMargins.Right);
                }
                else
                {
                    // Stick to right side
                    x = this.Width - (child.PreferredWidth + childMargins.Right);
                    child.Width = child.PreferredWidth;
                }
            }
            else
            {
                child.Width = child.PreferredWidth;
            }

            if (childAnchors.HasFlag(Anchors.Bottom))
            {
                if (childAnchors.HasFlag(Anchors.Top))
                {
                    // Stretch element vertically
                    y = childMargins.Top;
                    child.Height = this.Height - (childMargins.Top + childMargins.Bottom);
                }
                else
                {
                    // Stick to bottom
                    y = this.Height - (child.PreferredHeight + childMargins.Bottom);
                    child.Height = child.PreferredHeight;
                }
            }
            else
            {
                child.Height = child.PreferredHeight;
            }

            child.Position = new Point(x, y);
        }
    }
}
