using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr.Reactive;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
    public class ListBox<T> : StackPanel
    {
        #region ItemsSource

        private readonly ReactiveProperty<ReactiveList<T>> _itemsSource = new ReactiveProperty<ReactiveList<T>>();

        public ReactiveList<T> ItemsSource
        {
            get => _itemsSource.Value;
            set => _itemsSource.Value = value;
        }

        public IObservable<ReactiveList<T>> ItemsSourceChanged => _itemsSource.Changed;

        #endregion

        private readonly Button _addButton;
        private readonly Button _moveUpButton;
        private readonly Button _moveDownButton;
        private readonly Button _deleteButton;

        public ListView<T> ListView { get; }

        public ListBox(Func<T> itemFactory, Func<T, UIElement> viewFactory)
        {
            this.StackOrientation = Orientation.Vertical;

            ListView = new ListView<T>(viewFactory);
            this.ItemsSourceChanged.Subscribe(list => ListView.ItemsSource = list);

            // Add move up button.
            _addButton = new Button
            {
                Label = "Add new"
            };
            _addButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.Add(itemFactory());
            });

            // Add move up button.
            _moveUpButton = new Button
            {
                Label = "Move up"
            };
            _moveUpButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.MoveByIndex(ListView.SelectedIndex, ListView.SelectedIndex - 1);
            });
            ListView.SelectedIndexChanged
                .Select(index => index > 0)
                .Subscribe(canMoveUp => _moveUpButton.IsEnabled = canMoveUp);

            // Add move down button.
            _moveDownButton = new Button
            {
                Label = "Move down"
            };
            _moveDownButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.MoveByIndex(ListView.SelectedIndex, ListView.SelectedIndex + 1);
            });
            ListView.SelectedIndexChanged
                .Select(index => index >= 0 && index < ItemsSource.Count-1)
                .Subscribe(canMoveDown => _moveDownButton.IsEnabled = canMoveDown);

            // Add delete button.
            _deleteButton = new Button
            {
                Label = "Delete"
            };
            _deleteButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.RemoveAt(ListView.SelectedIndex);
            });
            ListView.SelectedIndexChanged
                .Select(index => index >= 0)
                .Subscribe(canDelete => _deleteButton.IsEnabled = canDelete);

            var stackPanel = new StackPanel
            {
                Children =
                {
                    _addButton,
                    _moveUpButton,
                    _moveDownButton,
                    _deleteButton
                },
                StackOrientation = Orientation.Horizontal
            };
            this.Children.Add(stackPanel);
            this.Children.Add(ListView);
        }

        public override void PaintElement(Graphics g)
        {
            g.DrawRectangle(Pens.Black, 0, 0, Width-1, Height-1);
        }
    }
}
