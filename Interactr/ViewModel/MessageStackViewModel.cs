using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interactr.Model;
using Interactr.Reactive;
using Message = Interactr.Model.Message;

namespace Interactr.ViewModel
{
    public class MessageStackViewModel
    {
        private readonly Diagram _diagram;

        private ReactiveList<MessageViewModel> _messageViewModels;

        private readonly ReactiveList<ActivationBarViewModel> _activationBars =
            new ReactiveList<ActivationBarViewModel>();

        public MessageStackViewModel(Diagram diagram)
        {
            _diagram = diagram;

            //When the diagram changes, recalculate layout.
            Observable.Merge(
                diagram.Messages.OnAdd.Select(_ => Unit.Default),
                diagram.Messages.OnDelete.Select(_ => Unit.Default),
                diagram.Messages.ObserveEach(msg => msg.ReceiverChanged).Select(_ => Unit.Default),
                diagram.Messages.ObserveEach(msg => msg.SenderChanged).Select(_ => Unit.Default)
            ).Subscribe(_ => CalculateLayout());

            //Map Messages to MessageViewModels
            _messageViewModels = _diagram.Messages.CreateDerivedList(msg => new MessageViewModel(msg, 0)).ResultList;
        }

        private void CalculateLayout()
        {
            _activationBars.Clear();

            // Handle the edge case where there are no messages.
            if (_diagram.Messages.Count == 0)
            {
                return;
            }

            // Iterate over messages, tick of message = index in list
            for (int i = 0; i < _messageViewModels.Count; i++)
            {
                _messageViewModels[i].Tick = i;
            }

            // Iterate over messages, maintain stack, create activation bar on pop.
            // Push = invocation message
            // Pop = result message
            Stack<(Party Party, int StartTick)> stack = new Stack<(Party Party, int Index)>();
            foreach (MessageViewModel messageVM in _messageViewModels)
            {
                Message message = messageVM.Message;

                if (message.Type == Message.MessageType.Invocation)
                {
                    stack.Push((message.Receiver, messageVM.Tick));
                }
                else
                {
                    (Party party, int startTick) = stack.Pop();
                    _activationBars.Add(new ActivationBarViewModel(message.Receiver, startTick, messageVM.Tick));
                }
            }

            // Add activation bar for initiator starting at tick 0, ending at last message tick.
            _activationBars.Add(new ActivationBarViewModel(_messageViewModels[0].Message.Sender, 0,
                _messageViewModels.Last().Tick));
        }
    }
}

}