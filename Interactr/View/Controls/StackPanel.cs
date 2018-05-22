using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A layout UIElement that stacks its children either horizontally or vertically.
    /// </summary>
    public class StackPanel : UIElement
    {
        #region StackOrientation

        private readonly ReactiveProperty<Orientation> _stackOrientation = new ReactiveProperty<Orientation>();

        public Orientation StackOrientation
        {
            get => _stackOrientation.Value;
            set
            {
                if (Enum.GetValues(typeof(Orientation)).Cast<Orientation>().Count(f => value.HasFlag(f)) != 1)
                {
                    throw new ArgumentException("The stackpanel must have one single orientation.");
                }

                _stackOrientation.Value = value;
            }
        }

        public IObservable<Orientation> StackOrientationChanged => _stackOrientation.Changed;

        #endregion

        #region AutoCompactEnabled

        private readonly ReactiveProperty<bool> _autoCompactEnabled = new ReactiveProperty<bool>();

        /// <summary>
        /// If true, this panel will set its preferred size to the minimal size needed to display its contents.
        /// If false, the panel will not update its preferred size.
        /// True by default.
        /// </summary>
        public bool AutoCompactEnabled
        {
            get => _autoCompactEnabled.Value;
            set => _autoCompactEnabled.Value = value;
        }

        public IObservable<bool> AutoCompactEnabledChanged => _autoCompactEnabled.Changed;

        #endregion
        
        public StackPanel()
        {
            AutoCompactEnabled = true;

            // Set can be focused false.
            CanBeFocused = false;

            //Horizontal stack by default
            StackOrientation = Orientation.Horizontal;

            // Update layout when the width, height or orientation of this panel changes.
            ReactiveExtensions.MergeEvents(
                WidthChanged,
                HeightChanged,
                StackOrientationChanged
            ).Subscribe(_ => UpdateLayout());

            // Update a child when its PreferredWidth/Height property changes.
            Observable.Merge(
                Children.ObserveEach(child => child.PreferredHeightChanged),
                Children.ObserveEach(child => child.PreferredWidthChanged)
            ).Subscribe(p => UpdateLayout());

            // Update the positions of the children when a child is added or removed.
            ReactiveExtensions.MergeEvents(
                Children.OnDelete,
                Children.OnAdd
            ).Subscribe(_ => UpdateLayout());

            AutoCompactEnabledChanged.Subscribe(_ => UpdateLayout());
        }

        /// <summary>
        /// Updates the position and size of the children.
        /// </summary>
        private void UpdateLayout()
        {
            if (StackOrientation == Orientation.Horizontal)
            {
                UpdateHorizontalLayout();
            }
            else
            {
                UpdateVerticalLayout();
            }

            UpdatePreferredSize();
        }

        /// <summary>
        /// Stacks the children horizontally.
        /// The width of each child is set to its preferred width.
        /// The height of each child is set to the panel height.
        /// The positions of the elements stack left-to-right.
        /// </summary>
        private void UpdateHorizontalLayout()
        {
            int curX = 0;
            foreach (UIElement child in Children)
            {
                child.Position = new Point(curX, 0);
                child.Width = child.PreferredWidth;
                child.Height = Height;
                curX += child.PreferredWidth;
            }
        }

        /// <summary>
        /// Stacks the children vertically.
        /// The width of each child is set to the panel width.
        /// The height of each child is set to its preferred height.
        /// The positions of the elements stack top-to-bottom.
        /// </summary>
        private void UpdateVerticalLayout()
        {
            int curY = 0;
            foreach (UIElement child in Children)
            {
                child.Position = new Point(0, curY);
                child.Width = Width;
                child.Height = child.PreferredHeight;
                curY += child.PreferredHeight;
            }
        }

        private void UpdatePreferredSize()
        {
            if (AutoCompactEnabled)
            {
                PreferredWidth = Children.Select(c => c.Position.X + c.PreferredWidth).Concat(new []{ 0 }).Max();
                PreferredHeight = Children.Select(c => c.Position.Y + c.PreferredHeight).Concat(new[] { 0 }).Max();
            }
        }
    }
}
