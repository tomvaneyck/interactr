using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private Button _moveUpButton;
        private Button _moveDownButton;
        private Button _deleteButton;
        private ListView<T> _listBox;

        public ListBox(Func<T, UIElement> viewFactory)
        {
            _moveUpButton = new Button
            {
                Label = "Move up"
            };
            _moveUpButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.MoveByIndex(_listBox.SelectedIndex, _listBox.SelectedIndex - 1);
            });
            _moveDownButton = new Button
            {
                Label = "Move down"
            };
            _moveDownButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.MoveByIndex(_listBox.SelectedIndex, _listBox.SelectedIndex + 1);
            });
            _deleteButton = new Button
            {
                Label = "Delete"
            };
            _moveUpButton.OnButtonClick.Subscribe(_ =>
            {
                ItemsSource.RemoveAt(_listBox.SelectedIndex);
            });

            this.Children.Add(new StackPanel
            {
                Children =
                {
                    _moveUpButton,
                    _moveDownButton,
                    _deleteButton
                },
                StackOrientation = Orientation.Horizontal
            });

            _listBox = new ListView<T>(viewFactory);
            this.ItemsSourceChanged.Subscribe(list => _listBox.ItemsSource = list);
            this.Children.Add(_listBox);
        }
    }
}
