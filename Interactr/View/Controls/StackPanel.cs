﻿using System;
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

        public StackPanel()
        {
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
    }
}
