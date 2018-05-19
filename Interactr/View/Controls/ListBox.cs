using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.View.Framework;

namespace Interactr.View.Controls
{
    public class ListBox<T> : StackPanel
    {
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
            _moveDownButton = new Button
            {
                Label = "Move down"
            };
            _deleteButton = new Button
            {
                Label = "Delete"
            };

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
            this.Children.Add(_listBox);
        }
    }
}
