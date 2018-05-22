using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Interactr.View.Framework;
using Interactr.ViewModel;
using Interactr.Window;

namespace Interactr.View.Controls
{
    public class ListView<T> : StackPanel
    {
        #region ItemsSource

        private readonly ReactiveProperty<IReadOnlyReactiveList<T>> _itemsSource = new ReactiveProperty<IReadOnlyReactiveList<T>>();

        public IReadOnlyReactiveList<T> ItemsSource
        {
            get => _itemsSource.Value;
            set => _itemsSource.Value = value;
        }

        public IObservable<IReadOnlyReactiveList<T>> ItemsSourceChanged => _itemsSource.Changed;

        #endregion
        
        #region SelectedItem
        public T SelectedItem
        {
            get => SelectedIndex != -1 ? ItemsSource[SelectedIndex] : default(T);
            set => SelectedIndex = ItemsSource.IndexOf(value);
        }
        #endregion

        #region SelectedIndex

        private readonly ReactiveProperty<int> _selectedIndex = new ReactiveProperty<int>();

        public int SelectedIndex
        {
            get => _selectedIndex.Value;
            set => _selectedIndex.Value = value;
        }

        public IObservable<int> SelectedIndexChanged => _selectedIndex.Changed;

        #endregion

        private ItemContainer _selectedContainer;

        public ListView(Func<T, UIElement> viewFactory)
        {
            this.StackOrientation = Orientation.Vertical;

            // Select nothing by default.
            SelectedIndex = -1;

            var elements = ItemsSourceChanged.CreateDerivedListBinding(e => new ItemContainer(e, viewFactory(e))).ResultList;
            elements.OnAdd.Subscribe(e => Children.Insert(e.Index, e.Element));
            elements.OnDelete.Subscribe(e => this.Children.RemoveAt(e.Index));

            // When a child is clicked, mark it as selected
            this.Children.ObserveEach(e => e.MouseEventPreviewOccured).Subscribe(e =>
            {
                if (e.Value.Id == MouseEvent.MOUSE_PRESSED)
                {
                    ItemContainer selectedContainer = (ItemContainer)e.Element;
                    selectedContainer.IsSelected = true;
                    e.Value.IsHandled = true;
                }
            });

            // When a child becomes selected, unselect all other items and set SelectedIndex
            this.Children.ObserveEach(e => ((ItemContainer)e).IsSelectedChanged).Subscribe(e =>
            {
                // If an item was selected.
                if (e.Value)
                {
                    // Unselect the previous selection, if any.
                    if (_selectedContainer != null)
                    {
                        _selectedContainer.IsSelected = false;
                    }

                    // Store the new selection.
                    _selectedContainer = (ItemContainer)e.Element;
                    SelectedIndex = Children.IndexOf(_selectedContainer);
                }
            });

            // When the selected element is deleted, select a neighbour or nothing
            this.Children.OnDelete.Where(e => e.Index == SelectedIndex).Subscribe(e =>
            {
                if (this.Children.Count == 0)
                {
                    // Clear selection.
                    this.SelectedIndex = -1;
                }
                else
                {
                    // Select next element, except if the previous selection was the last element in the list.
                    this.SelectedIndex = e.Index == Children.Count ? e.Index-1 : e.Index + 1;
                }
            });

            // When the selected element is moved, update the selected index.
            Children.OnMoved
                .WithLatestFrom(SelectedIndexChanged, (e, i) => (EventData: e, LatestIndex: i))
                // Find the change where the currently selected index was moved, if any such change exists.
                .Where(e => e.EventData.Changes.Any(change => change.OldIndex == e.LatestIndex))
                .Subscribe(e =>
                {
                    // Set the new index.
                    SelectedIndex = e.EventData.Changes.First(c => c.OldIndex == e.LatestIndex).NewIndex;
                });
        }

        private class ItemContainer : AnchorPanel
        {
            public T Item { get; }
            
            #region IsSelected

            private readonly ReactiveProperty<bool> _isSelected = new ReactiveProperty<bool>();

            public bool IsSelected
            {
                get => _isSelected.Value;
                set => _isSelected.Value = value;
            }

            public IObservable<bool> IsSelectedChanged => _isSelected.Changed;

            #endregion
            
            public ItemContainer(T item, UIElement view)
            {
                Item = item;

                MarginsProperty.SetValue(view, new Margins(3, 3, 3, 3));
                this.Children.Add(view);

                IsSelectedChanged.Subscribe(_ => Repaint());
            }

            public override void PaintElement(Graphics g)
            {
                g.FillRectangle(IsSelected ? Brushes.LightBlue : Brushes.White, 0, 0, Width, Height);
                g.DrawRectangle(Pens.Black, 0, 0, Width-1, Height-1);
            }
        }
    }
}
