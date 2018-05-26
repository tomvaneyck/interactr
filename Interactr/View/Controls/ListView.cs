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
            set
            {
                if (value >= Children.Count || value < -1)
                {
                    throw new IndexOutOfRangeException();
                }

                _selectedIndex.Value = value;
            }
        }

        public IObservable<int> SelectedIndexChanged => _selectedIndex.Changed;

        #endregion

        private ItemContainer _selectedContainer;

        public ListView(Func<T, UIElement> viewFactory)
        {
            this.StackOrientation = Orientation.Vertical;

            // Select nothing by default.
            SelectedIndex = -1;

            // Setup list content view elements.
            var elements = ItemsSourceChanged.CreateDerivedListBinding(e => new ItemContainer(e, viewFactory(e))).ResultList;
            elements.OnAdd.Subscribe(e => Children.Insert(e.Index, e.Element));
            elements.OnDelete.Subscribe(e => Children.RemoveAt(e.Index));
            elements.OnMoved.Subscribe(e =>
            {
                if (e.Reason == MoveReason.Reordering)
                {
                    Children.ApplyPermutation(e.Changes.Select(c => (c.OldIndex, c.NewIndex)));
                }
            });

            SetupSelectionBindings();
        }

        private void SetupSelectionBindings()
        {
            // When a child is clicked, set SelectedIndex.
            Children.ObserveEach(e => e.MouseEventPreviewOccured).Subscribe(e =>
            {
                if (e.Value.Id == MouseEvent.MOUSE_PRESSED)
                {
                    SelectedIndex = Children.IndexOf(e.Element);
                }
            });

            // If an item is inserted, shift SelectedIndex one place if necessary.
            Children.OnAdd.Subscribe(e =>
            {
                if (SelectedIndex >= e.Index)
                {
                    SelectedIndex++;
                }
            });

            // If an item is removed, shift SelectedIndex one place if necessary.
            Children.OnDelete.Subscribe(e =>
            {
                if (SelectedIndex >= e.Index)
                {
                    SelectedIndex--;
                }
            });

            // If the selected item is moved, change the selected index accordingly.
            Children.OnMoved.Subscribe(e =>
            {
                if (e.Reason == MoveReason.Reordering)
                {
                    foreach (var change in e.Changes.Where(c => c.OldIndex == SelectedIndex))
                    {
                        SelectedIndex = change.NewIndex;
                        break;
                    }
                }
            });

            // When the selected index changes, mark the previously selected ItemContainer as not-selected
            // and mark the newly selected ItemContainer as selected.
            SelectedIndexChanged.Subscribe(newIndex =>
            {
                if (_selectedContainer != null)
                {
                    _selectedContainer.IsSelected = false;
                    _selectedContainer = null;
                }

                if (newIndex >= 0)
                {
                    _selectedContainer = (ItemContainer)Children[newIndex];
                    _selectedContainer.IsSelected = true;
                }
            });
        }

        public int GetSourceIndexOfView(UIElement e)
        {
            ItemContainer container = Children.OfType<ItemContainer>().FirstOrDefault(c => c.View == e);
            return Children.IndexOf(container);
        }

        private class ItemContainer : AnchorPanel
        {
            public T Item { get; }
            public UIElement View { get; }
            
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
                View = view;

                MarginsProperty.SetValue(View, new Margins(3, 3, 3, 3));
                this.Children.Add(View);

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
