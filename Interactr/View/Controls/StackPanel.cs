using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Orientation StackOrientation
        {
            get => _stackOrientation.Value;
            set => _stackOrientation.Value = value;
        }
        private readonly ReactiveProperty<Orientation> _stackOrientation = new ReactiveProperty<Orientation>();
        public IObservable<Orientation> StackOrientationChanged => _stackOrientation.Changed;
        #endregion

        public StackPanel()
        {
            // Update layout when the width, height or orientation of this panel changes.
            Observable.Merge(
                WidthChanged.Select(_ => Unit.Default), 
                HeightChanged.Select(_ => Unit.Default),
                StackOrientationChanged.Select(_ => Unit.Default)
            ).Subscribe(_ => UpdateLayout());

            // Update a child when its PreferredWidth/Height property changes.
            Observable.Merge(
                Children.ObserveEach(child => child.PreferredHeightChanged),
                Children.ObserveEach(child => child.PreferredWidthChanged)
            ).Subscribe(p => UpdateLayout());
        }

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
