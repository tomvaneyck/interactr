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
        public Diagram Diagram { get; }

        public ReactiveList<MessageViewModel> MessageViewModels { get; }

        public ReactiveList<ActivationBarViewModel> ActivationBars { get; } = new ReactiveList<ActivationBarViewModel>();

        public MessageStackViewModel(Diagram diagram)
        {
            Diagram = diagram;
            
            // Map Messages to MessageViewModels
            MessageViewModels = Diagram.Messages.CreateDerivedList(msg => new MessageViewModel(msg, 0)).ResultList;

            // When the diagram changes, recalculate layout.
            Observable.Merge(
                diagram.Messages.OnAdd.Select(_ => Unit.Default),
                diagram.Messages.OnDelete.Select(_ => Unit.Default),
                diagram.Messages.ObserveEach(msg => msg.ReceiverChanged).Select(_ => Unit.Default),
                diagram.Messages.ObserveEach(msg => msg.SenderChanged).Select(_ => Unit.Default)
            ).Subscribe(_ => CalculateLayout());
        }

        private void CalculateLayout()
        {
            ActivationBars.Clear();

            // Handle the edge case where there are no messages.
            if (Diagram.Messages.Count == 0)
            {
                return;
            }

            // Iterate over messages, tick of message = index in list
            for (int i = 0; i < MessageViewModels.Count; i++)
            {
                MessageViewModels[i].Tick = i;
            }

            // Iterate over messages, maintain stack, create activation bar on pop.
            // Push = invocation message
            // Pop = result message
            Stack<(Party Party, int StartTick)> stack = new Stack<(Party Party, int Index)>();
            foreach (MessageViewModel messageVM in MessageViewModels)
            {
                Message message = messageVM.Message;

                if (message.Type == Message.MessageType.Invocation)
                {
                    stack.Push((message.Receiver, messageVM.Tick));
                }
                else
                {
                    (Party receivingParty, int startTick) = stack.Pop();
                    if (receivingParty != messageVM.Receiver)
                    {
                        // Messages are not balanced, abort!!!
                        return;
                    }
                    ActivationBars.Add(new ActivationBarViewModel(message.Receiver, startTick, messageVM.Tick));
                }
            }

            // Add activation bar for initiator starting at tick 0, ending at last message tick.
            ActivationBars.Add(new ActivationBarViewModel(MessageViewModels[0].Message.Sender, 0, MessageViewModels.Last().Tick));
        }

        public LifeLineViewModel CreateLifeLineForParty(PartyViewModel party)
        {
            return new LifeLineViewModel(this, party);
        }
    }
}